using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Zipkin.Tracer.Core;

namespace Zipkin.Tracer.AspNet.Example
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ZipkinTracer.StartClientTracer("短信");
            ZipkinTracer.RecordClient("格式化信息");
            ZipkinTracer.RecordClient("message", "发送");
            System.Threading.Thread.Sleep(200);
            ZipkinTracer.StopClientTracer();

            ZipkinTracer.StartClientTracer("推送");
            ZipkinTracer.RecordClient("推送信息");
            ZipkinTracer.RecordClient("message", "发送");
            System.Threading.Thread.Sleep(500);
            ZipkinTracer.StopClientTracer();
        }
    }
}