using System;
using System.ComponentModel;
using System.Globalization;

namespace Log4JDash.Web.Models
{
    public sealed class LogSourceInputConverter : TypeConverter
    {
        private static LogSourceModelConverter ValueConverter = new LogSourceModelConverter ();

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

            var logSource = ValueConverter.ConvertFrom (context, culture, value) as LogSourceModel;
            if (logSource != null)
            {
                return new LogSourceInput (logSource, new[] { logSource });
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
                var logSourceInput = value as LogSourceInput;
                if (logSourceInput != null)
                {
                    return ValueConverter.ConvertTo (context, culture, logSourceInput.Value, destinationType);
                }
            }

            return base.ConvertTo (context, culture, value, destinationType);
        }
    }
}
