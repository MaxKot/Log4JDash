
namespace Log4JDash.Web.Models
{
    public sealed class EventsQuantity
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

        public static implicit operator int (EventsQuantity quantity)
        {
            return quantity.Value;
        }
    }
}
