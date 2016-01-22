using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Zipkin.Tracer.AspNet.HttpModule
{
    public class ZipkinTraceModule : IHttpModule
    {

        public void Init(HttpApplication context)
        {
            context.BeginRequest += BeginRequest;
            context.EndRequest += EndRequest;
        }

        private void BeginRequest(object sender, EventArgs e)
        {
            HttpApplication context = sender as HttpApplication;

        }

        private void EndRequest(object sender, EventArgs e)
        {
            HttpApplication context = sender as HttpApplication;

        }

        public void Dispose()
        {

        }
    }
}
