using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Web.Routing;
using Log4JParserNet;

namespace Log4JDash.Web.Models
{
    [TypeConverter (typeof (LogLevelInputConverter))]
    public sealed class LogLevelInput : ICloneable
    {
        private static readonly string[] DefaultLevels = new[]
        {
            Level.Debug,
            Level.Info,
            Level.Warn,
            Level.Error,
            Level.Fatal
        };

        public string Value { get; set; }

        public ICollection<string> Levels { get; set; }

        public LogLevelInput ()
            : this (DefaultLevels[0], DefaultLevels)
        {

        }

        public LogLevelInput (string value)
            : this (value, DefaultLevels)
        {

        }

        private LogLevelInput (string value, ICollection<string> levels)
        {
            Debug.Assert (levels != null, "LogLevelEditor.ctor: levels is null.");

            Value = value;
            Levels = levels;
        }

        public LogLevelInput (LogLevelInput other)
        {
            if (other == null)
            {
                throw new ArgumentNullException ("other");
            }

            Value = other.Value;
        }

        public LogLevelInput Clone ()
        {
            return new LogLevelInput (this);
        }

        object ICloneable.Clone ()
        {
            return Clone ();
        }

        public RouteValueDictionary GetRouteValues ()
        {
            var result = new RouteValueDictionary ();

            if (!Level.Equals (Value, DefaultLevels[0]))
            {
                result.Add ("Value", Value);
            }

            return result;
        }
    }
}
