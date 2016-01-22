using System;
using System.Threading;

namespace Zipkin.Tracer.Core
{
    public sealed class ThreadLocalServerClientAndLocalSpanState : IServerClientAndLocalSpanState
    {
        private readonly static ThreadLocal<ServerSpan> currentServerSpan = new ThreadLocal<ServerSpan>() { Value = new ServerSpan() };
        private readonly static ThreadLocal<Span> currentClientSpan = new ThreadLocal<Span>();
        private readonly static ThreadLocal<String> currentClientServiceName = new ThreadLocal<String>();
        private readonly static ThreadLocal<Span> currentLocalSpan = new ThreadLocal<Span>();
        private readonly Endpoint endpoint;

        public ThreadLocalServerClientAndLocalSpanState(int ip, int port, String serviceName)
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
                    endpoint.Service_name = currentClientServiceName.Value;
                }
                return endpoint;
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
