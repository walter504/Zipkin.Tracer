using System;

namespace Zipkin.Tracer.Core
{
    public interface ICommonSpanState
    {
        bool? Sample { get; }
    }
}
