using System;

namespace Zipkin.Tracer.Core
{
    public class ServerSpan
    {
        public static readonly ServerSpan EMPTY = new ServerSpan();
        public static readonly ServerSpan NOT_SAMPLED = new ServerSpan(false);

        public Span Span { get; private set; }
        public bool? Sample { get; private set; }

        public ServerSpan()
            :this(null)
        {
        }

        public ServerSpan(bool? sample)
            : this(null, sample)
        {
        }

        public ServerSpan(Span span, bool? sample)
        {
            Span = span;
            Sample = sample;
        }

        public ServerSpan(long traceId, long spanId, long? parentSpanId, string name) 
        {
            Span span = new Span()
            {
                Trace_id = traceId,
                Id = spanId,
                Name = name
            };
            if (parentSpanId != null) 
            {
                span.Parent_id = parentSpanId.Value;
            }
            this.Span = span;
            Sample = true;
        }
    }
}
