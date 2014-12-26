using System;
using System.Runtime.InteropServices;

namespace Log4JParserNet
{
    internal static class Log4JParserC
    {
        [DllImport ("Log4JParserC.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static void Log4JEventLevel
            (EventHandle log4JEvent, out IntPtr value, out UIntPtr size);

        [DllImport ("Log4JParserC.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static void Log4JEventLogger
            (EventHandle log4JEvent, out IntPtr value, out UIntPtr size);

        [DllImport ("Log4JParserC.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static void Log4JEventThread
            (EventHandle log4JEvent, out IntPtr value, out UIntPtr size);

        [DllImport ("Log4JParserC.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static void Log4JEventTimestamp
            (EventHandle log4JEvent, out IntPtr value, out UIntPtr size);
        [DllImport ("Log4JParserC.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static Int64 Log4JEventTime (EventHandle log4JEvent);

        [DllImport ("Log4JParserC.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static void Log4JEventMessage
            (EventHandle log4JEvent, out IntPtr value, out UIntPtr size);

        [DllImport ("Log4JParserC.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static void Log4JEventThrowable
            (EventHandle log4JEvent, out IntPtr value, out UIntPtr size);

        [DllImport ("Log4JParserC.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static void Log4JEventSourceInitXmlFile
            (out EventSourceHandle self, [MarshalAs (UnmanagedType.LPStr)] string filePath);

        [DllImport("Log4JParserC.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static void Log4JEventSourceInitXmlString
            (out EventSourceHandle self, string xmlString);

        [DllImport ("Log4JParserC.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static void Log4JEventSourceDestroy (IntPtr self);

        [DllImport ("Log4JParserC.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static EventHandle Log4JEventSourceFirst (EventSourceHandle self);

        [DllImport ("Log4JParserC.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static EventHandle Log4JEventSourceNext
            (EventSourceHandle self, EventHandle @event);

        [DllImport ("Log4JParserC.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static void Log4JFilterDestroy (IntPtr self);

        [DllImport ("Log4JParserC.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static bool Log4JFilterApply (FilterHandle self, EventHandle @event);

        [DllImport ("Log4JParserC.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static void Log4JFilterInitLevelI
            (out FilterHandle self, Int32 min, Int32 max);

        [DllImport ("Log4JParserC.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static void Log4JFilterInitLevelC
            (
            out FilterHandle self,
            [MarshalAs (UnmanagedType.LPStr)]
            string min,
            [MarshalAs (UnmanagedType.LPStr)]
            string max
            );

        [DllImport ("Log4JParserC.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static void Log4JFilterInitLoggerNt
            (out FilterHandle self, [MarshalAs (UnmanagedType.LPStr)] string logger);

        [DllImport ("Log4JParserC.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static void Log4JFilterInitMessageNt
            (out FilterHandle self, [MarshalAs (UnmanagedType.LPStr)] string message);

        [DllImport ("Log4JParserC.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static void Log4JFilterInitTimestamp
            (out FilterHandle self, Int64 min, Int64 max);

        [DllImport ("Log4JParserC.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static void Log4JFilterInitAll (out FilterHandle self);

        [DllImport ("Log4JParserC.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static void Log4JFilterAllAdd (FilterHandle self, FilterHandle childFilter);

        [DllImport ("Log4JParserC.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static void Log4JFilterAllRemove
            (FilterHandle self, FilterHandle childFilter);

        [DllImport ("Log4JParserC.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static void Log4JFilterInitAny (out FilterHandle self);

        [DllImport ("Log4JParserC.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static void Log4JFilterAnyAdd (FilterHandle self, FilterHandle childFilter);

        [DllImport ("Log4JParserC.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static void Log4JFilterAnyRemove
            (FilterHandle self, FilterHandle childFilter);

        [DllImport ("Log4JParserC.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static void Log4JFilterInitNot
            (out FilterHandle self, FilterHandle childFilter);

        [DllImport ("Log4JParserC.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static void Log4JIteratorDestroy (IntPtr self);

        [DllImport ("Log4JParserC.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static bool Log4JIteratorMoveNext (IteratorHandle self);

        [DllImport ("Log4JParserC.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static EventHandle Log4JIteratorCurrent (IteratorHandle self);

        [DllImport ("Log4JParserC.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static void Log4JIteratorInitEventSource
            (out IteratorHandle self, EventSourceHandle source);

        [DllImport ("Log4JParserC.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static void Log4JIteratorInitFilter
            (out IteratorHandle self, IteratorHandle inner, FilterHandle filter);
    }
}
