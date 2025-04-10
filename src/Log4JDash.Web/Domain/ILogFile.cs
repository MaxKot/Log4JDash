﻿using System;
using Log4JParserNet;

namespace Log4JDash.Web.Domain
{
    internal interface ILogFile
        : ICloneable
        , IDisposable
    {
        string FileName { get; }

        long Size { get; }

        IEnumerableOfEvents GetEvents ();

        IEnumerableOfEvents GetEventsReverse ();
    }
}
