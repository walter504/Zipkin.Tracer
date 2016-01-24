using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zipkin.Tracer.AspNet.Example
{
    public class AutowireApplication : HttpApplication
    {
        public override void Init()
        {
            base.Init();
            // Register all system components
            InitializeModules();
            InitializeComponents();
        }

        protected virtual void InitializeModules()
        {
            var modules = ServiceLocator.Current.GetAllInstances<IHttpModule>();
            if (modules == null) return;
            foreach (IHttpModule module in modules)
            {
                module.Init(this);
            }
        }

        protected virtual void InitializeComponents()
        {
        }

        protected virtual void RegisterComponents()
        {
            Type[] webTypes = typeof(HttpContext).Assembly.GetTypes();
            Type webModule = typeof(IHttpModule);

            List<Type> moduleTypes = (from moduleType in webTypes
                                      where webModule.IsAssignableFrom(moduleType) &&
                                            moduleType != webModule &&
                                            moduleType.IsPublic
                                      select moduleType).ToList();

            foreach (Type moduleType in moduleTypes)
            {
                //locator.Register<IHttpModule>(moduleType);
            }
        }
    }
}