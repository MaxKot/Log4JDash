using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Log4JDash.Web.Domain
{
    public class RegexConverter : TypeConverter
    {
        /// <inheritdoc />
        public override bool CanConvertFrom (ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof (string))
            {
                return true;
            }

            return base.CanConvertFrom (context, sourceType);
        }

        /// <inheritdoc />
        public override bool CanConvertTo (ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof (InstanceDescriptor))
            {
                return true;
            }
            return base.CanConvertTo (context, destinationType);
        }

        /// <inheritdoc />
        public override object ConvertFrom (ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                var text = (string) value;

                return new Regex (text);
            }

            return base.ConvertFrom (context, culture, value);
        }

        /// <inheritdoc />
        public override object ConvertTo (ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException ("destinationType");
            }

            if (destinationType == typeof (string) && value is Regex)
            {
                var regex = (Regex) value;
                return regex.ToString ();
            }
            if (destinationType == typeof (InstanceDescriptor) && value is CultureInfo)
            {
                var regex = (Regex) value;
                var ctor = typeof (Regex).GetConstructor (new[] { typeof (string) });
                if (ctor != null)
                {
                    return new InstanceDescriptor (ctor, new object[] { regex.ToString () });
                }
            }

            return base.ConvertTo (context, culture, value, destinationType);
        }
    }
}
