using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tracing.Tracer.Core
{
    public interface IServerSpanState : ICommonSpanState
    {
        ServerSpan CurrentServerSpan { get; set; }
        Endpoint ServerEndpoint { get; }
    }
}
