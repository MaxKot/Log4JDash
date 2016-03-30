using System;
using System.Reflection;
using System.Web.Mvc;
using Log4JDash.Web.Configuration;

namespace Log4JDash.Web.Mvc
{
    public class ConfigurableRequireHttpsAttribute : RequireHttpsAttribute
    {
        private static readonly StringComparer MethodComparer = StringComparer.OrdinalIgnoreCase;

        private readonly IRequireHttpsConfig config_;

        public ConfigurableRequireHttpsAttribute ()
        {
            config_ = KnownSections.RequireHttps ();
        }

        public ConfigurableRequireHttpsAttribute
            (Type configurationProviderType, string configurationProviderMethod)
        {
            if (configurationProviderType == null)
            {
                throw new ArgumentNullException (nameof (configurationProviderType));
            }
            if (configurationProviderMethod == null)
            {
                throw new ArgumentNullException (nameof (configurationProviderMethod));
            }

            const BindingFlags bindingFlags = BindingFlags.InvokeMethod |
                                              BindingFlags.Public |
                                              BindingFlags.Static;

            MethodInfo configurationProvider;
            try
            {
                configurationProvider = configurationProviderType.GetMethod
                    (configurationProviderMethod, bindingFlags, null, Type.EmptyTypes, null);
            }
            catch (AmbiguousMatchException ex)
            {
                const string message = "The specified method name is ambiguous.";
                throw new ArgumentException (message, nameof (configurationProviderMethod), ex);
            }

            var configObject = configurationProvider.Invoke (null, new object[0]);
            if (configObject == null)
            {
                const string message = "The specified method returned null.";
                throw new InvalidOperationException (message);
            }

            if (!(configObject is IRequireHttpsConfig))
            {
                const string message = "The specified method returned invalid configuration " +
                                       "object: the return value is not assignable to " +
                                       "IRequireHttpsConfig.";
                throw new InvalidOperationException (message);
            }

            config_ = (IRequireHttpsConfig) configObject;
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
