using System.Text;
using System.Text.RegularExpressions;

namespace Log4JDash.Web.Domain
{
    internal interface ILogDirectoryConfig
    {
        string Name { get; }

        string DirectoryPath { get; }

        Regex FilenamePattern { get; }

        string DateFormat { get; }

        Encoding Encoding { get; }
    }
}
