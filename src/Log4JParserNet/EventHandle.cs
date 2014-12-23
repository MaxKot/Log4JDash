using System;
using System.Runtime.InteropServices;

namespace Log4JParserNet
{
    internal sealed class EventHandle : SafeHandle
    {
        public EventHandle (bool ownsHandle)
            : base (IntPtr.Zero, ownsHandle)
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
