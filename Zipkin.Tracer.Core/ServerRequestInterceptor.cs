using System;

namespace Zipkin.Tracer.Core
{
    public class ServerRequestInterceptor
    {
        private ServerTracer serverTracer;

        public ServerRequestInterceptor(ServerTracer serverTracer)
        {
            this.serverTracer = Ensure.ArgumentNotNull(serverTracer, "Null serverTracer");
        }

        public void Handle(IServerRequestAdapter adapter)
        {
            serverTracer.ClearCurrentSpan();
            TraceData traceData = adapter.GetTraceData();

            if (traceData.Sample.HasValue && !traceData.Sample.Value)
            {
                serverTracer.SetStateNoTracing();
            }
            else
            {
                if (traceData.SpanId != null)
                {
                    SpanId spanId = traceData.SpanId;
                    serverTracer.SetStateCurrentTrace(spanId.TraceId, spanId.Id,
                            spanId.ParentSpanId, adapter.GetSpanName());
                }
                else
                {
                    serverTracer.SetStateUnknown(adapter.GetSpanName());
                }
                serverTracer.SetServerReceived();
                foreach (KeyValueAnnotation annotation in adapter.RequestAnnotations())
                {
                    serverTracer.SubmitBinaryAnnotation(annotation.Key, annotation.Value);
                }
            }
        }
    }
}
