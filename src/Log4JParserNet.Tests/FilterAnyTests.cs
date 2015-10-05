using System.Text;
using NUnit.Framework;

namespace Log4JParserNet.Tests
{
    [TestFixture]
    public class FilterAnyTests
    {
        [Test]
        public void MatchesAnyInnerFilters ()
        {
            var expected = new[]
            {
                new EventExpectation
                {
                    Level = Level.Info,
                    Logger = "Root.ChildA.LoggerA2",
                    Thread = "Thread-1",
                    Timestamp = 1411231353782L,
                    Message = "#1. Test event A.",
                    Throwable = null,
                    Id = 0
                },
                new EventExpectation
                {
                    Level = Level.Debug,
                    Logger = "Root.ChildB.LoggerB2",
                    Thread = "Thread-2",
                    Timestamp = 1411231353792L,
                    Message = "#2. Test event B.",
                    Throwable = null,
                    Id = 479
                },
                new EventExpectation
                {
                    Level = Level.Fatal,
                    Logger = "Root.ChildA.LoggerA2",
                    Thread = "Thread-3",
                    Timestamp = 1411231353792L,
                    Message = "#3. Test event C. С кирилицей.",
                    Throwable = null,
                    Id = 958
                }
            };

            using (var source = new Log4JFile ("sample-1.xml"))
            using (var childFilter1 = new FilterLevel (Level.MinValue, Level.Info))
            using (var childFilter2 = new FilterTimestamp (1411231353792L, 1411231353792L))
            using (var subject = new FilterAny ())
            {
                source.Encoding = Encoding.GetEncoding (1251);

                subject.Add (childFilter1);
                subject.Add (childFilter2);

                var actual = source.GetEvents ().Where (subject);
                Assert.That (actual, Is.EqualTo (expected));
            }
        }
    }
}
