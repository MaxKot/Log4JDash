﻿using Log4JParserNet;

namespace Log4JDash.Web.Domain
{
    public interface ILogQuery
    {
        string SourceId { get; }

        string Snapshot { get; }

        long MinTimestamp { get; }

        long MaxTimestamp { get; }

        int Offset { get; }

        int Quantity { get; }

        Filter CreateFilter ();
    }
}
