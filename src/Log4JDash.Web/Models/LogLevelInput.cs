using System.Collections.Generic;
using System.Linq;
using Log4JParserNet;

namespace Log4JDash.Web.Models
{
    public class LogLevelInput
    {
        public ICollection<string> Levels { get; set; }

        public string Value { get; set; }

        private static readonly string[] DefaultLevels = new[]
        {
            Level.Debug,
            Level.Info,
            Level.Warn,
            Level.Error,
            Level.Fatal
        };

        public LogLevelInput ()
        {
            Levels = DefaultLevels;
            Value = Levels.First ();
        }
    }
}
