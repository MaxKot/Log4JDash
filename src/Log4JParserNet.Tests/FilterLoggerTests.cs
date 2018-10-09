using System.IO;
using System.Text;
using Log4JParserNet.Tests.NUnit;
using NUnit.Framework;

namespace Log4JParserNet.Tests
{
    [TestFixture]
    public class FilterLoggerTests
    {
        private const string SamplePreludeWin1251 = @"<?xml version=""1.0"" encoding=""windows-1251""?>";

        private const string SamplePreludeUtf8 = @"<?xml version=""1.0"" encoding=""utf8""?>";

        private const string Sample = @"
<log4j:event logger=""Root.ChildA.LoggerA2"" timestamp=""1411231353782"" level=""INFO"" thread=""Thread-1""><log4j:message>#1. Test event A.</log4j:message></log4j:event>
<log4j:event logger=""Root.ChildB.LoggerB2"" timestamp=""1411231353792"" level=""DEBUG"" thread=""Thread-2""><log4j:message>#2. Test event B.</log4j:message></log4j:event>
<log4j:event logger=""Root.ChildA.LoggerA2"" timestamp=""1411231353792"" level=""FATAL"" thread=""Thread-3""><log4j:message>#3. Test event C. С кирилицей.</log4j:message></log4j:event>
<log4j:event logger=""Root.ChildA.LoggerA1"" timestamp=""1411231353793"" level=""WARN"" thread=""Thread-4""><log4j:message>#4. Test event E.</log4j:message></log4j:event>
<log4j:event logger=""Root.ChildA.LoggerA1"" timestamp=""1411231353795"" level=""ERROR"" thread=""Thread-5""><log4j:message>#5. Test event F.</log4j:message></log4j:event>
<log4j:event logger=""Root.ЛоггерК.ЛоггерК1"" timestamp=""1411231353796"" level=""WARN"" thread=""Thread-6""><log4j:message>#6. Test event G.</log4j:message></log4j:event>
<log4j:event logger=""Root.ЛоггерК.ЛоггерК2"" timestamp=""1411231353797"" level=""ERROR"" thread=""Thread-7""><log4j:message>#7. Test event H.</log4j:message></log4j:event>
";

        private static readonly byte[] sampleBytesWin1251 = Encoding.GetEncoding (1251).GetBytes (SamplePreludeWin1251 + Sample);

        private static readonly byte[] sampleBytesUtf8 = Encoding.UTF8.GetBytes (SamplePreludeUtf8 + Sample);

