using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace ProductionPortal_Solb
{
    public class MvcApplication : HttpApplication
    {
        private const string DefaultLanguage = "en";

        private static readonly HashSet<string>
            AllowedLanguages =
                new HashSet<string>(
                    StringComparer.OrdinalIgnoreCase)
                {
                    "en",
                    "en-US",
                    "ar",
                    "ar-SA"
                };

        private static readonly Dictionary<string, CultureInfo>
            Cultures =
                new Dictionary<string, CultureInfo>(
                    StringComparer.OrdinalIgnoreCase)
                {
                    {
                        "en",
                        CreateCulture("en")
                    },
                    {
                        "en-US",
                        CreateCulture("en-US")
                    },
                    {
                        "ar",
                        CreateCulture("ar")
                    },
                    {
                        "ar-SA",
                        CreateCulture("ar-SA")
                    }
                };

        private static readonly HashSet<string>
            StaticExtensions =
                new HashSet<string>(
                    StringComparer.OrdinalIgnoreCase)
                {
                    ".css",
                    ".js",
                    ".png",
                    ".jpg",
                    ".jpeg",
                    ".gif",
                    ".webp",
                    ".svg",
                    ".ico",
                    ".woff",
                    ".woff2",
                    ".ttf",
                    ".eot",
                    ".map",
                    ".pdf",
                    ".xlsx",
                    ".xls"
                };

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            FilterConfig.RegisterGlobalFilters(
                GlobalFilters.Filters
            );

            RouteConfig.RegisterRoutes(
                RouteTable.Routes
            );

            BundleConfig.RegisterBundles(
                BundleTable.Bundles
            );

#if DEBUG
            BundleTable.EnableOptimizations = false;
#else
            BundleTable.EnableOptimizations = true;
#endif
        }

        protected void Application_BeginRequest(
            object sender,
            EventArgs e)
        {
            string path =
                Request.CurrentExecutionFilePath;

            if (IsStaticResource(path))
            {
                return;
            }

            string language =
                GetRequestLanguage();

            CultureInfo culture;

            if (!Cultures.TryGetValue(
                language,
                out culture))
            {
                culture =
                    Cultures[DefaultLanguage];
            }

            Thread.CurrentThread.CurrentCulture =
                culture;

            Thread.CurrentThread.CurrentUICulture =
                culture;
        }

        protected void Application_PostAuthenticateRequest(
            object sender,
            EventArgs e)
        {
            if (Context.User == null)
            {
                return;
            }

            HttpCookie authCookie =
                Request.Cookies[
                    FormsAuthentication.FormsCookieName
                ];

            if (authCookie == null ||
                string.IsNullOrWhiteSpace(
                    authCookie.Value))
            {
                return;
            }

            try
            {
                FormsAuthenticationTicket ticket =
                    FormsAuthentication.Decrypt(
                        authCookie.Value
                    );

                if (ticket == null ||
                    ticket.Expired ||
                    string.IsNullOrWhiteSpace(
                        ticket.Name))
                {
                    return;
                }

                string role =
                    string.Empty;

                string userId =
                    string.Empty;

                if (!string.IsNullOrWhiteSpace(
                    ticket.UserData))
                {
                    string[] data =
                        ticket.UserData.Split('|');

                    if (data.Length > 0)
                    {
                        role =
                            data[0].Trim();
                    }

                    if (data.Length > 1)
                    {
                        userId =
                            data[1].Trim();
                    }
                }

                GenericIdentity identity =
                    new GenericIdentity(
                        ticket.Name,
                        "Forms"
                    );

                string[] roles =
                    string.IsNullOrWhiteSpace(role)
                        ? new string[0]
                        : new[] { role };

                GenericPrincipal principal =
                    new GenericPrincipal(
                        identity,
                        roles
                    );

                Context.User =
                    principal;

                Thread.CurrentPrincipal =
                    principal;

                Context.Items["UserID"] =
                    userId;

                Context.Items["UserRole"] =
                    role;
            }
            catch
            {
                FormsAuthentication.SignOut();

                ExpireAuthenticationCookie();
            }
        }

        protected void Application_AcquireRequestState(
            object sender,
            EventArgs e)
        {
            if (Context.Session == null)
            {
                return;
            }

            /*
                Session sirf tab populate karo jab
                current session mein values missing hon.
            */

            if (Context.Session["UserID"] == null)
            {
                string userId =
                    Convert.ToString(
                        Context.Items["UserID"]
                    );

                if (!string.IsNullOrWhiteSpace(
                    userId))
                {
                    Context.Session["UserID"] =
                        userId;
                }
            }

            if (Context.Session["UserRole"] == null)
            {
                string role =
                    Convert.ToString(
                        Context.Items["UserRole"]
                    );

                if (!string.IsNullOrWhiteSpace(
                    role))
                {
                    Context.Session["UserRole"] =
                        role;
                }
            }
        }

        protected void Application_Error(
            object sender,
            EventArgs e)
        {
            Exception exception =
                Server.GetLastError();

            if (exception == null)
            {
                return;
            }

            /*
                Yahan logging add kar sakte ho.
                Response.Write ya heavy DB logging
                har error par mat karna.
            */
        }

        private string GetRequestLanguage()
        {
            HttpCookie cookie =
                Request.Cookies["lang"];

            if (cookie == null ||
                string.IsNullOrWhiteSpace(
                    cookie.Value))
            {
                return DefaultLanguage;
            }

            string language =
                cookie.Value.Trim();

            return AllowedLanguages.Contains(
                language)
                    ? language
                    : DefaultLanguage;
        }

        private static CultureInfo CreateCulture(
            string language)
        {
            CultureInfo culture;

            try
            {
                culture =
                    new CultureInfo(language);
            }
            catch (CultureNotFoundException)
            {
                culture =
                    new CultureInfo(
                        DefaultLanguage
                    );
            }

            culture.DateTimeFormat.Calendar =
                new GregorianCalendar();

            return CultureInfo.ReadOnly(
                culture
            );
        }

        private static bool IsStaticResource(
            string path)
        {
            if (string.IsNullOrWhiteSpace(
                path))
            {
                return false;
            }

            string extension =
                VirtualPathUtility
                    .GetExtension(path);

            return
                !string.IsNullOrWhiteSpace(
                    extension) &&
                StaticExtensions.Contains(
                    extension);
        }

        private void ExpireAuthenticationCookie()
        {
            HttpCookie cookie =
                new HttpCookie(
                    FormsAuthentication
                        .FormsCookieName,
                    string.Empty
                );

            cookie.Expires =
                DateTime.UtcNow.AddDays(-1);

            cookie.HttpOnly =
                true;

            cookie.Secure =
                Request.IsSecureConnection;

            Response.Cookies.Add(cookie);
        }
    }
}