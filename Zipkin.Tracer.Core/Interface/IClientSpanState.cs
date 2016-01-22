using System;
using System.Collections.Generic;

namespace Zipkin.Tracer.Core
{
    public interface IClientSpanState : ICommonSpanState
    {
        Span CurrentClientSpan { get; set; }

        Endpoint ClientEndpoint { get; }
        string CurrentClientServiceName { set; }
    }
}
