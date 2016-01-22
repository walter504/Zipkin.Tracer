using System;

namespace Zipkin.Tracer.Core
{
    public interface ISpanCollector
    {
        void Collect(Span span);
    }
}
