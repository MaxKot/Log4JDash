using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Log4JParserNet;

namespace Log4JDash.Web.Domain
{
    internal sealed class LogFileStatsCache
    {
        private sealed class FindUnstatable
            : IFilterVisitor
        {
            public static Filter Apply (Filter filter)
            {
                Filter result;
                if (filter != null)
                {
                    var finder = new FindUnstatable ();
                    filter.AcceptVisitor (finder);

                    result = finder.lastResult_;
                }
                else
                {
                    result = null;
                }

                return result;
            }

            private Filter lastResult_;

            private FindUnstatable ()
            {

            }

            void IFilterVisitor.Visit (FilterAll filter)
            {
                var childrenFilters = filter.Children
                    .Select (Apply)
                    .Where (c => c != null)
                    .ToList ();

                lastResult_ = childrenFilters.Any ()
                    ? Filter.All (childrenFilters)
                    : null;
            }

            void IFilterVisitor.Visit (FilterAny filter)
            {
                var childrenFilters = filter.Children
                    .Select (Apply)
                    .Where (c => c != null)
                    .ToList ();

                lastResult_ = childrenFilters.Any ()
                    ? Filter.Any (childrenFilters)
                    : null;
            }

            void IFilterVisitor.Visit (FilterNot filter)
            {
                var childResult = Apply (filter.Child);

                lastResult_ = childResult != null
                    ? Filter.Not (childResult)
                    : null;
            }

            void IFilterVisitor.Visit (FilterLevel filter)
                => lastResult_ = null;

            void IFilterVisitor.Visit (FilterLogger filter)
                => lastResult_ = null;

            void IFilterVisitor.Visit (FilterMessage filter)
                => lastResult_ = filter;

            void IFilterVisitor.Visit (FilterTimestamp filter)
                => lastResult_ = null;
        }

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

        private ConcurrentDictionary<Key, LogFileStats> impl_
            = new ConcurrentDictionary<Key, LogFileStats> (KeyComparer.Instance);

        public LogFileStats GetStats (ILogFile logFile, Filter filter)
            => impl_.GetOrAdd (new Key (logFile, filter), k => LogFileStats.GatherStats (logFile, k.UnstatableFilter));

        public static LogFileStatsCache Default { get; } = new LogFileStatsCache ();
    }
}
