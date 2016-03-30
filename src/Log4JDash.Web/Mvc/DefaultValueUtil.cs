using System.Collections.Generic;
using System.Web.Mvc;

namespace Log4JDash.Web.Mvc
{
    public static class DefaultValueUtil
    {
        public const string MetadataKey = "DefaultValueUtil.Value";

        public static void PopulateHtmlAttributes (ModelMetadata modelMetadata, IDictionary<string, object> htmlAttributes)
        {
            object value;
            if (modelMetadata.AdditionalValues.TryGetValue (MetadataKey, out value))
            {
                htmlAttributes.Add ("data-default", value);
            }
        }
    }
}
