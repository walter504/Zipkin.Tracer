using System;
using log4net;

namespace Zipkin.Tracer.SpanCollector
{
    public interface ISpanCollectorBuilder
    {
        SpanCollector Build(Uri uri, int maxProcessorBatchSize, ILog logger);
    }
}
