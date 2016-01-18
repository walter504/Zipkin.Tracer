using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tracing.Tracer.Core
{
    public interface IClientRequestAdapter
    {
        string SpanName { get; set; }
        void AddSpanIdToRequest(SpanId spanId);
        string ClientServiceName { get; set; }

        Collection<KeyValueAnnotation> RequestAnnotations { get; set; }
    }
}
