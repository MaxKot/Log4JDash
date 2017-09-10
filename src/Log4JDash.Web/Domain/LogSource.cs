using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Hosting;
using Log4JDash.Web.Models;

namespace Log4JDash.Web.Domain
{
    internal sealed class LogSource
    {
        public const string PatternGroupDate = "date";

        public const string PatternGroupRolloverNeg = "roll_neg";

        public const string PatternGroupRolloverPos = "roll_pos";

        public static IFormatProvider Format = CultureInfo.InvariantCulture;

        private readonly ILogDirectoryConfig config_;

        public string Name => config_.Name;

        private string DirectoryPath => !Path.IsPathRooted (config_.DirectoryPath)
            ? Path.Combine (HostingEnvironment.MapPath ("~"), config_.DirectoryPath)
            : config_.DirectoryPath;

        private LogFileStatsCache statsCache_;

        public LogSource (ILogDirectoryConfig config, LogFileStatsCache statsCache)
        {
            Debug.Assert (config != null, "LogSource.ctor: config is null.");
            Debug.Assert (statsCache != null, "LogSource.ctor: statsCache is null.");
            config_ = config;
            statsCache_ = statsCache;
        }

        public EventsCollection GetEvents (LogQuery query)
        {
            var files = GetFiles (true);
            var encoding = config_.Encoding;

            var logFiles = new LogFilesCollection (files, encoding, query.SourceSize);
            var size = query.SourceSize ?? logFiles.Sum (f => f.Size);

            var events = logFiles
                .Scan (new LogAccumulator (statsCache_, query), (a, f) => a.Consume (f))
                .TakeWhile (a => !a.IsComplete)
                .Last ()
                .Events
                .ToList ();
            events.Reverse ();

            return new EventsCollection (events, Name, size);
        }

        public bool IsEmpty ()
        {
            var files = GetFiles (false);
            return files.Count <= 0;
        }

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
            var result = TryParseDate (dateMatch, out var date)
                ? date
                : DateTime.MinValue;

            return result;
        }

        private bool TryParseDate (Group dateMatch, out DateTime result)
        {
            const DateTimeStyles styles = DateTimeStyles.None;
            var dateFormat = config_.DateFormat;

            bool success;
            if (!dateMatch.Success)
            {
                success = false;
                result = default (DateTime);
            }
            else if (string.IsNullOrWhiteSpace (dateFormat))
            {
                success = DateTime.TryParse (dateMatch.Value, Format, styles, out result);
            }
            else
            {
                success = DateTime.TryParseExact
                    (dateMatch.Value, dateFormat, Format, styles, out result);
            }

            return success;
        }

        private long GetRolloverAsc (Match filenameMatch)
        {
            var rolloverMatch = filenameMatch.Groups[PatternGroupRolloverNeg];
            var result = TryParseRollover (rolloverMatch, out var rollover)
                ? rollover
                : Int64.MinValue;

            return result;
        }

        private long GetRolloverDesc (Match filenameMatch)
        {
            var rolloverMatch = filenameMatch.Groups[PatternGroupRolloverPos];
            var result = TryParseRollover (rolloverMatch, out var rollover)
                ? rollover
                : Int64.MaxValue;

            return result;
        }

        private bool TryParseRollover (Group rolloverMatch, out long result)
        {
            result = default (long);
            return rolloverMatch.Success &&
                   Int64.TryParse (rolloverMatch.Value, NumberStyles.Integer, Format, out result);
        }
    }
}
