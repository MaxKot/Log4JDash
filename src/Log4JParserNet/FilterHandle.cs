using System;
using System.Runtime.InteropServices;

namespace Log4JParserNet
{
    internal sealed class FilterHandle : SafeHandle
    {
        public FilterHandle (bool ownsHandle)
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
            Log4JParserC.Log4JFilterDestroy (this);
            handle = IntPtr.Zero;
            return true;
        }
    }
}
