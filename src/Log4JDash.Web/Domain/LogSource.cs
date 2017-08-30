using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
            var files = GetFiles (true);

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
            var files = GetFiles (false);
            return files.Count <= 0;
        }

        public const string PatternGroupDate = "date";

        public const string PatternGroupRolloverAsc = "roll_asc";

        public const string PatternGroupRolloverDesc = "roll_desc";

        private IReadOnlyCollection<string> GetFiles (bool ordered)
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

            var filenamePattern = config_.FilenamePattern;
            var matchingFiles = files
                .Select (f => new { Filename = f, Match = filenamePattern.Match (f) })
                .Where (mf => mf.Match.Success);

            IEnumerable<string> result;
            if (ordered)
            {
                // OrderBy is documented to perform stable sort. It will not change the order of
                // elements in the result if a pattern group is missing.
                result = matchingFiles
                    .OrderByDescending (mf => GetDate (mf.Match))
                    .ThenBy (mf => GetRolloverAsc (mf.Match))
                    .ThenByDescending (mf => GetRolloverDesc (mf.Match))
                    .Select (f => f.Filename)
                    .ToList ();
            }
            else
            {
                result = matchingFiles.Select (f => f.Filename);
            }

            return result.ToList ();
        }

        private DateTime GetDate (Match filenameMatch)
        {
            var dateMatch = filenameMatch.Groups[PatternGroupDate];
            var result = dateMatch.Success && DateTime.TryParse (dateMatch.Value, out var date)
                ? date
                : DateTime.MinValue;

            return result;
        }

        private long GetRolloverAsc (Match filenameMatch)
        {
            var rolloverMatch = filenameMatch.Groups[PatternGroupRolloverAsc];
            var result = rolloverMatch.Success && Int64.TryParse (rolloverMatch.Value, out var rollover)
                ? rollover
                : Int64.MinValue;

            return result;
        }

        private long GetRolloverDesc (Match filenameMatch)
        {
            var rolloverMatch = filenameMatch.Groups[PatternGroupRolloverDesc];
            var result = rolloverMatch.Success && Int64.TryParse (rolloverMatch.Value, out var rollover)
                ? rollover
                : Int64.MaxValue;

            return result;
        }
    }
}
