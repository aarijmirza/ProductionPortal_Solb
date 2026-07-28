using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;

namespace ProductionPortal_Solb.Controllers
{
    [SessionState(
    SessionStateBehavior.ReadOnly
    )]
    public class SecurityController : Controller
    {
        // GET: Security
        public ActionResult KPIlist()
        {
            return View();
        }
        public ActionResult KPIAdd()
        {
            return View();
        }

        public ActionResult FSadd()
        {
            return View("~/Views/Security/FireSystemKPI/FSadd.cshtml");
        }
    }
}