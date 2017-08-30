using System.ComponentModel;
using System.Configuration;
using System.Text;
using System.Text.RegularExpressions;

namespace Log4JDash.Web.Domain
{
    public class LogDirectoryElement
        : ConfigurationElement
        , ILogDirectoryConfig
    {
        #region Name

        /// <summary>The name of the configuration tag attribute defining <see cref="Name" />.</summary>
        private const string NameAttribute = "name";

        /// <inheritdoc />
        [ConfigurationProperty (NameAttribute, IsRequired = true)]
        public string Name
        {
            get => (string) base[NameAttribute];
            set => base[NameAttribute] = value;
        }

        #endregion

        #region DirectoryPath

        /// <summary>The name of the configuration tag attribute defining <see cref="DirectoryPath" />.</summary>
        private const string DirectoryPathAttribute = "directoryPath";

        /// <inheritdoc />
        [ConfigurationProperty (DirectoryPathAttribute, IsRequired = true)]
        public string DirectoryPath
        {
            get => (string) base[DirectoryPathAttribute];
            set => base[DirectoryPathAttribute] = value;
        }

        #endregion

        #region FilenamePattern

        /// <summary>The name of the configuration tag attribute defining <see cref="FilenamePattern" />.</summary>
        private const string FilenamePatternAttribute = "filenamePattern";

        /// <inheritdoc />
        [ConfigurationProperty (FilenamePatternAttribute, IsRequired = false, DefaultValue = @".+\.xml")]
        [TypeConverter (typeof (RegexConverter))]
        public Regex FilenamePattern
        {
            get => (Regex) base[FilenamePatternAttribute];
            set => base[FilenamePatternAttribute] = value;
        }

        #endregion

        #region DateFormat

        /// <summary>The name of the configuration tag attribute defining <see cref="DateFormat" />.</summary>
        private const string DateFormatAttribute = "dateFormat";

        /// <inheritdoc />
        [ConfigurationProperty (DateFormatAttribute, IsRequired = false, DefaultValue = null)]
        public string DateFormat
        {
            get => (string) base[DateFormatAttribute];
            set => base[DateFormatAttribute] = value;
        }

        #endregion

        #region Encoding

        /// <summary>The name of the configuration tag attribute defining <see cref="Encoding" />.</summary>
        private const string EncodingAttribute = "encoding";

        /// <inheritdoc />
        [ConfigurationProperty (EncodingAttribute, IsRequired = false, DefaultValue = "ASCII")]
        [TypeConverter (typeof (EncodingConverter))]
        public Encoding Encoding
        {
            get => (Encoding) base[EncodingAttribute];
            set => base[EncodingAttribute] = value;
        }

        #endregion
    }
}
