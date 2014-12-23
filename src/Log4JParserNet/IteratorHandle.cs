using System;
using System.Runtime.InteropServices;

namespace Log4JParserNet
{
    internal sealed class IteratorHandle : SafeHandle
    {
        private IteratorHandle (bool ownsHandle)
            : base (IntPtr.Zero, ownsHandle)
        {

        }

        public IteratorHandle ()
            : this (true)
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
            Log4JParserC.Log4JIteratorDestroy (handle);
            handle = IntPtr.Zero;
            return true;
        }
    }
}
