using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Log4JParserNet;

namespace Log4JDash.Web.Domain
{
    internal abstract class EventGroupFilter
    {
        private sealed class Level : EventGroupFilter
        {
            private readonly string min_;

            private readonly string max_;

            public Level (string min, string max)
            {
                min_ = min;
                max_ = max;
            }

            public override bool Apply (LogFileStats.EventGroupKey groupKey)
                => FilterLevelBuilder.LevelComparer.Compare (min_, groupKey.Level) <= 0 &&
                   FilterLevelBuilder.LevelComparer.Compare(groupKey.Level, max_) <= 0;
        }

        private sealed class Logger : EventGroupFilter
        {
            private readonly string logger_;

            public Logger (string logger)
            {
                logger_ = logger;
            }

            public override bool Apply (LogFileStats.EventGroupKey groupKey)
                => groupKey.Logger.StartsWith (logger_, FilterLoggerBuilder.LoggerComparison);
        }

        private sealed class Not : EventGroupFilter
        {
            private readonly EventGroupFilter wrapped_;

            public Not (EventGroupFilter wrapped)
            {
                Debug.Assert (wrapped != null, "EventGroupFilter.All.ctor: wrapped is null.");
                wrapped_ = wrapped;
            }

            public override bool Apply (LogFileStats.EventGroupKey groupKey)
                => !wrapped_.Apply (groupKey);
        }

        private sealed class All : EventGroupFilter
        {
            private readonly IReadOnlyCollection<EventGroupFilter> children_;

            public All (IReadOnlyCollection<EventGroupFilter> children)
            {
                Debug.Assert (children != null, "EventGroupFilter.All.ctor: children is null.");
                children_ = children;
            }

            public override bool Apply (LogFileStats.EventGroupKey groupKey)
                => children_.All (c => c.Apply (groupKey));
        }

        private sealed class Any : EventGroupFilter
        {
            private readonly IReadOnlyCollection<EventGroupFilter> children_;

            public Any (IReadOnlyCollection<EventGroupFilter> children)
            {
                Debug.Assert (children != null, "EventGroupFilter.Any.ctor: children is null.");
                children_ = children;
            }

            public override bool Apply (LogFileStats.EventGroupKey groupKey)
                => children_.Any (c => c.Apply (groupKey));
        }

        private sealed class MatchAll : EventGroupFilter
        {
            public static EventGroupFilter Instance { get; } = new MatchAll ();

            private MatchAll ()
            {

            }

            public override bool Apply (LogFileStats.EventGroupKey groupKey)
                => true;
        }

        private sealed class DoConvert
            : IFilterBuilderVisitor
        {
            public static EventGroupFilter Apply (FilterBuilder filter)
            {
                EventGroupFilter result;
                if (filter != null)
                {
                    var finder = new DoConvert ();
                    filter.AcceptVisitor (finder);

                    result = finder.lastResult_;
                }
                else
                {
                    result = null;
                }

                return result;
            }

            private EventGroupFilter lastResult_;

            private DoConvert ()
            {

            }

            void IFilterBuilderVisitor.Visit (FilterAllBuilder filter)
            {
                var childrenFilters = filter.Children
                    .Select (Apply)
                    .Where (c => c != null)
                    .ToList ();

                lastResult_ = childrenFilters.Any ()
                    ? new All (childrenFilters)
                    : null;
            }

            void IFilterBuilderVisitor.Visit (FilterAnyBuilder filter)
            {
                var childrenFilters = filter.Children
                    .Select (Apply)
                    .Where (c => c != null)
                    .ToList ();

                lastResult_ = childrenFilters.Any ()
                    ? new Any (childrenFilters)
                    : null;
            }

            void IFilterBuilderVisitor.Visit (FilterNotBuilder filter)
            {
                var childResult = Apply (filter.Child);

                lastResult_ = childResult != null
                    ? new Not (childResult)
                    : null;
            }

            void IFilterBuilderVisitor.Visit (FilterLevelBuilder filter)
                => lastResult_ = new Level (filter.Min, filter.Max);

            void IFilterBuilderVisitor.Visit (FilterLoggerBuilder filter)
                => lastResult_ = new Logger (filter.Logger);

            void IFilterBuilderVisitor.Visit (FilterMessageBuilder filter)
                => lastResult_ = null;

            void IFilterBuilderVisitor.Visit (FilterTimestampBuilder filter)
                => lastResult_ = null;
        }

        private EventGroupFilter ()
        {

        }

        public abstract bool Apply (LogFileStats.EventGroupKey groupKey);

        public static EventGroupFilter Convert (FilterBuilder filter)
            => DoConvert.Apply (filter) ?? MatchAll.Instance;
    }
}
