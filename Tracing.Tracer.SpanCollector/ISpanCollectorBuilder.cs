using System;
using log4net;

namespace Tracing.Tracer.SpanCollector
{
    public interface ISpanCollectorBuilder
    {
        SpanCollector Build(Uri uri, int maxProcessorBatchSize, ILog logger);
    }
}
