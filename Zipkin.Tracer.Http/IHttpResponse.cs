using System;

namespace Zipkin.Tracer.Http
{
    public interface IHttpResponse
    {
        int GetHttpStatusCode();
    }
}
