using System;
using System.ComponentModel.DataAnnotations;

namespace Log4JDash.Web.Models
{
    public sealed class LogQuery
    {
        public int SourceId { get; set; }

        public int? MinId { get; set; }

        public int MaxId { get; set; }

        public LogLevelInput MinLevel { get; set; }

        public string Logger { get; set; }

        public string Thread { get; set; }

        public DateTime MinTime { get; set; }

        public DateTime MaxTime { get; set; }

        public string Message { get; set; }

        public string Throwable { get; set; }

        [Display (Name = "")]
        public EventsQuantity Quantity { get; set; }

        public LogQuery ()
        {
            SourceId = 1;
            MaxId = Int32.MaxValue;
            MinLevel = new LogLevelInput ();
            MinTime = DateTime.MinValue;
            MaxTime = DateTime.MaxValue;
            Quantity = new EventsQuantity ();
        }
    }
}
