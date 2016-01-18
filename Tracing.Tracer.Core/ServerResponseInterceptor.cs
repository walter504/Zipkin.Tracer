using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tracing.Tracer.Core
{
    public class ServerResponseInterceptor
    {
        private readonly ServerTracer serverTracer;

        public ServerResponseInterceptor(ServerTracer serverTracer)
        {
            this.serverTracer = Ensure.ArgumentNotNull(serverTracer, "Null serverTracer");
        }

        public void Handle(IServerResponseAdapter adapter)
        {
            // We can submit this in any case. When server state is not set or
            // we should not trace this request nothing will happen.
            try
            {
                foreach (KeyValueAnnotation annotation in adapter.ResponseAnnotations)
                {
                    serverTracer.SubmitBinaryAnnotation(annotation.Key, annotation.Value);
                }
                serverTracer.SetServerSend();
            }
            finally
            {
                serverTracer.ClearCurrentSpan();
            }
        }
    }
}
