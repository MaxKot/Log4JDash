﻿using System;
using System.Runtime.InteropServices;

namespace Log4JParserNet
{
    internal static class Log4JParserC
    {
        public enum Status
        {
            Success = 0,
            MemoryError = 10,
            DocumentErrors = 20
        }

        [UnmanagedFunctionPointer (CallingConvention.Cdecl)]
        public delegate IntPtr Alloc (UIntPtr size);

        [UnmanagedFunctionPointer (CallingConvention.Cdecl)]
        public delegate void Free (IntPtr ptr);

        [DllImport ("Log4JParserC.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static void Log4JSetAllocator (Alloc alloc, Free free);

        [DllImport ("Log4JParserC.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static void Log4JSetDefaultAllocator ();

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
        extern public static Int64 Log4JEventTimestamp (EventHandle log4JEvent);

        [DllImport ("Log4JParserC.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static void Log4JEventMessage
            (EventHandle log4JEvent, out IntPtr value, out UIntPtr size);

        [DllImport ("Log4JParserC.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static void Log4JEventThrowable
            (EventHandle log4JEvent, out IntPtr value, out UIntPtr size);

        [DllImport ("Log4JParserC.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static UIntPtr Log4JEventProperties
            (
            EventHandle log4JEvent,
            UIntPtr skip,
            [Out]
            [MarshalAs (UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct, SizeParamIndex = 3)]
            Log4JEventProperty[] properties,
            UIntPtr propertiesSize
            );

        [DllImport("Log4JParserC.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static Status Log4JEventSourceInitXmlString
            (out EventSourceHandle self, IntPtr xmlString);

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
        [return: MarshalAs(UnmanagedType.U1)]
        extern public static bool Log4JFilterApply (FilterHandle self, EventHandle @event);

        [DllImport ("Log4JParserC.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static Status Log4JFilterInitLevelC
            (
            out FilterHandle self,
            [MarshalAs (UnmanagedType.LPStr)]
            string min,
            [MarshalAs (UnmanagedType.LPStr)]
            string max
            );

        [DllImport ("Log4JParserC.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static Status Log4JFilterInitLoggerFs
            (out FilterHandle self, [MarshalAs (UnmanagedType.LPArray)] byte[] logger, UIntPtr loggerSize);

        [DllImport ("Log4JParserC.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static Status Log4JFilterInitMessageFs
            (out FilterHandle self, [MarshalAs (UnmanagedType.LPArray)] byte[] message, UIntPtr messageSize);

        [DllImport ("Log4JParserC.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static Status Log4JFilterInitTimestamp
            (out FilterHandle self, Int64 min, Int64 max);

        [DllImport ("Log4JParserC.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static Status Log4JFilterInitAll (out FilterHandle self);

        [DllImport ("Log4JParserC.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static Status Log4JFilterAllAdd (FilterHandle self, FilterHandle childFilter);

        [DllImport ("Log4JParserC.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static void Log4JFilterAllRemove
            (FilterHandle self, FilterHandle childFilter);

        [DllImport ("Log4JParserC.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static Status Log4JFilterInitAny (out FilterHandle self);

        [DllImport ("Log4JParserC.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static Status Log4JFilterAnyAdd (FilterHandle self, FilterHandle childFilter);

        [DllImport ("Log4JParserC.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static void Log4JFilterAnyRemove
            (FilterHandle self, FilterHandle childFilter);

        [DllImport ("Log4JParserC.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static Status Log4JFilterInitNot
            (out FilterHandle self, FilterHandle childFilter);

        [DllImport ("Log4JParserC.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static void Log4JIteratorDestroy (IntPtr self);

        [DllImport ("Log4JParserC.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        extern public static bool Log4JIteratorMoveNext (IteratorHandle self);

        [DllImport ("Log4JParserC.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static EventHandle Log4JIteratorCurrent (IteratorHandle self, out UIntPtr id);

        [DllImport ("Log4JParserC.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static void Log4JIteratorInitEventSource
            (out IteratorHandle self, EventSourceHandle source);

        [DllImport ("Log4JParserC.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static void Log4JIteratorInitEventSourceReverse
            (out IteratorHandle self, EventSourceHandle source);

        [DllImport ("Log4JParserC.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static void Log4JIteratorInitFilter
            (out IteratorHandle self, IteratorHandle inner, FilterHandle filter);

        [DllImport ("Log4JParserC.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static Int32 Log4JGetLevelValueNt
            ([MarshalAs (UnmanagedType.LPStr)] string value);

        [DllImport ("Log4JParserC.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static void Log4JLevelAll (out IntPtr value);

        [DllImport ("Log4JParserC.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static void Log4JLevelDebug (out IntPtr value);

        [DllImport ("Log4JParserC.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static void Log4JLevelError (out IntPtr value);

        [DllImport ("Log4JParserC.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static void Log4JLevelFatal (out IntPtr value);

        [DllImport ("Log4JParserC.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static void Log4JLevelInfo (out IntPtr value);

        [DllImport ("Log4JParserC.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static void Log4JLevelOff (out IntPtr value);

        [DllImport ("Log4JParserC.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static void Log4JLevelWarn (out IntPtr value);
    }
}
