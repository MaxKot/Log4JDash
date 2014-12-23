using System;
using System.Runtime.InteropServices;

namespace Log4JParserNet
{
    internal sealed class EventSourceHandle : SafeHandle
    {
        public EventSourceHandle (bool ownsHandle)
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
            try
            {

            }
            finally
            {
                Log4JParserC.Log4JEventSourceDestroy (this);
                handle = IntPtr.Zero;
            }
            return true;
        }
    }
}
