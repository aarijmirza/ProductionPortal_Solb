using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProductionPortal_Solb.Controllers
{
    public class HomeController : Controller
    {
        [Route("dashboard")]
        public ActionResult Index()
        {
            return View();
        }
    }
}