using System;
using System.Collections.Generic;

namespace Log4JParserNet
{
    public sealed class FilterMessageBuilder
        : FilterBuilder
        , IEquatable<FilterMessageBuilder>
    {
        private static readonly IEqualityComparer<string> MessageComparer
            = StringComparer.Ordinal;

        private readonly string message_;

        public FilterMessageBuilder (string message)
        {
            message_ = message;
        }

        public override bool Equals (object obj)
            => obj is FilterMessageBuilder other && Equals (other);

        public bool Equals (FilterMessageBuilder other)
            => other != null && MessageComparer.Equals (message_, other.message_);

        public override int GetHashCode ()
            => 2114237065 + MessageComparer.GetHashCode (message_);

        public override Filter Build ()
        {
            FilterHandle result = null;
            try
            {
                Log4JParserC.Log4JFilterInitMessageNt (out result, message_);
                return Filter.Simple (result);
            }
            catch (Exception ex)
            {
                Disposable.DisposeAggregateErrors (result, ex);
                throw;
            }
        }
    }
}
