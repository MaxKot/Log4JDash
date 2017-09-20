using System;

namespace Log4JParserNet
{
    public static class Timestamp
    {
        private static readonly DateTime TimestampZero
            = new DateTime (1970, 01, 01, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime ToDateTime (Int64 timestamp)
            => TimestampZero.AddMilliseconds (timestamp);

        public static Int64 FromDateTime (DateTime date)
            => (Int64) date.ToUniversalTime ().Subtract (TimestampZero).TotalMilliseconds;

        public static readonly Int64 MinValue
            = Int64.MinValue;

        public static readonly Int64 MaxValue
            = Int64.MaxValue;
    }
}
