using System.Collections.Generic;

namespace Log4JDash.Web.Domain
{
    internal interface ILogSourceProviderConfig
    {
        IReadOnlyCollection<ILogDirectoryConfig> Directories { get; }
    }
}