        [Test]
        [VerifyLog4JAllocator]
        public void FiltersEventsWithLoggerStartingWithPrefix ()
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
                    Level = Level.Fatal,
                    Logger = "Root.ChildA.LoggerA2",
                    Thread = "Thread-3",
                    Timestamp = 1411231353792L,
                    Message = "#3. Test event C. С кирилицей.",
                    Throwable = null,
                    Id = 329
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
                    Id = 671
                }
            };

            var subject = Filter.Logger ("Root.ChildA");

            using (var sourceStream = new MemoryStream (sampleBytesWin1251))
            using (var source = Log4JFile.Create (sourceStream))
            {
                source.Encoding = Encoding.GetEncoding (1251);
                var actual = source.GetEvents ().Where (subject);
                Assert.That (actual, Is.EqualTo (expected));
            }
        }

        [Test]
        [VerifyLog4JAllocator]
        public void FiltersEventsWithLoggerStartingWithNonAsciiPrefixInWindows1251 ()
        {
            var expected = new[]
            {
                new EventExpectation
                {
                    Level = Level.Warn,
                    Logger = "Root.ЛоггерК.ЛоггерК1",
                    Thread = "Thread-6",
                    Timestamp = 1411231353796L,
                    Message = "#6. Test event G.",
                    Throwable = null,
                    Id = 836
                },
                new EventExpectation
                {
                    Level = Level.Error,
                    Logger = "Root.ЛоггерК.ЛоггерК2",
                    Thread = "Thread-7",
                    Timestamp = 1411231353797L,
                    Message = "#7. Test event H.",
                    Throwable = null,
                    Id = 1001
                }
            };

            var subject = Filter.Logger ("Root.ЛоггерК");

            using (var sourceStream = new MemoryStream (sampleBytesWin1251))
            using (var source = Log4JFile.Create (sourceStream))
            {
                source.Encoding = Encoding.GetEncoding (1251);
                var actual = source.GetEvents ().Where (subject);
                Assert.That (actual, Is.EqualTo (expected));
            }
        }

        [Test]
        [VerifyLog4JAllocator]
        public void FiltersEventsWithLoggerStartingWithNonAsciiPrefixInUtf8 ()
        {
            var expected = new[]
            {
                new EventExpectation
                {
                    Level = Level.Warn,
                    Logger = "Root.ЛоггерК.ЛоггерК1",
                    Thread = "Thread-6",
                    Timestamp = 1411231353796L,
                    Message = "#6. Test event G.",
                    Throwable = null,
                    Id = 846
                },
                new EventExpectation
                {
                    Level = Level.Error,
                    Logger = "Root.ЛоггерК.ЛоггерК2",
                    Thread = "Thread-7",
                    Timestamp = 1411231353797L,
                    Message = "#7. Test event H.",
                    Throwable = null,
                    Id = 1025
                }
            };

            var subject = Filter.Logger ("Root.ЛоггерК");

            using (var sourceStream = new MemoryStream (sampleBytesUtf8))
            using (var source = Log4JFile.Create (sourceStream))
            {
                source.Encoding = Encoding.UTF8;
                var actual = source.GetEvents ().Where (subject);
                Assert.That (actual, Is.EqualTo (expected));
            }
        }

        [Test]
        [VerifyLog4JAllocator]
        public void FiltersEventsWithLoggerExactMatch ()
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
                    Id = 671
                }
            };

            var subject = Filter.Logger ("Root.ChildA.LoggerA1");

            using (var sourceStream = new MemoryStream (sampleBytesWin1251))
            using (var source = Log4JFile.Create (sourceStream))
            {
                source.Encoding = Encoding.GetEncoding (1251);
                var actual = source.GetEvents ().Where (subject);
                Assert.That (actual, Is.EqualTo (expected));
            }
        }

        [Test]
        [VerifyLog4JAllocator]
        public void FiltersEventsWithLoggerExactMatchWithNonAsciiCharactersInWindows1251 ()
        {
            var expected = new[]
            {
                new EventExpectation
                {
                    Level = Level.Warn,
                    Logger = "Root.ЛоггерК.ЛоггерК1",
                    Thread = "Thread-6",
                    Timestamp = 1411231353796L,
                    Message = "#6. Test event G.",
                    Throwable = null,
                    Id = 836
                }
            };

            var subject = Filter.Logger ("Root.ЛоггерК.ЛоггерК1");

            using (var sourceStream = new MemoryStream (sampleBytesWin1251))
            using (var source = Log4JFile.Create (sourceStream))
            {
                source.Encoding = Encoding.GetEncoding (1251);
                var actual = source.GetEvents ().Where (subject);
                Assert.That (actual, Is.EqualTo (expected));
            }
        }

        [Test]
        [VerifyLog4JAllocator]
        public void FiltersEventsWithLoggerExactMatchWithNonAsciiCharactersInUtf8 ()
        {
            var expected = new[]
            {
                new EventExpectation
                {
                    Level = Level.Warn,
                    Logger = "Root.ЛоггерК.ЛоггерК1",
                    Thread = "Thread-6",
                    Timestamp = 1411231353796L,
                    Message = "#6. Test event G.",
                    Throwable = null,
                    Id = 846
                }
            };

            var subject = Filter.Logger ("Root.ЛоггерК.ЛоггерК1");

            using (var sourceStream = new MemoryStream (sampleBytesUtf8))
            using (var source = Log4JFile.Create (sourceStream))
            {
                source.Encoding = Encoding.UTF8;
                var actual = source.GetEvents ().Where (subject);
                Assert.That (actual, Is.EqualTo (expected));
            }
        }

        [Test]
        public void IsEqualToSameFilter ()
        {
            var subjectA = Filter.Logger ("Root.ChildA.LoggerA1");
            var subjectB = Filter.Logger ("Root.ChildA.LoggerA1");

            var actualEquals = Equals (subjectA, subjectB);
            var actualHashCodeEquals = subjectA.GetHashCode () == subjectB.GetHashCode ();

            Assert.That (actualEquals, Is.True);
            Assert.That (actualHashCodeEquals, Is.True);
        }

        [Test]
        public void IsNotEqualToDifferentFilter ()
        {
            var subjectA = Filter.Logger ("Root.ChildA.LoggerA1");
            var subjectB = Filter.Logger ("Root.ChildA");

            var actualEquals = Equals (subjectA, subjectB);

            Assert.That (actualEquals, Is.False);
        }
    }
}
