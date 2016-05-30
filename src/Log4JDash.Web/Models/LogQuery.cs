using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Routing;
using Log4JDash.Web.Mvc;

namespace Log4JDash.Web.Models
{
    public sealed class LogQuery : ICloneable
    {
        public LogSourceInput Source { get; set; }

        private static readonly string DefaultMinLevel = LogLevelInput.DefaultLevels[0];

        [DefaultValueSource ("DefaultMinLevel")]
        public LogLevelInput MinLevel { get; set; }

        private const string DefaultLogger = null;

        [DefaultValue (DefaultLogger)]
        public string Logger { get; set; }

        private const string DefaultThread = null;

        [DefaultValue (DefaultThread)]
        public string Thread { get; set; }

        private const string DateFormat = "O";

        private const string DateFormatSting = "{0:" + DateFormat + "}";

        private static readonly DateTime DefaultMinTime = DateTime.MinValue;

        [DefaultValueSource ("DefaultMinTime")]
        [DisplayFormat (ApplyFormatInEditMode = true, DataFormatString = DateFormatSting)]
        public DateTime MinTime { get; set; }

        private static readonly DateTime DefaultMaxTime = DateTime.MaxValue;

        [DefaultValueSource ("DefaultMaxTime")]
        [DisplayFormat (ApplyFormatInEditMode = true, DataFormatString = DateFormatSting)]
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
            MinLevel = new LogLevelInput (DefaultMinLevel);
            Logger = DefaultLogger;
            Thread = DefaultThread;
            MinTime = DefaultMinTime;
            MaxTime = DefaultMaxTime;
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
            return GetRouteValues (null);
        }

        public RouteValueDictionary GetRouteValues (string memberName)
        {
            var prefix = String.IsNullOrWhiteSpace (memberName)
                ? null
                : memberName + '.';
            var result = new RouteValueDictionary ();

            if (Source != null)
            {
                foreach (var item in Source.GetRouteValues ("Source"))
                {
                    result.Add (prefix + item.Key, item.Value);
                }
            }
            if (MinLevel != null && MinLevel.Value != DefaultMinLevel)
            {
                foreach (var item in MinLevel.GetRouteValues ("MinLevel"))
                {
                    result.Add (prefix + item.Key, item.Value);
                }
            }
            if (!String.IsNullOrWhiteSpace (Logger))
            {
                result.Add (prefix + "Logger", Logger);
            }
            if (!String.IsNullOrWhiteSpace (Thread))
            {
                result.Add (prefix + "Thread", Thread);
            }
            if (MinTime > DefaultMinTime)
            {
                result.Add (prefix + "MinTime", MinTime.ToString (DateFormat));
            }
            if (MaxTime < DefaultMaxTime)
            {
                result.Add (prefix + "MaxTime", MaxTime.ToString (DateFormat));
            }
            if (!String.IsNullOrWhiteSpace (Message))
            {
                result.Add (prefix + "Message", Message);
            }
            if (!String.IsNullOrWhiteSpace (Throwable))
            {
                result.Add (prefix + "Throwable", Throwable);
            }
            if (Quantity != null)
            {
                foreach (var item in Quantity.GetRouteValues ("Quantity"))
                {
                    result.Add (prefix + item.Key, item.Value);
                }
            }
            if (Offset > DefaultOffset)
            {
                result.Add (prefix + "Offset", Offset);
            }

            return result;
        }
    }
}
