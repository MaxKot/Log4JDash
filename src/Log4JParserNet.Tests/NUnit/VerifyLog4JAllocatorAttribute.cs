using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Commands;

namespace Log4JParserNet.Tests.NUnit
{
    [AttributeUsage (AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class VerifyLog4JAllocatorAttribute : Attribute, IWrapSetUpTearDown
    {
        public class VerifyLog4JAllocatorCommand : BeforeAndAfterTestCommand
        {
            private struct Allocation
            {
                private static int LastId = 0;

                public int Id { get; }

                public IntPtr Ptr { get; }

                public UIntPtr Size { get; }

                public string StackTace { get; }

                public Allocation (IntPtr ptr, UIntPtr size)
                {
                    Id = ++LastId;
                    Ptr = ptr;
                    Size = size;

                    StackTace = Helpers.GetStackTrace ();
                }
            }

            static VerifyLog4JAllocatorCommand ()
            {
                Log4JParserC.Log4JSetAllocator (Alloc, Free);
            }

            /// <summary>The allocator callback.
            /// </summary>
            /// <remarks>
            /// The delegate MUST be saved in a static field to pevent it from being garbage
            /// collected.
            /// </remarks>
            private static readonly Log4JParserC.Alloc Alloc = size =>
            {
                var result = Marshal.AllocHGlobal (checked((int) size));

                var testProperties = TestContext.CurrentContext?.Test?.Properties;
                var allocations = GetAllocations (testProperties);
                allocations?.TryAdd (result, new Allocation (result, size));

                return result;
            };

            /// <summary>The deallocator callback.
            /// </summary>
            /// <remarks>
            /// The delegate MUST be saved in a static field to pevent it from being garbage
            /// collected.
            /// </remarks>
            private static readonly Log4JParserC.Free Free = ptr =>
            {
                Marshal.FreeHGlobal (ptr);

                var testProperties = TestContext.CurrentContext?.Test?.Properties;
                var allocations = GetAllocations (testProperties);
                allocations?.TryRemove (ptr, out _);
            };

            private const string AllocationsKey = "VerifyLog4JAllocatorCommand.Allocations";

            private static ConcurrentDictionary<IntPtr, Allocation> GetAllocations (TestContext.PropertyBagAdapter testProperties)
                => testProperties[AllocationsKey]
                    .OfType<ConcurrentDictionary<IntPtr, Allocation>> ()
                    ?.LastOrDefault ();

            public void PushAllocations (IPropertyBag testProperties)
            {
                var newAllocations = new ConcurrentDictionary<IntPtr, Allocation> ();
                testProperties[AllocationsKey].Add (newAllocations);
            }

            private static ICollection<Allocation> PopAllocations (IPropertyBag testProperties)
            {
                var allocationsList = testProperties[AllocationsKey];

                for (var i = allocationsList.Count - 1; i >= 0; --i)
                {
                    if (allocationsList[i] is ConcurrentDictionary<IntPtr, Allocation> currentAllocations)
                    {
                        allocationsList.RemoveAt (i);
                        return currentAllocations.Values;
                    }
                }

                return null;
            }

            public VerifyLog4JAllocatorCommand (TestCommand innerCommand)
                : base (innerCommand)
            {
                BeforeTest = context => PushAllocations (context.CurrentTest.Properties);

                AfterTest = context => Assert.Multiple (() =>
                {
                    var allocations = PopAllocations (context.CurrentTest.Properties);
                    foreach (var allocation in allocations)
                    {
                        var id = allocation.Id;
                        var address = allocation.Ptr;
                        var size = allocation.Size;
                        var stackTace = allocation.StackTace;

                        const string format = "Memory allocation #{0} leaked: {1} bytes at {2}. Allocation stack trace:\r\n---\r\n{3}...";
                        Assert.Fail (format, id, size, address, stackTace);
                    }
                });
            }
        }

        public TestCommand Wrap (TestCommand command)
            => new VerifyLog4JAllocatorCommand (command);
    }
}
