using System;
using System.Collections.Generic;

namespace Zipkin.Tracer.Core
{
    public interface IServerRequestAdapter
    {
        TraceData GetTraceData();
        string GetSpanName();
        IEnumerable<KeyValueAnnotation> RequestAnnotations();
    }
}
