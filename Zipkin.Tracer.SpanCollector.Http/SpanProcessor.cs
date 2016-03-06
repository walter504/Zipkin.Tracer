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
        //send contents of queue if it has pending items but less than max batch size after doing max number of polls
        private const int MAX_NUMBER_OF_POLLS = 5;
        private const int MAX_SUBSEQUENT_EMPTY_BATCHES = 2;
        private int connectTimeout = 30 * 1000;
        private int readTimeout = 60 * 1000;
        private volatile bool stop = false;
        private int processedSpansNum = 0;
        private int subsequentPollCount;
        private int maxBatchSize;
        private readonly string url;
        private readonly ILog logger;
        private readonly SpanProcessorTaskFactory taskFactory;
        private readonly BlockingCollection<Span> queue;
        //using a queue because even as we pop items to send to zipkin, another 
        //thread can be adding spans if someone shares the span processor accross threads
        private readonly ConcurrentQueue<SerializableSpan> serializableSpans = new ConcurrentQueue<SerializableSpan>();

        public SpanProcessor(string baseUrl, BlockingCollection<Span> queue, int maxBatchSize, ILog logger)
        {
            if (queue == null)
            {
                throw new ArgumentNullException("queue");
            }
            if (baseUrl == null)
            {
                throw new ArgumentNullException("baseUrl");
            }

            this.url = baseUrl + (baseUrl.EndsWith("/") ? "" : "/") + "api/v1/spans";
            this.queue = queue;
            this.maxBatchSize = maxBatchSize;
            this.logger = logger;
            taskFactory = new SpanProcessorTaskFactory(logger);
        }

        public int Execute()
        {
            int subsequentEmptyBatches = 0;
            do
            {
                try
                {
                    Span span;
                    if (queue.TryTake(out span))
                    {

                    }
                    else
                    {
                        subsequentEmptyBatches++;
                    }

                    if (subsequentEmptyBatches >= MAX_SUBSEQUENT_EMPTY_BATCHES)
                    {
                        subsequentEmptyBatches = 0;
                    }
                }
                catch (Exception e)
                {
                    logger.Warn("Unexpected exception flushing spans", e);
                }
            } while (stop);
            return processedSpansNum;
        }

        public void Stop()
        {
            taskFactory.StopTask();
            LogSubmittedSpans();
        }

        public void Start()
        {
            taskFactory.CreateAndStart(LogSubmittedSpans);
        }

        private void LogSubmittedSpans()
        {
            var anyNewSpans = ProcessQueuedSpans();

            if (anyNewSpans) subsequentPollCount = 0;
            else if (serializableSpans.Count > 0) subsequentPollCount++;

            if (ShouldSendQueuedSpansOverWire())
            {
                SendSpansOverHttp();
            }
        }

        private void PostSpans(string spans)
        {
            if (spans == null) throw new ArgumentNullException("spans");
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(new Uri(this.url));
            req.Timeout = connectTimeout;
            req.ReadWriteTimeout = readTimeout;
            req.Method = "POST";
            req.ContentType = "application/json";
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(spans);
            req.ContentLength = buffer.Length;

            try
            {
                using (var reqStream = req.GetRequestStream())
                {
                    reqStream.Write(buffer, 0, buffer.Length);
                }
                req.GetResponse();
            }
            catch (WebException ex)
            {
                LogHttpErrorMessage(ex);
                throw ex;
            }
        }

        private bool ShouldSendQueuedSpansOverWire()
        {
            return serializableSpans.Any() &&
                   (serializableSpans.Count() >= maxBatchSize
                   || taskFactory.IsTaskCancelled()
                   || subsequentPollCount > MAX_NUMBER_OF_POLLS);
        }

        private bool ProcessQueuedSpans()
        {
            Span span;
            var anyNewSpansQueued = false;
            while (queue.TryTake(out span))
            {
                serializableSpans.Enqueue(new SerializableSpan(span));
                anyNewSpansQueued = true;
            }
            return anyNewSpansQueued;
        }

        private void SendSpansOverHttp()
        {
            var spansJsonRepresentation = GetSpansJSONRepresentation();
            PostSpans(spansJsonRepresentation);
            subsequentPollCount = 0;
        }

        private string GetSpansJSONRepresentation()
        {
            SerializableSpan span;
            var spanList = new List<SerializableSpan>();
            //using Dequeue into a list so that the span is removed from the queue as we add it to list
            while (serializableSpans.TryDequeue(out span))
            {
                spanList.Add(span);
            }
            var spansJsonRepresentation = JsonConvert.SerializeObject(spanList);
            return spansJsonRepresentation;
        }

        private void LogHttpErrorMessage(WebException ex)
        {
            var response = ex.Response as HttpWebResponse;
            if (response == null) return;
            string content = string.Empty;
            var receiveStream = response.GetResponseStream();
            if (receiveStream != null)
            {
                using(StreamReader readStream = new StreamReader(receiveStream, System.Text.Encoding.UTF8))
                {
                    content = readStream.ReadToEnd();
                }
            }
            response.Close();
            logger.ErrorFormat("Failed to send spans to Zipkin server (HTTP status code returned: {0}). Exception message: {1}, response from server: {2}",
                response.StatusCode, ex.Message, content);
        }
    }
}
