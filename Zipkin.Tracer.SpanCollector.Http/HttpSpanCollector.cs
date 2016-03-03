using log4net;
using System;
using System.Collections.Concurrent;
using Zipkin.Tracer.Core;

namespace Zipkin.Tracer.SpanCollector.Http
{
    public class HttpSpanCollector : ISpanCollector, IDisposable
    {
        private readonly BlockingCollection<Span> pending = new BlockingCollection<Span>(1000);

        private SpanProcessor spanProcessor;

        public HttpSpanCollector(string baseUrl, int maxProcessorBatchSize, ILog logger)
        {
            spanProcessor = new SpanProcessor(baseUrl, pending, maxProcessorBatchSize, logger);
            spanProcessor.Start();
        }

        public void Collect(Span span)
        {
            pending.Add(span);
        }

        public void Dispose()
        {
            spanProcessor.Stop();
        }
    }
}
