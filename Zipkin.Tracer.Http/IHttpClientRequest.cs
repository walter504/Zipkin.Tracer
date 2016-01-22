using System;

namespace Zipkin.Tracer.Http
{
    public interface IHttpClientRequest : IHttpRequest
    {
        /**
         * Adds headers to request.
         *
         * @param header header name.
         * @param value header value.
         */
        void AddHeader(string header, string value);
    }
}
