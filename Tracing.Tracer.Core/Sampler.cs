using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tracing.Tracer.Core
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
