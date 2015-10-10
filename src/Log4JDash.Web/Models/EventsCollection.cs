using System.Collections.Generic;

namespace Log4JDash.Web.Models
{
    public sealed class EventsCollection
    {
        public ICollection<EventModel> Events { get; }

        public string Source { get; }

        public long SourceSize { get; }

        public EventsCollection (ICollection<EventModel> events, string source, long sourceSize)
        {
            Events = events;
            Source = source;
            SourceSize = sourceSize;
        }
    }
}
