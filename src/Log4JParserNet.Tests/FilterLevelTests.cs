using System.IO;
using System.Text;
using NUnit.Framework;

namespace Log4JParserNet.Tests
{
    [TestFixture]
    public class FilterLevelTests
    {
        private const string Sample = @"<?xml version=""1.0"" encoding=""windows-1251""?>
<log4j:event logger=""Root.ChildA.LoggerA2"" timestamp=""1411231353782"" level=""INFO"" thread=""Thread-1""><log4j:message>#1. Test event A.</log4j:message></log4j:event>
<log4j:event logger=""Root.ChildB.LoggerB2"" timestamp=""1411231353792"" level=""DEBUG"" thread=""Thread-2""><log4j:message>#2. Test event B.</log4j:message></log4j:event>
<log4j:event logger=""Root.ChildA.LoggerA2"" timestamp=""1411231353792"" level=""FATAL"" thread=""Thread-3""><log4j:message>#3. Test event C. С кирилицей.</log4j:message></log4j:event>
<log4j:event logger=""Root.ChildA.LoggerA1"" timestamp=""1411231353793"" level=""WARN"" thread=""Thread-4""><log4j:message>#4. Test event E.</log4j:message></log4j:event>
<log4j:event logger=""Root.ChildA.LoggerA1"" timestamp=""1411231353795"" level=""ERROR"" thread=""Thread-5""><log4j:message>#5. Test event F.</log4j:message></log4j:event>
";

        private static readonly byte[] sampleBytes = Encoding.GetEncoding (1251).GetBytes (Sample);

        [Test]
        public void FiltersEventsWithMinLevel ()
        {
            var expected = new[]
            {
                new EventExpectation
                {
                    Level = Level.Fatal,
                    Logger = "Root.ChildA.LoggerA2",
                    Thread = "Thread-3",
                    Timestamp = 1411231353792L,
                    Message = "#3. Test event C. С кирилицей.",
                    Throwable = null,
                    Id = 330
                },
                new EventExpectation
                {
                    Level = Level.Warn,
                    Logger = "Root.ChildA.LoggerA1",
                    Thread = "Thread-4",
                    Timestamp = 1411231353793L,
                    Message = "#4. Test event E.",
                    Throwable = null,
                    Id = 507
                },
                new EventExpectation
                {
                    Level = Level.Error,
                    Logger = "Root.ChildA.LoggerA1",
                    Thread = "Thread-5",
                    Timestamp = 1411231353795L,
                    Message = "#5. Test event F.",
                    Throwable = null,
                    Id = 672
                }
            };

            using (var sourceStream = new MemoryStream (sampleBytes))
            using (var source = Log4JFile.Create (sourceStream))
            using (var subject = new FilterLevel (Level.Warn, Level.MaxValue))
            {
                source.Encoding = Encoding.GetEncoding (1251);
                var actual = source.GetEvents ().Where (subject);
                Assert.That (actual, Is.EqualTo (expected));
            }
        }

        [Test]
        public void FiltersEventsWithMaxLevel ()
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
                    Id = 165
                },
                new EventExpectation
                {
                    Level = Level.Warn,
                    Logger = "Root.ChildA.LoggerA1",
                    Thread = "Thread-4",
                    Timestamp = 1411231353793L,
                    Message = "#4. Test event E.",
                    Throwable = null,
                    Id = 507
                }
            };

            using (var sourceStream = new MemoryStream (sampleBytes))
            using (var source = Log4JFile.Create (sourceStream))
            using (var subject = new FilterLevel (Level.MinValue, Level.Warn))
            {
                source.Encoding = Encoding.GetEncoding (1251);
                var actual = source.GetEvents ().Where (subject);
                Assert.That (actual, Is.EqualTo (expected));
            }
        }

        [Test]
        public void FiltersEventsWithLevelInterval ()
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
                    Level = Level.Warn,
                    Logger = "Root.ChildA.LoggerA1",
                    Thread = "Thread-4",
                    Timestamp = 1411231353793L,
                    Message = "#4. Test event E.",
                    Throwable = null,
                    Id = 507
                },
                new EventExpectation
                {
                    Level = Level.Error,
                    Logger = "Root.ChildA.LoggerA1",
                    Thread = "Thread-5",
                    Timestamp = 1411231353795L,
                    Message = "#5. Test event F.",
                    Throwable = null,
                    Id = 672
                }
            };

            using (var sourceStream = new MemoryStream (sampleBytes))
            using (var source = Log4JFile.Create (sourceStream))
            using (var subject = new FilterLevel (Level.Info, Level.Error))
            {
                source.Encoding = Encoding.GetEncoding (1251);
                var actual = source.GetEvents ().Where (subject);
                Assert.That (actual, Is.EqualTo (expected));
            }
        }

        [Test]
        public void FiltersEventsWithLevelExactValue ()
        {
            var expected = new[]
            {
                new EventExpectation
                {
                    Level = Level.Error,
                    Logger = "Root.ChildA.LoggerA1",
                    Thread = "Thread-5",
                    Timestamp = 1411231353795L,
                    Message = "#5. Test event F.",
                    Throwable = null,
                    Id = 672
                }
            };

            using (var sourceStream = new MemoryStream (sampleBytes))
            using (var source = Log4JFile.Create (sourceStream))
            using (var subject = new FilterLevel (Level.Error, Level.Error))
            {
                source.Encoding = Encoding.GetEncoding (1251);
                var actual = source.GetEvents ().Where (subject);
                Assert.That (actual, Is.EqualTo (expected));
            }
        }
    }
}
