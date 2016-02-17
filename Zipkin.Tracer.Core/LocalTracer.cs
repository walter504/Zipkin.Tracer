using System;

namespace Zipkin.Tracer.Core
{
    public class LocalTracer : AnnotationSubmitter
    {
        LocalSpanAndEndpoint SpanAndEndpoint { get; set; }
        ISpanCollector spanCollector { get; set; }
        Sampler traceSampler { get; set; }

        public LocalTracer(IServerClientAndLocalSpanState state, ISpanCollector spanCollector, Sampler traceSampler)
            : this(new LocalSpanAndEndpoint(state), spanCollector, traceSampler)
        {
        }

        LocalTracer(LocalSpanAndEndpoint spanAndEndpoint, ISpanCollector spanCollector, Sampler traceSampler)
        {
            base.SpanAndEndpoint = SpanAndEndpoint = spanAndEndpoint;
            this.spanCollector = spanCollector;
            this.traceSampler = traceSampler;
        }

        /**
         * Request a new local span, which starts now.
         *
         * @param component {@link zipkinCoreConstants#LOCAL_COMPONENT component} responsible for the operation
         * @param operation name of the operation that's begun
         * @return metadata about the new span or null if one wasn't started due to sampling policy.
         * @see zipkinCoreConstants#LOCAL_COMPONENT
         */
        public SpanId StartNewSpan(string component, string operation)
        {
            SpanId spanId = StartNewSpan(component, operation, Util.CurrentTimeMicroseconds());
            if (spanId == null) return null;
            SpanAndEndpoint.Span.startTick = DateTime.Now.Ticks; // embezzle start tick into an internal field.
            return spanId;
        }

        private SpanId GetNewSpanId()
        {
            Span currentServerSpan = SpanAndEndpoint.State.CurrentServerSpan.Span;
            long newSpanId = Util.NextLong();
            if (currentServerSpan == null)
            {
                return new SpanId(newSpanId, newSpanId, null);
            }
            return new SpanId(currentServerSpan.Trace_id, newSpanId, currentServerSpan.Id);
        }

        /**
         * Request a new local span, which started at the given timestamp.
         *
         * @param component {@link zipkinCoreConstants#LOCAL_COMPONENT component} responsible for the operation
         * @param operation name of the operation that's begun
         * @param timestamp time the operation started, in epoch microseconds.
         * @return metadata about the new span or null if one wasn't started due to sampling policy.
         * @see zipkinCoreConstants#LOCAL_COMPONENT
         */
        public SpanId StartNewSpan(string component, string operation, long timestamp)
        {

            bool? sample = SpanAndEndpoint.State.Sample;
            if (sample.HasValue || !sample.Value)
            {
                SpanAndEndpoint.State.CurrentLocalSpan = null;
                return null;
            }

            SpanId newSpanId = GetNewSpanId();
            if (sample == null)
            {
                // No sample indication is present.
                if (!traceSampler.IsSampled(newSpanId.TraceId))
                {
                    SpanAndEndpoint.State.CurrentLocalSpan = null;
                    return null;
                }
            }

            Span newSpan = new Span();
            newSpan.Id = newSpanId.Id;
            newSpan.Trace_id = newSpanId.TraceId;
            if (newSpanId.ParentSpanId.HasValue)
            {
                newSpan.Parent_id = newSpanId.ParentSpanId.Value;
            }
            newSpan.Name = operation;
            newSpan.Timestamp = timestamp;
            newSpan.Binary_annotations.Add(
                new BinaryAnnotation()
                {
                    Key = zipkinCoreConstants.LOCAL_COMPONENT,
                    Value = component,
                    Host = SpanAndEndpoint.Endpoint
                });
            SpanAndEndpoint.State.CurrentLocalSpan = newSpan;
            return newSpanId;
        }

        /**
         * Completes the span, assigning the most precise duration possible.
         */
        public void FinishSpan()
        {
            long endTick = DateTime.Now.Ticks;

            Span span = SpanAndEndpoint.Span;
            if (span == null) return;

            long duration;
            if (span.startTick.HasValue)
            {
                duration = (endTick - span.startTick.Value) / 10;
            }
            else
            {
                duration = Util.CurrentTimeMicroseconds() - span.Timestamp;
            }
            FinishSpan(duration);
        }

        /**
         * Completes the span, which took {@code duration} microseconds.
         */
        public void FinishSpan(long duration)
        {
            Span span = SpanAndEndpoint.Span;
            if (span == null) return;
            lock (span)
            {
                span.Duration = duration;
                spanCollector.Collect(span);
            }
            SpanAndEndpoint.State.CurrentLocalSpan = null;
        }
    }
}
