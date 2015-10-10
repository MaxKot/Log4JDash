using System.Linq;
using System.Web.Mvc;
using Log4JDash.Web.Models;

namespace Log4JDash.Web.Controllers
{
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
            var eventsCollection = repository_.GetEvents (formModel);

            var source = eventsCollection.Source;
            var sources = repository_.GetSources ().ToList ();
            formModel.Source = new LogSourceInput (source, sources);
            formModel.Size = eventsCollection.SourceSize;

            var viewModel = new LogIndexViewModel
            {
                Query = formModel,
                Events = eventsCollection.Events
            };

            return View (viewModel);
        }
    }
}
