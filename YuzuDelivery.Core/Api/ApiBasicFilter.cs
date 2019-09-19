using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http.Controllers;
using System.Web.Security;
using System.Web.Mvc;

namespace YuzuDelivery.Core
{
    //TODO: This needs to be changed:
    // * Authentication cannot happen in a filter, only Authorization
    // * The filter must be an AuthorizationFilter, not an ActionFilter
    // * Authorization must be done using the Umbraco logic - it is very specific for claim checking for ASP.Net Identity
    // * Theoretically this shouldn't be required whatsoever because when we authenticate a request that has Basic Auth (i.e. for
    //   VS to work, it will add the correct Claims to the Identity and it will automatically be authorized.
    //
    // we *do* have POC supporting ASP.NET identity, however they require some config on the server
    // we'll keep using this quick-and-dirty method for the time being

    public class ApiBasicAuthFilter : System.Web.Http.Filters.ActionFilterAttribute // use the http one, not mvc, with api controllers!
    {
        private static readonly char[] Separator = ":".ToCharArray();
        private readonly string role;

        public ApiBasicAuthFilter(string role)
        {
            this.role = role;
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            try
            {
                if (!Authorise(actionContext.Request, role))
                {
                    actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                }
                base.OnActionExecuting(actionContext);
            }
            catch
            {
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }
        }

        private static bool Authorise(HttpRequestMessage request, string role)
        {
            var ah = request.Headers.Authorization;
            if (ah == null || ah.Scheme != "Basic")
                return false;

            var token = ah.Parameter;
            var credentials = Encoding.ASCII
                .GetString(Convert.FromBase64String(token))
                .Split(Separator);
            if (credentials.Length != 2)
                return false;

            var username = DecodeTokenElement(credentials[0]);
            var password = DecodeTokenElement(credentials[1]);

            var cmsSpecificAuthoriser = DependencyResolver.Current.GetService<IAuthoriseApi>();
            return cmsSpecificAuthoriser.Authorise(username, password, role);
        }

        public static string EncodeTokenElement(string s)
        {
            return s.Replace("%", "%a").Replace(":", "%b");
        }

        public static string DecodeTokenElement(string s)
        {
            return s.Replace("%b", ":").Replace("%a", "%");
        }
    }
}
