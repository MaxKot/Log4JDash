using System;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace Log4JParserNet.Tests
{
    [TestFixture]
    public class Log4JFileTests
    {
        private const string Sample1 = @"<log4j:event logger=""Root.ChildA.LoggerA2"" timestamp=""1411231353782"" level=""INFO"" thread=""Thread-1""><log4j:message>#1. Test event A.</log4j:message><log4j:properties><log4j:data name=""log4jmachinename"" value=""EXAMPLE_PC"" /><log4j:data name=""log4japp"" value=""LogGenerator.exe"" /><log4j:data name=""log4net:Identity"" value="""" /><log4j:data name=""log4net:UserName"" value=""EXAMPLE_PC\Dev"" /><log4j:data name=""log4net:HostName"" value=""EXAMPLE_PC"" /></log4j:properties></log4j:event>
<log4j:event logger=""Root.ChildB.LoggerB2"" timestamp=""1411231353792"" level=""DEBUG"" thread=""Thread-2""><log4j:message>#2. Test event B.</log4j:message><log4j:properties><log4j:data name=""log4jmachinename"" value=""EXAMPLE_PC"" /><log4j:data name=""log4japp"" value=""LogGenerator.exe"" /><log4j:data name=""log4net:Identity"" value="""" /><log4j:data name=""log4net:UserName"" value=""EXAMPLE_PC\Dev"" /><log4j:data name=""log4net:HostName"" value=""EXAMPLE_PC"" /></log4j:properties></log4j:event>
<log4j:event logger=""Root.ChildA.LoggerA2"" timestamp=""1411231353792"" level=""FATAL"" thread=""Thread-3""><log4j:message>#3. Test event C. С кирилицей.</log4j:message><log4j:properties><log4j:data name=""log4jmachinename"" value=""EXAMPLE_PC"" /><log4j:data name=""log4japp"" value=""LogGenerator.exe"" /><log4j:data name=""log4net:Identity"" value="""" /><log4j:data name=""log4net:UserName"" value=""EXAMPLE_PC\Dev"" /><log4j:data name=""log4net:HostName"" value=""EXAMPLE_PC"" /></log4j:properties></log4j:event>
<log4j:event logger=""Root.ChildA.LoggerA1"" timestamp=""1411231353793"" level=""WARN"" thread=""Thread-4""><log4j:message>#4. Test event E.</log4j:message><log4j:properties><log4j:data name=""log4jmachinename"" value=""EXAMPLE_PC"" /><log4j:data name=""log4japp"" value=""LogGenerator.exe"" /><log4j:data name=""log4net:Identity"" value="""" /><log4j:data name=""log4net:UserName"" value=""EXAMPLE_PC\Dev"" /><log4j:data name=""log4net:HostName"" value=""EXAMPLE_PC"" /></log4j:properties></log4j:event>
<log4j:event logger=""Root.ChildA.LoggerA1"" timestamp=""1411231353795"" level=""ERROR"" thread=""Thread-5""><log4j:message>#5. Test event F.</log4j:message><log4j:properties><log4j:data name=""log4jmachinename"" value=""EXAMPLE_PC"" /><log4j:data name=""log4japp"" value=""LogGenerator.exe"" /><log4j:data name=""log4net:Identity"" value="""" /><log4j:data name=""log4net:UserName"" value=""EXAMPLE_PC\Dev"" /><log4j:data name=""log4net:HostName"" value=""EXAMPLE_PC"" /></log4j:properties></log4j:event>
";

        private static readonly byte[] sample1Bytes = Encoding.GetEncoding (1251).GetBytes (Sample1);

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
                    Properties =
                    {
                        { "log4jmachinename", "EXAMPLE_PC" },
                        { "log4japp", "LogGenerator.exe" },
                        { "log4net:Identity", "" },
                        { "log4net:UserName", "EXAMPLE_PC\\Dev" },
                        { "log4net:HostName", "EXAMPLE_PC" }
                    },
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
                    Properties =
                    {
                        { "log4jmachinename", "EXAMPLE_PC" },
                        { "log4japp", "LogGenerator.exe" },
                        { "log4net:Identity", "" },
                        { "log4net:UserName", "EXAMPLE_PC\\Dev" },
                        { "log4net:HostName", "EXAMPLE_PC" }
                    },
                    Id = 478
                },
                new EventExpectation
                {
                    Level = Level.Fatal,
                    Logger = "Root.ChildA.LoggerA2",
                    Thread = "Thread-3",
                    Timestamp = 1411231353792L,
                    Message = "#3. Test event C. С кирилицей.",
                    Throwable = null,
                    Properties =
                    {
                        { "log4jmachinename", "EXAMPLE_PC" },
                        { "log4japp", "LogGenerator.exe" },
                        { "log4net:Identity", "" },
                        { "log4net:UserName", "EXAMPLE_PC\\Dev" },
                        { "log4net:HostName", "EXAMPLE_PC" }
                    },
                    Id = 957
                },
                new EventExpectation
                {
                    Level = Level.Warn,
                    Logger = "Root.ChildA.LoggerA1",
                    Thread = "Thread-4",
                    Timestamp = 1411231353793L,
                    Message = "#4. Test event E.",
                    Throwable = null,
                    Properties =
                    {
                        { "log4jmachinename", "EXAMPLE_PC" },
                        { "log4japp", "LogGenerator.exe" },
                        { "log4net:Identity", "" },
                        { "log4net:UserName", "EXAMPLE_PC\\Dev" },
                        { "log4net:HostName", "EXAMPLE_PC" }
                    },
                    Id = 1449
                },
                new EventExpectation
                {
                    Level = Level.Error,
                    Logger = "Root.ChildA.LoggerA1",
                    Thread = "Thread-5",
                    Timestamp = 1411231353795L,
                    Message = "#5. Test event F.",
                    Throwable = null,
                    Properties =
                    {
                        { "log4jmachinename", "EXAMPLE_PC" },
                        { "log4japp", "LogGenerator.exe" },
                        { "log4net:Identity", "" },
                        { "log4net:UserName", "EXAMPLE_PC\\Dev" },
                        { "log4net:HostName", "EXAMPLE_PC" }
                    },
                    Id = 1927
                }
            };

            using (var source = new MemoryStream (sample1Bytes))
            using (var subject = Log4JFile.Create (source))
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
                    Timestamp = 1411231353795L,
                    Message = "#5. Test event F.",
                    Throwable = null,
                    Properties =
                    {
                        { "log4jmachinename", "EXAMPLE_PC" },
                        { "log4japp", "LogGenerator.exe" },
                        { "log4net:Identity", "" },
                        { "log4net:UserName", "EXAMPLE_PC\\Dev" },
                        { "log4net:HostName", "EXAMPLE_PC" }
                    },
                    Id = 1927
                },
                new EventExpectation
                {
                    Level = Level.Warn,
                    Logger = "Root.ChildA.LoggerA1",
                    Thread = "Thread-4",
                    Timestamp = 1411231353793L,
                    Message = "#4. Test event E.",
                    Throwable = null,
                    Properties =
                    {
                        { "log4jmachinename", "EXAMPLE_PC" },
                        { "log4japp", "LogGenerator.exe" },
                        { "log4net:Identity", "" },
                        { "log4net:UserName", "EXAMPLE_PC\\Dev" },
                        { "log4net:HostName", "EXAMPLE_PC" }
                    },
                    Id = 1449
                },
                new EventExpectation
                {
                    Level = Level.Fatal,
                    Logger = "Root.ChildA.LoggerA2",
                    Thread = "Thread-3",
                    Timestamp = 1411231353792L,
                    Message = "#3. Test event C. С кирилицей.",
                    Throwable = null,
                    Properties =
                    {
                        { "log4jmachinename", "EXAMPLE_PC" },
                        { "log4japp", "LogGenerator.exe" },
                        { "log4net:Identity", "" },
                        { "log4net:UserName", "EXAMPLE_PC\\Dev" },
                        { "log4net:HostName", "EXAMPLE_PC" }
                    },
                    Id = 957
                },
                new EventExpectation
                {
                    Level = Level.Debug,
                    Logger = "Root.ChildB.LoggerB2",
                    Thread = "Thread-2",
                    Timestamp = 1411231353792L,
                    Message = "#2. Test event B.",
                    Throwable = null,
                    Properties =
                    {
                        { "log4jmachinename", "EXAMPLE_PC" },
                        { "log4japp", "LogGenerator.exe" },
                        { "log4net:Identity", "" },
                        { "log4net:UserName", "EXAMPLE_PC\\Dev" },
                        { "log4net:HostName", "EXAMPLE_PC" }
                    },
                    Id = 478
                },
                new EventExpectation
                {
                    Level = Level.Info,
                    Logger = "Root.ChildA.LoggerA2",
                    Thread = "Thread-1",
                    Timestamp = 1411231353782L,
                    Message = "#1. Test event A.",
                    Throwable = null,
                    Properties =
                    {
                        { "log4jmachinename", "EXAMPLE_PC" },
                        { "log4japp", "LogGenerator.exe" },
                        { "log4net:Identity", "" },
                        { "log4net:UserName", "EXAMPLE_PC\\Dev" },
                        { "log4net:HostName", "EXAMPLE_PC" }
                    },
                    Id = 0
                }
            };

            using (var source = new MemoryStream (sample1Bytes))
            using (var subject = Log4JFile.Create (source))
            {
                subject.Encoding = Encoding.GetEncoding (1251);
                var actual = subject.GetEventsReverse ();
                Assert.That (actual, Is.EqualTo (expected));
            }
        }

        private const string SampleIncomplete = @"<?xml version=""1.0"" encoding=""windows-1251""?>
