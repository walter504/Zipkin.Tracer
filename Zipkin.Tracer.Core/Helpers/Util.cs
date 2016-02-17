using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zipkin.Tracer.Core
{
    public class Util
    {
        private static Random rnd = new Random(DateTime.Now.Millisecond);
        private static DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        
        public static long NextLong()
        {
            return (long)(rnd.NextDouble() * long.MaxValue);
        }

        public static long HexToLong(string hex)
        {
            return long.Parse(hex, System.Globalization.NumberStyles.HexNumber);
        }

        public static string LongToHex(long input)
        {
            return input.ToString("x4");
        }

        public static long CurrentTimeSeconds()
        {
            return ToUnixTimeSeconds(DateTime.Now);
        }

        public static long CurrentTimeMilliseconds()
        {
            return ToUnixTimeMilliseconds(DateTime.Now);
        }

        public static long CurrentTimeMicroseconds()
        {
            return ToUnixTimMicroseconds(DateTime.Now);
        }

        public static long ToUnixTimeSeconds(DateTime dt)
        {
            TimeSpan timespan = TimeZoneInfo.ConvertTimeToUtc(dt).Subtract(UnixEpoch);
            return timespan.Ticks / 10000000;
        }

        public static long ToUnixTimeMilliseconds(DateTime dt)
        {
            TimeSpan timespan = TimeZoneInfo.ConvertTimeToUtc(dt).Subtract(UnixEpoch);
            return timespan.Ticks / 10000;
        }

        public static long ToUnixTimMicroseconds(DateTime dt)
        {
            TimeSpan timespan = TimeZoneInfo.ConvertTimeToUtc(dt).Subtract(UnixEpoch);
            return timespan.Ticks / 10;
        }
    }
}
