using Newtonsoft.Json;
using System;
using System.Text;

namespace Zipkin.Tracer.SpanCollector
{
    public class SerializableBinaryAnnotation
    {
        public SerializableBinaryAnnotation(BinaryAnnotation ba)
        {
            key = ba.Key;
            value = ba.Value;
            annotationType = ba.Annotation_type.ToString();
            endpoint = new SerializableEndpoint(ba.Host);
            switch (ba.Annotation_type)
            {
                case AnnotationType.BOOL: value = BitConverter.ToBoolean(ba.Value, 0); break;
                case AnnotationType.BYTES: value = Convert.ToBase64String(ba.Value); break;
                case AnnotationType.I16: value = BitConverter.ToInt16(ba.Value, 0); break;
                case AnnotationType.I32: value = BitConverter.ToInt32(ba.Value, 0); break;
                case AnnotationType.I64: value = BitConverter.ToInt64(ba.Value, 0); break;
                case AnnotationType.DOUBLE: value = BitConverter.ToDouble(ba.Value, 0); break;
                case AnnotationType.STRING: value = Encoding.UTF8.GetString(ba.Value); break;
                default: throw new Exception(string.Format("Unsupported annotation type: {0}", ba));
            }
        }

        public string key { get; private set; }

        public object value { get; private set; }

        public string annotationType { get; private set; }

        public SerializableEndpoint endpoint { get; private set; }
    }
}
