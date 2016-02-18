using Newtonsoft.Json;

namespace Zipkin.Tracer.SpanCollector
{
    [JsonObject]
    public class SerializableAnnotation
    {
        public SerializableAnnotation(Annotation annotation)
        {
            timestamp = annotation.Timestamp;
            value = annotation.Value;
            endpoint = new SerializableEndpoint(annotation.Host);
        }

        public long timestamp { get; private set; }
        
        public string value { get; private set; }
        
        public SerializableEndpoint endpoint { get; private set; }
    }
}
