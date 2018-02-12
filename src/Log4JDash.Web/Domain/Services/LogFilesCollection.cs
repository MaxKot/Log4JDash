using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Log4JDash.Web.Domain.Services
{
    internal sealed class LogFilesCollection
         : IEnumerable<LogFile>
    {
        private sealed class Snapshot
        {
            private static readonly IFormatProvider Format = CultureInfo.InvariantCulture;

            private const char Separator = '/';

            public int Count { get; }

            public long FirstFileSize { get; }

            private Snapshot (int count, long firstFileSize)
            {
                Debug.Assert (count >= 0, "LogFilesCollection.Snapshot.ctor: count is less than zero.");
                Debug.Assert (firstFileSize >= 0, "LogFilesCollection.Snapshot.ctor: firstFileSize is less than zero.");

                Count = count;
                FirstFileSize = firstFileSize;
            }

            public override string ToString ()
                => String.Format (Format, "{0}" + Separator + "{1}", Count, FirstFileSize);

            public static Snapshot Parse (string s)
            {
                Debug.Assert (s != null, "LogFilesCollection.Snapshot.ctor: s is null.");

                var separatorIndex = s.IndexOf (Separator);
                Debug.Assert (separatorIndex >= 0, "LogFilesCollection.Snapshot.ctor: separatorIndex is less than zero.");

                var countString = s.Remove (separatorIndex);
                var count = Int32.Parse (countString, Format);

                var firstFileSizeString = s.Substring (separatorIndex + 1);
                var firstFileSize = Int64.Parse (firstFileSizeString, Format);

                return new Snapshot (count, firstFileSize);
            }

            public static Snapshot Create (IReadOnlyList<string> files, Encoding encoding)
            {
                Debug.Assert (files != null, "LogFilesCollection.Snapshot.ctor: files is null.");
                Debug.Assert (encoding != null, "LogFilesCollection.Snapshot.ctor: encoding is null.");

                var count = files.Count;
                long firstFileSize;
                if (files.Any ())
                {
                    using (var firstFile = OpenFile (files.First (), encoding, null))
                    {
                        firstFileSize = firstFile.Size;
                    }
                }
                else
                {
                    firstFileSize = 0;
                }

                return new Snapshot (count, firstFileSize);
            }
        }

        private sealed class Enumerator : IEnumerator<LogFile>
        {
            private readonly IEnumerator<string> filesEnumerator_;

            private readonly Encoding encoding_;

            private readonly Snapshot snapshot_;

            private LogFile current_;

            public LogFile Current => current_;

            object IEnumerator.Current => Current;

            private long? nextFileMaxSize_;

            public Enumerator (IReadOnlyList<string> files, Encoding encoding, Snapshot snapshot)
            {
                Debug.Assert (files != null, "LogFilesCollection.Enumerator.ctor: files is null.");
                Debug.Assert (encoding != null, "LogFilesCollection.Enumerator.ctor: encoding is null.");
                Debug.Assert (snapshot != null, "LogFilesCollection.Enumerator.ctor: snapshot is null.");

                filesEnumerator_ = files.Skip (files.Count - snapshot.Count).GetEnumerator ();
                encoding_ = encoding;
                snapshot_ = snapshot;

                nextFileMaxSize_ = snapshot_.FirstFileSize;
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

                if (!filesEnumerator_.MoveNext ())
                {
                    return false;
                }

                current_ = OpenFile (filesEnumerator_.Current, encoding_, nextFileMaxSize_);
                nextFileMaxSize_ = null;

                return true;
            }

            public void Reset ()
            {
                filesEnumerator_.Reset ();

                current_?.Dispose ();
                current_ = null;
                nextFileMaxSize_ = 0;
            }
        }

        private readonly IReadOnlyList<string> files_;

        private readonly Encoding encoding_;

        private readonly Snapshot snapshot_;

        public LogFilesCollection (IEnumerable<string> files, Encoding encoding, string snapshot = null)
        {
            Debug.Assert (files != null, "LogFilesCollection.ctor: files is null.");
            Debug.Assert (encoding != null, "LogFilesCollection.ctor: encoding is null.");

            files_ = files is IReadOnlyList<string> filesList
                ? filesList
                : files.ToList ();
            encoding_ = encoding;

            snapshot_ = !String.IsNullOrWhiteSpace (snapshot)
                ? Snapshot.Parse (snapshot)
                : Snapshot.Create (files_, encoding_);
        }

        public IEnumerator<LogFile> GetEnumerator ()
            => new Enumerator (files_, encoding_, snapshot_);

        IEnumerator IEnumerable.GetEnumerator ()
            => GetEnumerator ();

        public string GetSnapshot ()
            => snapshot_.ToString ();

        private static LogFile OpenFile (string fileName, Encoding encoding, long? maxSize)
        {
            Trace.WriteLine ($"Opening log4j file. File name: '{fileName}'.", "Log4JDash.Web.Domain.Log4JFilesCollection");
            var file = new LogFile (fileName, maxSize, encoding);

            return file;
        }
    }
}
