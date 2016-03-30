using System.Configuration;
using Log4JDash.Web.Domain;
using Log4JDash.Web.Mvc;

namespace Log4JDash.Web.Configuration
{
    internal static class KnownSections
    {
        public const string LogSourceProviderName = "log4JDash/logSourceProvider";

        public static LogSourceProviderSection LogSourceProvider ()
        {
            var section = ConfigurationManager.GetSection (LogSourceProviderName);
            ValidateSection (section, LogSourceProviderName);

            return (LogSourceProviderSection) section;
        }

        public const string RequireHttpsName = "log4JDash/requireHttps";

        public static RequireHttpsSection RequireHttps ()
        {
            var section = (RequireHttpsSection) ConfigurationManager.GetSection (RequireHttpsName);

            return section ?? new RequireHttpsSection ();
        }

        private static void ValidateSection (object section, string sectionName)
        {
            if (section == null)
            {
                var message = $"Mandatory configuration section {sectionName} is missing.";
                throw new ConfigurationErrorsException (message);
            }
        }
    }
}
