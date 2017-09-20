using System.Linq;
using Log4JDash.Web.Domain;
using Log4JParserNet;
using NUnit.Framework;

namespace Log4JDash.Web.Tests
{
    [TestFixture]
    public class LogAccumulatorTest
    {
        private const string Sample1 = @"<?xml version=""1.0"" encoding=""utf-8""?>
<log4j:event logger=""Root.ChildA.LoggerA2"" timestamp=""1500000031000"" level=""INFO"" thread=""Thread-1""><log4j:message>#1. Test event A.</log4j:message></log4j:event>
<log4j:event logger=""Root.ChildB.LoggerB2"" timestamp=""1500000032000"" level=""DEBUG"" thread=""Thread-2""><log4j:message>#2. Test event B.</log4j:message></log4j:event>
<log4j:event logger=""Root.ChildA.LoggerA2"" timestamp=""1500000033000"" level=""FATAL"" thread=""Thread-3""><log4j:message>#3. Test event C. С кирилицей.</log4j:message></log4j:event>
<log4j:event logger=""Root.ChildA.LoggerA1"" timestamp=""1500000034000"" level=""WARN"" thread=""Thread-4""><log4j:message>#4. Test event E.</log4j:message></log4j:event>
<log4j:event logger=""Root.ChildA.LoggerA1"" timestamp=""1500000035000"" level=""ERROR"" thread=""Thread-5""><log4j:message>#5. Test event F.</log4j:message></log4j:event>
";

        private static readonly EventExpectation[] Sample1Events =
        {
            new EventExpectation
            {
                Level = Level.Error,
                Logger = "Root.ChildA.LoggerA1",
                Thread = "Thread-5",
                Time = Timestamp.ToDateTime (1500000035000L),
                Message = "#5. Test event F.",
                Throwable = null,
                Id = 681
            },
            new EventExpectation
            {
                Level = Level.Warn,
                Logger = "Root.ChildA.LoggerA1",
                Thread = "Thread-4",
                Time = Timestamp.ToDateTime (1500000034000L),
                Message = "#4. Test event E.",
                Throwable = null,
                Id = 517
            },
            new EventExpectation
            {
                Level = Level.Fatal,
                Logger = "Root.ChildA.LoggerA2",
                Thread = "Thread-3",
                Time = Timestamp.ToDateTime (1500000033000L),
                Message = "#3. Test event C. С кирилицей.",
                Throwable = null,
                Id = 329
            },
            new EventExpectation
            {
                Level = Level.Debug,
                Logger = "Root.ChildB.LoggerB2",
                Thread = "Thread-2",
                Time = Timestamp.ToDateTime (1500000032000L),
                Message = "#2. Test event B.",
                Throwable = null,
                Id = 164
            },
            new EventExpectation
            {
                Level = Level.Info,
                Logger = "Root.ChildA.LoggerA2",
                Thread = "Thread-1",
                Time = Timestamp.ToDateTime (1500000031000L),
                Message = "#1. Test event A.",
                Throwable = null,
                Id = 0
            }
        };

        private const string Sample2 = @"<?xml version=""1.0"" encoding=""utf-8""?>
<log4j:event logger=""Root.ChildA.LoggerA2"" timestamp=""1500000021000"" level=""INFO"" thread=""Thread-1""><log4j:message>#1. Test event A.</log4j:message></log4j:event>
<log4j:event logger=""Root.ChildB.LoggerB2"" timestamp=""1500000022000"" level=""DEBUG"" thread=""Thread-2""><log4j:message>#2. Test event B.</log4j:message></log4j:event>
<log4j:event logger=""Root.ChildA.LoggerA2"" timestamp=""1500000023000"" level=""FATAL"" thread=""Thread-3""><log4j:message>#3. Test event C. С кирилицей.</log4j:message></log4j:event>
<log4j:event logger=""Root.ChildA.LoggerA1"" timestamp=""1500000024000"" level=""WARN"" thread=""Thread-4""><log4j:message>#4. Test event E.</log4j:message></log4j:event>
<log4j:event logger=""Root.ChildA.LoggerA1"" timestamp=""1500000025000"" level=""ERROR"" thread=""Thread-5""><log4j:message>#5. Test event F.</log4j:message></log4j:event>
";

