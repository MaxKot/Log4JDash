using System;

namespace Log4JParserNet
{
    public sealed class FilterMessageBuilder : FilterBuilder
    {
        private readonly string message_;

        public FilterMessageBuilder (string message)
        {
            message_ = message;
        }

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
