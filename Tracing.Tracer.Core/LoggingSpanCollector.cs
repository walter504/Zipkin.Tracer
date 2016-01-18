using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tracing.Tracer.Core
{
    public class LoggingSpanCollector : ISpanCollector
    {
        private static string UTF_8 = "UTF-8";
        public void Collect(Span span)
        {
            Debug.Print(span.ToString());
        }
    }
}
