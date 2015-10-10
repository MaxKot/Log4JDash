using System.IO;

namespace Log4JDash.Web.Domain
{
    internal sealed class LogSource
    {
        public string Name
        { get; }

        private readonly string file_;

        public LogSource (string name, string file)
        {
            Name = name;
            file_ = file;
        }

        public Stream Open ()
        {
            return new FileStream (file_, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }
    }
}
