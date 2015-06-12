using System;
using System.Runtime.InteropServices;

namespace Log4JParserNet
{
    internal sealed class EventSourceHandle : SafeHandle
    {
        private EventSourceHandle (bool ownsHandle)
            : base (IntPtr.Zero, ownsHandle)
        {

        }

        public EventSourceHandle ()
            : this (true)
        {

        }

        /// <inheritdoc />
        public override bool IsInvalid => handle == IntPtr.Zero;

        /// <inheritdoc />
        protected override bool ReleaseHandle ()
        {
            Log4JParserC.Log4JEventSourceDestroy (handle);
            return true;
        }
    }
}
