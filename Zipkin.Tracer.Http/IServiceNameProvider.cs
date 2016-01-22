using System;

namespace Zipkin.Tracer.Http
{
    public interface IServiceNameProvider
    {
        string ServiceName(IHttpRequest request);
    }
}
