using System.Web.Mvc;
using System.Web.Routing;

namespace Log4JDash.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            var controllerActivator = new MyControllerActivator ();
            var controllerFactory = new DefaultControllerFactory (controllerActivator);
            ControllerBuilder.Current.SetControllerFactory (controllerFactory);
        }
    }
}
