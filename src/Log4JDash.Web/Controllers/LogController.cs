using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;
using Log4JDash.Web.Models;
using Log4JParserNet;

namespace Log4JDash.Web.Controllers
{
    internal static class EnumerableExt
    {
        private struct CycleBuffer<T> : IEnumerable<T>
        {
            private readonly T[] impl_;

            private int nextWriteIndex_;

            private bool isFull_;

            public CycleBuffer (int quantity)
            {
                impl_ = new T[quantity];
                nextWriteIndex_ = 0;
                isFull_ = false;
            }

            public void Add (T element)
            {
                impl_[nextWriteIndex_] = element;
                var newNextWriteIndex = (nextWriteIndex_ + 1) % impl_.Length;

                isFull_ |= newNextWriteIndex < nextWriteIndex_;
                nextWriteIndex_ = newNextWriteIndex;
            }

            public IEnumerator<T> GetEnumerator ()
            {
                return impl_
                    .Concat (impl_)
                    .Skip (isFull_ ? nextWriteIndex_ : 0)
                    .Take (impl_.Length)
                    .GetEnumerator ();
            }

            IEnumerator IEnumerable.GetEnumerator ()
            {
                return GetEnumerator ();
            }
        }

        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> coll, int quantity)
        {
            var result = new CycleBuffer<T> (quantity);

            using (var enumerator = coll.GetEnumerator ())
            {
                while (enumerator.MoveNext ())
                {
                    result.Add (enumerator.Current);
                }

                foreach (var element in result)
                {
                    yield return element;
                }
            }
        }
    }

    public sealed class LogIndexFormModel
    {
        public string FileName { get; set; }

        public int? Offset { get; set; }

        public int Quantity { get; set; }
    }

    public sealed class LogSourceModel
    {
        public FileEventSource GetEvents (string fileName)
        {
            return new FileEventSource (fileName);
        }
    }

    public sealed class EventModel
    {
        public EventModel (Event e)
        {
            if (e == null)
            {
                throw new ArgumentNullException ("e");
            }

            Id = e.Id;
            Level = e.Level;
            Logger = e.Logger;
            Thread = e.Thread;
            Time = e.Time;
            Message = e.Message;
            Throwable = e.Throwable;
        }

        public int Id { get; set; }

        public string Level { get; set; }

        public string Logger { get; set; }

        public string Thread { get; set; }

        public DateTime Time { get; set; }

        public string Message { get; set; }

        public string Throwable { get; set; }
    }

    public sealed class LogIndexViewModel
    {
        public LogQuery Query { get; set; }

        public IEnumerable<EventModel> Events { get; set; }
    }

    public sealed class LogRepository
    {
        public IEnumerable<EventModel> GetEvents (LogQuery query)
        {
            string sourceFile;
            switch (query.SourceId)
            {
                case 1:
                    sourceFile = Path.Combine (HostingEnvironment.MapPath ("~"), @"..\Log4JDash\test-log.cyr.xml");
                    break;

                default:
                    throw new ArgumentException ("Unrecognized log source identifier.", nameof (query));
            }

            using (var source = new FileEventSource (sourceFile))
            using (var filters = new List<FilterBase> ().ToDisposable ())
            {
                if (query.MinLevel.Value != Level.Debug)
                {
                    filters.Elements.Add (new FilterLevel (query.MinLevel.Value, Level.Off));
                }

                IEnumerable<Event> filteredEvents;
                switch (filters.Elements.Count)
                {
                    case 0:
                        filteredEvents = source;
                        break;

                    case 1:
                        filteredEvents = source.Where (filters.Elements.Single ());
                        break;

                    default:
                        FilterAll rootFilter = null;
                        try
                        {
                            rootFilter = new FilterAll ();
                            foreach (var filter in filters.Elements)
                            {
                                rootFilter.Add (filter);
                            }
                        }
                        finally
                        {
                            if (rootFilter != null)
                            {
                                filters.Elements.Add (rootFilter);
                            }
                        }
                        filteredEvents = source.Where (rootFilter);
                        break;
                }

                IEnumerable<Event> eventsWindow;
                if (query.MinId == null)
                {
                    eventsWindow = filteredEvents
                        .TakeLast (query.Quantity);
                }
                else
                {
                    eventsWindow = filteredEvents
                        .Skip ((int) query.MinId)
                        .Take (query.Quantity);
                }

                return eventsWindow
                    .Select (x => new EventModel (x))
                    .ToList ();
            }
        }
    }

    public sealed class LogController : Controller
    {
        private readonly LogRepository repository_;

        public LogController ()
        {
            repository_ = new LogRepository ();
        }

        // GET: Log
        public ActionResult List (LogQuery formModel)
        {
            var events = repository_.GetEvents (formModel);

            var viewModel = new LogIndexViewModel
            {
                Query = formModel,
                Events = events
            };

            return View (viewModel);
        }
    }
}
