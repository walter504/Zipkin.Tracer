using System;

namespace Zipkin.Tracer.Http
{
    public interface ISpanNameProvider
    {
        string SpanName(IHttpRequest request);
    }
}