        private static readonly EventExpectation[] Sample2Events =
        {
            new EventExpectation
            {
                Level = Level.Error,
                Logger = "Root.ChildA.LoggerA1",
                Thread = "Thread-5",
                Time = Timestamp.ToDateTime (1500000025000L),
                Message = "#5. Test event F.",
                Throwable = null,
                Id = 681
            },
            new EventExpectation
            {
                Level = Level.Warn,
                Logger = "Root.ChildA.LoggerA1",
                Thread = "Thread-4",
                Time = Timestamp.ToDateTime (1500000024000L),
                Message = "#4. Test event E.",
                Throwable = null,
                Id = 517
            },
            new EventExpectation
            {
                Level = Level.Fatal,
                Logger = "Root.ChildA.LoggerA2",
                Thread = "Thread-3",
                Time = Timestamp.ToDateTime (1500000023000L),
                Message = "#3. Test event C. С кирилицей.",
                Throwable = null,
                Id = 329
            },
            new EventExpectation
            {
                Level = Level.Debug,
                Logger = "Root.ChildB.LoggerB2",
                Thread = "Thread-2",
                Time = Timestamp.ToDateTime (1500000022000L),
                Message = "#2. Test event B.",
                Throwable = null,
                Id = 164
            },
            new EventExpectation
            {
                Level = Level.Info,
                Logger = "Root.ChildA.LoggerA2",
                Thread = "Thread-1",
                Time = Timestamp.ToDateTime (1500000021000L),
                Message = "#1. Test event A.",
                Throwable = null,
                Id = 0
            }
        };

        private const string Sample3 = @"<?xml version=""1.0"" encoding=""utf-8""?>
<log4j:event logger=""Root.ChildA.LoggerA2"" timestamp=""1500000011000"" level=""INFO"" thread=""Thread-1""><log4j:message>#1. Test event A.</log4j:message></log4j:event>
<log4j:event logger=""Root.ChildB.LoggerB2"" timestamp=""1500000012000"" level=""DEBUG"" thread=""Thread-2""><log4j:message>#2. Test event B.</log4j:message></log4j:event>
<log4j:event logger=""Root.ChildA.LoggerA2"" timestamp=""1500000013000"" level=""FATAL"" thread=""Thread-3""><log4j:message>#3. Test event C. С кирилицей.</log4j:message></log4j:event>
<log4j:event logger=""Root.ChildA.LoggerA1"" timestamp=""1500000014000"" level=""WARN"" thread=""Thread-4""><log4j:message>#4. Test event E.</log4j:message></log4j:event>
<log4j:event logger=""Root.ChildA.LoggerA1"" timestamp=""1500000015000"" level=""ERROR"" thread=""Thread-5""><log4j:message>#5. Test event F.</log4j:message></log4j:event>
";

        private static readonly EventExpectation[] Sample3Events =
        {
            new EventExpectation
            {
                Level = Level.Error,
                Logger = "Root.ChildA.LoggerA1",
                Thread = "Thread-5",
                Time = Timestamp.ToDateTime (1500000015000L),
                Message = "#5. Test event F.",
                Throwable = null,
                Id = 681
            },
            new EventExpectation
            {
                Level = Level.Warn,
                Logger = "Root.ChildA.LoggerA1",
                Thread = "Thread-4",
                Time = Timestamp.ToDateTime (1500000014000L),
                Message = "#4. Test event E.",
                Throwable = null,
                Id = 517
            },
            new EventExpectation
            {
                Level = Level.Fatal,
                Logger = "Root.ChildA.LoggerA2",
                Thread = "Thread-3",
                Time = Timestamp.ToDateTime (1500000013000L),
                Message = "#3. Test event C. С кирилицей.",
                Throwable = null,
                Id = 329
            },
            new EventExpectation
            {
                Level = Level.Debug,
                Logger = "Root.ChildB.LoggerB2",
                Thread = "Thread-2",
                Time = Timestamp.ToDateTime (1500000012000L),
                Message = "#2. Test event B.",
                Throwable = null,
                Id = 164
            },
            new EventExpectation
            {
                Level = Level.Info,
                Logger = "Root.ChildA.LoggerA2",
                Thread = "Thread-1",
                Time = Timestamp.ToDateTime (1500000011000L),
                Message = "#1. Test event A.",
                Throwable = null,
                Id = 0
            }
        };

        [Test]
        public void DoesNotConsumeEventsZeroQuantity ()
        {
            using (var file = new StringLogFile ("sample-1", Sample1))
            {
                var statsProvider = new LogFileStatsCache ();
                var query = new SimpleLogQuery
                {
                    Quantity = 0
                };

                file.InitializeStats (query, statsProvider);

                var subject = new LogAccumulator (statsProvider, query);
                Assert.That (subject.IsComplete);

                var actual = subject.Consume (file);

                Assert.That (file.WasRead, Is.False);
                Assert.That (actual.Events, Is.Empty);
            }
        }

