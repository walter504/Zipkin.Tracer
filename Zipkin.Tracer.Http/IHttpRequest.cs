using System;

namespace Zipkin.Tracer.Http
{
    public interface IHttpRequest
    {
        /**
         * Get request URI.
         *
         * @return Request URI.
         */
        Uri GetUri();

        /**
         * Returns the http method for request (GET, PUT, POST,...)
         *
         * @return Http Method for request.
         */
        string GetHttpMethod();
    }
}
