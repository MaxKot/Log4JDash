using System;
using System.Diagnostics;
using System.Web.Mvc;
using System.Web.Routing;
using Log4JDash.Web.Controllers;
using Log4JDash.Web.Models;

namespace Log4JDash.Web
{
    internal sealed class MyControllerActivator : IControllerActivator
    {
        private readonly LogRepository logRepository_;

        public MyControllerActivator (LogRepository logRepository)
        {
            Debug.Assert (logRepository != null, "MyControllerActivator.ctor: logRepository is null.");

            logRepository_ = logRepository;
        }

        public IController Create (RequestContext requestContext, Type controllerType)
        {
            IController result;
            if (controllerType == typeof (LogController))
            {
                result = new LogController (logRepository_);
            }
            else
            {
                result = null;
            }

            return result;
        }
    }
}
