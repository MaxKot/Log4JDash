using System;
using System.Collections.Generic;

namespace Log4JParserNet
{
    public sealed class FilterLevelBuilder
        : FilterBuilder
        , IEquatable<FilterLevelBuilder>
    {
        public sealed class LevelComparerImpl
            : IComparer<string>
            , IEqualityComparer<string>
        {
            private static readonly IEqualityComparer<string> EqualityComparer = StringComparer.Ordinal;

            public static readonly LevelComparerImpl Instance = new LevelComparerImpl ();

            private LevelComparerImpl ()
            {

            }

            public int Compare (string x, string y)
            {
                var xVal = Log4JParserC.Log4JGetLevelValueNt (x);
                if (xVal < 0 && xVal != Int32.MinValue)
                {
                    throw new ArgumentException ("Unrecognized log level.", nameof (x));
                }

                var yVal = Log4JParserC.Log4JGetLevelValueNt (y);
                if (yVal < 0 && yVal != Int32.MinValue)
                {
                    throw new ArgumentException ("Unrecognized log level.", nameof (y));
                }

                return Comparer<int>.Default.Compare (xVal, yVal);
            }

            public bool Equals (string x, string y)
                => EqualityComparer.Equals (x, y);

            public int GetHashCode (string obj)
                => EqualityComparer.GetHashCode (obj);
        }

        public static readonly LevelComparerImpl LevelComparer
            = LevelComparerImpl.Instance;

        public const StringComparison LevelComparison
            = StringComparison.Ordinal;

        public string Min { get; }

        public string Max { get; }

        public FilterLevelBuilder (string min, string max)
        {
            Min = min;
            Max = max;
        }

        public override bool Equals (object obj)
            => obj is FilterLevelBuilder other && Equals (other);

        public bool Equals (FilterLevelBuilder other)
            => other != null &&
               LevelComparer.Equals (Min, other.Min) &&
               LevelComparer.Equals (Max, other.Max);

        public override int GetHashCode ()
        {
            var hashCode = -320226678;
            hashCode = hashCode * -1521134295 + LevelComparer.GetHashCode (Min);
            hashCode = hashCode * -1521134295 + LevelComparer.GetHashCode (Max);
            return hashCode;
        }

        internal override HandleGraph<FilterHandle> Build ()
        {
            FilterHandle result = null;
            try
            {
                Log4JParserC.Log4JFilterInitLevelC (out result, Min, Max);
                return HandleGraph.Simple (result);
            }
            catch (Exception ex)
            {
                Disposable.DisposeAggregateErrors (result, ex);
                throw;
            }
        }

        public override void AcceptVisitor (IFilterBuilderVisitor visitor)
            => (visitor ?? throw new ArgumentNullException (nameof (visitor))).Visit (this);
    }
}
