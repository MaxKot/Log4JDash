using System;
using System.Text;
using NUnit.Framework;

namespace Log4JParserNet.Tests
{
    [TestFixture]
    public class FilterAllTests
    {
        [Test]
        public void MatchesAllInnerFilters ()
        {
            var expected = new[]
            {
                new EventExpectation
                {
                    Level = Level.Debug,
                    Logger = "Root.ChildB.LoggerB2",
                    Thread = "Thread-2",
                    Timestamp = 1411231353792L,
                    Message = "#2. Test event B.",
                    Throwable = null,
                    Id = 479
                }
            };

            using (var source = Log4JFile.Create ("sample-1.xml"))
            using (var childFilter1 = new FilterLevel (Level.MinValue, Level.Info))
            using (var childFilter2 = new FilterTimestamp (1411231353792L, Int64.MaxValue))
            using (var subject = new FilterAll ())
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
