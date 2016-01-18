using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tracing.Tracer.Core
{
    public class Util
    {
        private static Random randomGenerator = new Random(DateTime.Now.Millisecond);
        public static long GetCurrentTimeStamp()
        {
            var t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            return Convert.ToInt64(t.TotalMilliseconds * 1000);
        }

        public static long GetRandomId()
        {
            byte[] buffer = new byte[8];
            randomGenerator.NextBytes(buffer);
            return BitConverter.ToInt64(buffer, 0);
        }
    }
}
