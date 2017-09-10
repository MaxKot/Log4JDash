using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Log4JParserNet;

namespace Log4JDash.Web.Domain
{
    internal sealed class LazyLogFile : IDisposable
    {
        public string FileName => fileStream_.Name;

        private long? maxSize_;

        private Encoding encoding_;

        private readonly Lazy<Log4JFile> file_;

        private readonly Lazy<long> size_;

        public long Size => maxSize_ != null
            ? Math.Min (maxSize_.Value, size_.Value)
            : size_.Value;

        private readonly FileStream fileStream_;

        public LazyLogFile (string fileName, long? maxSize, Encoding encoding)
        {
            maxSize_ = maxSize;
            encoding_ = encoding;
            file_ = new Lazy<Log4JFile> (OpenLog4JFile);
            size_ = new Lazy<long> (() => fileStream_.Length);
            fileStream_ = File.Open (fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }

        private Log4JFile OpenLog4JFile ()
        {
            Trace.WriteLine ($"Reading log4j file. File name: '{fileStream_.Name}'.", "Log4JDash.Web.Domain.LazyLogFile");

            EnsureSizeIsRead ();
            var effectiveMaxSize = maxSize_ ?? Size;
            var result = Log4JFile.Create (fileStream_, effectiveMaxSize);
            result.Encoding = encoding_;
            fileStream_.Dispose ();

            return result;
        }

        [MethodImpl (MethodImplOptions.NoInlining)]
        private void EnsureSizeIsRead () => _ = size_.Value;

        public IEnumerableOfEvents GetEvents () => file_.Value.GetEvents ();

        public IEnumerableOfEvents GetEventsReverse () => file_.Value.GetEventsReverse ();

        public void Dispose ()
        {
            try
            {
                fileStream_.Dispose ();
            }
            finally
            {
                if (file_.IsValueCreated)
                {
                    file_.Value.Dispose ();
                }
            }
        }
    }
}
