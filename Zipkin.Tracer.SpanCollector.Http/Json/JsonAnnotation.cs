using System;

namespace Zipkin.Tracer.SpanCollector.Http
{
    public class JsonAnnotation
    {
        public JsonAnnotation(Annotation annotation)
        {
            timestamp = annotation.Timestamp;
            value = annotation.Value;
            endpoint = new JsonEndpoint(annotation.Host);
        }

        public long timestamp { get; private set; }
        
        public string value { get; private set; }
        
        public JsonEndpoint endpoint { get; private set; }
    }
}
