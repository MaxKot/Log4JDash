using System;
using System.Collections.Generic;
using Log4JParserNet;

namespace Log4JDash.Web.Domain
{
    internal sealed partial class LogFileStatsCache
    {
        private struct Key : IEquatable<Key>
        {
            // NOTE: Using case-sensitive comparison regardless of actual filesystem for 2 reasons:
            // 1) recomputing statistics only impacts perfomance and does not cause faulty
            // bahavior; hence it is safer;
            // 2) assuming that filenames are acquired using Directory.GetFiles or similar methods,
            // case of the character in filenames should not change between calls.
            private static readonly IEqualityComparer<string> FileNameComparer
                = StringComparer.Ordinal;

            private readonly string fileName_;

            private readonly long size_;

            public Filter UnstatableFilter { get; }

            public Key (string fileName, long size, Filter filter)
            {
                fileName_ = fileName;
                size_ = size;
                UnstatableFilter = FindUnstatable.Apply (filter);
            }

            public Key (ILogFile logFile, Filter filter)
                : this (logFile.FileName, logFile.Size, filter)
            {

            }

            public override bool Equals (object obj)
                => obj is Key && Equals ((Key) obj);

            public bool Equals (Key other)
                => FileNameComparer.Equals (fileName_, other.fileName_) &&
                   size_ == other.size_ &&
                   Equals (UnstatableFilter, other.UnstatableFilter);

            public override int GetHashCode ()
            {
                var hashCode = 881837526;
                hashCode = hashCode * -1521134295 + base.GetHashCode ();
                hashCode = hashCode * -1521134295 + (fileName_ != null ? FileNameComparer.GetHashCode (fileName_) : 0);
                hashCode = hashCode * -1521134295 + size_.GetHashCode ();
                hashCode = hashCode * -1521134295 + (UnstatableFilter != null ? UnstatableFilter.GetHashCode () : 0);
                return hashCode;
            }
        }
    }
}
