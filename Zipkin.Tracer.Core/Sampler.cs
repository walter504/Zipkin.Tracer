using System;

namespace Zipkin.Tracer.Core
{
    public abstract class Sampler
    {
        public abstract bool IsSampled(long traceId);

        private static readonly Sampler ALWAYS_SAMPLE = new AlwaysSampler();
        private static readonly Sampler NEVER_SAMPLE = new NeverSampler();

        public static Sampler Create(float rate)
        {
            if (rate == 0.0F) return NEVER_SAMPLE;
            if (rate == 1.0F) return ALWAYS_SAMPLE;
            return new ZipkinSampler(rate);
        }


        /**
         * Accepts a percentage of trace ids by comparing their absolute value against a boundary. eg
         * {@code iSampled == abs(traceId) < boundary}
         *
         * <p>While idempotent, this implementation's sample rate won't exactly match the input rate
         * because trace ids are not perfectly distributed across 64bits. For example, tests have shown an
         * error rate of 3% when trace ids are {@link java.util.Random#nextLong random}.
         */
        public class ZipkinSampler : Sampler
        {
            private readonly long boundary;

            public ZipkinSampler(float rate)
            {
                Ensure.ArgumentAssert(rate > 0 && rate < 1, "rate should be between 0 and 1: was {0}", rate);
                this.boundary = (long)(long.MaxValue * rate);
            }

            public override bool IsSampled(long traceId)
            {
                // The absolute value of Long.MIN_VALUE is larger than a long, so Math.abs returns identity.
                // This converts to MAX_VALUE to avoid always dropping when traceId == Long.MIN_VALUE
                long t = traceId == long.MinValue ? long.MaxValue : Math.Abs(traceId);
                return t < boundary;
            }
        }

        private class AlwaysSampler : Sampler
        {
            public override bool IsSampled(long traceId)
            {
                return true;
            }
        }

        private class NeverSampler : Sampler
        {
            public override bool IsSampled(long traceId)
            {
                return false;
            }
        }
    }
}
