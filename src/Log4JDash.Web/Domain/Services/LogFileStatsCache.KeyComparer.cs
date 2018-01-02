using System.Collections.Generic;

namespace Log4JDash.Web.Domain.Services
{
    internal sealed partial class LogFileStatsCache
    {
        private sealed class KeyComparer : IEqualityComparer<Key>
        {
            public bool Equals (Key x, Key y)
                => x.Equals (y);

            public int GetHashCode (Key obj)
                => obj.GetHashCode ();

            private KeyComparer ()
            {

            }

            public static IEqualityComparer<Key> Instance { get; }
                = new KeyComparer ();
        }
    }
}
