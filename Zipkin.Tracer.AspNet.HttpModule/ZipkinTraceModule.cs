using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Zipkin.Tracer.Core;
using Zipkin.Tracer.Http;

namespace Zipkin.Tracer.AspNet.HttpModule
{
    public class ZipkinTraceModule : IHttpModule
    {
        private readonly ServerRequestInterceptor requestInterceptor;
        private readonly ServerResponseInterceptor responseInterceptor;
        private readonly ISpanNameProvider spanNameProvider;

        public ZipkinTraceModule(ServerRequestInterceptor requestInterceptor, ServerResponseInterceptor responseInterceptor, ISpanNameProvider spanNameProvider)
        {
            this.requestInterceptor = requestInterceptor;
            this.spanNameProvider = spanNameProvider;
            this.responseInterceptor = responseInterceptor;
        }

        public void Init(HttpApplication context)
        {
            context.PreRequestHandlerExecute += PreRequestHandlerExecute;
            context.PostRequestHandlerExecute += PostRequestHandlerExecute;
        }

        private void PreRequestHandlerExecute(object sender, EventArgs e)
        {
            var app = sender as HttpApplication;
            if (app.Context.Handler is System.Web.UI.Page)
            {
                requestInterceptor.Handle(new HttpServerRequestAdapter(new AspNetHttpServerRequest(app.Request), spanNameProvider));
            }
        }

        private void PostRequestHandlerExecute(object sender, EventArgs e)
        {
            HttpApplication app = sender as HttpApplication;
            if (app.Context.Handler is System.Web.UI.Page)
            {
                responseInterceptor.Handle(new HttpServerResponseAdapter(new AspNetHttpServerResponse(app.Response)));
            }
        }

        public void Dispose()
        {
        }

        class AspNetHttpServerRequest : IHttpServerRequest
        {
            private readonly HttpRequest request;
            public AspNetHttpServerRequest(HttpRequest request)
            {
                this.request = request;
            }

            public Uri GetUri()
            {
                return request.Url;
            }

            public string GetHttpMethod()
            {
                return request.HttpMethod;
            }

            public string GetHttpHeaderValue(string headerName)
            {
                return request.Headers[headerName];
            }
        }

        class AspNetHttpServerResponse : IHttpResponse
        {
            private readonly HttpResponse response;
            public AspNetHttpServerResponse(HttpResponse response)
            {
                this.response = response;
            }

            public int GetHttpStatusCode()
            {
                return response.StatusCode;
            }
        }
    }
}
