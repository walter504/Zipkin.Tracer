using System;

namespace Tracing.Tracer.Core
{
    public class ClientResponseInterceptor
    {
        private ClientTracer clientTracer;

        public ClientResponseInterceptor(ClientTracer clientTracer)
        {
            this.clientTracer = clientTracer;
        }

        /**
         * Handle a client response.
         *
         * @param adapter Adapter that hides implementation details.
         */
        public void Handle(IClientResponseAdapter adapter)
        {
            try
            {
                foreach (KeyValueAnnotation annotation in adapter.ResponseAnnotations)
                {
                    clientTracer.SubmitBinaryAnnotation(annotation.Key, annotation.Value);
                }
            }
            finally
            {
                clientTracer.SetClientReceived();
            }
        }
    }
}
