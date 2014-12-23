using System;
using System.Runtime.InteropServices;

namespace Log4JParserNet
{
    internal sealed class IteratorHandle : SafeHandle
    {
        public IteratorHandle (bool ownsHandle)
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
            Log4JParserC.Log4JIteratorDestroy (this);
            handle = IntPtr.Zero;
            return true;
        }
    }
}
