using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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

        public static IPAddress GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            return host.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
        }

        public static int GetLocalIPInt()
        {
            var ip = GetLocalIPAddress();
            if (ip != null)
            {
                byte[] bytes = ip.GetAddressBytes();
                Array.Reverse(bytes);
                return BitConverter.ToInt32(bytes, 0);
            }
            return 127 << 24 | 1;
        }
    }
}
