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

        new public string Message { get; }

        public FilterMessageBuilder (string message)
        {
            Message = message;
        }

        public override bool Equals (object obj)
            => obj is FilterMessageBuilder other && Equals (other);

        public bool Equals (FilterMessageBuilder other)
            => other != null && MessageComparer.Equals (Message, other.Message);

        public override int GetHashCode ()
            => 2114237065 + MessageComparer.GetHashCode (Message);

        public override Filter Build ()
        {
            FilterHandle result = null;
            try
            {
                Log4JParserC.Log4JFilterInitMessageNt (out result, Message);
                return Filter.Simple (result);
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
