using System;
using System.Collections.ObjectModel;

namespace Tracing.Tracer.Core
{
    public interface IServerResponseAdapter
    {
        Collection<KeyValueAnnotation> ResponseAnnotations { get; set; }
    }
}
