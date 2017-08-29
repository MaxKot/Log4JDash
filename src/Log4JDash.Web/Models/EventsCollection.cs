using System.Collections.Generic;

namespace Log4JDash.Web.Models
{
    public sealed class EventsCollection
    {
        public IReadOnlyList<EventModel> Events { get; }

        public string Source { get; }

        public long SourceSize { get; }

        public EventsCollection (IReadOnlyList<EventModel> events, string source, long sourceSize)
        {
            Events = events;
            Source = source;
            SourceSize = sourceSize;
        }
    }
}
