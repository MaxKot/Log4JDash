using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Log4JParserNet.Tests
{
    public sealed class EventExpectation : IEquatable<Event>
    {
        public string Level { get; set; }

        public string Logger { get; set; }

        public string Thread { get; set; }

        public long Timestamp { get; set; }

        public string Message { get; set; }

        public string Throwable { get; set; }

        public IDictionary<string, string> Properties { get; } = new Dictionary<string, string> ();

        public ulong Id { get; set; }

        public bool Equals (Event other)
        {
            Assert.That (other.Level, Is.EqualTo (Level));
            Assert.That (other.Logger, Is.EqualTo (Logger));
            Assert.That (other.Thread, Is.EqualTo (Thread));
            Assert.That (other.Timestamp, Is.EqualTo (Timestamp));
            Assert.That (other.Message, Is.EqualTo (Message));
            Assert.That (other.Throwable, Is.EqualTo (Throwable));
            var properties = other.GetProperties ();
            Assert.That (properties, Is.EquivalentTo (Properties));
            Assert.That (other.Id, Is.EqualTo (Id));

            return true;
        }
    }
}
