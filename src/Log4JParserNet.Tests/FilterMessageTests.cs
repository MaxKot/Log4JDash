using System.Text;
using NUnit.Framework;

namespace Log4JParserNet.Tests
{
    [TestFixture]
    public class FilterMessageTests
    {
        [Test]
        public void FiltersEventsWithMessageContainingString ()
        {
            var expected = new[]
            {
                new EventExpectation
                {
                    Level = Level.Warn,
                    Logger = "Root.ChildA.LoggerA1",
                    Thread = "Thread-4",
                    Timestamp = 1411231353793L,
                    Message = "#4. Test event E.",
                    Throwable = null,
                    Id = 1449
                }
            };

            using (var source = new Log4JFile ("sample-1.xml"))
            using (var subject = new FilterMessage ("nt E"))
            {
                source.Encoding = Encoding.GetEncoding (1251);
                var actual = source.GetEvents ().Where (subject);
                Assert.That (actual, Is.EqualTo (expected));
            }
        }
    }
}
