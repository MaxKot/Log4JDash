using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;

namespace Log4JDash.Web.Domain
{
    internal sealed class LogSourceProvider
    {
        private readonly ILogSourceProviderConfig config_;

        public LogSourceProvider (ILogSourceProviderConfig config)
        {
            if (config == null)
            {
                throw new ArgumentNullException (nameof (config));
            }

            config_ = config;
        }

        private IReadOnlyDictionary<string, string> DoGetSources ()
        {
            var result = new Dictionary<string, string> ();

            foreach (var directory in config_.Directories)
            {
                var directoryPath = !Path.IsPathRooted (directory.DirectoryPath)
                    ? Path.Combine (HostingEnvironment.MapPath ("~"), directory.DirectoryPath)
                    : directory.DirectoryPath;
                string[] files;
                try
                {
                    files = Directory.GetFiles (directoryPath);
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
                foreach (var fullPath in files)
                {
                    if (directory.FilenamePattern.IsMatch (fullPath))
                    {
                        var fileId = GetFileId (directory, fullPath);
                        result.Add (fileId, fullPath);
                    }
                }
            }

            return result;
        }

        private static string GetFileId (ILogDirectoryConfig directory, string fullPath)
        {
            var fileName = Path.GetFileName (fullPath);
            return String.Format ("{0}-{1}", directory.Name, fileName).ToLower ();
        }

        public IEnumerable<string> GetSources ()
        {
            return DoGetSources ().Keys.OrderBy (k => k);
        }

        public LogSource GetSource (string sourceId)
        {
            var sources = DoGetSources ();
            var key = String.IsNullOrWhiteSpace (sourceId)
                ? GetSources ().First ()
                : sourceId;

            string file;
            try
            {
                file = sources[key];
            }
            catch (KeyNotFoundException ex)
            {
                throw new ArgumentOutOfRangeException ("Invalid log source.", ex);
            }

            return new LogSource (key, file);
        }
    }
}
