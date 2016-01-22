using System;

namespace Zipkin.Tracer.Core
{
    public class ClientTracer : AnnotationSubmitter
    {
        public ClientSpanAndEndpoint SpanAndEndpoint { get; set; }
        Random randomGenerator { get; set; }
        ISpanCollector spanCollector { get; set; }
        Sampler traceSampler { get; set; }

        public ClientTracer(IServerClientAndLocalSpanState state)
            :this(new ClientSpanAndEndpoint(state))
        {
        }

        public ClientTracer(ClientSpanAndEndpoint spanAndEndpoint)
        {
            SpanAndEndpoint = spanAndEndpoint;
        }

        public void SetClientSent()
        {
            SubmitStartAnnotation(zipkinCoreConstants.CLIENT_SEND);
        }

        public void SetClientSent(int ipv4, int port, string serviceName) {
            SubmitAddress(zipkinCoreConstants.SERVER_ADDR, ipv4, port, serviceName);
            SubmitStartAnnotation(zipkinCoreConstants.CLIENT_SEND);
        }

        public void SetClientReceived()
        {
            if (SubmitEndAnnotation(zipkinCoreConstants.CLIENT_RECV, spanCollector))
            {
                SpanAndEndpoint.State.CurrentClientSpan = null;
                SpanAndEndpoint.State.CurrentClientServiceName = null;
            }
        }

        public SpanId StartNewSpan(String requestName)
        {
            bool? sample = SpanAndEndpoint.State.Sample;
            if (sample.HasValue && !sample.Value)
            {
                SpanAndEndpoint.State.CurrentClientSpan = null;
                SpanAndEndpoint.State.CurrentClientServiceName = null;
                return null;
            }

            SpanId newSpanId = GetNewSpanId();
            if (!sample.HasValue)
            {
                // No sample indication is present.
                if (!traceSampler.IsSampled(newSpanId.TraceId))
                {
                    SpanAndEndpoint.State.CurrentClientSpan = null;
                    SpanAndEndpoint.State.CurrentClientServiceName = null;
                    return null;
                }
            }

            Span newSpan = new Span();
            newSpan.Id = newSpanId.Id;
            newSpan.Trace_id = newSpanId.TraceId;
            if (newSpanId.ParentSpanId != null)
            {
                newSpan.Parent_id = newSpanId.ParentSpanId.Value;
            }
            newSpan.Name = requestName;
            SpanAndEndpoint.State.CurrentClientSpan = newSpan;
            return newSpanId;
        }

        public void SetCurrentClientServiceName(String serviceName)
        {
            SpanAndEndpoint.State.CurrentClientServiceName = serviceName;
        }

        private SpanId GetNewSpanId()
        {
            Span parentSpan = SpanAndEndpoint.State.CurrentLocalSpan;
            if (parentSpan == null)
            {
                ServerSpan serverSpan = SpanAndEndpoint.State.CurrentServerSpan;
                if (serverSpan != null)
                {
                    parentSpan = serverSpan.Span;
                }
            }
            long newSpanId = Util.GetRandomId();
            if (parentSpan == null)
            {
                return new SpanId(newSpanId, newSpanId, null);
            }

            return new SpanId(parentSpan.Trace_id, newSpanId, parentSpan.Id);
        }
    }
}
