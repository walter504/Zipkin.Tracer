using System;
using System.Collections.Generic;

namespace Zipkin.Tracer.Core
{
    public interface IServerResponseAdapter
    {
        IEnumerable<KeyValueAnnotation> ResponseAnnotations();
    }
}
