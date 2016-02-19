using System;

namespace Zipkin.Tracer.Core
{
    public class LocalSpanThreadBinder
    {
        private readonly ILocalSpanState state;

        public LocalSpanThreadBinder(ILocalSpanState state)
        {
            this.state = Ensure.ArgumentNotNull(state, "state");
        }

        public Span GetCurrentLocalSpan()
        {
            return state.CurrentLocalSpan;
        }

        public void SetCurrentSpan(Span span)
        {
            state.CurrentLocalSpan = span;
        }
    }
}
