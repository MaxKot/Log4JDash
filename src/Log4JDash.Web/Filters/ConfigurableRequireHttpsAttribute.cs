using System;
using System.Web.Mvc;
using Log4JDash.Web.Configuration;

namespace Log4JDash.Web.Filters
{
    public class ConfigurableRequireHttpsAttribute : RequireHttpsAttribute
    {
        private static readonly StringComparer MethodComparer = StringComparer.OrdinalIgnoreCase;

        private readonly IRequireHttpsConfig config_;

        public ConfigurableRequireHttpsAttribute ()
        {
            config_ = KnownSections.RequireHttps ();
        }

        /// <inheritdoc />
        public override void OnAuthorization (AuthorizationContext filterContext)
        {
            if (!config_.AllowHttp)
            {
                base.OnAuthorization (filterContext);
            }
        }

        /// <inheritdoc />
        protected override void HandleNonHttpsRequest (AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException (nameof (filterContext));
            }

            var httpsPort = config_.HttpsPort;

            if (httpsPort == null)
            {
                base.HandleNonHttpsRequest (filterContext);
            }
            // only redirect for GET requests, otherwise the browser might not propagate the verb
            // and request body correctly.
            else if (!MethodComparer.Equals (filterContext.HttpContext.Request.HttpMethod, "GET"))
            {
                base.HandleNonHttpsRequest (filterContext);
            }
            else
            {
                var request = filterContext.HttpContext.Request;

                var port = (int) httpsPort;

                var uriRebuilder = new UriBuilder (request.Url);
                uriRebuilder.Scheme = "https";
                uriRebuilder.Port = port;

                var newUrl = uriRebuilder.Uri.ToString ();

                filterContext.Result = new RedirectResult (newUrl, true);
            }
        }
    }
}
