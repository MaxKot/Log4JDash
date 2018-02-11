using System.Collections.Generic;

namespace Log4JDash.Web.Models
{
    public sealed class EventsCollection
    {
        public IReadOnlyList<EventModel> Events { get; }

        public string Source { get; }

        public string Snapshot { get; }

        public EventsCollection (IReadOnlyList<EventModel> events, string source, string snapshot)
        {
            Events = events;
            Source = source;
            Snapshot = snapshot;
        }
    }
}
