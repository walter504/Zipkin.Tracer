using System;
using System.Collections.Generic;
using System.Linq;
using Zipkin.Tracer.Core;

namespace Zipkin.Tracer.Http
{
    public class HttpClientResponseAdapter : IClientResponseAdapter
    {
        private readonly IHttpResponse response;

        public HttpClientResponseAdapter(IHttpResponse response)
        {
            this.response = response;
        }

        public IEnumerable<KeyValueAnnotation> ResponseAnnotations()
        {
            int httpStatus = response.GetHttpStatusCode();
            if (httpStatus < 200 || httpStatus > 299)
            {
                return new List<KeyValueAnnotation>() { new KeyValueAnnotation("http.responsecode", httpStatus.ToString()) };
            }
            return Enumerable.Empty<KeyValueAnnotation>();
        }
    }
}
