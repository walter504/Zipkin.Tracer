using System;

namespace Zipkin.Tracer.Core
{
    public class KeyValueAnnotation
    {
        public string Key { get; private set; }
        public string Value { get; private set; }
        public KeyValueAnnotation(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}
