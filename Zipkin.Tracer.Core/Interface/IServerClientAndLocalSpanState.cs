using System;

namespace Zipkin.Tracer.Core
{
    public interface IServerClientAndLocalSpanState : IServerSpanState, IClientSpanState, ILocalSpanState
    {
    }
}
