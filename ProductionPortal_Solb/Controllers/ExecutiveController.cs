using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProductionPortal_Solb.Controllers
{
    public class ExecutiveController : Controller
    {
        // GET: Executive
        public ActionResult list()
        {
            return View();
        }
        public ActionResult dashboard()
        {
            return View();
        }
    }
}