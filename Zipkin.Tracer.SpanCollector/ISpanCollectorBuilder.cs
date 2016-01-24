using System;
using log4net;
using Zipkin.Tracer.Core;

namespace Zipkin.Tracer.SpanCollector
{
    public interface ISpanCollectorBuilder
    {
        HttpSpanCollector Build(Uri uri, int maxProcessorBatchSize, ILog logger);
    }
}
