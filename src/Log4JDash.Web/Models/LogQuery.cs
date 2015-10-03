using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Routing;

namespace Log4JDash.Web.Models
{
    public sealed class LogQuery : ICloneable
    {
        public int SourceId { get; set; }

        public LogLevelInput MinLevel { get; set; }

        public string Logger { get; set; }

        public string Thread { get; set; }

        public DateTime MinTime { get; set; }

        public DateTime MaxTime { get; set; }

        public string Message { get; set; }

        public string Throwable { get; set; }

        [Display (Name = "")]
        public EventsQuantity Quantity { get; set; }

        public int Offset { get; set; }

        public LogQuery ()
        {
            SourceId = 1;
            MinLevel = new LogLevelInput ();
            Logger = null;
            Thread = null;
            MinTime = DateTime.MinValue;
            MaxTime = DateTime.MaxValue;
            Message = null;
            Throwable = null;
            Quantity = new EventsQuantity ();
            Offset = 0;
        }

        public LogQuery (LogQuery other)
        {
            if (other == null)
            {
                throw new ArgumentNullException ("other");
            }

            SourceId = other.SourceId;
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

            if (SourceId != 1)
            {
                result.Add ("SourceId", SourceId);
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
            if (MinTime > DateTime.MinValue)
            {
                result.Add ("MinTime", MinTime);
            }
            if (MaxTime < DateTime.MaxValue)
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
            if (Offset > 0)
            {
                result.Add ("Offset", Offset);
            }

            return result;
        }
    }
}
