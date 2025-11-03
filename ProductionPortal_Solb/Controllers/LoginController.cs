using BAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;

namespace ProductionPortal_Solb.Controllers
{
    public class LoginController : Controller
    {
        LoginRepository repo;
        public LoginController()
        {
            repo = new LoginRepository();
        } 
        // GET: Login
        [AllowAnonymous]
        public ActionResult login()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult login(string Username, string Password)
        {
            if (!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password))
            {
                var user = repo.GetAuthenticateUser(Username, Password); // get user from DB

                if (user != null)
                {
                    // Example: Get role from user object
                    var roles = user.UserRole[0].RoleName; // e.g., "Admin" or "User"
                    var userid = user.ID;

                    var userData = $"{roles}|{userid}";

                    // Create auth ticket with roles embedded
                    var authTicket = new FormsAuthenticationTicket(
                        1,
                        user.Name,                        // username
                        DateTime.Now,
                        DateTime.Now.AddMinutes(30),
                        false,                            // persistent?
                                                          //user.UserRole[0].RoleName,
                        userData                          // roles stored in UserData
                    );

                    // Encrypt the ticket
                    string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                    var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                    Response.Cookies.Add(cookie);
                    // Also store in Session for easy use in views
                    Session["UserID"] = userid;
                    Session["UserName"] = user.Name;
                    //Session["UserRole"] = user.UserRole[0].RoleName;
                    Session["UserRole"] = roles;

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["Error"] = "Invalid email or password.";
                    return RedirectToAction("Login", "Login");
                }
            }

            TempData["Error"] = "Please enter email and password.";
            return RedirectToAction("Login", "Login");
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("login", "Login");
        }
    }
}