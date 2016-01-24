using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zipkin.Tracer.Core
{
    public class ZipkinWrapper
    {
        private readonly ServerTracer serverTracer;
        private readonly ClientTracer clientTracer;
        private readonly LocalTracer localTracer;
        private readonly ServerRequestInterceptor serverRequestInterceptor;
        private readonly ServerResponseInterceptor serverResponseInterceptor;
        private readonly ClientRequestInterceptor clientRequestInterceptor;
        private readonly ClientResponseInterceptor clientResponseInterceptor;
        private readonly AnnotationSubmitter serverSpanAnnotationSubmitter;
        private readonly ServerSpanThreadBinder serverSpanThreadBinder;
        private readonly ClientSpanThreadBinder clientSpanThreadBinder;

        /**
         * Builds Brave api objects with following defaults if not overridden:
         * <p>
         * <ul>
         * <li>ThreadLocalServerClientAndLocalSpanState which binds trace/span state to current thread.</li>
         * <li>LoggingSpanCollector</li>
         * <li>Sampler that samples all traces</li>
         * </ul>
         */
        public class Builder
        {
            internal readonly IServerClientAndLocalSpanState state;
            internal ISpanCollector spanCollector = new LoggingSpanCollector();
            // default added so callers don't need to check null.
            internal Sampler sampler = Sampler.create(1.0f);

            /**
             * Builder which initializes with serviceName = "unknown".
             * <p>
             * When using this builder constructor we will try to 'guess' ip address by using java.net.* utility classes.
             * This might be convenient but not necessary what you want.
             * It is preferred to use constructor that takes ip, port and service name instead.
             * </p>
             */
            public Builder()
                : this("unknown")
            {
            }

            /**
             * Builder.
             * <p>
             * When using this builder constructor we will try to 'guess' ip address by using java.net.* utility classes.
             * This might be convenient but not necessary what you want.
             * It is preferred to use constructor that takes ip, port and service name instead.
             * </p>
             *
             * @param serviceName Name of service. Is only relevant when we do server side tracing.
             */
            public Builder(string serviceName)
            {
                try
                {
                    int ip = (int)System.Net.Dns.GetHostAddresses("localhost")[1].Address;
                    state = new ThreadLocalServerClientAndLocalSpanState(ip, 0, serviceName);
                }
                catch (System.Net.Sockets.SocketException e)
                {
                    throw new Exception("Unable to get Inet address", e);
                }
            }

            /**
             * Builder.
             *
             * @param ip          ipv4 host address as int. Ex for the ip 1.2.3.4, it would be (1 << 24) | (2 << 16) | (3 << 8) | 4
             * @param port        Port for service
             * @param serviceName Name of service. Is only relevant when we do server side tracing.
             */
            public Builder(int ip, int port, string serviceName)
            {
                state = new ThreadLocalServerClientAndLocalSpanState(ip, port, serviceName);
            }

            /**
             * Use for control of how tracing state propagates across threads.
             */
            public Builder(IServerClientAndLocalSpanState state)
            {
                this.state = Ensure.ArgumentNotNull(state, "state must be specified.");
            }

            public Builder traceSampler(Sampler sampler)
            {
                this.sampler = sampler;
                return this;
            }

            /**
             * @param spanCollector
             */
            public Builder SpanCollector(ISpanCollector spanCollector)
            {
                this.spanCollector = spanCollector;
                return this;
            }

            public ZipkinWrapper build()
            {
                return new ZipkinWrapper(this);
            }
        }

        /**
         * Client Tracer.
         * <p>
         * It is advised that you use ClientRequestInterceptor and ClientResponseInterceptor instead.
         * Those api's build upon ClientTracer and have a higher level api.
         * </p>
         *
         * @return ClientTracer implementation.
         */
        public ClientTracer ClientTracer()
        {
            return clientTracer;
        }

        /**
         * Returns a tracer used to log in-process activity.
         *
         * @since 3.2
         */
        public LocalTracer LocalTracer()
        {
            return localTracer;
        }

        /**
         * Server Tracer.
         * <p>
         * It is advised that you use ServerRequestInterceptor and ServerResponseInterceptor instead.
         * Those api's build upon ServerTracer and have a higher level api.
         * </p>
         *
         * @return ClientTracer implementation.
         */
        public ServerTracer ServerTracer()
        {
            return serverTracer;
        }

        public ClientRequestInterceptor ClientRequestInterceptor()
        {
            return clientRequestInterceptor;
        }

        public ClientResponseInterceptor ClientResponseInterceptor()
        {
            return clientResponseInterceptor;
        }

        public ServerRequestInterceptor ServerRequestInterceptor()
        {
            return serverRequestInterceptor;
        }

        public ServerResponseInterceptor ServerResponseInterceptor()
        {
            return serverResponseInterceptor;
        }

        /**
         * Helper object that can be used to propogate server trace state. Typically over different threads.
         *
         * @return {@link ServerSpanThreadBinder}.
         * @see ServerSpanThreadBinder
         */
        public ServerSpanThreadBinder ServerSpanThreadBinder()
        {
            return serverSpanThreadBinder;
        }

        /**
         * Helper object that can be used to propagate client trace state. Typically over different threads.
         *
         * @return {@link ClientSpanThreadBinder}.
         * @see ClientSpanThreadBinder
         */
        public ClientSpanThreadBinder ClientSpanThreadBinder()
        {
            return clientSpanThreadBinder;
        }

        /**
         * Can be used to submit application specific annotations to the current server span.
         *
         * @return Server span {@link AnnotationSubmitter}.
         */
        public AnnotationSubmitter ServerSpanAnnotationSubmitter()
        {
            return serverSpanAnnotationSubmitter;
        }

        private ZipkinWrapper(Builder builder)
        {
            serverTracer = new ServerTracer(builder.state, builder.spanCollector, builder.sampler);
            clientTracer = new ClientTracer(builder.state, builder.spanCollector, builder.sampler);
            localTracer = new LocalTracer(builder.state, builder.spanCollector, builder.sampler);

            serverRequestInterceptor = new ServerRequestInterceptor(serverTracer);
            serverResponseInterceptor = new ServerResponseInterceptor(serverTracer);
            clientRequestInterceptor = new ClientRequestInterceptor(clientTracer);
            clientResponseInterceptor = new ClientResponseInterceptor(clientTracer);
            //serverSpanAnnotationSubmitter = AnnotationSubmitter.create(SpanAndEndpoint.ServerSpanAndEndpoint.create(builder.state));
            serverSpanThreadBinder = new ServerSpanThreadBinder(builder.state);
            clientSpanThreadBinder = new ClientSpanThreadBinder(builder.state);
        }
    }
}
