﻿using System;
using System.Runtime.InteropServices;

namespace Log4JParserNet
{
    internal sealed class FilterHandle : SafeHandle
    {
        private FilterHandle (bool ownsHandle)
            : base (IntPtr.Zero, ownsHandle)
        {

        }

        public FilterHandle ()
            : this (true)
        {

        }

        /// <inheritdoc />
        public override bool IsInvalid => handle == IntPtr.Zero;

        /// <inheritdoc />
        protected override bool ReleaseHandle ()
        {
            Log4JParserC.Log4JFilterDestroy (handle);
            return true;
        }
    }
}
