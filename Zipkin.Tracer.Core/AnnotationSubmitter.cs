using System;
using System.Collections.Generic;

namespace Zipkin.Tracer.Core
{
    public abstract class AnnotationSubmitter
    {
        public ISpanAndEndpoint SpanAndEndpoint { get; set; }

        public void SubmitAnnotation(string value)
        {
            Span span = SpanAndEndpoint.Span;
            if (span != null)
            {
                Annotation annotation = new Annotation();
                annotation.Timestamp = Util.CurrentTimeMicroseconds();
                annotation.Host = SpanAndEndpoint.Endpoint;
                annotation.Value = value;
                AddAnnotation(span, annotation);
            }
        }

        public void SubmitAnnotation(string value, long timestamp)
        {
            Span span = SpanAndEndpoint.Span;
            if (span != null)
            {
                Annotation annotation = new Annotation();
                annotation.Timestamp = timestamp;
                annotation.Host = SpanAndEndpoint.Endpoint;
                annotation.Value = value;
                AddAnnotation(span, annotation);
            }
        }

        public void SubmitStartAnnotation(string annotationName)
        {
            Span span = SpanAndEndpoint.Span;
            if (span != null)
            {
                Annotation annotation = new Annotation();
                annotation.Timestamp = Util.CurrentTimeMicroseconds();
                annotation.Host = SpanAndEndpoint.Endpoint;
                annotation.Value = annotationName;
                lock (span)
                {
                    span.Timestamp = annotation.Timestamp;
                    span.Annotations.Add(annotation);
                }
            }
        }

        public bool SubmitEndAnnotation(string annotationName, ISpanCollector spanCollector)
        {
            Span span = SpanAndEndpoint.Span;
            if (span == null)
            {
                return false;
            }
            Annotation annotation = new Annotation();
            annotation.Timestamp = Util.CurrentTimeMicroseconds();
            annotation.Host = SpanAndEndpoint.Endpoint;
            annotation.Value = annotationName;
            span.Annotations.Add(annotation);
            span.Duration = annotation.Timestamp - span.Timestamp;
            spanCollector.Collect(span);
            return true;
        }

        public void SubmitAddress(string key, int ipv4, int port, string serviceName)
        {
            Span span = SpanAndEndpoint.Span;
            if (span != null)
            {
                serviceName = serviceName != null ? serviceName : "unknown";
                BinaryAnnotation ba = new BinaryAnnotation();
                ba.Key = key;
                ba.Value = string.Empty;
                ba.Annotation_type = AnnotationType.BOOL;
                ba.Host = new Endpoint() { Ipv4 = ipv4, Port = (short)port, Service_name = serviceName };
                AddBinaryAnnotation(span, ba);
            }
        }

        public void SubmitBinaryAnnotation(string key, string value)
        {
            Span span = SpanAndEndpoint.Span;
            if (span != null)
            {
                BinaryAnnotation ba = new BinaryAnnotation()
                {
                    Key = key,
                    Value = value,
                    Host = SpanAndEndpoint.Endpoint
                };
                AddBinaryAnnotation(span, ba);
            }
        }

        public void SubmitBinaryAnnotation(string key, int value)
        {
            // Zipkin v1 UI and query only support String annotations.
            SubmitBinaryAnnotation(key, value.ToString());
        }

        private void AddAnnotation(Span span, Annotation annotation)
        {
            lock (span)
            {
                if (span.Annotations == null)
                {
                    span.Annotations = new List<Annotation>();
                }
                span.Annotations.Add(annotation);
            }
        }

        private void AddBinaryAnnotation(Span span, BinaryAnnotation ba)
        {
            lock (span)
            {
                if (span.Binary_annotations == null)
                {
                    span.Binary_annotations = new List<BinaryAnnotation>();
                }
                span.Binary_annotations.Add(ba);
            }
        }
    }
}
