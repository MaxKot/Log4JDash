using System.IO;
using System.Text;
using Log4JParserNet.Tests.NUnit;
using NUnit.Framework;

namespace Log4JParserNet.Tests
{
    [TestFixture]
    public class FilterMessageTests
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
        [VerifyLog4JAllocator]
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
                    Id = 507
                }
            };

            var subject = Filter.Message ("nt E");

            using (var sourceStream = new MemoryStream (sampleBytes))
            using (var source = Log4JFile.Create (sourceStream))
            {
                source.Encoding = Encoding.GetEncoding (1251);
                var actual = source.GetEvents ().Where (subject);
                Assert.That (actual, Is.EqualTo (expected));
            }
        }

        [Test]
        public void IsEqualToSameFilter ()
        {
            var subjectA = Filter.Message ("nt E");
            var subjectB = Filter.Message ("nt E");

            var actualEquals = Equals (subjectA, subjectB);
            var actualHashCodeEquals = subjectA.GetHashCode () == subjectB.GetHashCode ();

            Assert.That (actualEquals, Is.True);
            Assert.That (actualHashCodeEquals, Is.True);
        }

        [Test]
        public void IsNotEqualToDifferentFilter ()
        {
            var subjectA = Filter.Message ("nt E");
            var subjectB = Filter.Message ("nt");

            var actualEquals = Equals (subjectA, subjectB);

            Assert.That (actualEquals, Is.False);
        }
    }
}
