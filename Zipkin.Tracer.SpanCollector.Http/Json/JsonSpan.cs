using System.Collections.Generic;
using Newtonsoft.Json;

namespace Zipkin.Tracer.SpanCollector.Http
{
    [JsonObject]
    public class JsonSpan
    {
        public JsonSpan(Span span)
        {
            traceId = span.Trace_id.ToString("x4");
            name = span.Name;
            id = span.Id.ToString("x4");
            parentId = span.__isset.parent_id ? span.Parent_id.ToString("x4") : null;
            if (span.__isset.timestamp)
            {
                timestamp = span.Timestamp;
            }
            if (span.__isset.duration)
            {
                duration = span.Duration;
            }
            annotations = span.Annotations == null ? null : span.Annotations.ConvertAll(t => new JsonAnnotation(t));
            binaryAnnotations = span.Binary_annotations == null ? null : span.Binary_annotations.ConvertAll(t => new JsonBinaryAnnotation(t));
        }

        public string traceId { get; private set; }

        public string name { get; private set; }

        public string id { get; private set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string parentId { get; private set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public long? timestamp { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public long? duration { get; set; }

        public List<JsonAnnotation> annotations { get; private set; }

        public List<JsonBinaryAnnotation> binaryAnnotations { get; private set; }
    }
}