<log4j:event logger=""Root.ChildA.LoggerA2"" timestamp=""1411231353782"" level=""INFO"" thread=""Thread-1""><log4j:message>#1. Test event A.</log4j:message><log4j:properties><log4j:data name=""log4jmachinename"" value=""EXAMPLE_PC"" /><log4j:data name=""log4japp"" value=""LogGenerator.exe"" /><log4j:data name=""log4net:Identity"" value="""" /><log4j:data name=""log4net:UserName"" value=""EXAMPLE_PC\Dev"" /><log4j:data name=""log4net:HostName"" value=""EXAMPLE_PC"" /></log4j:properties></log4j:event>
<log4j:event logger=""Root.ChildB.LoggerB2"" timestamp=""1411231353792"" level=""DEBUG"" thread=""Thread-2""><log4j:message>#2. Test event B.</log4j:message><log4j:properties><log4j:data name=""log4jmachinename"" value=""EXAMPLE_PC"" /><log4j:data name=""log4japp"" value=""LogGenerator.exe"" /><log4j:data name=""log4net:Identity"" value="""" /><log4j:data name=""log4net:UserName"" value=""EXAMPLE_PC\Dev"" /><log4j:data name=""log4net:HostName"" value=""EXAMPLE_PC"" /></log4j:properties></log4j:event>
<log4j:event logger=""Root.ChildA.LoggerA2"" timesta";

        private static readonly byte[] sampleIncompleteBytes = Encoding.GetEncoding (1251).GetBytes (SampleIncomplete);

        [Test]
        public void CanReadIncompleteFile ()
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
                    Properties =
                    {
                        { "log4jmachinename", "EXAMPLE_PC" },
                        { "log4japp", "LogGenerator.exe" },
                        { "log4net:Identity", "" },
                        { "log4net:UserName", "EXAMPLE_PC\\Dev" },
                        { "log4net:HostName", "EXAMPLE_PC" }
                    },
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
                    Properties =
                    {
                        { "log4jmachinename", "EXAMPLE_PC" },
                        { "log4japp", "LogGenerator.exe" },
                        { "log4net:Identity", "" },
                        { "log4net:UserName", "EXAMPLE_PC\\Dev" },
                        { "log4net:HostName", "EXAMPLE_PC" }
                    },
                    Id = 478
                }
            };

            using (var source = new MemoryStream (sampleIncompleteBytes))
            using (var subject = Log4JFile.Create (source))
            {
                subject.Encoding = Encoding.GetEncoding (1251);
                var actual = subject.GetEvents ();
                Assert.That (actual, Is.EqualTo (expected));
            }
        }

        [Test]
        public void CanReadIncompleteFileInReverseOrder ()
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
                    Properties =
                    {
                        { "log4jmachinename", "EXAMPLE_PC" },
                        { "log4japp", "LogGenerator.exe" },
                        { "log4net:Identity", "" },
                        { "log4net:UserName", "EXAMPLE_PC\\Dev" },
                        { "log4net:HostName", "EXAMPLE_PC" }
                    },
                    Id = 478
                },
                new EventExpectation
                {
                    Level = Level.Info,
                    Logger = "Root.ChildA.LoggerA2",
                    Thread = "Thread-1",
                    Timestamp = 1411231353782L,
                    Message = "#1. Test event A.",
                    Throwable = null,
                    Properties =
                    {
                        { "log4jmachinename", "EXAMPLE_PC" },
                        { "log4japp", "LogGenerator.exe" },
                        { "log4net:Identity", "" },
                        { "log4net:UserName", "EXAMPLE_PC\\Dev" },
                        { "log4net:HostName", "EXAMPLE_PC" }
                    },
                    Id = 0
                }
            };

            using (var source = new MemoryStream (sampleIncompleteBytes))
            using (var subject = Log4JFile.Create (source))
            {
                subject.Encoding = Encoding.GetEncoding (1251);
                var actual = subject.GetEventsReverse ();
                Assert.That (actual, Is.EqualTo (expected));
            }
        }

        private const string SampleErrorInMiddle = @"<?xml version=""1.0"" encoding=""windows-1251""?>
