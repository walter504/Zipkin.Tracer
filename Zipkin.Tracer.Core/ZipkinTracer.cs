using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zipkin.Tracer.Core
{
    public static class ZipkinTracer
    {
        private static ZipkinWrapper zipkin;

        public static void Initialize(ZipkinWrapper zipkinWrapper)
        {
            zipkin = zipkinWrapper;
        }

        public static void RecordServer(string value)
        {
            zipkin.ServerTracer().SubmitAnnotation(value);
        }

        public static void RecordServer(string name, string value)
        {
            zipkin.ServerTracer().SubmitBinaryAnnotation(name, value);
        }

        public static void RecordClient(string value)
 	    {
            zipkin.ClientTracer().SubmitAnnotation(value);
 	    }

        public static void RecordClient(string name, string value)
        {
            zipkin.ClientTracer().SubmitBinaryAnnotation(name, value);
        }

        public static void StartClientTracer(string requestName)
 	    {
            zipkin.ClientTracer().StartNewSpan(requestName);
            zipkin.ClientTracer().SetClientSent();
 	    }

        public static void StopClientTracer()
        {
            zipkin.ClientTracer().SetClientReceived();
        }

        public static void StartLocalTracer(string component, string operation)
        {
            zipkin.LocalTracer().StartNewSpan(component, operation);
        }

        public static void StopLocalTracer()
        {
            zipkin.LocalTracer().FinishSpan();
        }
    }
}
