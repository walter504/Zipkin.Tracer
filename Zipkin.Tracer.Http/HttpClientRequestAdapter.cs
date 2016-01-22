using System;
using System.Collections.Generic;
using Zipkin.Tracer.Core;

namespace Zipkin.Tracer.Http
{
    public class HttpClientRequestAdapter : IClientRequestAdapter
    {
        private readonly IHttpClientRequest request;
        private readonly IServiceNameProvider serviceNameProvider;
        private readonly ISpanNameProvider spanNameProvider;

        public HttpClientRequestAdapter(IHttpClientRequest request, IServiceNameProvider serviceNameProvider, ISpanNameProvider spanNameProvider)
        {
            this.request = request;
            this.serviceNameProvider = serviceNameProvider;
            this.spanNameProvider = spanNameProvider;
        }

        public string SpanName
        {
            get
            {
                return spanNameProvider.SpanName(request);
            }
        }

        public string ClientServiceName
        {
            get
            {
                return serviceNameProvider.ServiceName(request);
            }
        }

        public void AddSpanIdToRequest(SpanId spanId)
        {
            if (spanId == null)
            {
                request.AddHeader(ZipkinHttpHeaders.Sampled, "0");
            }
            else
            {
                request.AddHeader(ZipkinHttpHeaders.Sampled, "1");
                request.AddHeader(ZipkinHttpHeaders.TraceId, spanId.TraceId.ToString("x4"));
                request.AddHeader(ZipkinHttpHeaders.SpanId, spanId.Id.ToString("x4"));
                if (spanId.ParentSpanId.HasValue)
                {
                    request.AddHeader(ZipkinHttpHeaders.ParentSpanId, spanId.ParentSpanId.Value.ToString("x4"));
                }
            }
        }

        public IEnumerable<KeyValueAnnotation> RequestAnnotations()
        {
            Uri uri = request.GetUri();
            return new List<KeyValueAnnotation>() { new KeyValueAnnotation("http.uri", uri.ToString()) };
        }
    }
}
