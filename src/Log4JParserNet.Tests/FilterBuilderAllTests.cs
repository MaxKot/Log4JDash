﻿using System;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace Log4JParserNet.Tests
{
    [TestFixture]
    public class FilterBuilderAllTests
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
                    Id = 164
                }
            };

            var subject = FilterBuilder.All
            (
                FilterBuilder.Level (Level.MinValue, Level.Info),
                FilterBuilder.Timestamp (1411231353792L, Int64.MaxValue)
            );

            using (var sourceStream = new MemoryStream (sampleBytes))
            using (var source = Log4JFile.Create (sourceStream))
            using (var filter = subject.Build ())
            {
                source.Encoding = Encoding.GetEncoding (1251);

                var actual = source.GetEvents ().Where (filter);
                Assert.That (actual, Is.EqualTo (expected));
            }
        }
    }
}