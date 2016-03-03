using Autofac;
using Autofac.Extras.CommonServiceLocator;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Zipkin.Tracer.AspNet.HttpModule;
using Zipkin.Tracer.Core;
using Zipkin.Tracer.Http;
using Zipkin.Tracer.SpanCollector.Http;

namespace Zipkin.Tracer.AspNet.Example
{
    public class Global : AutowireApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            var builder = new ContainerBuilder();
            Register(builder);
            // container 必须在注册完所有模块后创建
            var container = builder.Build();
            ServiceLocator.SetLocatorProvider(() => new AutofacServiceLocator(container));
        }

        private void Register(ContainerBuilder builder)
        {
            log4net.ILog logger = log4net.LogManager.GetLogger("ZipkinLogger");
            builder.RegisterInstance(logger).SingleInstance();
            var spanCollector = new HttpSpanCollector("http://192.168.1.232:9411", 1, logger);
            ZipkinWrapper.Builder zipkinBuilder = new ZipkinWrapper.Builder("ZipkinTracerIntegration")
                    .SpanCollector(spanCollector).TraceSampler(Sampler.Create(0.5F));
            var wrapper = zipkinBuilder.Build();
            ZipkinTracer.Initialize(wrapper);
            builder.RegisterInstance(wrapper.ServerTracer()).SingleInstance();
            builder.RegisterInstance(wrapper.ClientTracer()).SingleInstance();
            builder.RegisterInstance(wrapper.LocalTracer()).SingleInstance();
            builder.RegisterInstance(wrapper.ServerRequestInterceptor()).SingleInstance();
            builder.RegisterInstance(wrapper.ServerResponseInterceptor()).SingleInstance();
            builder.RegisterInstance(wrapper.ClientRequestInterceptor()).SingleInstance();
            builder.RegisterInstance(wrapper.ClientResponseInterceptor()).SingleInstance();
            builder.RegisterInstance(wrapper.ServerSpanThreadBinder()).SingleInstance();
            builder.RegisterInstance(wrapper.ClientSpanThreadBinder()).SingleInstance();

            builder.RegisterType<DefaultSpanNameProvider>().As<ISpanNameProvider>().SingleInstance();
            builder.RegisterType<ZipkinTraceModule>().As<IHttpModule>().SingleInstance();

        }
    }
}