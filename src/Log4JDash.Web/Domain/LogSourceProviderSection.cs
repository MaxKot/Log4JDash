using System.Collections.Generic;
using System.Configuration;

namespace Log4JDash.Web.Domain
{
    public class LogSourceProviderSection
        : ConfigurationSection
        , ILogSourceProviderConfig
    {
        #region Directories

        /// <summary>The name of the configuration tag attribute defining <see cref="Directories" />.</summary>
        private const string DirectoriesAttribute = "directories";

        [ConfigurationProperty (DirectoriesAttribute, IsRequired = true)]
        public LogDirectoryElementCollection Directories
        {
            get { return (LogDirectoryElementCollection) base[DirectoriesAttribute]; }
            set { base[DirectoriesAttribute] = value; }
        }

        /// <inheritdoc />
        IReadOnlyCollection<ILogDirectoryConfig> ILogSourceProviderConfig.Directories => Directories;

        #endregion
    }
}