<log4j:event logger=""Root.ChildA.LoggerA2"" timestamp=""1411231353782"" level=""INFO"" thread=""Thread-1""><log4j:message>#1. Test event A.</log4j:message><log4j:properties><log4j:data name=""log4jmachinename"" value=""EXAMPLE_PC"" /><log4j:data name=""log4japp"" value=""LogGenerator.exe"" /><log4j:data name=""log4net:Identity"" value="""" /><log4j:data name=""log4net:UserName"" value=""EXAMPLE_PC\Dev"" /><log4j:data name=""log4net:HostName"" value=""EXAMPLE_PC"" /></log4j:properties></log4j:event>
<log4j:event logger=""Root.ChildA.LoggerA2"" timesta
<log4j:event logger=""Root.ChildB.LoggerB2"" timestamp=""1411231353792"" level=""DEBUG"" thread=""Thread-2""><log4j:message>#2. Test event B.</log4j:message><log4j:properties><log4j:data name=""log4jmachinename"" value=""EXAMPLE_PC"" /><log4j:data name=""log4japp"" value=""LogGenerator.exe"" /><log4j:data name=""log4net:Identity"" value="""" /><log4j:data name=""log4net:UserName"" value=""EXAMPLE_PC\Dev"" /><log4j:data name=""log4net:HostName"" value=""EXAMPLE_PC"" /></log4j:properties></log4j:event>
";

        private static readonly byte[] sampleErrorInMiddleBytes = Encoding.GetEncoding (1251).GetBytes (SampleErrorInMiddle);

        [Test]
        public void CanReadFileWithErrorInMiddle ()
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
                    Properties =
                    {
                        { "log4jmachinename", "EXAMPLE_PC" },
                        { "log4japp", "LogGenerator.exe" },
                        { "log4net:Identity", "" },
                        { "log4net:UserName", "EXAMPLE_PC\\Dev" },
                        { "log4net:HostName", "EXAMPLE_PC" }
                    },
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
                    Properties =
                    {
                        { "log4jmachinename", "EXAMPLE_PC" },
                        { "log4japp", "LogGenerator.exe" },
                        { "log4net:Identity", "" },
                        { "log4net:UserName", "EXAMPLE_PC\\Dev" },
                        { "log4net:HostName", "EXAMPLE_PC" }
                    },
                    Id = 530
                }
            };

            using (var source = new MemoryStream (sampleErrorInMiddleBytes))
            using (var subject = Log4JFile.Create (source))
            {
                subject.Encoding = Encoding.GetEncoding (1251);
                var actual = subject.GetEvents ();
                Assert.That (actual, Is.EqualTo (expected));
            }
        }

        [Test]
        public void CanReadFileWithErrorInMiddleInReverseOrder ()
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
                    Properties =
                    {
                        { "log4jmachinename", "EXAMPLE_PC" },
                        { "log4japp", "LogGenerator.exe" },
                        { "log4net:Identity", "" },
                        { "log4net:UserName", "EXAMPLE_PC\\Dev" },
                        { "log4net:HostName", "EXAMPLE_PC" }
                    },
                    Id = 530
                },
                new EventExpectation
                {
                    Level = Level.Info,
                    Logger = "Root.ChildA.LoggerA2",
                    Thread = "Thread-1",
                    Timestamp = 1411231353782L,
                    Message = "#1. Test event A.",
                    Throwable = null,
                    Properties =
                    {
                        { "log4jmachinename", "EXAMPLE_PC" },
                        { "log4japp", "LogGenerator.exe" },
                        { "log4net:Identity", "" },
                        { "log4net:UserName", "EXAMPLE_PC\\Dev" },
                        { "log4net:HostName", "EXAMPLE_PC" }
                    },
                    Id = 0
                }
            };

            using (var source = new MemoryStream (sampleErrorInMiddleBytes))
            using (var subject = Log4JFile.Create (source))
            {
                subject.Encoding = Encoding.GetEncoding (1251);
                var actual = subject.GetEventsReverse ();
                Assert.That (actual, Is.EqualTo (expected));
            }
        }

        private const string SampleProperties16 = @"<?xml version=""1.0"" encoding=""windows-1251""?>
