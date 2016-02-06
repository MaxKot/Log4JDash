using System;
using System.ComponentModel;
using System.Globalization;

namespace Log4JDash.Web.Models
{
    public sealed class LogLevelInputConverter : TypeConverter
    {
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
            if (s != null)
            {
                return new LogLevelInput (s);
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
                var logLevelInput = (LogLevelInput) value;
                return logLevelInput.Value;
            }

            return base.ConvertTo (context, culture, value, destinationType);
        }
    }
}
