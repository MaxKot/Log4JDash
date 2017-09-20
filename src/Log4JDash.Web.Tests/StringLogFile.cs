using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Log4JDash.Web.Domain;
using Log4JParserNet;

namespace Log4JDash.Web.Tests
{
    internal sealed class StringLogFile
        : ILogFile
        , ICloneable
    {
        private static readonly Encoding Encoding = Encoding.UTF8;

        public string FileName { get; }

        private readonly byte[] content_;

        public long Size => content_.LongLength;

        private readonly Lazy<Log4JFile> file_;

        public bool WasRead => file_.IsValueCreated;

        public StringLogFile (string fileName, string content)
        {
            Debug.Assert (content != null, "StringLogFile.ctor: content is null.");

            FileName = fileName;
            content_ = Encoding.GetBytes (content);
            file_ = new Lazy<Log4JFile> (ReadFile);
        }

        public StringLogFile (StringLogFile other)
        {
            Debug.Assert (other != null, "StringLogFile.ctor: other is null.");

            FileName = other.FileName;
            content_ = other.content_;
            file_ = new Lazy<Log4JFile> (ReadFile);
        }

        public StringLogFile Clone ()
            => new StringLogFile (this);

        object ICloneable.Clone ()
            => Clone ();

        public void InitializeDefaultStats (ILogFileStatsProvider statsProvider)
            => InitializeStats ((FilterBuilder) null, statsProvider);

        public void InitializeStats (ILogQuery query, ILogFileStatsProvider statsProvider)
            => InitializeStats (query.CreateFilter (), statsProvider);

        public void InitializeStats (FilterBuilder filter, ILogFileStatsProvider statsProvider)
        {
            using (var copy = Clone ())
            {
                statsProvider.GetStats (copy, filter);
            }
        }

        private Log4JFile ReadFile ()
        {
            using (var contentStream = new MemoryStream (content_))
            {
                var result = Log4JFile.Create (contentStream);
                result.Encoding = Encoding;
                return result;
            }
        }

        public IEnumerableOfEvents GetEvents ()
            => file_.Value.GetEvents ();

        public IEnumerableOfEvents GetEventsReverse ()
            => file_.Value.GetEventsReverse ();

        public void Dispose ()
        {
            if (file_.IsValueCreated)
            {
                file_.Value.Dispose ();
            }
        }
    }
}
