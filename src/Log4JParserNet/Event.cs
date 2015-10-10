using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Log4JParserNet
{
    public sealed class Event
    {
        private readonly EventHandle impl_;

        private readonly ulong id_;

        private readonly IEventSource owner_;

        internal EventHandle Handle
        {
            get { return impl_; }
        }

        internal Event (EventHandle impl, ulong id, IEventSource owner)
        {
            Debug.Assert (impl != null, "Event.ctor: impl is null.");
            Debug.Assert (owner != null, "Event.ctor: owner is null.");

            impl_ = impl;
            id_ = id;
            owner_ = owner;
        }

        private unsafe string PtrToString (IntPtr value, UIntPtr size)
        {
            var intSize = (int) size.ToUInt32 ();
            string result = null;

            if (value != IntPtr.Zero)
            {
                var buffer = (byte*) value.ToPointer ();

                var encoding = owner_.Encoding;
                var charCount = encoding.GetCharCount (buffer, intSize);

                var decodeBuffer = new char[charCount];
                if (decodeBuffer.Length > 0)
                {
                    fixed (char* decodeBufferPtr = &decodeBuffer[0])
                    {
                        encoding.GetChars (buffer, intSize, decodeBufferPtr, decodeBuffer.Length);
                    }
                }

                result = new String (decodeBuffer);
            }

            return result;
        }

        public string Level
        {
            get
            {
                GuardState ();

                IntPtr value;
                UIntPtr size;
                Log4JParserC.Log4JEventLevel (impl_, out value, out size);

                return PtrToString (value, size);
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

                return PtrToString (value, size);
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

                return PtrToString (value, size);
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
            get { return Log4JParserNet.Timestamp.ToDateTime (Timestamp); }
        }

        public string Message
        {
            get
            {
                GuardState ();

                IntPtr value;
                UIntPtr size;
                Log4JParserC.Log4JEventMessage (impl_, out value, out size);

                return PtrToString (value, size);
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

                return PtrToString (value, size);
            }
        }

        public IList<KeyValuePair<string, string>> GetProperties ()
        {
            const int bufferSize = 16;
            var buffer = new Log4JEventProperty[bufferSize];

            var result = new List<KeyValuePair<string, string>> (16);

            uint totalEvents;
            var totalEventsRead = 0U;
            do
            {
                totalEvents = Log4JParserC
                    .Log4JEventProperties (impl_, new UIntPtr (totalEventsRead), buffer, new UIntPtr ((uint) buffer.Length))
                    .ToUInt32 ();

                var eventsRemaining = totalEvents - totalEventsRead;
                var eventsRead = eventsRemaining > bufferSize ? bufferSize : eventsRemaining;

                for (var i = 0U; i < eventsRead; ++i)
                {
                    var name = PtrToString (buffer[i].Name, buffer[i].NameSize);
                    var value = PtrToString (buffer[i].Value, buffer[i].ValueSize);
                    result.Add (new KeyValuePair<string, string> (name, value));
                }

                totalEventsRead += eventsRead;
            } while (totalEventsRead != totalEvents);

            return result;
        }

        public ulong Id
        {
            get { return id_; }
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
