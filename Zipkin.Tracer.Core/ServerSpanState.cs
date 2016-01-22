using System;

namespace Zipkin.Tracer.Core
{
    public interface IServerSpanState : ICommonSpanState
    {
        ServerSpan CurrentServerSpan { get; set; }
        Endpoint ServerEndpoint { get; }
    }
}
