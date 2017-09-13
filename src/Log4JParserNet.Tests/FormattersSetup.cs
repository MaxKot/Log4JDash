using System.Collections.Generic;
using NUnit.Framework;

namespace Log4JParserNet.Tests
{
    [SetUpFixture]
    public class FormattersSetup
    {
        [OneTimeSetUp]
        public void SetUp ()
        {
            TestContext.AddFormatter<EventExpectation> (o => Formatter.Format ((EventExpectation) o));
            TestContext.AddFormatter<EventExpectation[]> (o => Formatter.Format ((EventExpectation[]) o));
            TestContext.AddFormatter<Event> (o => Formatter.Format ((Event) o));
            TestContext.AddFormatter<IEnumerable<Event>> (o => Formatter.Format ((IEnumerable<Event>) o));
        }
    }
}
