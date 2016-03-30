using System;
using System.ComponentModel;
using System.Web.Mvc;

namespace Log4JDash.Web.Mvc
{
    internal sealed class DefaultValueAttribute : MetadataAttributeBase
    {
        private readonly object value_;

        public DefaultValueAttribute (object value)
        {
            value_ = value;
        }

        internal override void GetMetadataForProperty
            (Func<object> modelAccessor, Type containerType, PropertyDescriptor propertyDescriptor, ModelMetadata result)
        {
            result.AdditionalValues.Add (DefaultValueUtil.MetadataKey, value_);
        }
    }
}
