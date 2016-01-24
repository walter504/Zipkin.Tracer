using System;

namespace Zipkin.Tracer.Http
{
    public class StringServiceNameProvider : IServiceNameProvider
    {
        private readonly string serviceName;
        public StringServiceNameProvider(string serviceName) 
        {
            this.serviceName = serviceName;
        }

        public string ServiceName(IHttpRequest request) 
        {
            return serviceName;
        }
    }
}
