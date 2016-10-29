using System;
using System.IO;
using System.Reflection;
using System.Text;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using NUnit.Framework;

namespace Log4JParserNet.Tests
{
    [TestFixture]
    public class TimestampTests
    {
        [Test]
        public void ConvertsDateTimeToTimestamp ()
        {
            var sample = new DateTime (2014, 09, 20, 20, 42, 51, 554, DateTimeKind.Local);
            var expected = GetLog4NetTimestamp (sample);

            var actual = Timestamp.FromDateTime (sample);

            Assert.That (actual, Is.EqualTo (expected));
        }

        [Test]
        public void ConvertsTimestampToDateTime ()
        {
            var expected = new DateTime (2014, 09, 20, 20, 42, 51, 554, DateTimeKind.Local);
            var sample = GetLog4NetTimestamp (expected);

            var actual = Timestamp.ToDateTime (sample).ToLocalTime ();

            Assert.That (actual, Is.EqualTo (expected));
        }

        private long GetLog4NetTimestamp (DateTime dateTime)
        {
            var logXmlBuilder = new StringBuilder ();
            using (var writer = new StringWriter (logXmlBuilder))
            {
                var layout = new XmlLayoutSchemaLog4j ();
                var appender = new TextWriterAppender
                {
                    ImmediateFlush = true,
                    Layout = layout,
                    Writer = writer
                };
                var repository = LogManager.GetRepository (Assembly.GetCallingAssembly ());

                var eventData = new LoggingEventData
                {
                    LoggerName = "TestLogger",
                    TimeStamp = dateTime,
                    Level = log4net.Core.Level.Debug,
                    ThreadName = "TestThread",
                    Message = "Test message."
                };
                var @event = new LoggingEvent (typeof (TimestampTests), repository, eventData);
                appender.DoAppend (@event);

                writer.Flush ();

                appender.Close ();
            }

            var logXml = logXmlBuilder.ToString ();

            const string timestampStartMarker = "timestamp=\"";
            var timestampStart = logXml.IndexOf (timestampStartMarker) + timestampStartMarker.Length;
            Assume.That (timestampStart, Is.GreaterThanOrEqualTo (0));
            var timestampEnd = logXml.IndexOf ('"', timestampStart);
            Assume.That (timestampEnd, Is.GreaterThanOrEqualTo (0));

            var timestampString = logXml.Substring (timestampStart, timestampEnd - timestampStart);

            var timestamp = Int64.Parse (timestampString);

            return timestamp;
        }
    }
}
