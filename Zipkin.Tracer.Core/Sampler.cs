using System;

namespace Zipkin.Tracer.Core
{
    public class Sampler
    {
        public virtual bool IsSampled(long traceId)
        {
            return true;
        }
        public static Sampler create(float rate)
        {
            return new ZipkinSampler(rate);
        }

        protected class ZipkinSampler : Sampler
        {
            public ZipkinSampler(float rate)
            {
            }
            public override bool IsSampled(long traceId)
            {
                return true;
            }
        }
    }
}
