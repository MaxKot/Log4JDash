using System;

namespace Log4JParserNet
{
    internal static class Timestamp
    {
        private static readonly DateTime TimestampZero = new DateTime (1970, 01, 01, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime ToDateTime (Int64 timestamp)
        {
            return TimestampZero.AddMilliseconds (timestamp);
        }

        public static Int64 FromDateTime (DateTime date)
        {
            return (Int64) date.ToUniversalTime ().Subtract (TimestampZero).TotalMilliseconds;
        }
    }
}
