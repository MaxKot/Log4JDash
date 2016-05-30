using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Log4JDash.Web.Mvc
{
    public static class DefaultValueUtil
    {
        public const string MetadataKey = "DefaultValueUtil.Value";

        public const string HtmlAttibute = "data-default";

        public static object GetDefaultValue (ModelMetadata modelMetadata)
        {
            object result;

            object rawValue;
            if (!modelMetadata.AdditionalValues.TryGetValue (MetadataKey, out rawValue))
            {
                result = null;
            }
            else if (rawValue == null)
            {
                result = modelMetadata.NullDisplayText;
            }
            else if (modelMetadata.EditFormatString != null)
            {
                result = String.Format (modelMetadata.EditFormatString, rawValue);
            }
            else
            {
                result = rawValue;
            }

            return result;
        }

        public static void PopulateHtmlAttributes (ModelMetadata modelMetadata, IDictionary<string, object> htmlAttributes)
        {
            var attributeValue = GetDefaultValue (modelMetadata);
            htmlAttributes.Add (HtmlAttibute, attributeValue);
        }
    }
}
