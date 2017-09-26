using System.Collections.Generic;

namespace Log4JParserNet
{
    public interface IEnumerableOfEvents : IEnumerable<Event>
    {
        IEnumerableOfEvents Where (FilterBuilder filter);
    }
}
