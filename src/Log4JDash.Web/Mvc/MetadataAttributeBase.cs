using System;
using System.ComponentModel;
using System.Web.Mvc;

namespace Log4JDash.Web.Mvc
{
    internal abstract class MetadataAttributeBase : Attribute
    {
        internal abstract void GetMetadataForProperty
            (
            Func<object> modelAccessor,
            Type containerType,
            PropertyDescriptor propertyDescriptor,
            ModelMetadata result
            );
    }
}
