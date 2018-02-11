using System;
using Log4JDash.Web.Domain;
using Log4JParserNet;

namespace Log4JDash.Web.Tests
{
    internal sealed class SimpleLogQuery : ILogQuery
    {
        public string SourceId { get; set; }

        public string Snapshot { get; set; }

        public long MinTimestamp { get; set; } = Int64.MinValue;

        public long MaxTimestamp { get; set; } = Int64.MaxValue;

        public int Offset { get; set; }

        public int Quantity { get; set; }

        public Filter Filter { get; set; }

        Filter ILogQuery.CreateFilter () => Filter;
    }
}
