using log4net;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zipkin.Tracer.Core;

namespace Zipkin.Tracer.SpanCollector.Http
{
    public class HttpSpanCollector : ISpanCollector, IDisposable
    {
        private readonly BlockingCollection<Span> pending = new BlockingCollection<Span>(1000);
        private SpanProcessor spanProcessor;
        private ILog logger;
        private List<Task<int>> tasks = new List<Task<int>>();

        public HttpSpanCollector(string baseUrl, int maxProcessorBatchSize, ILog logger)
        {
            this.logger = logger;
            spanProcessor = new SpanProcessor(baseUrl, pending, maxProcessorBatchSize, logger);
            
        }

        public void Collect(Span span)
        {
            long start = DateTime.Now.Ticks;
            if (pending.TryAdd(span))
            {
                long end = DateTime.Now.Ticks;
                if (logger.IsInfoEnabled)
                {
                    logger.Info("Adding span to queue took " + (end - start) / 10000.0 + "ms.");
                }
            }
            else
            {
                logger.Warn("Queue rejected Span, span not submitted: " + span);
            }
        }

        public void Dispose()
        {
            spanProcessor.Stop();
        }
    }
}