<log4j:event logger=""L1"" timestamp=""0"" level=""INFO"" thread=""1"">
  <log4j:message>Msg</log4j:message>
  <log4j:properties>
    <log4j:data name=""log4jmachinename"" value=""EXAMPLE_PC"" />
    <log4j:data name=""log4japp"" value=""LogGenerator.exe"" />
    <log4j:data name=""log4net:Identity"" value="""" />
    <log4j:data name=""log4net:UserName"" value=""EXAMPLE_PC\Dev"" />
    <log4j:data name=""log4net:HostName"" value=""EXAMPLE_PC"" />
    <log4j:data name=""prop-06"" value=""#06"" />
    <log4j:data name=""prop-07"" value=""#07"" />
    <log4j:data name=""prop-08"" value=""#08"" />
    <log4j:data name=""prop-09"" value=""#09"" />
    <log4j:data name=""prop-10"" value=""#10"" />
    <log4j:data name=""prop-11"" value=""#11"" />
    <log4j:data name=""prop-12"" value=""#12"" />
    <log4j:data name=""prop-13"" value=""#13"" />
    <log4j:data name=""prop-14"" value=""#14"" />
    <log4j:data name=""prop-15"" value=""#15"" />
    <log4j:data name=""prop-16"" value=""#16"" />
  </log4j:properties>
</log4j:event>
";

        private static readonly byte[] SampleProperties16Bytes = Encoding.GetEncoding (1251).GetBytes (SampleProperties16);

        [Test]
        public void CanReadEventWithPropertiesCountEqualToPropertyBufferSize ()
        {
            var expected = new[]
            {
                new EventExpectation
                {
                    Level = Level.Info,
                    Logger = "L1",
                    Thread = "1",
                    Timestamp = 0L,
                    Message = "Msg",
                    Throwable = null,
                    Properties =
                    {
                        { "log4jmachinename", "EXAMPLE_PC" },
                        { "log4japp", "LogGenerator.exe" },
                        { "log4net:Identity", "" },
                        { "log4net:UserName", "EXAMPLE_PC\\Dev" },
                        { "log4net:HostName", "EXAMPLE_PC" },
                        { "prop-06", "#06" },
                        { "prop-07", "#07" },
                        { "prop-08", "#08" },
                        { "prop-09", "#09" },
                        { "prop-10", "#10" },
                        { "prop-11", "#11" },
                        { "prop-12", "#12" },
                        { "prop-13", "#13" },
                        { "prop-14", "#14" },
                        { "prop-15", "#15" },
                        { "prop-16", "#16" }
                    },
                    Id = 0
                }
            };

            Assume.That (expected[0].Properties.Count, Is.EqualTo (16));

            using (var source = new MemoryStream (SampleProperties16Bytes))
            using (var subject = Log4JFile.Create (source))
            {
                subject.Encoding = Encoding.GetEncoding (1251);
                var actual = subject.GetEventsReverse ();
                Assert.That (actual, Is.EqualTo (expected));
            }
        }

        private const string SampleProperties17 = @"<?xml version=""1.0"" encoding=""windows-1251""?>
<log4j:event logger=""L1"" timestamp=""0"" level=""INFO"" thread=""1"">
  <log4j:message>Msg</log4j:message>
  <log4j:properties>
    <log4j:data name=""log4jmachinename"" value=""EXAMPLE_PC"" />
    <log4j:data name=""log4japp"" value=""LogGenerator.exe"" />
    <log4j:data name=""log4net:Identity"" value="""" />
    <log4j:data name=""log4net:UserName"" value=""EXAMPLE_PC\Dev"" />
    <log4j:data name=""log4net:HostName"" value=""EXAMPLE_PC"" />
    <log4j:data name=""prop-06"" value=""#06"" />
    <log4j:data name=""prop-07"" value=""#07"" />
    <log4j:data name=""prop-08"" value=""#08"" />
    <log4j:data name=""prop-09"" value=""#09"" />
    <log4j:data name=""prop-10"" value=""#10"" />
    <log4j:data name=""prop-11"" value=""#11"" />
    <log4j:data name=""prop-12"" value=""#12"" />
    <log4j:data name=""prop-13"" value=""#13"" />
    <log4j:data name=""prop-14"" value=""#14"" />
    <log4j:data name=""prop-15"" value=""#15"" />
    <log4j:data name=""prop-16"" value=""#16"" />
    <log4j:data name=""prop-17"" value=""#17"" />
  </log4j:properties>
