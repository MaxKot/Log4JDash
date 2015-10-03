using System;
using System.Web.Routing;

namespace Log4JDash.Web.Models
{
    public sealed class EventsQuantity : ICloneable
    {
        public int Value { get; set; }

        public EventsQuantity ()
            : this (20)
        {

        }

        public EventsQuantity (int value)
        {
            Value = value;
        }

        public EventsQuantity (EventsQuantity other)
        {
            if (other == null)
            {
                throw new ArgumentNullException ("other");
            }

            Value = other.Value;
        }

        public static implicit operator int (EventsQuantity quantity)
        {
            return quantity.Value;
        }

        public EventsQuantity Clone ()
        {
            return new EventsQuantity (this);
        }

        object ICloneable.Clone ()
        {
            return Clone ();
        }

        public RouteValueDictionary GetRouteValues ()
        {
            var result = new RouteValueDictionary ();

            if (Value != 20)
            {
                result.Add ("Value", Value);
            }

            return result;
        }
    }
}
