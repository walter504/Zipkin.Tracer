using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tracing.Tracer.Core
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
        public String toString()
        {
            return "[trace id: " + TraceId + ", span id: " + Id + ", parent span id: "
                   + ParentSpanId + "]";
        }
    }
}
