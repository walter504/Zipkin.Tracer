using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tracing.Tracer.Core
{
    public class ServerTracer : AnnotationSubmitter
    {
        public ServerSpanAndEndpoint SpanAndEndpoint { get; set; }
        Random randomGenerator { get; set; }
        ISpanCollector spanCollector { get; set; }
        ITraceSampler traceSampler { get; set; }

        public ServerTracer(IServerSpanState state)
            : this(new ServerSpanAndEndpoint(state))
        {
        }

        public ServerTracer(ServerSpanAndEndpoint spanAndEndpoint)
        {
            SpanAndEndpoint = spanAndEndpoint;
        }

        public void ClearCurrentSpan()
        {
            SpanAndEndpoint.State.CurrentServerSpan = null;
        }

        public void SetStateCurrentTrace(long traceId, long spanId, long? parentSpanId, string name) {
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
            long newTraceId = GetRandomId();
            if (!traceSampler.Test(newTraceId))
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

        public void setServerReceived(int ipv4, int port, string clientService) {
            SubmitAddress(zipkinCoreConstants.CLIENT_ADDR, ipv4, port, clientService);
            SubmitStartAnnotation(zipkinCoreConstants.SERVER_RECV);
        }

        public void SetServerSend() {
            if (SubmitEndAnnotation(zipkinCoreConstants.SERVER_SEND, spanCollector))
            {
                SpanAndEndpoint.State.CurrentServerSpan = null;
            }
        }

        private long GetRandomId()
        {
            byte[] buffer = new byte[8];
            randomGenerator.NextBytes(buffer);
            return BitConverter.ToInt64(buffer, 0);
        }
    }
}
