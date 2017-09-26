using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Routing;
using Log4JDash.Web.Domain;
using Log4JDash.Web.Mvc;
using Log4JParserNet;

namespace Log4JDash.Web.Models
{
    public sealed class LogQuery
        : ICloneable
        , ILogQuery
    {
        public LogSourceInput Source { get; set; }

        public string SourceId => Source.Value?.Id;

        public long? SourceSize => Source.Value?.Size;

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

        public Int64 MinTimestamp => Timestamp.FromDateTime (MinTime);

        private static readonly DateTime DefaultMaxTime = DateTime.MaxValue;

        [DefaultValueSource ("DefaultMaxTime")]
        [DisplayFormat (ApplyFormatInEditMode = true, DataFormatString = DateFormatSting)]
        public DateTime MaxTime { get; set; }

        public Int64 MaxTimestamp => Timestamp.FromDateTime (MaxTime);

        private const string DefaultMessage = null;

        [DefaultValue (DefaultMessage)]
        public string Message { get; set; }

        private const string DefaultThrowable = null;

        [DefaultValue (DefaultThrowable)]
        public string Throwable { get; set; }

        private const int DefaultQuantity = 20;

        [Display (Name = "")]
        [DefaultValue (DefaultQuantity)]
        public EventsQuantity Quantity { get; set; }

        int ILogQuery.Quantity => Quantity;

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
            Quantity = new EventsQuantity (DefaultQuantity);
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
            if (Quantity != null && Quantity.Value != DefaultQuantity)
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

        public Filter CreateFilter ()
        {
            var filters = new List<Filter> (4);

            if (MinLevel.Value != Level.Debug)
            {
                var filter = Filter.Level (MinLevel.Value, Level.MaxValue);
                filters.Add (filter);
            }

            if (!String.IsNullOrWhiteSpace (Logger))
            {
                var filter = Filter.Logger (Logger);
                filters.Add (filter);
            }

            if (!String.IsNullOrWhiteSpace (Message))
            {
                var filter = Filter.Message (Message);
                filters.Add (filter);
            }

            if (MinTime > DateTime.MinValue || MaxTime < DateTime.MaxValue)
            {
                var filter = Filter.Timestamp (MinTime, MaxTime);
                filters.Add (filter);
            }

            Filter result;
            switch (filters.Count)
            {
                case 0:
                    result = null;
                    break;

                case 1:
                    result = filters.Single ();
                    break;

                default:
                    result = Filter.All (filters);
                    break;
            }

            return result;
        }
    }
}
