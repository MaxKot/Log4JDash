using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Web.Mvc;
using System.Web.Routing;

namespace Log4JDash.Web.Models
{
    [TypeConverter (typeof (LogSourceInputConverter))]
    [Bind (Exclude = "Sources")]
    public sealed class LogSourceInput : ICloneable
    {
        public LogSourceModel Value { get; set; }

        public ICollection<LogSourceModel> Sources { get; set; }

        public LogSourceInput (LogSourceModel value, ICollection<LogSourceModel> sources)
        {
            Debug.Assert (sources != null, "LogSourceInput.ctor: levels is null.");

            Value = value;
            Sources = sources;
        }

        public LogSourceInput () : this (null, new LogSourceModel[0])
        {

        }

        public LogSourceInput (LogSourceInput other)
        {
            if (other == null)
            {
                throw new ArgumentNullException ("other");
            }

            Value = other.Value;
            Sources = other.Sources;
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
            return GetRouteValues (null);
        }

        public RouteValueDictionary GetRouteValues (string memberName)
        {
            var result = new RouteValueDictionary ();

            if (Value != null)
            {
                result.Add (memberName, Convert.ToString (Value));
            }

            return result;
        }
    }
}
