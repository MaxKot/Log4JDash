using System;
using System.Collections.Generic;
using System.Text;

namespace Log4JParserNet
{
    public sealed class FilterMessage
        : Filter
        , IEquatable<FilterMessage>
    {
        public static readonly IEqualityComparer<string> MessageComparer
            = StringComparer.Ordinal;

        public const StringComparison MessageComparison
            = StringComparison.Ordinal;

        new public string Message { get; }

        public FilterMessage (string message)
        {
            Message = message;
        }

        public override bool Equals (object obj)
            => obj is FilterMessage other && Equals (other);

        public bool Equals (FilterMessage other)
            => other != null && MessageComparer.Equals (Message, other.Message);

        public override int GetHashCode ()
            => 2114237065 + MessageComparer.GetHashCode (Message);

        internal override HandleGraph<FilterHandle> Build (Encoding encoding)
        {
            if (encoding == null)
            {
                throw new ArgumentNullException (nameof (encoding));
            }

            var message = encoding.GetBytes (Message);
            var messageSize = new UIntPtr ((uint) message.Length);

            FilterHandle result = null;
            try
            {
                Log4JParserC.Log4JFilterInitMessageFs (out result, message, messageSize);
                return HandleGraph.Simple (result);
            }
            catch (Exception ex)
            {
                Disposable.DisposeAggregateErrors (result, ex);
                throw;
            }
        }

        public override void AcceptVisitor (IFilterVisitor visitor)
            => (visitor ?? throw new ArgumentNullException (nameof (visitor))).Visit (this);
    }
}
