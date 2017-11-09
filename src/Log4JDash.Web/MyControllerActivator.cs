using System;
using System.Web.Mvc;
using System.Web.Routing;
using Log4JDash.Web.Configuration;
using Log4JDash.Web.Controllers;
using Log4JDash.Web.Domain;
using Log4JDash.Web.Models;

namespace Log4JDash.Web
{
    internal sealed class MyControllerActivator : IControllerActivator
    {
        public IController Create (RequestContext requestContext, Type controllerType)
        {
            IController result;
            if (controllerType == typeof (LogController))
            {
                var logSourceProviderConfig = KnownSections.LogSourceProvider ();
                var statsCache = LogFileStatsCache.Default;
                var logSourceProvider = new LogSourceProvider (logSourceProviderConfig, statsCache);
                var logRepository = new LogRepository (logSourceProvider);

                result = new LogController (logRepository);
            }
            else
            {
                result = null;
            }

            return result;
        }
    }
}
