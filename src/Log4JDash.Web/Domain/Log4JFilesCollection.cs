using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Log4JParserNet;

namespace Log4JDash.Web.Domain
{
    internal sealed class Log4JFilesCollection
         : IEnumerable<Log4JFile>
    {
        private sealed class Enumerator : IEnumerator<Log4JFile>
        {
            private readonly IEnumerator<string> filesEnumerator_;

            private readonly Encoding encoding_;

            private readonly long? maxSize_;

            private Log4JFile current_;

            public Log4JFile Current => current_;

            object IEnumerator.Current => Current;

            private long? remainingSize_;

            public Enumerator (IEnumerator<string> filesEnumerator, Encoding encoding, long? maxSize)
            {
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

                var fileName = filesEnumerator_.Current;
                Trace.WriteLine ($"Opening log4j file. File name: '{fileName}'.", "Log4JDash.Web.Domain.Log4JFilesCollection");
                var file = Log4JFile.Create (fileName, remainingSize_);
                file.Encoding = encoding_;
                remainingSize_ -= file.Size;

                current_ = file;
                return true;
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

        public Log4JFilesCollection (IEnumerable<string> files, Encoding encoding, long? maxSize = null)
        {
            if (files == null)
            {
                throw new ArgumentNullException (nameof (files));
            }
            if (encoding == null)
            {
                throw new ArgumentNullException (nameof (encoding));
            }

            files_ = files;
            encoding_ = encoding;
            maxSize_ = maxSize;
        }

        public IEnumerator<Log4JFile> GetEnumerator ()
            => new Enumerator (files_.GetEnumerator (), encoding_, maxSize_);

        IEnumerator IEnumerable.GetEnumerator ()
            => GetEnumerator ();
    }
}
