using BAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebAPICode.Helpers;

namespace ProductionPortal_Solb.Controllers
{
    public class HomeController : BaseController
    {
        [Authorize(Roles = "Administrator, Rolling Mill User, User, QC User")]

        [Route("dashboard")]
        //public ActionResult Index()
        //{
        //    return View();
        //}

        public ActionResult Index()
        {
            var repo = new RollingMillRepository();

            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            bool hasTodayData = repo.RollingMillDetails()
                .Any(x => x.Date >= today && x.Date < tomorrow);

            ViewBag.HasRollingMillTodayData = hasTodayData;

            return View();
        }
    }
}