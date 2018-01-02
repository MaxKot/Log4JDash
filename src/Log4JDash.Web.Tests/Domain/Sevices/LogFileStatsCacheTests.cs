using System.Threading;
using Log4JDash.Web.Domain.Services;
using Log4JParserNet;
using NUnit.Framework;

namespace Log4JDash.Web.Tests.Domain.Services
{
    [TestFixture]
    public class LogFileStatsCacheTests
    {
        private const string Sample = @"<?xml version=""1.0"" encoding=""utf-8""?>
<log4j:event logger=""Root.ChildA.LoggerA2"" timestamp=""1500000001000"" level=""INFO"" thread=""Thread-1""><log4j:message>#1. Test event A.</log4j:message></log4j:event>
<log4j:event logger=""Root.ChildB.LoggerB2"" timestamp=""1500000002000"" level=""DEBUG"" thread=""Thread-2""><log4j:message>#2. Test event B.</log4j:message></log4j:event>
<log4j:event logger=""Root.ChildA.LoggerA2"" timestamp=""1500000003000"" level=""FATAL"" thread=""Thread-3""><log4j:message>#3. Test event C. С кирилицей.</log4j:message></log4j:event>
<log4j:event logger=""Root.ChildA.LoggerA1"" timestamp=""1500000004000"" level=""WARN"" thread=""Thread-4""><log4j:message>#4. Test event E.</log4j:message></log4j:event>
<log4j:event logger=""Root.ChildA.LoggerA1"" timestamp=""1500000005000"" level=""ERROR"" thread=""Thread-5""><log4j:message>#5. Test event F.</log4j:message></log4j:event>
<log4j:event logger=""Root.ChildB.LoggerB2"" timestamp=""1500000006000"" level=""DEBUG"" thread=""Thread-6""><log4j:message>#6. Test event B.</log4j:message></log4j:event>
";

        [Test]
        public void GathersStatsFromFile ()
        {
            using (var file = new StringLogFile ("sample", Sample))
            {
                var subject = new LogFileStatsCache ();
                var actual = subject.GetStats (file, null);

                Assert.That (actual.EventCount, Is.EqualTo (6));
                Assert.That (actual.GroupStats.Count, Is.EqualTo (5));
                Assert.That (actual.GroupStats[new LogFileStats.EventGroupKey ("INFO", "Root.ChildA.LoggerA2")], Is.EqualTo (1));
                Assert.That (actual.GroupStats[new LogFileStats.EventGroupKey ("DEBUG", "Root.ChildB.LoggerB2")], Is.EqualTo (2));
                Assert.That (actual.GroupStats[new LogFileStats.EventGroupKey ("FATAL", "Root.ChildA.LoggerA2")], Is.EqualTo (1));
                Assert.That (actual.GroupStats[new LogFileStats.EventGroupKey ("WARN", "Root.ChildA.LoggerA1")], Is.EqualTo (1));
                Assert.That (actual.GroupStats[new LogFileStats.EventGroupKey ("ERROR", "Root.ChildA.LoggerA1")], Is.EqualTo (1));
                Assert.That (actual.EarliestTimestamp, Is.EqualTo (1500000001000L));
                Assert.That (actual.LatestTimestamp, Is.EqualTo (1500000006000L));
            }
        }

        [Test]
        public void GatheredStatsRespectUnstatableFilters ()
        {
            using (var file = new StringLogFile ("sample", Sample))
            {
                var filter = Filter.All
                (
                    Filter.Logger ("Root.ChildA.LoggerA2"),
                    Filter.Timestamp (1500000003000L, 1500000003000L),
                    Filter.Message ("Test event B.")
                );
                var subject = new LogFileStatsCache ();
                var actual = subject.GetStats (file, filter);

                Assert.That (actual.EventCount, Is.EqualTo (2));
                Assert.That (actual.GroupStats.Count, Is.EqualTo (1));
                Assert.That (actual.GroupStats[new LogFileStats.EventGroupKey ("DEBUG", "Root.ChildB.LoggerB2")], Is.EqualTo (2));
                Assert.That (actual.EarliestTimestamp, Is.EqualTo (1500000002000L));
                Assert.That (actual.LatestTimestamp, Is.EqualTo (1500000006000L));
            }
        }

        [Test]
        public void DoesNotRereadUsedFiles ()
        {
            using (var firstAccess = new StringLogFile ("sample", Sample))
            using (var secondAccess = firstAccess.Clone ())
            {
                var subject = new LogFileStatsCache ();

                subject.GetStats (firstAccess, null);
                subject.GetStats (secondAccess, null);

                Assert.That (secondAccess.WasRead, Is.False);
            }
        }

        [Test]
        public void HintAddsFilesToCache ()
        {
            using (var file = new StringLogFile ("sample", Sample))
            {
                var subject = new LogFileStatsCache ();

                using (subject.StartPrecacheThread ())
                {
                    subject.Hint (file, null);
                    Thread.Sleep (100);

                    subject.GetStats (file, null);
                }

                Assert.That (file.WasRead, Is.False);
            }
        }
    }
}
