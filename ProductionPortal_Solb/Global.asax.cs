using System;
using System.Collections.Concurrent;
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
        private static readonly HashSet<string> AllowedLanguages =
            new HashSet<string>(
                StringComparer.OrdinalIgnoreCase)
            {
                "en",
                "en-US",
                "ar",
                "ar-SA"
            };

        private static readonly ConcurrentDictionary<string, CultureInfo>
            CultureCache =
                new ConcurrentDictionary<string, CultureInfo>(
                    StringComparer.OrdinalIgnoreCase);

        private static readonly HashSet<string> StaticExtensions =
            new HashSet<string>(
                StringComparer.OrdinalIgnoreCase)
            {
                ".css",
                ".js",
                ".png",
                ".jpg",
                ".jpeg",
                ".gif",
                ".svg",
                ".ico",
                ".woff",
                ".woff2",
                ".ttf",
                ".map"
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

            // Production mein CSS/JS bundles minify honge.
            BundleTable.EnableOptimizations = true;

            // Common cultures ko application start par cache kar do.
            GetOrCreateCulture("en");
            GetOrCreateCulture("en-US");
            GetOrCreateCulture("ar");
            GetOrCreateCulture("ar-SA");
        }

        protected void Application_BeginRequest()
        {
            // Static files ke liye culture processing ki zarurat nahi.
            if (IsStaticResource(Request.CurrentExecutionFilePath))
            {
                return;
            }

            string language = GetRequestLanguage();

            CultureInfo cultureInfo =
                GetOrCreateCulture(language);

            Thread.CurrentThread.CurrentCulture =
                cultureInfo;

            Thread.CurrentThread.CurrentUICulture =
                cultureInfo;
        }

        protected void Application_PostAuthenticateRequest(
            object sender,
            EventArgs e)
        {
            HttpCookie authCookie =
                Request.Cookies[
                    FormsAuthentication.FormsCookieName
                ];

            if (authCookie == null ||
                string.IsNullOrWhiteSpace(authCookie.Value))
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
                    string.IsNullOrWhiteSpace(ticket.UserData))
                {
                    return;
                }

                string[] data =
                    ticket.UserData.Split('|');

                if (data.Length < 2)
                {
                    return;
                }

                string role =
                    data[0].Trim();

                string userId =
                    data[1].Trim();

                GenericIdentity identity =
                    new GenericIdentity(
                        ticket.Name,
                        "Forms"
                    );

                GenericPrincipal principal =
                    new GenericPrincipal(
                        identity,
                        string.IsNullOrWhiteSpace(role)
                            ? new string[0]
                            : new[] { role }
                    );

                Context.User = principal;
                Thread.CurrentPrincipal = principal;

                // Context.Items sirf current request ke liye hota hai
                // aur Session ke muqable mein lightweight hai.
                Context.Items["AuthenticatedUserID"] =
                    userId;

                Context.Items["AuthenticatedUserRole"] =
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

            string userId =
                Convert.ToString(
                    Context.Items[
                        "AuthenticatedUserID"
                    ]
                );

            string role =
                Convert.ToString(
                    Context.Items[
                        "AuthenticatedUserRole"
                    ]
                );

            // Session ko har request par blindly overwrite na karo.
            if (!string.IsNullOrWhiteSpace(userId) &&
                !string.Equals(
                    Convert.ToString(
                        Context.Session["UserID"]
                    ),
                    userId,
                    StringComparison.Ordinal))
            {
                Context.Session["UserID"] =
                    userId;
            }

            if (!string.IsNullOrWhiteSpace(role) &&
                !string.Equals(
                    Convert.ToString(
                        Context.Session["UserRole"]
                    ),
                    role,
                    StringComparison.Ordinal))
            {
                Context.Session["UserRole"] =
                    role;
            }
        }

        private string GetRequestLanguage()
        {
            HttpCookie languageCookie =
                Request.Cookies["lang"];

            string language =
                languageCookie != null
                    ? languageCookie.Value
                    : null;

            if (string.IsNullOrWhiteSpace(language) ||
                !AllowedLanguages.Contains(language))
            {
                return "en";
            }

            return language.Trim();
        }

        private static CultureInfo GetOrCreateCulture(
            string language)
        {
            return CultureCache.GetOrAdd(
                language,
                key =>
                {
                    CultureInfo culture;

                    try
                    {
                        culture =
                            new CultureInfo(key);
                    }
                    catch (CultureNotFoundException)
                    {
                        culture =
                            new CultureInfo("en");
                    }

                    culture.DateTimeFormat.Calendar =
                        new GregorianCalendar();

                    // Cached instance ko read-only banana thread-safe hai.
                    return CultureInfo.ReadOnly(culture);
                }
            );
        }

        private static bool IsStaticResource(
            string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return false;
            }

            string extension =
                VirtualPathUtility.GetExtension(path);

            return
                !string.IsNullOrWhiteSpace(extension) &&
                StaticExtensions.Contains(extension);
        }

        private void ExpireAuthenticationCookie()
        {
            HttpCookie cookie =
                new HttpCookie(
                    FormsAuthentication.FormsCookieName,
                    string.Empty
                )
                {
                    Expires =
                        DateTime.UtcNow.AddDays(-1),

                    HttpOnly = true
                };

            Response.Cookies.Add(cookie);
        }
    }
}