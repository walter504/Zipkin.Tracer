using System;

namespace Zipkin.Tracer.Http
{
    public static class ZipkinHttpHeaders
    {
        public const string TraceId = "X-B3-TraceId";
        public const string SpanId = "X-B3-SpanId";
        public const string ParentSpanId = "X-B3-ParentSpanId";
        public const string Sampled = "X-B3-Sampled";
    }
}
