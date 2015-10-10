using System;
using System.Collections.Generic;
using Log4JParserNet;

namespace Log4JDash.Web.Models
{
    public sealed class EventModel
    {
        public EventModel (Event e)
        {
            if (e == null)
            {
                throw new ArgumentNullException (nameof (e));
            }

            Id = e.Id;
            Level = e.Level;
            Logger = e.Logger;
            Thread = e.Thread;
            Time = e.Time;
            Message = e.Message;
            Throwable = e.Throwable;
            Properties = e.GetProperties ();
        }

        public ulong Id { get; set; }

        public string Level { get; set; }

        public string Logger { get; set; }

        public string Thread { get; set; }

        public DateTime Time { get; set; }

        public string Message { get; set; }

        public string Throwable { get; set; }

        public IList<KeyValuePair<string, string>> Properties { get; set; }
    }
}
