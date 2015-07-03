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

            var viewModel = new LogIndexViewModel
            {
                Query = formModel,
                Events = events
            };

            return View (viewModel);
        }
    }
}
