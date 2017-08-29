using System;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using Log4JDash.Web.Models;

namespace Log4JDash.Web.Domain
{
    internal sealed class LogSource
    {
        private readonly ILogDirectoryConfig config_;

        public string Name => config_.Name;

        private string DirectoryPath => !Path.IsPathRooted (config_.DirectoryPath)
            ? Path.Combine (HostingEnvironment.MapPath ("~"), config_.DirectoryPath)
            : config_.DirectoryPath;

        public LogSource (ILogDirectoryConfig config)
        {
            config_ = config;
        }

        public EventsCollection GetEvents (LogQuery query)
        {
            var files = GetFiles ();
            var size = query.SourceSize ?? files.Sum (f => GetFileSize (f));

            var encoding = config_.Encoding;

            var filterBuilder = query.CreateFilter ();
            using (var filter = filterBuilder?.Build ())
            {
                var log4JFiles = new Log4JFilesCollection (files, encoding, size);

                var events = log4JFiles
                    .Select (f => f.GetEventsReverse ())
                    .SelectMany (es => filter != null ? es.Where (filter) : es)
                    .Skip (query.Offset)
                    .Take (query.Quantity)
                    .Select (x => new EventModel (x))
                    .ToList ();
                events.Reverse ();

                return new EventsCollection (events, Name, size);
            }
        }

        private long GetFileSize (string fileName)
        {
            using (var file = File.OpenRead (fileName))
            {
                return file.Length;
            }
        }

        public bool IsEmpty ()
        {
            var files = GetFiles ();
            return files.Length <= 0;
        }

        private string[] GetFiles ()
        {
            string[] files;

            try
            {
                files = Directory.GetFiles (DirectoryPath);
            }
            catch (DirectoryNotFoundException)
            {
                files = new string[0];
            }
            catch (PathTooLongException)
            {
                files = new string[0];
            }
            catch (UnauthorizedAccessException)
            {
                files = new string[0];
            }
            catch (IOException)
            {
                files = new string[0];
            }

            var result = Array.FindAll (files, config_.FilenamePattern.IsMatch);
            return result;
        }
    }
}
