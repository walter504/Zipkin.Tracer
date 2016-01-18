using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tracing.Tracer.Core
{
    public class TraceData
    {
        public SpanId SpanId { get; set; }
        public bool? Sample { get; set; }
    }
}
