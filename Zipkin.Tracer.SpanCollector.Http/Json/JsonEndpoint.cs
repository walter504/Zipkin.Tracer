using System;
using System.Net;

namespace Zipkin.Tracer.SpanCollector.Http
{
    public class JsonEndpoint
    {
        public JsonEndpoint(Endpoint endpoint)
        {
            serviceName = endpoint.Service_name;
            ipv4 = new IPAddress(BitConverter.GetBytes(endpoint.Ipv4)).ToString();
            port = endpoint.Port;
        }

        public string serviceName { get; private set; }

        public string ipv4 { get; private set; }

        public int port { get; private set; }
    }
}