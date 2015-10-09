using System.IO;
using System.Text;
using NUnit.Framework;

namespace Log4JParserNet.Tests
{
    [TestFixture]
    public class FilterNotTests
    {
        private const string Sample = @"<?xml version=""1.0"" encoding=""windows-1251""?>
<log4j:event logger=""Root.ChildA.LoggerA2"" timestamp=""1411231353782"" level=""INFO"" thread=""Thread-1""><log4j:message>#1. Test event A.</log4j:message><log4j:properties><log4j:data name=""log4jmachinename"" value=""EXAMPLE_PC"" /><log4j:data name=""log4japp"" value=""LogGenerator.exe"" /><log4j:data name=""log4net:Identity"" value="""" /><log4j:data name=""log4net:UserName"" value=""EXAMPLE_PC\Dev"" /><log4j:data name=""log4net:HostName"" value=""EXAMPLE_PC"" /></log4j:properties></log4j:event>
<log4j:event logger=""Root.ChildB.LoggerB2"" timestamp=""1411231353792"" level=""DEBUG"" thread=""Thread-2""><log4j:message>#2. Test event B.</log4j:message><log4j:properties><log4j:data name=""log4jmachinename"" value=""EXAMPLE_PC"" /><log4j:data name=""log4japp"" value=""LogGenerator.exe"" /><log4j:data name=""log4net:Identity"" value="""" /><log4j:data name=""log4net:UserName"" value=""EXAMPLE_PC\Dev"" /><log4j:data name=""log4net:HostName"" value=""EXAMPLE_PC"" /></log4j:properties></log4j:event>
<log4j:event logger=""Root.ChildA.LoggerA2"" timestamp=""1411231353792"" level=""FATAL"" thread=""Thread-3""><log4j:message>#3. Test event C. С кирилицей.</log4j:message><log4j:properties><log4j:data name=""log4jmachinename"" value=""EXAMPLE_PC"" /><log4j:data name=""log4japp"" value=""LogGenerator.exe"" /><log4j:data name=""log4net:Identity"" value="""" /><log4j:data name=""log4net:UserName"" value=""EXAMPLE_PC\Dev"" /><log4j:data name=""log4net:HostName"" value=""EXAMPLE_PC"" /></log4j:properties></log4j:event>
<log4j:event logger=""Root.ChildA.LoggerA1"" timestamp=""1411231353793"" level=""WARN"" thread=""Thread-4""><log4j:message>#4. Test event E.</log4j:message><log4j:properties><log4j:data name=""log4jmachinename"" value=""EXAMPLE_PC"" /><log4j:data name=""log4japp"" value=""LogGenerator.exe"" /><log4j:data name=""log4net:Identity"" value="""" /><log4j:data name=""log4net:UserName"" value=""EXAMPLE_PC\Dev"" /><log4j:data name=""log4net:HostName"" value=""EXAMPLE_PC"" /></log4j:properties></log4j:event>
<log4j:event logger=""Root.ChildA.LoggerA1"" timestamp=""1411231353795"" level=""ERROR"" thread=""Thread-5""><log4j:message>#5. Test event F.</log4j:message><log4j:properties><log4j:data name=""log4jmachinename"" value=""EXAMPLE_PC"" /><log4j:data name=""log4japp"" value=""LogGenerator.exe"" /><log4j:data name=""log4net:Identity"" value="""" /><log4j:data name=""log4net:UserName"" value=""EXAMPLE_PC\Dev"" /><log4j:data name=""log4net:HostName"" value=""EXAMPLE_PC"" /></log4j:properties></log4j:event>
";

        private static readonly byte[] sampleBytes = Encoding.GetEncoding (1251).GetBytes (Sample);

        [Test]
        public void NegatesInnerFilter ()
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

            using (var sourceStream = new MemoryStream (sampleBytes))
            using (var source = Log4JFile.Create (sourceStream))
            using (var childFilter = new FilterLevel (Level.Info, Level.Error))
            using (var subject = new FilterNot (childFilter))
            {
                source.Encoding = Encoding.GetEncoding (1251);
                var actual = source.GetEvents ().Where (subject);
                Assert.That (actual, Is.EqualTo (expected));
            }
        }
    }
}
