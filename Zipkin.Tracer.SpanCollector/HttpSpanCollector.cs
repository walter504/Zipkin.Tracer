using log4net;
using System;
using System.Collections.Concurrent;
using Zipkin.Tracer.Core;

namespace Zipkin.Tracer.SpanCollector
{
    public class HttpSpanCollector : ISpanCollector
    {
        private const int MAX_QUEUE_SIZE = 100;
        internal static BlockingCollection<Span> spanQueue;

        internal SpanProcessor spanProcessor;

        private static HttpSpanCollector instance;

        public static HttpSpanCollector GetInstance(Uri uri, int maxProcessorBatchSize, ILog logger)
        {
            if (instance == null)
            {
                instance = new HttpSpanCollector(uri, maxProcessorBatchSize, logger);
                instance.Start();
            }
            return instance;
        }

        public HttpSpanCollector(Uri uri, int maxProcessorBatchSize, ILog logger)
        {
            if ( spanQueue == null)
            {
                spanQueue = new BlockingCollection<Span>(MAX_QUEUE_SIZE);
            }

            spanProcessor = new SpanProcessor(uri, spanQueue, maxProcessorBatchSize, logger);
        }

        public virtual void Collect(Span span)
        {
            spanQueue.Add(span);
        }

        public virtual void Start()
        {
            spanProcessor.Start();
        }

        public virtual void Stop()
        {
            spanProcessor.Stop();
        }
    }
}
