using System;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;

namespace Log4JDash.Web.Mvc
{
    internal sealed class MyModelMetadataProvider : DataAnnotationsModelMetadataProvider
    {
        protected override ModelMetadata GetMetadataForProperty
            (Func<object> modelAccessor, Type containerType, PropertyDescriptor propertyDescriptor)
        {
            var result =
                base.GetMetadataForProperty (modelAccessor, containerType, propertyDescriptor);

            var attributes = propertyDescriptor.Attributes.OfType<MetadataAttributeBase> ();
            foreach (var attribute in attributes)
            {
                attribute.GetMetadataForProperty
                    (modelAccessor, containerType, propertyDescriptor, result);
            }

            return result;
        }
    }
}
