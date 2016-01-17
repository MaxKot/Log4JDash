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

            var s = value as string;
            if (s != null)
            {
                var separator = s.LastIndexOf ('*');
                if (separator >= 0)
                {
                    var id = s.Substring (0, separator);

                    var sizeString = s.Substring (separator + 1);
                    long sizeLong;
                    var size = Int64.TryParse (sizeString, out sizeLong)
                        ? (long?) sizeLong
                        : null;

                    return new LogSourceModel (id, size);
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
                return String.Format ("{0}*{1}", logSource.Id, logSource.Size);
            }

            return base.ConvertTo (context, culture, value, destinationType);
        }
    }
}
