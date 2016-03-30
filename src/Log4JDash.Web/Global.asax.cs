using System.Web.Mvc;
using System.Web.Routing;
using Log4JDash.Web.Mvc;

namespace Log4JDash.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            ModelMetadataProviders.Current = new MyModelMetadataProvider ();
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}
