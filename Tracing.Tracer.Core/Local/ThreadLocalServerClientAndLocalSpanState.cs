using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tracing.Tracer.Core
{
    public sealed class ThreadLocalServerClientAndLocalSpanState : IServerClientAndLocalSpanState
    {
        private readonly static ThreadLocal<ServerSpan> currentServerSpan = new ThreadLocal<ServerSpan>() { Value = new ServerSpan() };
        private readonly static ThreadLocal<Span> currentClientSpan = new ThreadLocal<Span>();
        private readonly static ThreadLocal<String> currentClientServiceName = new ThreadLocal<String>();
        private readonly static ThreadLocal<Span> currentLocalSpan = new ThreadLocal<Span>();
        private readonly Endpoint endpoint;

        public ThreadLocalServerClientAndLocalSpanState(int ip, int port, String serviceName) {
        Ensure.ArgumentNotNull(serviceName, "Service name must be specified.");
        endpoint = new Endpoint() { Ipv4 = ip, Port = (short) port, Service_name = serviceName};
    }

    /**
     * {@inheritDoc}
     */
    public ServerSpan CurrentServerSpan  
    {
        get
        {
            return currentServerSpan.Value;
        }
    }

    public Endpoint ServerEndpoint {
        get
        {
        return endpoint;
        }
    }

    /**
     * {@inheritDoc}
     */
    public void SetCurrentServerSpan(ServerSpan span) {
        if (span == null) {
            currentServerSpan.remove();
        } else {
            currentServerSpan.set(span);
        }
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public Endpoint getClientEndpoint() {
        final String serviceName = currentClientServiceName.get();
        if (serviceName == null) {
            return endpoint;
        } else {
            return new Endpoint(endpoint).setService_name(serviceName);
        }
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public Span getCurrentClientSpan() {
        return currentClientSpan.get();
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public void setCurrentClientSpan(final Span span) {
        currentClientSpan.set(span);
    }

    @Override
    public void setCurrentClientServiceName(final String serviceName) {
        currentClientServiceName.set(serviceName);
    }

    @Override
    public Boolean sample() {
        return currentServerSpan.get().getSample();
    }

    @Override
    public Span getCurrentLocalSpan() {
        return currentLocalSpan.get();
    }

    @Override
    public void setCurrentLocalSpan(Span span) {
        if (span == null) {
            currentLocalSpan.remove();
        } else {
            currentLocalSpan.set(span);
        }
    }
    }
}
