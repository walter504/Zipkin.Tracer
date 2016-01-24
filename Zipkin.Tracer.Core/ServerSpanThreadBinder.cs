using System;

namespace Zipkin.Tracer.Core
{
    public class ServerSpanThreadBinder
    {
        private readonly IServerSpanState state;
        /**
         * Creates a new instance.
         *
         * @param state Server span state, should not be <code>null</code>
         */
        public ServerSpanThreadBinder(IServerSpanState state)
        {
            this.state = Ensure.ArgumentNotNull(state, "state");
        }

        /**
         * This should be called in the thread in which the request was received before executing code in new threads.
         * <p>
         * It returns the current server span which you can keep and bind to a new thread using
         * {@link ServerSpanThreadBinder#setCurrentSpan(ServerSpan)}.
         *
         * @see ServerSpanThreadBinder#setCurrentSpan(ServerSpan)
         * @return Returned Span can be bound to different executing threads.
         */
        public ServerSpan GetCurrentServerSpan()
        {
            return state.CurrentServerSpan;
        }

        /**
         * Binds given span to current thread. This should typically be called when code is invoked in new thread to bind the
         * span from the thread in which we received the request to the new execution thread.
         *
         * @param span Span to bind to current execution thread. Should not be <code>null</code>.
         */
        public void SetCurrentSpan(ServerSpan span)
        {
            state.CurrentServerSpan = span;
        }
    }
}
