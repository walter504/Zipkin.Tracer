using System;

namespace Zipkin.Tracer.Core
{
    public class ClientSpanThreadBinder
    {
        private readonly IClientSpanState state;
        /**
         * Creates a new instance.
         *
         * @param state client span state, should not be <code>null</code>
         */
        public ClientSpanThreadBinder(IClientSpanState state)
        {
            this.state = Ensure.ArgumentNotNull(state, "state");
        }

        /**
         * This should be called in the thread in which the client request made after starting new client span.
         * <p>
         * It returns the current client span which you can keep and bind to the callback thread
         * @see #setCurrentSpan(Span)
         * @return Returned Span can be bound to different callback thread.
         */
        public Span GetCurrentClientSpan()
        {
            return state.CurrentClientSpan;
        }

        /**
         * Binds given span to current thread. This should typically be called when code is invoked in async client callback
         * before the {@link ClientTracer#setClientReceived()}
         *
         * @param span Span to bind to current execution thread. Should not be <code>null</code>.
         */
        public void SetCurrentSpan(Span span)
        {
            state.CurrentClientSpan = span;
        }
    }
}
