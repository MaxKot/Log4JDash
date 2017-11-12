using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Log4JDash.Web.Configuration;
using Log4JDash.Web.Domain;
using Log4JDash.Web.Models;

namespace Log4JDash.Web
{
    public class MvcApplication : HttpApplication
    {
        private static IDisposable precacheThread_;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            var logSourceProviderConfig = KnownSections.LogSourceProvider ();
            var statsCache = LogFileStatsCache.Default;
            var logSourceProvider = new LogSourceProvider (logSourceProviderConfig, statsCache);
            var logRepository = new LogRepository (logSourceProvider);

            var controllerActivator = new MyControllerActivator (logRepository);
            var controllerFactory = new DefaultControllerFactory (controllerActivator);
            ControllerBuilder.Current.SetControllerFactory (controllerFactory);

            precacheThread_ = statsCache.StartPrecacheThread ();
        }

        protected void Application_Stop ()
        {
            precacheThread_?.Dispose ();
        }
    }
}
