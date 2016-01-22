using System;

namespace Zipkin.Tracer.Http
{
    public class StringServiceNameProvider : IServiceNameProvider
    {
        private readonly string serviceName;

        public StringServiceNameProvider(String serviceName) {
            this.serviceName = serviceName;
        }

        public String ServiceName(IHttpRequest request) {
            return serviceName;
        }
    }
}
