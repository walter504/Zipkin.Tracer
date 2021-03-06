﻿using System;

namespace Zipkin.Tracer.Core
{
    public class ClientRequestInterceptor
    {

        private ClientTracer clientTracer;

        public ClientRequestInterceptor(ClientTracer clientTracer)
        {
            this.clientTracer = clientTracer;
        }

        /**
         * Handles outgoing request.
         *
         * @param adapter The adapter deals with implementation specific details.
         */
        public void Handle(IClientRequestAdapter adapter)
        {
            SpanId spanId = clientTracer.StartNewSpan(adapter.SpanName);
            if (spanId == null)
            {
                // We will not trace this request.
                adapter.AddSpanIdToRequest(null);
            }
            else
            {
                adapter.AddSpanIdToRequest(spanId);
                clientTracer.SetCurrentClientServiceName(adapter.ClientServiceName);
                foreach (KeyValueAnnotation annotation in adapter.RequestAnnotations())
                {
                    clientTracer.SubmitBinaryAnnotation(annotation.Key, annotation.Value);
                }
                clientTracer.SetClientSent();
            }
        }
    }
}
