using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tracing.Tracer.Core
{
    public interface IClientSpanState : ICommonSpanState
    {
        Span CurrentClientSpan { get; set; }

        Endpoint ClientEndpoint { get; }
        string CurrentClientServiceName { set; }
    }
}
