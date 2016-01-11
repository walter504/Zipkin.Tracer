using System;
using System.Collections.ObjectModel;

namespace Tracing.Tracer.Core
{
    public interface IServerRequestAdapter
    {
        TraceData TraceData { get; set; }
        string SpanName { get; set; }

        Collection<KeyValueAnnotation> RequestAnnotations { get; set; }
    }
}
