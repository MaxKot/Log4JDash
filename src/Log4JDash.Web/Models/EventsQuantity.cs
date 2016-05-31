using System;
using System.ComponentModel;
using System.Web.Routing;

namespace Log4JDash.Web.Models
{
    [TypeConverter (typeof (EventsQuantityConverter))]
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
            return GetRouteValues (null);
        }

        public RouteValueDictionary GetRouteValues (string memberName)
        {
            var result = new RouteValueDictionary
            {
                { memberName, Value }
            };

            return result;
        }
    }
}
