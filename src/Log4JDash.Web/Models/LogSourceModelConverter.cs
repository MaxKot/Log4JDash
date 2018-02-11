using System;
using System.ComponentModel;
using System.Globalization;

namespace Log4JDash.Web.Models
{
    public sealed class LogSourceModelConverter : TypeConverter
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

            if (value is string s)
            {
                var separator = s.LastIndexOf ('*');
                if (separator >= 0)
                {
                    var id = s.Substring (0, separator);
                    var snapshot = s.Substring (separator + 1);

                    return new LogSourceModel (id, snapshot);
                }
                else
                {
                    return new LogSourceModel (s);
                }
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
                var logSource = (LogSourceModel) value;
                return String.Format ("{0}*{1}", logSource.Id, logSource.Snapshot);
            }

            return base.ConvertTo (context, culture, value, destinationType);
        }
    }
}