</log4j:event>
";

        private static readonly byte[] SampleProperties17Bytes = Encoding.GetEncoding (1251).GetBytes (SampleProperties17);

        [Test]
        public void CanReadEventWithPropertiesCountGreaterThanPropertyBufferSize ()
        {
            var expected = new[]
            {
                new EventExpectation
                {
                    Level = Level.Info,
                    Logger = "L1",
                    Thread = "1",
                    Timestamp = 0L,
                    Message = "Msg",
                    Throwable = null,
                    Properties =
                    {
                        { "log4jmachinename", "EXAMPLE_PC" },
                        { "log4japp", "LogGenerator.exe" },
                        { "log4net:Identity", "" },
                        { "log4net:UserName", "EXAMPLE_PC\\Dev" },
                        { "log4net:HostName", "EXAMPLE_PC" },
                        { "prop-06", "#06" },
                        { "prop-07", "#07" },
                        { "prop-08", "#08" },
                        { "prop-09", "#09" },
                        { "prop-10", "#10" },
                        { "prop-11", "#11" },
                        { "prop-12", "#12" },
                        { "prop-13", "#13" },
                        { "prop-14", "#14" },
                        { "prop-15", "#15" },
                        { "prop-16", "#16" },
                        { "prop-17", "#17" }
                    },
                    Id = 0
                }
            };

            Assume.That (expected[0].Properties.Count, Is.EqualTo (17));

            using (var source = new MemoryStream (SampleProperties17Bytes))
            using (var subject = Log4JFile.Create (source))
            {
                subject.Encoding = Encoding.GetEncoding (1251);
                var actual = subject.GetEventsReverse ();
                Assert.That (actual, Is.EqualTo (expected));
            }
        }

        private const string SampleProperties32 = @"<?xml version=""1.0"" encoding=""windows-1251""?>
