//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using System.Web.Optimization;
//using System.Web.Routing;

//namespace ProductionPortal_Solb
//{
//    public class MvcApplication : System.Web.HttpApplication
//    {
//        protected void Application_Start()
//        {
//            AreaRegistration.RegisterAllAreas();
//            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
//            RouteConfig.RegisterRoutes(RouteTable.Routes);
//        }
//    }
//}
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace ProductionPortal_Solb
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_BeginRequest()
        {
            HttpCookie langCookie = HttpContext.Current.Request.Cookies["lang"];
            string lang = langCookie?.Value ?? "en"; // default to English

            try
            {
                var cultureInfo = new CultureInfo(lang);
                cultureInfo.DateTimeFormat.Calendar = new GregorianCalendar();

                Thread.CurrentThread.CurrentCulture = cultureInfo;
                Thread.CurrentThread.CurrentUICulture = cultureInfo;
            }
            catch
            {
                // fallback to default if culture not found
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
            }
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            HttpCookie authCookie = Context.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);
                if (ticket != null && !string.IsNullOrEmpty(ticket.UserData))
                {
                    string[] data = ticket.UserData.Split('|');
                    if (data.Length == 2)
                    {
                        string role = data[0];
                        string userId = data[1];

                        // Set principal
                        HttpContext.Current.User = new System.Security.Principal.GenericPrincipal(
                            new System.Security.Principal.GenericIdentity(ticket.Name),
                            new[] { role }
                        );

                        // Store UserID in Session (only if not already set)
                        if (HttpContext.Current.Session != null)
                        {
                            HttpContext.Current.Session["UserID"] = userId;
                            HttpContext.Current.Session["UserRole"] = role;
                        }
                    }
                }
            }
        }

    }
}
