
namespace Log4JParserNet
{
    public interface IFilterBuilderVisitor
    {
        void Visit (FilterAllBuilder filter);

        void Visit (FilterAnyBuilder filter);

        void Visit (FilterNotBuilder filter);

        void Visit (FilterLevelBuilder filter);

        void Visit (FilterLoggerBuilder filter);

        void Visit (FilterMessageBuilder filter);

        void Visit (FilterTimestampBuilder filter);
    }
}
