using System;
using System.ComponentModel;
using System.Globalization;

namespace Log4JDash.Web.Models
{
    public sealed class EventsQuantityConverter : TypeConverter
    {
        private static readonly IFormatProvider Format = CultureInfo.InvariantCulture;

        /// <inheritdoc />
        public override bool CanConvertFrom (ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof (String) ||
                   base.CanConvertFrom (context, sourceType);
        }

        /// <inheritdoc />
        public override bool CanConvertTo (ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof (String) ||
                   base.CanConvertTo (context, destinationType);
        }

        /// <inheritdoc />
        public override object ConvertFrom (ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
            {
                return null;
            }

            var s = value as string;
            int q;
            if (s != null && Int32.TryParse (s, NumberStyles.Integer, Format, out q))
            {
                return new EventsQuantity (q);
            }

            return base.ConvertFrom (context, culture, value);
        }

        /// <inheritdoc />
        public override object ConvertTo (ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value == null)
            {
                return null;
            }

            if (destinationType == typeof (string))
            {
                var eventsQuantity = (EventsQuantity) value;
                return eventsQuantity.Value.ToString (Format);
            }

            return base.ConvertTo (context, culture, value, destinationType);
        }
    }
}
