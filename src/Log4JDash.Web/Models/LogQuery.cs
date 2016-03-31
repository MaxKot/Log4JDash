using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Routing;
using Log4JDash.Web.Mvc;

namespace Log4JDash.Web.Models
{
    public sealed class LogQuery : ICloneable
    {
        public LogSourceInput Source { get; set; }

        public LogLevelInput MinLevel { get; set; }

        private const string DefaultLogger = null;

        [DefaultValue (DefaultLogger)]
        public string Logger { get; set; }

        private const string DefaultThread = null;

        [DefaultValue (DefaultThread)]
        public string Thread { get; set; }

        private static DateTime DefaultMinTime () => DateTime.MinValue;

        [DefaultValueFactory ("DefaultMinTime")]
        [DisplayFormat (ApplyFormatInEditMode = true, DataFormatString = "{0:O}")]
        public DateTime MinTime { get; set; }

        private static DateTime DefaultMaxTime () => DateTime.MaxValue;

        [DefaultValueFactory ("DefaultMaxTime")]
        [DisplayFormat (ApplyFormatInEditMode = true, DataFormatString = "{0:O}")]
        public DateTime MaxTime { get; set; }

        private const string DefaultMessage = null;

        [DefaultValue (DefaultMessage)]
        public string Message { get; set; }

        private const string DefaultThrowable = null;

        [DefaultValue (DefaultThrowable)]
        public string Throwable { get; set; }

        [Display (Name = "")]
        public EventsQuantity Quantity { get; set; }

        private const int DefaultOffset = 0;

        [DefaultValue (DefaultOffset)]
        public int Offset { get; set; }

        public LogQuery ()
        {
            Source = new LogSourceInput ();
            MinLevel = new LogLevelInput ();
            Logger = DefaultLogger;
            Thread = DefaultThread;
            MinTime = DefaultMinTime ();
            MaxTime = DefaultMaxTime ();
            Message = DefaultMessage;
            Throwable = DefaultThrowable;
            Quantity = new EventsQuantity ();
            Offset = DefaultOffset;
        }

        public LogQuery (LogQuery other)
        {
            if (other == null)
            {
                throw new ArgumentNullException ("other");
            }

            Source = other.Source != null
                ? other.Source.Clone ()
                : null;
            MinLevel = other.MinLevel != null
                ? other.MinLevel.Clone ()
                : null;
            Logger = other.Logger;
            Thread = other.Thread;
            MinTime = other.MinTime;
            MaxTime = other.MaxTime;
            Message = other.Message;
            Throwable = other.Throwable;
            Quantity = other.Quantity != null
                ? other.Quantity.Clone ()
                : null;
            Offset = other.Offset;
        }

        public LogQuery Clone ()
        {
            return new LogQuery (this);
        }

        object ICloneable.Clone ()
        {
            return Clone ();
        }

        public RouteValueDictionary GetRouteValues ()
        {
            var result = new RouteValueDictionary ();

            if (Source != null)
            {
                foreach (var item in Source.GetRouteValues ())
                {
                    result.Add ("Source." + item.Key, item.Value);
                }
            }
            if (MinLevel != null)
            {
                foreach (var item in MinLevel.GetRouteValues ())
                {
                    result.Add ("MinLevel." + item.Key, item.Value);
                }
            }
            if (!String.IsNullOrWhiteSpace (Logger))
            {
                result.Add ("Logger", Logger);
            }
            if (!String.IsNullOrWhiteSpace (Thread))
            {
                result.Add ("Thread", Thread);
            }
            if (MinTime > DefaultMinTime ())
            {
                result.Add ("MinTime", MinTime);
            }
            if (MaxTime < DefaultMaxTime ())
            {
                result.Add ("MaxTime", MaxTime);
            }
            if (!String.IsNullOrWhiteSpace (Message))
            {
                result.Add ("Message", Message);
            }
            if (!String.IsNullOrWhiteSpace (Throwable))
            {
                result.Add ("Throwable", Throwable);
            }
            if (Quantity != null)
            {
                foreach (var item in Quantity.GetRouteValues ())
                {
                    result.Add ("Quantity." + item.Key, item.Value);
                }
            }
            if (Offset > DefaultOffset)
            {
                result.Add ("Offset", Offset);
            }

            return result;
        }
    }
}
