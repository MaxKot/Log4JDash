using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Log4JDash.Web.Domain
{
    internal sealed class LogFilesCollection
         : IEnumerable<LazyLogFile>
    {
        private sealed class Enumerator : IEnumerator<LazyLogFile>
        {
            private readonly IEnumerator<string> filesEnumerator_;

            private readonly Encoding encoding_;

            private readonly long? maxSize_;

            private LazyLogFile current_;

            public LazyLogFile Current => current_;

            object IEnumerator.Current => Current;

            private long? remainingSize_;

            public Enumerator (IEnumerator<string> filesEnumerator, Encoding encoding, long? maxSize)
            {
                Debug.Assert (filesEnumerator != null, "LogFilesCollection.Enumerator.ctor: filesEnumerator is null.");
                Debug.Assert (encoding != null, "LogFilesCollection.Enumerator.ctor: encoding is null.");

                filesEnumerator_ = filesEnumerator;
                encoding_ = encoding;
                maxSize_ = maxSize;
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

            private LazyLogFile OpenFile (string fileName, long? maxSize)
            {
                Trace.WriteLine ($"Opening log4j file. File name: '{fileName}'.", "Log4JDash.Web.Domain.Log4JFilesCollection");
                var file = new LazyLogFile (fileName, maxSize, encoding_);

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

        private readonly long? maxSize_;

        public LogFilesCollection (IEnumerable<string> files, Encoding encoding, long? maxSize = null)
        {
            Debug.Assert (files != null, "LogFilesCollection.ctor: files is null.");
            Debug.Assert (encoding != null, "LogFilesCollection.ctor: encoding is null.");

            files_ = files;
            encoding_ = encoding;
            maxSize_ = maxSize;
        }

        public IEnumerator<LazyLogFile> GetEnumerator ()
            => new Enumerator (files_.GetEnumerator (), encoding_, maxSize_);

        IEnumerator IEnumerable.GetEnumerator ()
            => GetEnumerator ();
    }
}
