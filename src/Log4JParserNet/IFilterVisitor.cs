
namespace Log4JParserNet
{
    public interface IFilterVisitor
    {
        void Visit (FilterAll filter);

        void Visit (FilterAny filter);

        void Visit (FilterNot filter);

        void Visit (FilterLevel filter);

        void Visit (FilterLogger filter);

        void Visit (FilterMessage filter);

        void Visit (FilterTimestamp filter);
    }
}
