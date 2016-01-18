using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tracing.Tracer.Core
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
            TraceData traceData = adapter.TraceData;

            bool sample = traceData.Sample;
            if (sample != null && !sample)
            {
                serverTracer.SetStateNoTracing();
            }
            else
            {
                if (traceData.SpanId != null)
                {
                    SpanId spanId = traceData.SpanId;
                    serverTracer.SetStateCurrentTrace(spanId.TraceId, spanId.Id,
                            spanId.ParentSpanId, adapter.SpanName);
                }
                else
                {
                    serverTracer.SetStateUnknown(adapter.SpanName);
                }
                serverTracer.SetServerReceived();
                foreach (KeyValueAnnotation annotation in adapter.RequestAnnotations)
                {
                    serverTracer.SubmitBinaryAnnotation(annotation.Key, annotation.Value);
                }
            }
        }
    }
}
