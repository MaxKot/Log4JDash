using System.Collections.Generic;
using System.Configuration;

namespace Log4JDash.Web.Domain
{
    [ConfigurationCollection (typeof (LogDirectoryElement))]
    public class LogDirectoryElementCollection
        : ConfigurationElementCollection
        , IReadOnlyCollection<LogDirectoryElement>
    {
        /// <inheritdoc />
        protected override ConfigurationElement CreateNewElement ()
        {
            return new LogDirectoryElement ();
        }

        /// <inheritdoc />
        protected override object GetElementKey (ConfigurationElement element)
        {
            return ((LogDirectoryElement) element).Name;
        }

        /// <inheritdoc />
        new public IEnumerator<LogDirectoryElement> GetEnumerator ()
        {
            var baseEnumerator = base.GetEnumerator ();
            while (baseEnumerator.MoveNext ())
            {
                yield return (LogDirectoryElement) baseEnumerator.Current;
            }
        }
    }
}
