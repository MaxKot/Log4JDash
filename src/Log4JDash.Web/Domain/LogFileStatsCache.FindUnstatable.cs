using System.Linq;
using Log4JParserNet;

namespace Log4JDash.Web.Domain
{
    internal sealed partial class LogFileStatsCache
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
    }
}
