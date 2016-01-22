using System;

namespace Zipkin.Tracer.Core
{
    public class SpanId
    {
        public SpanId(long traceId, long spanId, long? parentSpanId)
        {
            TraceId = traceId;
            Id = spanId;
            ParentSpanId = parentSpanId;
        }
        public long TraceId { get; set; }
        public long Id { get; set; }
        public long? ParentSpanId { get; set; }
        public string ToString()
        {
            return "[trace id: " + TraceId + ", span id: " + Id + ", parent span id: "
                   + ParentSpanId + "]";
        }
    }
}
