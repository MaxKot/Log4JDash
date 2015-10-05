using System;
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
            var events = repository_.GetEvents (formModel);

            var sources = repository_.GetSources ().ToList ();
            var source = String.IsNullOrWhiteSpace (formModel.Source.Value)
                ? sources.First ()
                : formModel.Source.Value;
            formModel.Source = new LogSourceInput (source, sources);

            var viewModel = new LogIndexViewModel
            {
                Query = formModel,
                Events = events
            };

            return View (viewModel);
        }
    }
}
