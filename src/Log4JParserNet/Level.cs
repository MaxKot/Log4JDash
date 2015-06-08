using System;
using System.Runtime.InteropServices;

namespace Log4JParserNet
{
    public static class Level
    {
        private delegate void GetUnmanagedString (out IntPtr value);

        public static string All { get; }

        public static string Debug { get; }

        public static string Error { get; }

        public static string Fatal { get; }

        public static string Info { get; }

        public static string Off { get; }

        public static string Warn { get; }

        static Level ()
        {
            All = ReadUnmanaged (Log4JParserC.Log4JLevelAll);
            Debug = ReadUnmanaged (Log4JParserC.Log4JLevelDebug);
            Error = ReadUnmanaged (Log4JParserC.Log4JLevelError);
            Fatal = ReadUnmanaged (Log4JParserC.Log4JLevelFatal);
            Info = ReadUnmanaged (Log4JParserC.Log4JLevelInfo);
            Off = ReadUnmanaged (Log4JParserC.Log4JLevelOff);
            Warn = ReadUnmanaged (Log4JParserC.Log4JLevelWarn);
        }

        private static string ReadUnmanaged (GetUnmanagedString impl)
        {
            IntPtr value;
            impl (out value);

            return value != IntPtr.Zero
                ? Marshal.PtrToStringAnsi (value)
                : null;
        }
    }
}
