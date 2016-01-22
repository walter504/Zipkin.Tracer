using System;

namespace Zipkin.Tracer.Http
{
    public class DefaultSpanNameProvider : ISpanNameProvider
    {
        public string SpanName(IHttpRequest request)
        {
            return request.GetHttpMethod();
        }
    }
}
