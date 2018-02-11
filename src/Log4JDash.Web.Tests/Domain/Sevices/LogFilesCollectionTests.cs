using System.IO;
using System.Text;
using Log4JDash.Web.Domain.Services;
using NUnit.Framework;

namespace Log4JDash.Web.Tests.Domain.Sevices
{
    [TestFixture]
    public class LogFilesCollectionTests
    {
        private const string ImmutableContent = @"<?xml version=""1.0"" encoding=""utf-8""?>
<log4j:event logger=""Root.ChildA.LoggerA2"" timestamp=""1500000001000"" level=""INFO"" thread=""Thread-1""><log4j:message>#1. Test event A.</log4j:message></log4j:event>
<log4j:event logger=""Root.ChildB.LoggerB2"" timestamp=""1500000002000"" level=""DEBUG"" thread=""Thread-2""><log4j:message>#2. Test event B.</log4j:message></log4j:event>
<log4j:event logger=""Root.ChildA.LoggerA2"" timestamp=""1500000003000"" level=""FATAL"" thread=""Thread-3""><log4j:message>#3. Test event C. С кирилицей.</log4j:message></log4j:event>
<log4j:event logger=""Root.ChildA.LoggerA1"" timestamp=""1500000004000"" level=""WARN"" thread=""Thread-4""><log4j:message>#4. Test event E.</log4j:message></log4j:event>
<log4j:event logger=""Root.ChildA.LoggerA1"" timestamp=""1500000005000"" level=""ERROR"" thread=""Thread-5""><log4j:message>#5. Test event F.</log4j:message></log4j:event>
<log4j:event logger=""Root.ChildB.LoggerB2"" timestamp=""1500000006000"" level=""DEBUG"" thread=""Thread-6""><log4j:message>#6. Test event B.</log4j:message></log4j:event>
";

        private const string MutableContentPart1 = @"<?xml version=""1.0"" encoding=""utf-8""?>
<log4j:event logger=""Root.ChildA.LoggerA2"" timestamp=""1500000011000"" level=""INFO"" thread=""Thread-1""><log4j:message>#1. Test event A.</log4j:message></log4j:event>
";

        private const string MutableContentPart2 = @"
<log4j:event logger=""Root.ChildB.LoggerB2"" timestamp=""1500000012000"" level=""DEBUG"" thread=""Thread-2""><log4j:message>#2. Test event B.</log4j:message></log4j:event>
";

        private string immutableFilePath_;

        private string mutableFilePath_;

        [SetUp]
        public void SetUp ()
        {
            immutableFilePath_ = Path.GetTempFileName ();
            mutableFilePath_ = Path.GetTempFileName ();
        }

        [TearDown]
        public void TearDown ()
        {
            try
            {
                File.Delete (mutableFilePath_);
            }
            finally
            {
                File.Delete (immutableFilePath_);
            }
        }

        private long GetSize (string path)
        {
            using (var file = File.OpenRead (path))
            {
                return file.Length;
            }
        }

        [Test]
        public void CollectionLimitsSizeOfTheFirstFile ()
        {
            var encoding = Encoding.UTF8;

            File.WriteAllText (immutableFilePath_, ImmutableContent, encoding);
            File.WriteAllText (mutableFilePath_, MutableContentPart1, encoding);

            var immutableSize = GetSize (immutableFilePath_);
            var mutableSize = GetSize (mutableFilePath_);

            var totalSize = immutableSize + mutableSize;

            var files = new[]
            {
                mutableFilePath_,
                immutableFilePath_
            };
            var subject = new LogFilesCollection (files, encoding, totalSize);

            File.AppendAllText (mutableFilePath_, MutableContentPart2);

            using (var enumerator = subject.GetEnumerator ())
            {
                var actualMovedToMutableFile = enumerator.MoveNext ();
                Assert.That (actualMovedToMutableFile, Is.True);

                var actualMutableFile = enumerator.Current;
                Assert.That (actualMutableFile.FileName, Is.EqualTo (mutableFilePath_));
                Assert.That (actualMutableFile.Size, Is.EqualTo (mutableSize));

                var actualMovedToImmutableFile = enumerator.MoveNext ();
                Assert.That (actualMovedToImmutableFile, Is.True);

                var actualImutableFile = enumerator.Current;
                Assert.That (actualImutableFile.FileName, Is.EqualTo (immutableFilePath_));
                var events = actualImutableFile.GetEvents ();
                Assert.That (actualImutableFile.Size, Is.EqualTo (immutableSize));

                var actualMovedPastFiles = enumerator.MoveNext ();
                Assert.That (actualMovedPastFiles, Is.False);
            }
        }
    }
}
