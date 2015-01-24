using System;
using System.Runtime.InteropServices;

namespace Log4JParserNet
{
    public sealed class Event
    {
        private static readonly DateTime TimestampZero = new DateTime (1970, 01, 01, 0, 0, 0, DateTimeKind.Utc);

        private readonly EventHandle impl_;

        internal EventHandle Handle
        {
            get { return impl_; }
        }

        internal Event (EventHandle impl)
        {
            impl_ = impl;
        }

        public string Level
        {
            get
            {
                IntPtr value;
                UIntPtr size;
                Log4JParserC.Log4JEventLevel (impl_, out value, out size);

                return value != IntPtr.Zero
                    ? Marshal.PtrToStringAnsi (value, checked ((int) size.ToUInt32 ()))
                    : null;
            }
        }

        public string Logger
        {
            get
            {
                IntPtr value;
                UIntPtr size;
                Log4JParserC.Log4JEventLogger (impl_, out value, out size);

                return value != IntPtr.Zero
                    ? Marshal.PtrToStringAnsi (value, checked ((int) size.ToUInt32 ()))
                    : null;
            }
        }

        public string Thread
        {
            get
            {
                IntPtr value;
                UIntPtr size;
                Log4JParserC.Log4JEventThread (impl_, out value, out size);

                return value != IntPtr.Zero
                    ? Marshal.PtrToStringAnsi (value, checked ((int) size.ToUInt32 ()))
                    : null;
            }
        }

        public Int64 Timestamp
        {
            get { return Log4JParserC.Log4JEventTimestamp (impl_); }
        }

        public DateTime Time
        {
            get { return TimestampZero.AddMilliseconds (Timestamp); }
        }

        public string Message
        {
            get
            {
                IntPtr value;
                UIntPtr size;
                Log4JParserC.Log4JEventMessage (impl_, out value, out size);

                return value != IntPtr.Zero
                    ? Marshal.PtrToStringAnsi (value, checked ((int) size.ToUInt32 ()))
                    : null;
            }
        }

        public string Throwable
        {
            get
            {
                IntPtr value;
                UIntPtr size;
                Log4JParserC.Log4JEventThrowable (impl_, out value, out size);

                return value != IntPtr.Zero
                    ? Marshal.PtrToStringAnsi (value, checked ((int) size.ToUInt32 ()))
                    : null;
            }
        }
    }
}
