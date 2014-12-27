using System;
using System.Runtime.InteropServices;

namespace Log4JParserNet
{
    internal sealed class UnmanagedMemoryHandle : SafeHandle
    {
        public UnmanagedMemoryHandle (long size) : base (IntPtr.Zero, true)
        {
            var intPtrSize = new IntPtr (size);

            try
            {

            }
            finally
            {
                handle = Marshal.AllocHGlobal (intPtrSize);
            }
        }

        public override bool IsInvalid
        {
            get { return handle == IntPtr.Zero; }
        }

        protected override bool ReleaseHandle ()
        {
            Marshal.FreeHGlobal (handle);
            handle = IntPtr.Zero;
            return true;
        }
    }
}
