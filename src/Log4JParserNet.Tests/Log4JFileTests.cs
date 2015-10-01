using NUnit.Framework;
using System.Text;

namespace Log4JParserNet.Tests
{
    [TestFixture]
    public class Log4JFileTests
    {
        [Test]
        public void EnumeratesEventsDirectly ()
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
                },
                new EventExpectation
                {
                    Level = Level.Warn,
                    Logger = "Root.ChildA.LoggerA1",
                    Thread = "Thread-4",
                    Timestamp = 1411231353792L,
                    Message = "#4. Test event E.",
                    Throwable = null,
                    Id = 1459
                },
                new EventExpectation
                {
                    Level = Level.Error,
                    Logger = "Root.ChildA.LoggerA1",
                    Thread = "Thread-5",
                    Timestamp = 1411231353793L,
                    Message = "#5. Test event F.",
                    Throwable = null,
                    Id = 1938
                }
            };

            using (var subject = new Log4JFile ("sample-1.xml"))
            {
                subject.Encoding = Encoding.GetEncoding (1251);
                var actual = subject.GetEvents ();
                Assert.That (actual, Is.EqualTo (expected));
            }
        }

        [Test]
        public void EnumeratesEventReversed ()
        {
            var expected = new[]
            {
                new EventExpectation
                {
                    Level = Level.Error,
                    Logger = "Root.ChildA.LoggerA1",
                    Thread = "Thread-5",
                    Timestamp = 1411231353793L,
                    Message = "#5. Test event F.",
                    Throwable = null,
                    Id = 1938
                },
                new EventExpectation
                {
                    Level = Level.Warn,
                    Logger = "Root.ChildA.LoggerA1",
                    Thread = "Thread-4",
                    Timestamp = 1411231353792L,
                    Message = "#4. Test event E.",
                    Throwable = null,
                    Id = 1459
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
                    Level = Level.Info,
                    Logger = "Root.ChildA.LoggerA2",
                    Thread = "Thread-1",
                    Timestamp = 1411231353782L,
                    Message = "#1. Test event A.",
                    Throwable = null,
                    Id = 0
                }
            };

            using (var subject = new Log4JFile ("sample-1.xml"))
            {
                subject.Encoding = Encoding.GetEncoding (1251);
                var actual = subject.GetEventsReverse ();
                Assert.That (actual, Is.EqualTo (expected));
            }
        }
    }
}
