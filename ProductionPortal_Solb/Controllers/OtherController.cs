using BAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProductionPortal_Solb.Controllers
{
    public class OtherController : Controller
    {
        OtherRepository repo;
        public OtherController()
        {
            repo = new OtherRepository();
        }
        // GET: Other
        public ActionResult Delays()
        {
            var requests = repo.GetAllDelays();
            return View(requests);
        }
    }
}