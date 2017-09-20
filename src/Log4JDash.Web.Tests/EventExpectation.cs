using System;
using System.Collections.Generic;
using System.Linq;
using Log4JDash.Web.Models;

namespace Log4JDash.Web.Tests
{
    public sealed class EventExpectation : IEquatable<EventModel>
    {
        public string Level { get; set; }

        public string Logger { get; set; }

        public string Thread { get; set; }

        public DateTime Time { get; set; }

        public string Message { get; set; }

        public string Throwable { get; set; }

        public IDictionary<string, string> Properties { get; } = new Dictionary<string, string> ();

        public ulong Id { get; set; }

        public bool Equals (EventModel other)
        {
            var stringComparer = StringComparer.Ordinal;

            return
                other != null &&
                stringComparer.Equals (Level, other.Level) &&
                stringComparer.Equals (Logger, other.Logger) &&
                stringComparer.Equals (Thread, other.Thread) &&
                Time == other.Time &&
                stringComparer.Equals (Message, other.Message) &&
                stringComparer.Equals (Throwable, other.Throwable) &&
                Equals (Properties, other.Properties) &&
                Id == other.Id;
        }

        private static bool Equals
        (
            ICollection<KeyValuePair<string, string>> x,
            ICollection<KeyValuePair<string, string>> y
        )
        {
            if (ReferenceEquals (x, y))
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            if (x.Count != y.Count)
            {
                return false;
            }

            var stringComparer = StringComparer.Ordinal;

            var xOrdered = x
                .OrderBy (kvp => kvp.Key, stringComparer)
                .ThenBy (kvp => kvp.Value, stringComparer);
            var yOrdered = y
                .OrderBy (kvp => kvp.Key, stringComparer)
                .ThenBy (kvp => kvp.Value, stringComparer);

            return System.Linq.Enumerable
                .Zip (xOrdered, yOrdered, Tuple.Create)
                .All (t => {
                    var xKey = t.Item1.Key;
                    var yKey = t.Item2.Key;
                    var xValue = t.Item1.Value;
                    var yValue = t.Item2.Value;
                    return stringComparer.Equals (xKey, yKey) &&
                           stringComparer.Equals (xValue, yValue);
                });
        }

        public override bool Equals (object obj)
            => obj is EventModel other && Equals (other);

        public override int GetHashCode ()
            => 0;
    }
}
