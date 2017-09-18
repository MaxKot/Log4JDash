using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Log4JParserNet;

namespace Log4JDash.Web.Domain
{
    internal sealed class LogFileStatsCache : ILogFileStatsProvider
    {
        private sealed class FindUnstatable
            : IFilterBuilderVisitor
        {
            public static FilterBuilder Apply (FilterBuilder filter)
            {
                FilterBuilder result;
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

            private FilterBuilder lastResult_;

            private FindUnstatable ()
            {

            }

            void IFilterBuilderVisitor.Visit (FilterAllBuilder filter)
            {
                var childrenFilters = filter.Children
                    .Select (Apply)
                    .Where (c => c != null)
                    .ToList ();

                lastResult_ = childrenFilters.Any ()
                    ? FilterBuilder.All (childrenFilters)
                    : null;
            }

            void IFilterBuilderVisitor.Visit (FilterAnyBuilder filter)
            {
                var childrenFilters = filter.Children
                    .Select (Apply)
                    .Where (c => c != null)
                    .ToList ();

                lastResult_ = childrenFilters.Any ()
                    ? FilterBuilder.Any (childrenFilters)
                    : null;
            }

            void IFilterBuilderVisitor.Visit (FilterNotBuilder filter)
            {
                var childResult = Apply (filter.Child);

                lastResult_ = childResult != null
                    ? FilterBuilder.Not (childResult)
                    : null;
            }

            void IFilterBuilderVisitor.Visit (FilterLevelBuilder filter)
                => lastResult_ = null;

            void IFilterBuilderVisitor.Visit (FilterLoggerBuilder filter)
                => lastResult_ = null;

            void IFilterBuilderVisitor.Visit (FilterMessageBuilder filter)
                => lastResult_ = filter;

            void IFilterBuilderVisitor.Visit (FilterTimestampBuilder filter)
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

            private readonly FilterBuilder unstatableFilter_;

            public Key (string fileName, long size, FilterBuilder filter)
            {
                fileName_ = fileName;
                size_ = size;
                unstatableFilter_ = FindUnstatable.Apply (filter);
            }

            public Key (LazyLogFile logFile, FilterBuilder filter)
                : this (logFile.FileName, logFile.Size, filter)
            {

            }

            public override bool Equals (object obj)
                => obj is Key && Equals ((Key) obj);

            public bool Equals (Key other)
                => FileNameComparer.Equals (fileName_, other.fileName_) &&
                   size_ == other.size_ &&
                   Equals (unstatableFilter_, other.unstatableFilter_);

            public override int GetHashCode ()
            {
                var hashCode = 881837526;
                hashCode = hashCode * -1521134295 + base.GetHashCode ();
                hashCode = hashCode * -1521134295 + (fileName_ != null ? FileNameComparer.GetHashCode (fileName_) : 0);
                hashCode = hashCode * -1521134295 + size_.GetHashCode ();
                hashCode = hashCode * -1521134295 + (unstatableFilter_ != null ? unstatableFilter_.GetHashCode () : 0);
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

        public LogFileStats GetStats (LazyLogFile logFile, FilterBuilder filter)
            => impl_.GetOrAdd (new Key (logFile, filter), _ => LogFileStats.GatherStats (logFile));

        public static LogFileStatsCache Default { get; } = new LogFileStatsCache ();
    }
}
