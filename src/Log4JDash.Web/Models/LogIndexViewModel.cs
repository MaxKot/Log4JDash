using System.Collections.Generic;

namespace Log4JDash.Web.Models
{
    public sealed class LogIndexViewModel
    {
        public LogQuery Query { get; set; }

        public IReadOnlyList<EventModel> Events { get; set; }
    }
}
