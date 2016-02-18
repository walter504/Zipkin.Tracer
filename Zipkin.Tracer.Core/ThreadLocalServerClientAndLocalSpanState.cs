using System;
using System.Threading;

namespace Zipkin.Tracer.Core
{
    public sealed class ThreadLocalServerClientAndLocalSpanState : IServerClientAndLocalSpanState
    {
        private readonly static ThreadLocal<ServerSpan> currentServerSpan = new ThreadLocal<ServerSpan>(() => new ServerSpan());
        private readonly static ThreadLocal<Span> currentClientSpan = new ThreadLocal<Span>();
        private readonly static ThreadLocal<string> currentClientServiceName = new ThreadLocal<string>();
        private readonly static ThreadLocal<Span> currentLocalSpan = new ThreadLocal<Span>();
        private readonly Endpoint endpoint;

        public ThreadLocalServerClientAndLocalSpanState(int ip, int port, string serviceName)
        {
            Ensure.ArgumentNotNull(serviceName, "Service name must be specified.");
            endpoint = new Endpoint() { Ipv4 = ip, Port = (short)port, Service_name = serviceName };
        }

        public ServerSpan CurrentServerSpan
        {
            get
            {
                return currentServerSpan.Value;
            }
            set
            {
                currentServerSpan.Value = value;
            }
        }

        public Endpoint ServerEndpoint
        {
            get
            {
                return endpoint;
            }
        }


        public Endpoint ClientEndpoint
        {
            get
            {
                if (currentClientServiceName.Value == null)
                {
                    return endpoint;
                }
                return new Endpoint() { Ipv4 = endpoint.Ipv4, Port = endpoint.Port, Service_name = currentClientServiceName.Value };
            }
        }
        public Span CurrentClientSpan
        {
            get
            {
                return currentClientSpan.Value;
            }
            set
            {
                currentClientSpan.Value = value;
            }
        }
        public string CurrentClientServiceName
        {
            set
            {
                currentClientServiceName.Value = value;
            }
        }

        public bool? Sample
        {
            get
            {
                return currentServerSpan.Value.Sample;
            }
        }

        public Span CurrentLocalSpan
        {
            get
            {
                return currentLocalSpan.Value;
            }
            set
            {
                currentLocalSpan.Value = value;
            }
        }
    }
}