<log4j:event logger=""L1"" timestamp=""0"" level=""INFO"" thread=""1"">
  <log4j:message>Msg</log4j:message>
  <log4j:properties>
    <log4j:data name=""log4jmachinename"" value=""EXAMPLE_PC"" />
    <log4j:data name=""log4japp"" value=""LogGenerator.exe"" />
    <log4j:data name=""log4net:Identity"" value="""" />
    <log4j:data name=""log4net:UserName"" value=""EXAMPLE_PC\Dev"" />
    <log4j:data name=""log4net:HostName"" value=""EXAMPLE_PC"" />
    <log4j:data name=""prop-06"" value=""#06"" />
    <log4j:data name=""prop-07"" value=""#07"" />
    <log4j:data name=""prop-08"" value=""#08"" />
    <log4j:data name=""prop-09"" value=""#09"" />
    <log4j:data name=""prop-10"" value=""#10"" />
    <log4j:data name=""prop-11"" value=""#11"" />
    <log4j:data name=""prop-12"" value=""#12"" />
    <log4j:data name=""prop-13"" value=""#13"" />
    <log4j:data name=""prop-14"" value=""#14"" />
    <log4j:data name=""prop-15"" value=""#15"" />
    <log4j:data name=""prop-16"" value=""#16"" />
    <log4j:data name=""prop-17"" value=""#17"" />
    <log4j:data name=""prop-18"" value=""#18"" />
    <log4j:data name=""prop-19"" value=""#19"" />
    <log4j:data name=""prop-20"" value=""#20"" />
    <log4j:data name=""prop-21"" value=""#21"" />
    <log4j:data name=""prop-22"" value=""#22"" />
    <log4j:data name=""prop-23"" value=""#23"" />
    <log4j:data name=""prop-24"" value=""#24"" />
    <log4j:data name=""prop-25"" value=""#25"" />
    <log4j:data name=""prop-26"" value=""#26"" />
    <log4j:data name=""prop-27"" value=""#27"" />
    <log4j:data name=""prop-28"" value=""#28"" />
    <log4j:data name=""prop-29"" value=""#29"" />
    <log4j:data name=""prop-30"" value=""#30"" />
    <log4j:data name=""prop-31"" value=""#31"" />
    <log4j:data name=""prop-32"" value=""#32"" />
  </log4j:properties>
</log4j:event>
";

        private static readonly byte[] SampleProperties32Bytes = Encoding.GetEncoding (1251).GetBytes (SampleProperties32);

        [Test]
        public void CanReadEventWithPropertiesCountMultiplierOfPropertyBufferSize ()
        {
            var expected = new[]
            {
                new EventExpectation
                {
                    Level = Level.Info,
                    Logger = "L1",
                    Thread = "1",
                    Timestamp = 0L,
                    Message = "Msg",
                    Throwable = null,
                    Properties =
                    {
                        { "log4jmachinename", "EXAMPLE_PC" },
                        { "log4japp", "LogGenerator.exe" },
                        { "log4net:Identity", "" },
                        { "log4net:UserName", "EXAMPLE_PC\\Dev" },
                        { "log4net:HostName", "EXAMPLE_PC" },
                        { "prop-06", "#06" },
                        { "prop-07", "#07" },
                        { "prop-08", "#08" },
                        { "prop-09", "#09" },
                        { "prop-10", "#10" },
                        { "prop-11", "#11" },
                        { "prop-12", "#12" },
                        { "prop-13", "#13" },
                        { "prop-14", "#14" },
                        { "prop-15", "#15" },
                        { "prop-16", "#16" },
                        { "prop-17", "#17" },
                        { "prop-18", "#18" },
                        { "prop-19", "#19" },
                        { "prop-20", "#20" },
                        { "prop-21", "#21" },
                        { "prop-22", "#22" },
                        { "prop-23", "#23" },
                        { "prop-24", "#24" },
                        { "prop-25", "#25" },
                        { "prop-26", "#26" },
                        { "prop-27", "#27" },
                        { "prop-28", "#28" },
                        { "prop-29", "#29" },
                        { "prop-30", "#30" },
                        { "prop-31", "#31" },
                        { "prop-32", "#32" }
                    },
                    Id = 0
                }
            };

            Assume.That (expected[0].Properties.Count, Is.EqualTo (32));

            using (var source = new MemoryStream (SampleProperties32Bytes))
            using (var subject = Log4JFile.Create (source))
            {
                subject.Encoding = Encoding.GetEncoding (1251);
                var actual = subject.GetEventsReverse ();
                Assert.That (actual, Is.EqualTo (expected));
            }
        }

        private const string SampleCData = @"<log4j:event logger=""Root.ChildA.LoggerA2"" timestamp=""1411231353782"" level=""INFO"" thread=""Thread-1"">
    <log4j:message><![CDATA[#1. Test event A.]]></log4j:message>
    <log4j:properties>
        <log4j:data name=""log4jmachinename"" value=""EXAMPLE_PC"" />
        <log4j:data name=""log4japp"" value=""LogGenerator.exe"" />
        <log4j:data name=""log4net:Identity"" value="""" />
        <log4j:data name=""log4net:UserName"" value=""EXAMPLE_PC\Dev"" />
        <log4j:data name=""log4net:HostName"" value=""EXAMPLE_PC"" />
    </log4j:properties>
    <log4j:throwable><![CDATA[System.FormatException: An error is present.]]></log4j:throwable>
