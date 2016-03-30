using System.Configuration;

namespace Log4JDash.Web.Mvc
{
    public class RequireHttpsSection
        : ConfigurationSection
        , IRequireHttpsConfig
    {
        #region AllowHttp

        /// <summary>The name of the configuration tag attribute defining <see cref="AllowHttp" />.</summary>
        private const string AllowHttpAttribute = "allowHttp";

        [ConfigurationProperty (AllowHttpAttribute, IsRequired = false, DefaultValue = false)]
        public bool AllowHttp
        {
            get { return (bool) base[AllowHttpAttribute]; }
            set { base[AllowHttpAttribute] = value; }
        }

        #endregion

        #region HttpsPort

        /// <summary>The name of the configuration tag attribute defining <see cref="HttpsPort" />.</summary>
        private const string HttpsPortAttribute = "httpsPort";

        [ConfigurationProperty (HttpsPortAttribute, IsRequired = false, DefaultValue = null)]
        public int? HttpsPort
        {
            get { return (int?) base[HttpsPortAttribute]; }
            set { base[HttpsPortAttribute] = value; }
        }

        #endregion
    }
}
