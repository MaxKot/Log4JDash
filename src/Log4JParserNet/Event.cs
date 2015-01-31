using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Log4JParserNet
{
    public sealed class Event
    {
        private static readonly DateTime TimestampZero = new DateTime (1970, 01, 01, 0, 0, 0, DateTimeKind.Utc);

        private readonly EventHandle impl_;

        private readonly SafeHandle owner_;

        internal EventHandle Handle
        {
            get { return impl_; }
        }

        internal Event (EventHandle impl, SafeHandle owner)
        {
            Debug.Assert (impl != null, "Event.ctor: impl is null.");
            Debug.Assert (owner != null, "Event.ctor: owner is null.");

            impl_ = impl;
            owner_ = owner;
        }

        public string Level
        {
            get
            {
                GuardState ();

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
                GuardState ();

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
                GuardState ();

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
            get
            {
                GuardState ();

                return Log4JParserC.Log4JEventTimestamp (impl_);
            }
        }

        public DateTime Time
        {
            get { return TimestampZero.AddMilliseconds (Timestamp); }
        }

        public string Message
        {
            get
            {
                GuardState ();

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
                GuardState ();

                IntPtr value;
                UIntPtr size;
                Log4JParserC.Log4JEventThrowable (impl_, out value, out size);

                return value != IntPtr.Zero
                    ? Marshal.PtrToStringAnsi (value, checked ((int) size.ToUInt32 ()))
                    : null;
            }
        }

        private void GuardState ()
        {
            if (owner_.IsInvalid)
            {
                throw new InvalidOperationException ("Owner object is invalid.");
            }
        }
    }
}
