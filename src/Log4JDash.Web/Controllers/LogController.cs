using System.Linq;
using System.Web.Mvc;
using Log4JDash.Web.Filters;
using Log4JDash.Web.Models;

namespace Log4JDash.Web.Controllers
{
    [ConfigurableRequireHttps]
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

            var sourceId = eventsCollection.Source;
            var sourceSize = eventsCollection.SourceSize;
            var source = new LogSourceModel (sourceId, sourceSize);

            var sources = repository_
                .GetSources ()
                .Select (id => new LogSourceModel (id, id == sourceId ? (long?) sourceSize : null))
                .ToList ();

            formModel.Source = new LogSourceInput (source, sources);

            var viewModel = new LogIndexViewModel
            {
                Query = formModel,
                Events = eventsCollection.Events
            };

            return View (viewModel);
        }
    }
}
