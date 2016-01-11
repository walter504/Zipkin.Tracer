using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tracing.Tracer.Core
{
    public interface ISpanAndEndpoint
    {
        Span Span { get; }
        Endpoint Endpoint { get; }
    }

    public class StaticSpanAndEndpoint : ISpanAndEndpoint
    {
        public Span Span { get; set; }
        public Endpoint Endpoint { get; set; }
        public StaticSpanAndEndpoint(Span span, Endpoint endpoint) 
        {
            Span = span;
            Endpoint = endpoint;
        }
    }

    public class ServerSpanAndEndpoint : ISpanAndEndpoint
    {
        public IServerSpanState State { get; set; }

        public ServerSpanAndEndpoint(IServerSpanState state)
        {
            State = state;
        }

        public Span Span
        {
            get
            {
                return State.CurrentServerSpan.Span;
            }
        }

        public Endpoint Endpoint
        {
            get
            {
                return State.ServerEndpoint;
            }
        }
    }

    public class ClientSpanAndEndpoint : ISpanAndEndpoint
    {
        public IServerClientAndLocalSpanState State { get; set; }

        public ClientSpanAndEndpoint(IServerClientAndLocalSpanState state)
        {
            State = state;
        }
        public Span Span
        {
            get
            {
                return State.CurrentClientSpan;
            }
        }
        public Endpoint Endpoint
        {
            get
            {
                return State.ClientEndpoint;
            }
        }
    }

    public class LocalSpanAndEndpoint : ISpanAndEndpoint
    {
        public IServerClientAndLocalSpanState State { get; set; }

        public LocalSpanAndEndpoint(IServerClientAndLocalSpanState state)
        {
            State = state;
        }

        public Span Span
        {
            get
            {
                return State.CurrentLocalSpan;
            }
        }

        /** The local endpoint is the same as the client endpoint. */
        public Endpoint Endpoint
        {
            get
            {
                return State.ClientEndpoint;
            }
        }
    }
}
