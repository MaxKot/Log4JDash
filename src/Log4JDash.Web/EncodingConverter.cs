using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Text;

namespace Log4JDash.Web
{
    public class EncodingConverter : TypeConverter
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
                var name = (string) value;
                return Encoding.GetEncoding (name);
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

            if (destinationType == typeof (string) && value is Encoding)
            {
                var encoding = (Encoding) value;
                return encoding.WebName;
            }
            if (destinationType == typeof (InstanceDescriptor) && value is Encoding)
            {
                var encoding = (Encoding) value;
                var ctor = typeof (Encoding).GetMethod ("GetEncoding", new[] { typeof (string) });
                if (ctor != null)
                {
                    return new InstanceDescriptor (ctor, new object[] { encoding.WebName });
                }
            }

            return base.ConvertTo (context, culture, value, destinationType);
        }
    }
}
