using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tracing.Tracer.Core
{
    public class LocalTracer : AnnotationSubmitter
    {
        public LocalSpanAndEndpoint SpanAndEndpoint { get; set; }
        ISpanCollector spanCollector { get; set; }
        Sampler traceSampler { get; set; }

        public LocalTracer()
        {
        }

        public LocalTracer(LocalSpanAndEndpoint spanAndEndpoint)
        {
            SpanAndEndpoint = spanAndEndpoint;
        }

        /**
         * Request a new local span, which starts now.
         *
         * @param component {@link zipkinCoreConstants#LOCAL_COMPONENT component} responsible for the operation
         * @param operation name of the operation that's begun
         * @return metadata about the new span or null if one wasn't started due to sampling policy.
         * @see zipkinCoreConstants#LOCAL_COMPONENT
         */
        public SpanId StartNewSpan(String component, String operation)
        {
            SpanId spanId = StartNewSpan(component, operation, Util.GetCurrentTimeStamp());
            if (spanId == null) return null;
            SpanAndEndpoint.Span.startTick = DateTime.Now.Ticks; // embezzle start tick into an internal field.
            return spanId;
        }

        private SpanId GetNewSpanId()
        {
            Span currentServerSpan = SpanAndEndpoint.State.CurrentServerSpan.Span;
            long newSpanId = Util.GetRandomId();
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

            var startTick = span.startTick;
            long duration;
            if (startTick.HasValue)
            {
                duration = (endTick - startTick.Value) / 1000;
            }
            else
            {
                duration = Util.GetCurrentTimeStamp() - span.Timestamp;
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
