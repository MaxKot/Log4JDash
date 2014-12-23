using System;
using System.Runtime.InteropServices;

namespace Log4JParserNet
{
    internal sealed class EventHandle : SafeHandle
    {
        private EventHandle (bool ownsHandle)
            : base (IntPtr.Zero, ownsHandle)
        {

        }

        public EventHandle ()
            : this (false)
        {

        }

        /// <inheritdoc />
        public override bool IsInvalid
        {
            get { return handle == IntPtr.Zero; }
        }

        /// <inheritdoc />
        protected override bool ReleaseHandle ()
        {
            handle = IntPtr.Zero;
            return true;
        }
    }
}
