using System;

namespace Zipkin.Tracer.Core
{
    public class ServerTracer : AnnotationSubmitter
    {
        private ServerSpanAndEndpoint SpanAndEndpoint { get; set; }
        private ISpanCollector spanCollector { get; set; }
        private Sampler traceSampler { get; set; }

        public ServerTracer(IServerSpanState state, ISpanCollector spanCollector, Sampler traceSampler)
            : this(new ServerSpanAndEndpoint(state), spanCollector, traceSampler)
        {
        }

        ServerTracer(ServerSpanAndEndpoint spanAndEndpoint, ISpanCollector spanCollector, Sampler traceSampler)
        {
            base.SpanAndEndpoint = SpanAndEndpoint = spanAndEndpoint;
            this.spanCollector = spanCollector;
            this.traceSampler = traceSampler;
        }

        public void ClearCurrentSpan()
        {
            SpanAndEndpoint.State.CurrentServerSpan = null;
        }

        public void SetStateCurrentTrace(long traceId, long spanId, long? parentSpanId, string name) 
        {
            Ensure.ArgumentNotNullOrEmptyString(name, "Null or blank span name");
            SpanAndEndpoint.State.CurrentServerSpan = new ServerSpan(traceId, spanId, parentSpanId, name);
        }

        public void SetStateNoTracing()
        {
            SpanAndEndpoint.State.CurrentServerSpan = ServerSpan.NOT_SAMPLED;
        }

        public void SetStateUnknown(string spanName)
        {
            Ensure.ArgumentNotNullOrEmptyString(spanName, "Null or blank span name");
            long newTraceId = Util.GetRandomId();
            if (!traceSampler.IsSampled(newTraceId))
            {
                SpanAndEndpoint.State.CurrentServerSpan = ServerSpan.NOT_SAMPLED;
                return;
            }
            SpanAndEndpoint.State.CurrentServerSpan = new ServerSpan(newTraceId, newTraceId, null, spanName);
        }

        public void SetServerReceived()
        {
            SubmitStartAnnotation(zipkinCoreConstants.SERVER_RECV);
        }

        public void SetServerReceived(int ipv4, int port, string clientService) 
        {
            SubmitAddress(zipkinCoreConstants.CLIENT_ADDR, ipv4, port, clientService);
            SubmitStartAnnotation(zipkinCoreConstants.SERVER_RECV);
        }

        public void SetServerSend() 
        {
            if (SubmitEndAnnotation(zipkinCoreConstants.SERVER_SEND, spanCollector))
            {
                SpanAndEndpoint.State.CurrentServerSpan = null;
            }
        }
    }
}
