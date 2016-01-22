using System;

namespace Zipkin.Tracer.Http
{
    public interface IHttpServerRequest : IHttpRequest
    {
        /**
         * Get http header value.
         *
         * @param headerName
         * @return
         */
        string GetHttpHeaderValue(string headerName);
    }
}