        [Test]
        public void DoesNotConsumeEventsBeyondCompletion ()
        {
            using (var file1 = new StringLogFile ("sample-1-a", Sample1))
            using (var file2 = new StringLogFile ("sample-1-b", Sample1))
            {
                var statsProvider = new LogFileStatsCache ();
                var query = new SimpleLogQuery
                {
                    Quantity = statsProvider.GetStats (file1, null).EventCount
                };

                file1.InitializeStats (query, statsProvider);
                file2.InitializeStats (query, statsProvider);

                var subject = new LogAccumulator (statsProvider, query);

                var actual = subject
                    .Consume (file1)
                    .Consume (file2);

                Assert.That (file1.WasRead, Is.True);
                Assert.That (file2.WasRead, Is.False);

                Assert.That (actual.Events, Is.EqualTo (Sample1Events));
            }
        }

        [Test]
        public void CanConsumeAllEvents ()
        {
            using (var file1 = new StringLogFile ("sample-1", Sample1))
            using (var file2 = new StringLogFile ("sample-2", Sample2))
            {
                var expected = Sample1Events.Concat (Sample2Events).ToList ();

                var statsProvider = new LogFileStatsCache ();
                file1.InitializeDefaultStats (statsProvider);
                file2.InitializeDefaultStats (statsProvider);

                var file1Events = statsProvider.GetStats (file1, null).EventCount;
                var file2Events = statsProvider.GetStats (file2, null).EventCount;
                var query = new SimpleLogQuery
                {
                    Quantity = file1Events + file2Events + 1
                };

                var subject = new LogAccumulator (statsProvider, query);

                var actual = subject
                    .Consume (file1)
                    .Consume (file2);

                Assert.That (actual.IsComplete, Is.False);

                Assert.That (file1.WasRead, Is.True);
                Assert.That (file2.WasRead, Is.True);

                Assert.That (actual.Events, Is.EqualTo (expected));
            }
        }

        [Test]
        public void SkipsFilesWithinOffset ()
        {
            using (var file1 = new StringLogFile ("sample-1", Sample1))
            using (var file2 = new StringLogFile ("sample-2", Sample2))
            using (var file3 = new StringLogFile ("sample-3", Sample3))
            {
                var statsProvider = new LogFileStatsCache ();
                file1.InitializeDefaultStats (statsProvider);
                file2.InitializeDefaultStats (statsProvider);
                file3.InitializeDefaultStats (statsProvider);

                var query = new SimpleLogQuery
                {
                    Offset = statsProvider.GetStats (file1, null).EventCount,
                    Quantity = statsProvider.GetStats (file2, null).EventCount
                };

                var subject = new LogAccumulator (statsProvider, query);

                var actual = subject
                    .Consume (file1)
                    .Consume (file2)
                    .Consume (file3);

                Assert.That (actual.IsComplete);

                Assert.That (file1.WasRead, Is.False);
                Assert.That (file2.WasRead, Is.True);
                Assert.That (file3.WasRead, Is.False);

                Assert.That (actual.Events, Is.EqualTo (Sample2Events));
            }
        }

        [Test]
        public void CanFillUpEventsFromMultipleFiles ()
        {
            using (var file1 = new StringLogFile ("sample-1", Sample1))
            using (var file2 = new StringLogFile ("sample-2", Sample2))
            using (var file3 = new StringLogFile ("sample-3", Sample3))
            {
                var statsProvider = new LogFileStatsCache ();
                file1.InitializeDefaultStats (statsProvider);
                file2.InitializeDefaultStats (statsProvider);
                file3.InitializeDefaultStats (statsProvider);

                var file1Events = statsProvider.GetStats (file1, null).EventCount;
                var file2Events = statsProvider.GetStats (file2, null).EventCount;
                var file3Events = statsProvider.GetStats (file2, null).EventCount;

                Assume.That (file2Events, Is.GreaterThanOrEqualTo (2));
                Assume.That (file3Events, Is.GreaterThan (file2Events / 2 + file2Events % 2));

                var offset = file1Events + file2Events / 2;
                var quantity = file3Events;

                var expected = Sample1Events.Concat (Sample2Events).Concat (Sample3Events)
                    .Skip (offset)
                    .Take (quantity);

                var query = new SimpleLogQuery
                {
                    Offset = offset,
                    Quantity = quantity
                };

                var subject = new LogAccumulator (statsProvider, query);

                var actual = subject
                    .Consume (file1)
                    .Consume (file2)
                    .Consume (file3);

                Assert.That (actual.IsComplete);

                Assert.That (file1.WasRead, Is.False);
                Assert.That (file2.WasRead, Is.True);
                Assert.That (file3.WasRead, Is.True);

                Assert.That (actual.Events, Is.EqualTo (expected));
            }
        }

