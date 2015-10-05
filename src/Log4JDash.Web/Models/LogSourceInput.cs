using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Routing;

namespace Log4JDash.Web.Models
{
    public sealed class LogSourceInput : ICloneable
    {
        public string Value
        { get; set; }

        public ICollection<string> Sources
        { get; set; }

        public LogSourceInput (string value, ICollection<string> sources)
        {
            Debug.Assert (sources != null, "LogSourceInput.ctor: levels is null.");

            Value = value;
            Sources = sources;
        }

        public LogSourceInput () : this (null, new string[0])
        {

        }

        public LogSourceInput (LogSourceInput other)
        {
            if (other == null)
            {
                throw new ArgumentNullException ("other");
            }

            Value = other.Value;
        }

        public LogSourceInput Clone ()
        {
            return new LogSourceInput (this);
        }

        object ICloneable.Clone ()
        {
            return Clone ();
        }

        public RouteValueDictionary GetRouteValues ()
        {
            var result = new RouteValueDictionary ();

            if (!String.IsNullOrWhiteSpace (Value))
            {
                result.Add ("Value", Value);
            }

            return result;
        }
    }
}
