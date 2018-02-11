using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Log4JDash.Web.Domain.Services
{
    internal sealed class LogFilesCollection
         : IEnumerable<LogFile>
    {
        private sealed class Enumerator : IEnumerator<LogFile>
        {
            private readonly IEnumerator<string> filesEnumerator_;

            private readonly Encoding encoding_;

            private readonly long? maxSize_;

            private LogFile current_;

            public LogFile Current => current_;

            object IEnumerator.Current => Current;

            private long? remainingSize_;

            public Enumerator (IEnumerable<string> files, Encoding encoding, string snapshot)
            {
                Debug.Assert (files != null, "LogFilesCollection.Enumerator.ctor: files is null.");
                Debug.Assert (encoding != null, "LogFilesCollection.Enumerator.ctor: encoding is null.");

                filesEnumerator_ = files.GetEnumerator ();
                encoding_ = encoding;
                maxSize_ = snapshot != null
                    ? (long?) Int64.Parse (snapshot, System.Globalization.CultureInfo.InvariantCulture)
                    : null;
                remainingSize_ = maxSize_;
            }

            public void Dispose ()
            {
                Exception filesEnumeratorException = null;
                try
                {
                    filesEnumerator_.Dispose ();
                }
                catch (Exception ex)
                {
                    filesEnumeratorException = ex;
                }
                finally
                {
                    try
                    {
                        current_?.Dispose ();
                    }
                    catch (Exception ex)
                    {
                        if (filesEnumeratorException != null)
                        {
                            throw new AggregateException (ex, filesEnumeratorException);
                        }

                        throw;
                    }
                }
            }

            public bool MoveNext ()
            {
                current_?.Dispose ();
                current_ = null;

                if (remainingSize_ <= 0L || !filesEnumerator_.MoveNext ())
                {
                    return false;
                }

                var file = OpenFile (filesEnumerator_.Current, remainingSize_);
                remainingSize_ -= file.Size;

                current_ = file;
                return true;
            }

            private LogFile OpenFile (string fileName, long? maxSize)
            {
                Trace.WriteLine ($"Opening log4j file. File name: '{fileName}'.", "Log4JDash.Web.Domain.Log4JFilesCollection");
                var file = new LogFile (fileName, maxSize, encoding_);

                return file;
            }

            public void Reset ()
            {
                filesEnumerator_.Reset ();

                current_?.Dispose ();
                current_ = null;
                remainingSize_ = maxSize_;
            }
        }

        private readonly IEnumerable<string> files_;

        private readonly Encoding encoding_;

        private readonly string snapshot_;

        public LogFilesCollection (IEnumerable<string> files, Encoding encoding, string snapshot = null)
        {
            Debug.Assert (files != null, "LogFilesCollection.ctor: files is null.");
            Debug.Assert (encoding != null, "LogFilesCollection.ctor: encoding is null.");

            files_ = files;
            encoding_ = encoding;
            snapshot_ = snapshot;
        }

        public IEnumerator<LogFile> GetEnumerator ()
            => new Enumerator (files_, encoding_, snapshot_);

        IEnumerator IEnumerable.GetEnumerator ()
            => GetEnumerator ();

        public string GetSnapshot ()
            => snapshot_ ?? Enumerable.Sum (this, f => f.Size).ToString (System.Globalization.CultureInfo.InvariantCulture);
    }
}