</log4j:event>";

        private static readonly byte[] sampleCDataBytes = Encoding.GetEncoding (1251).GetBytes (SampleCData);

        [Test]
        public void HandlesCDataNodes ()
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
                    Throwable = "System.FormatException: An error is present.",
                    Properties =
                    {
                        { "log4jmachinename", "EXAMPLE_PC" },
                        { "log4japp", "LogGenerator.exe" },
                        { "log4net:Identity", "" },
                        { "log4net:UserName", "EXAMPLE_PC\\Dev" },
                        { "log4net:HostName", "EXAMPLE_PC" }
                    },
                    Id = 0
                }
            };

            using (var source = new MemoryStream (sampleCDataBytes))
            using (var subject = Log4JFile.Create (source))
            {
                subject.Encoding = Encoding.GetEncoding (1251);
                var actual = subject.GetEvents ();
                Assert.That (actual, Is.EqualTo (expected));
            }
        }

        private const string SampleEscapedCData = @"<log4j:event logger=""Root.ChildA.LoggerA2"" timestamp=""1411231353782"" level=""INFO"" thread=""Thread-1"">
    <log4j:message><![CDATA[<nestedXml><![CDATA[Nested CDATA]]>]]<![CDATA[></nestedXml>]]></log4j:message>
    <log4j:properties>
        <log4j:data name=""log4jmachinename"" value=""EXAMPLE_PC"" />
        <log4j:data name=""log4japp"" value=""LogGenerator.exe"" />
        <log4j:data name=""log4net:Identity"" value="""" />
        <log4j:data name=""log4net:UserName"" value=""EXAMPLE_PC\Dev"" />
        <log4j:data name=""log4net:HostName"" value=""EXAMPLE_PC"" />
    </log4j:properties>
    <log4j:throwable><![CDATA[System.FormatException: An error is present.]]></log4j:throwable>
</log4j:event>";

        private static readonly byte[] sampleEscapedCDataBytes = Encoding.GetEncoding (1251).GetBytes (SampleEscapedCData);

        [Test]
        public void HandlesEscapedCDataNodes ()
        {
            var expected = new[]
            {
                new EventExpectation
                {
                    Level = Level.Info,
                    Logger = "Root.ChildA.LoggerA2",
                    Thread = "Thread-1",
                    Timestamp = 1411231353782L,
                    Message = "<nestedXml><![CDATA[Nested CDATA]]></nestedXml>",
                    Throwable = "System.FormatException: An error is present.",
                    Properties =
                    {
                        { "log4jmachinename", "EXAMPLE_PC" },
                        { "log4japp", "LogGenerator.exe" },
                        { "log4net:Identity", "" },
                        { "log4net:UserName", "EXAMPLE_PC\\Dev" },
                        { "log4net:HostName", "EXAMPLE_PC" }
                    },
                    Id = 0
                }
            };

            using (var source = new MemoryStream (sampleEscapedCDataBytes))
            using (var subject = Log4JFile.Create (source))
            {
                subject.Encoding = Encoding.GetEncoding (1251);
                var actual = subject.GetEvents ();
                Assert.That (actual, Is.EqualTo (expected));
            }
        }

        [Test]
        public void CanOpenFilesConcurrentlyWithLogWriter ()
        {
            const string sample = Sample1;
            var encoding = Encoding.UTF8;

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
                    Properties =
                    {
                        { "log4jmachinename", "EXAMPLE_PC" },
                        { "log4japp", "LogGenerator.exe" },
                        { "log4net:Identity", "" },
                        { "log4net:UserName", "EXAMPLE_PC\\Dev" },
                        { "log4net:HostName", "EXAMPLE_PC" }
                    },
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
                    Properties =
                    {
                        { "log4jmachinename", "EXAMPLE_PC" },
                        { "log4japp", "LogGenerator.exe" },
                        { "log4net:Identity", "" },
                        { "log4net:UserName", "EXAMPLE_PC\\Dev" },
                        { "log4net:HostName", "EXAMPLE_PC" }
                    },
                    Id = 478
                },
                new EventExpectation
                {
                    Level = Level.Fatal,
                    Logger = "Root.ChildA.LoggerA2",
                    Thread = "Thread-3",
                    Timestamp = 1411231353792L,
                    Message = "#3. Test event C. С кирилицей.",
                    Throwable = null,
                    Properties =
                    {
                        { "log4jmachinename", "EXAMPLE_PC" },
                        { "log4japp", "LogGenerator.exe" },
                        { "log4net:Identity", "" },
                        { "log4net:UserName", "EXAMPLE_PC\\Dev" },
                        { "log4net:HostName", "EXAMPLE_PC" }
                    },
                    Id = 957
                },
                new EventExpectation
                {
                    Level = Level.Warn,
                    Logger = "Root.ChildA.LoggerA1",
                    Thread = "Thread-4",
                    Timestamp = 1411231353793L,
                    Message = "#4. Test event E.",
                    Throwable = null,
                    Properties =
                    {
                        { "log4jmachinename", "EXAMPLE_PC" },
                        { "log4japp", "LogGenerator.exe" },
                        { "log4net:Identity", "" },
                        { "log4net:UserName", "EXAMPLE_PC\\Dev" },
                        { "log4net:HostName", "EXAMPLE_PC" }
                    },
                    Id = 1459
                },
                new EventExpectation
                {
                    Level = Level.Error,
                    Logger = "Root.ChildA.LoggerA1",
                    Thread = "Thread-5",
                    Timestamp = 1411231353795L,
                    Message = "#5. Test event F.",
                    Throwable = null,
                    Properties =
                    {
                        { "log4jmachinename", "EXAMPLE_PC" },
                        { "log4japp", "LogGenerator.exe" },
                        { "log4net:Identity", "" },
                        { "log4net:UserName", "EXAMPLE_PC\\Dev" },
                        { "log4net:HostName", "EXAMPLE_PC" }
                    },
                    Id = 1937
                }
            };

            Exception primaryException = null;
            string logFile = null;
            try
            {
                logFile = Path.GetTempFileName ();
                File.WriteAllBytes (logFile, encoding.GetBytes (sample));

                using (var fileHolder = new FileStream (logFile, FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
                using (var subject = Log4JFile.Create (logFile))
                {
                    subject.Encoding = encoding;
                    var actual = subject.GetEvents ();
                    Assert.That (actual, Is.EqualTo (expected));
                }
            }
            catch (Exception ex)
            {
                primaryException = ex;
                throw;
            }
            finally
            {
                try
                {
                    File.Delete (logFile);
                }
                catch (Exception cleanupEx)
                {
                    if (primaryException != null)
                    {
                        throw new AggregateException (primaryException, cleanupEx);
                    }

                    throw;
                }
            }
        }

        [Test]
        public void CanGetFileSizeLogWriter ()
        {
            const string sample = Sample1;

            var encoding = Encoding.UTF8;
            var expected = encoding.GetByteCount (sample);

            Exception primaryException = null;
            string logFile = null;
            try
            {
                logFile = Path.GetTempFileName ();
                File.WriteAllBytes (logFile, encoding.GetBytes (sample));

                using (var fileHolder = new FileStream (logFile, FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
                {
                    var actual = Log4JFile.GetSize (logFile);
                    Assert.That (actual, Is.EqualTo (expected));
                }
            }
            catch (Exception ex)
            {
                primaryException = ex;
                throw;
            }
            finally
            {
                try
                {
                    File.Delete (logFile);
                }
                catch (Exception cleanupEx)
                {
                    if (primaryException != null)
                    {
                        throw new AggregateException (primaryException, cleanupEx);
                    }

                    throw;
                }
            }
        }
    }
}
