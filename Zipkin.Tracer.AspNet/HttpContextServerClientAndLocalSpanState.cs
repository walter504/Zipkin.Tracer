using System;
using System.Threading;
using System.Web;
using Zipkin.Tracer.Core;

namespace Zipkin.Tracer.AspNet
{
    public sealed class HttpContextServerClientAndLocalSpanState : IServerClientAndLocalSpanState
    {
        private const string SERVER_SPAN = "Zipkin_State_Current_ServerSpan";
        private const string CLIENT_SPAN = "Zipkin_State_Current_ClientSpan";
        private const string CLIENT_SERVICE_NAME = "Zipkin_State_Current_ClientServiceName";
        private const string LOCAL_SPAN = "Zipkin_State_Current_LocalSpan";

        private readonly static ThreadLocal<ServerSpan> currentServerSpan = new ThreadLocal<ServerSpan>(() => new ServerSpan());
        private readonly static ThreadLocal<Span> currentClientSpan = new ThreadLocal<Span>();
        private readonly static ThreadLocal<string> currentClientServiceName = new ThreadLocal<string>();
        private readonly static ThreadLocal<Span> currentLocalSpan = new ThreadLocal<Span>();
        private readonly Endpoint endpoint;

        public HttpContextServerClientAndLocalSpanState(int ip, int port, string serviceName)
        {
            if (string.IsNullOrEmpty(serviceName))
            {
                throw new ArgumentNullException("Service name must be specified.");
            }
            endpoint = new Endpoint() { Ipv4 = ip, Port = (short)port, Service_name = serviceName };
        }

        public ServerSpan CurrentServerSpan
        {
            get
            {
                return HttpContext.Current == null 
                    ? currentServerSpan.Value
                    : HttpContext.Current.Items[SERVER_SPAN] as ServerSpan;
            }
            set
            {
                if (HttpContext.Current == null)
                {
                    currentServerSpan.Value = value;
                }
                else
                {
                    HttpContext.Current.Items[SERVER_SPAN] = value;
                }
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
                if (CurrentClientServiceName == null)
                {
                    return endpoint;
                }
                return new Endpoint() { Ipv4 = endpoint.Ipv4, Port = endpoint.Port, Service_name = CurrentClientServiceName };
            }
        }
        public Span CurrentClientSpan
        {
            get
            {
                return HttpContext.Current == null
                    ? currentClientSpan.Value
                    : HttpContext.Current.Items[CLIENT_SPAN] as Span;
            }
            set
            {
                if (HttpContext.Current == null)
                {
                    currentClientSpan.Value = value;
                }
                else
                {
                    HttpContext.Current.Items[CLIENT_SPAN] = value;
                }
            }
        }
        public string CurrentClientServiceName
        {
            get
            {
                return HttpContext.Current == null ? currentClientServiceName.Value 
                    : HttpContext.Current.Items[CLIENT_SERVICE_NAME] as string;
            }
            set
            {
                if (HttpContext.Current == null)
                {
                    currentClientServiceName.Value = value;
                }
                else
                {
                    HttpContext.Current.Items[CLIENT_SERVICE_NAME] = value;
                }
            }
        }

        public bool? Sample
        {
            get
            {
                return CurrentServerSpan == null ? null : CurrentServerSpan.Sample;
            }
        }

        public Span CurrentLocalSpan
        {
            get
            {
                return HttpContext.Current == null
                    ? currentLocalSpan.Value
                    : HttpContext.Current.Items[LOCAL_SPAN] as Span;
            }
            set
            {
                if (HttpContext.Current == null)
                {
                    currentLocalSpan.Value = value;
                }
                else
                {
                    HttpContext.Current.Items[LOCAL_SPAN] = value;
                }
            }
        }
    }
}
