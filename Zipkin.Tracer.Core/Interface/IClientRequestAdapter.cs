using System;
using System.Collections.Generic;

namespace Zipkin.Tracer.Core
{
    public interface IClientRequestAdapter
    {
        string SpanName { get; }
        string ClientServiceName { get; }
        void AddSpanIdToRequest(SpanId spanId);
        IEnumerable<KeyValueAnnotation> RequestAnnotations();
    }
}
