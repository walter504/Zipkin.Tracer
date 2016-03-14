using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using log4net;
using Newtonsoft.Json;

namespace Zipkin.Tracer.SpanCollector.Http
{
    public class SpanProcessor
    {
        private static ILog logger = log4net.LogManager.GetLogger(typeof(SpanProcessor));
        private const int MAX_SUBSEQUENT_EMPTY_BATCHES = 2;

        private volatile bool stop = false;
        private readonly string url;
        private readonly BlockingCollection<Span> queue;
        private readonly HttpSpanCollectorConfig config;
        private readonly List<JsonSpan> JsonSpans = new List<JsonSpan>();
        private int processedSpansNum = 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseUrl">Zipkin query service host</param>
        /// <param name="queue">BlockingCollection that will provide spans</param>
        /// <param name="config"></param>
        /// <param name="logger"></param>
        public SpanProcessor(string baseUrl, BlockingCollection<Span> queue,  HttpSpanCollectorConfig config)
        {
            if (string.IsNullOrEmpty(baseUrl))
            {
                throw new ArgumentNullException("baseUrl");
            }
            if (queue == null)
            {
                throw new ArgumentNullException("queue");
            }
            
            this.url = baseUrl + (baseUrl.EndsWith("/") ? "" : "/") + "api/v1/spans";
            this.queue = queue;
            this.config = config;
        }
        public void Stop()
        {
            this.stop = true;
        }

        public int Execute()
        {
            int subsequentEmptyBatches = 0;
            do
            {
                try
                {
                    Span span;
                    if (queue.TryTake(out span, 5000))
                    {
                        JsonSpans.Add(new JsonSpan(span));
                    }
                    else
                    {
                        subsequentEmptyBatches++;
                    }

                    if (subsequentEmptyBatches >= MAX_SUBSEQUENT_EMPTY_BATCHES && JsonSpans.Count != 0
                        || JsonSpans.Count >= config.BatchSize 
                        || stop && JsonSpans.Count != 0)
                    {
                        SubmitSpans(JsonSpans);
                        JsonSpans.Clear();
                        subsequentEmptyBatches = 0;
                    }
                }
                catch (Exception e)
                {
                    logger.Error("Unexpected exception flushing spans", e);
                }
            } while (!stop);
            return processedSpansNum;
        }
        
        private void SubmitSpans(List<JsonSpan> spans)
        {
            long start = DateTime.Now.Ticks;
            processedSpansNum += spans.Count;
            bool success = PostSpans(spans);
            if (success && logger.IsInfoEnabled)
            {
                logger.InfoFormat("Submitting {0} spans to service took {1}ms.", spans.Count, (DateTime.Now.Ticks - start) / 10000D);
            }
        }

        private bool PostSpans(List<JsonSpan> spans)
        {
            var json = JsonConvert.SerializeObject(spans);
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(this.url);
            req.Timeout = config.ConnectTimeout;
            req.ReadWriteTimeout = config.ReadWriteTimeout;
            req.Method = "POST";
            req.ContentType = "application/json";
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(json);
            req.ContentLength = buffer.Length;

            try
            {
                using (var reqStream = req.GetRequestStream())
                {
                    reqStream.Write(buffer, 0, buffer.Length);
                }
                using (req.GetResponse())
                {
                    return true;
                }
            }
            catch (WebException e)
            {
                LogHttpErrorMessage(e, spans);
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Send spans failed. {0} spans are lost! Exception message: {1}.", spans.Count, e.Message);
            }
            finally
            {
                if (req != null)
                {
                    req.Abort();
                }
            }
            return false;
        }

        private void LogHttpErrorMessage(WebException e, List<JsonSpan> spans)
        {
            var response = e.Response as HttpWebResponse;
            if (response == null)
            {
                logger.Error("Send spans failed. " + spans.Count + " spans are lost!", e);
            }
            else
            {
                using (Stream receiveStream = response.GetResponseStream())
                using (StreamReader readStream = new StreamReader(receiveStream, System.Text.Encoding.UTF8))
                {
                    logger.ErrorFormat("Failed to send spans to zipkin server (HTTP status code returned: {0}). {1} spans are lost! Exception message: {2}, response from server: {3}",
                    response.StatusCode, spans.Count, e.Message, readStream.ReadToEnd());
                }
                response.Close();
            }
        }
    }
}
