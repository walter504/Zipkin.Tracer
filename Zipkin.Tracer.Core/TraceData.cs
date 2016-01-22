using System;

namespace Zipkin.Tracer.Core
{
    public class TraceData
    {
        public SpanId SpanId { get; set; }
        public bool? Sample { get; set; }
    }
}
