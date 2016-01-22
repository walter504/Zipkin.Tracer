using System;

namespace Zipkin.Tracer.Core
{
    public interface ILocalSpanState : ICommonSpanState 
    {
        Span CurrentLocalSpan { get; set; }
    }
}
