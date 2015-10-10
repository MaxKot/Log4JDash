using System;
using System.Runtime.InteropServices;

namespace Log4JParserNet
{
    [StructLayout (LayoutKind.Sequential, Pack = 4)]
    internal struct Log4JEventProperty
    {
        public IntPtr Name;

        public UIntPtr NameSize;

        public IntPtr Value;

        public UIntPtr ValueSize;

        public Log4JEventProperty (IntPtr name, UIntPtr nameSize, IntPtr value, UIntPtr valueSize)
        {
            Name = name;
            NameSize = nameSize;
            Value = value;
            ValueSize = valueSize;
        }
    }
}
