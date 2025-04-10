﻿using System;
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
        public override bool IsInvalid => handle == IntPtr.Zero;

        /// <inheritdoc />
        protected override bool ReleaseHandle ()
        {
            Log4JParserC.Log4JIteratorDestroy (handle);
            return true;
        }
    }
}
