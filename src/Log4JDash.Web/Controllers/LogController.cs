using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Log4JParserNet;

namespace Log4JDash.Web.Controllers
{
    internal static class EnumerableExt
    {
        private struct CycleBuffer<T> : IEnumerable<T>
        {
            private T[] impl_;

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
                var startIndex = isFull_ ? (nextWriteIndex_ + 1) % impl_.Length : 0;
                for (var i = 0; i < impl_.Length; ++i)
                {
                    yield return impl_[(i + startIndex) % impl_.Length];
                }
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
        private sealed class EventsEnumerable : IEnumerable<Event>
        {
            private sealed class Enumerator : IEnumerator<Event>
            {
                EventSource eventSource_;

                EnumeratorEventSource impl_;

                public Enumerator (string fileName)
                {
                    eventSource_ = new EventSource (fileName);
                    impl_ = new EnumeratorEventSource (eventSource_);
                }

                public Event Current
                {
                    get { return impl_.Current; }
                }

                object IEnumerator.Current
                {
                    get { return Current; }
                }

                public void Dispose ()
                {
                    impl_.Dispose ();
                    eventSource_.Dispose ();
                }

                public bool MoveNext ()
                {
                    return impl_.MoveNext ();
                }

                public void Reset ()
                {
                    impl_.Reset ();
                }
            }

            private readonly string fileName_;

            public EventsEnumerable (string fileName)
            {
                fileName_ = fileName;
            }

            public IEnumerator<Event> GetEnumerator ()
            {
                return new Enumerator (fileName_);
            }

            IEnumerator IEnumerable.GetEnumerator ()
            {
                return GetEnumerator ();
            }
        }

        public IEnumerable<Event> GetEvents (string fileName)
        {
            return new EventsEnumerable (fileName);
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

            Level = e.Level;
            Logger = e.Logger;
            Thread = e.Thread;
            Time = e.Time;
            Message = e.Message;
            Throwable = e.Throwable;
        }

        public string Level { get; set; }

        public string Logger { get; set; }

        public string Thread { get; set; }

        public DateTime Time { get; set; }

        public string Message { get; set; }

        public string Throwable { get; set; }
    }

    public sealed class LogIndexViewModel
    {
        public LogIndexFormModel Form { get; set; }

        public IEnumerable<EventModel> Events { get; set; }
    }

    public sealed class LogController : Controller
    {
        private readonly LogSourceModel logSourceModel_;

        public LogController ()
        {
            logSourceModel_ = new LogSourceModel ();
        }

        // GET: Log
        public ActionResult List (LogIndexFormModel formModel)
        {
            var fileName = Path.Combine (Server.MapPath ("~"), @"..\Log4JDash\test-log.cyr.xml");

            var quantity = formModel.Quantity > 0
                ? formModel.Quantity
                : 20;

            var events = logSourceModel_
                .GetEvents (fileName);

            IEnumerable<Event> eventsWindow;
            if (formModel.Offset == null)
            {
                eventsWindow = logSourceModel_
                    .GetEvents (fileName)
                    .TakeLast (quantity);
            }
            else
            {
                var offset = (int) formModel.Offset;
                if (offset < 0)
                {
                    offset = 0;
                }

                eventsWindow = logSourceModel_
                    .GetEvents (fileName)
                    .Skip (offset)
                    .Take (quantity);
            }

            var viewModel = new LogIndexViewModel
            {
                Form = formModel,
                Events = eventsWindow.Select (x => new EventModel (x))
            };

            return View (viewModel);
        }
    }
}