        [Test]
        public void SkipsFilesOutsideQueriedTimeFrame ()
        {
            using (var file1 = new StringLogFile ("sample-1", Sample1))
            using (var file2 = new StringLogFile ("sample-2", Sample2))
            using (var file3 = new StringLogFile ("sample-3", Sample3))
            {
                var statsProvider = new LogFileStatsCache ();
                file1.InitializeDefaultStats (statsProvider);
                file2.InitializeDefaultStats (statsProvider);
                file3.InitializeDefaultStats (statsProvider);

                var file1Stats = statsProvider.GetStats (file1, null);
                var file2Stats = statsProvider.GetStats (file2, null);
                var file3Stats = statsProvider.GetStats (file3, null);
                Assume.That (file1Stats.EarliestTimestamp, Is.GreaterThan (file2Stats.LatestTimestamp));
                Assume.That (file2Stats.EarliestTimestamp, Is.GreaterThan (file3Stats.LatestTimestamp));

                Assume.That (file2Stats.EventCount, Is.GreaterThanOrEqualTo (4));

                var expected = Sample2Events
                    .Skip (1)
                    .Take (Sample2Events.Length - 2)
                    .ToList ();

                var minTime = expected.Last ().Time;
                var maxTime = expected.First ().Time;
                var query = new SimpleLogQuery
                {
                    Offset = 0,
                    Quantity = expected.Count,
                    MinTimestamp = Timestamp.FromDateTime (minTime),
                    MaxTimestamp = Timestamp.FromDateTime (maxTime),
                    Filter = FilterBuilder.Timestamp (minTime, maxTime)
                };

                var subject = new LogAccumulator (statsProvider, query);

                var actual = subject
                    .Consume (file1)
                    .Consume (file2)
                    .Consume (file3);

                Assert.That (actual.IsComplete);

                Assert.That (file1.WasRead, Is.False);
                Assert.That (file2.WasRead, Is.True);
                Assert.That (file3.WasRead, Is.False);

                Assert.That (actual.Events, Is.EqualTo (expected));
            }
        }

        [Test]
        public void SkipsFilesByLoggerCount ()
        {
            using (var file1 = new StringLogFile ("sample-1", Sample1))
            using (var file2 = new StringLogFile ("sample-2", Sample2))
            using (var file3 = new StringLogFile ("sample-3", Sample3))
            {
                var statsProvider = new LogFileStatsCache ();
                file1.InitializeDefaultStats (statsProvider);
                file2.InitializeDefaultStats (statsProvider);
                file3.InitializeDefaultStats (statsProvider);

                var file1Stats = statsProvider.GetStats (file1, null);
                var file2Stats = statsProvider.GetStats (file2, null);
                var file3Stats = statsProvider.GetStats (file3, null);

                const string logger = "Root.ChildA";
                var sample1Matching = Sample1Events
                    .Where (e => e.Logger.StartsWith (logger))
                    .ToList ();
                var sample2Matching = Sample2Events
                    .Where (e => e.Logger.StartsWith (logger))
                    .ToList ();

                Assume.That (sample1Matching.Count, Is.LessThan (file1Stats.EventCount));
                Assume.That (sample2Matching.Count, Is.LessThan (file2Stats.EventCount));

                var expected = sample2Matching;

                var minTime = expected.Last ().Time;
                var maxTime = expected.First ().Time;
                var query = new SimpleLogQuery
                {
                    Offset = sample1Matching.Count,
                    Quantity = expected.Count,
                    Filter = FilterBuilder.Logger (logger)
                };

                var subject = new LogAccumulator (statsProvider, query);

                var actual = subject
                    .Consume (file1)
                    .Consume (file2)
                    .Consume (file3);

                Assert.That (actual.IsComplete);

                Assert.That (file1.WasRead, Is.False);
                Assert.That (file2.WasRead, Is.True);
                Assert.That (file3.WasRead, Is.False);

                Assert.That (actual.Events, Is.EqualTo (expected));
            }
        }
    }
}
