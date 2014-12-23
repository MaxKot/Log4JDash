using System;
using System.Runtime.InteropServices;

namespace Log4JParserNet
{
    public sealed class Event
    {
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

                return Marshal.PtrToStringAnsi (value, checked ((int) size.ToUInt32 ()));
            }
        }

        public string Logger
        {
            get
            {
                IntPtr value;
                UIntPtr size;
                Log4JParserC.Log4JEventLogger (impl_, out value, out size);

                return Marshal.PtrToStringAnsi (value, checked ((int) size.ToUInt32 ()));
            }
        }

        public string Thread
        {
            get
            {
                IntPtr value;
                UIntPtr size;
                Log4JParserC.Log4JEventThread (impl_, out value, out size);

                return Marshal.PtrToStringAnsi (value, checked ((int) size.ToUInt32 ()));
            }
        }

        public string Timestamp
        {
            get
            {
                IntPtr value;
                UIntPtr size;
                Log4JParserC.Log4JEventTimestamp (impl_, out value, out size);

                return Marshal.PtrToStringAnsi (value, checked ((int) size.ToUInt32 ()));
            }
        }

        public Int64 Time
        {
            get { return Log4JParserC.Log4JEventTime (impl_); }
        }

        public string Message
        {
            get
            {
                IntPtr value;
                UIntPtr size;
                Log4JParserC.Log4JEventMessage (impl_, out value, out size);

                return Marshal.PtrToStringAnsi (value, checked ((int) size.ToUInt32 ()));
            }
        }

        public string Throwable
        {
            get
            {
                IntPtr value;
                UIntPtr size;
                Log4JParserC.Log4JEventThrowable (impl_, out value, out size);

                return Marshal.PtrToStringAnsi (value, checked ((int) size.ToUInt32 ()));
            }
        }
    }
}
