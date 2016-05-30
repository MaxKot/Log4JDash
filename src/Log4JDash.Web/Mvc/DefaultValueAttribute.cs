using System;
using System.Web.Mvc;

namespace Log4JDash.Web.Mvc
{
    internal sealed class DefaultValueAttribute : Attribute, IMetadataAware
    {
        private readonly object value_;

        public DefaultValueAttribute (object value)
        {
            value_ = value;
        }

        public void OnMetadataCreated (ModelMetadata metadata)
        {
            metadata.AdditionalValues.Add (DefaultValueUtil.MetadataKey, value_);
        }
    }
}
