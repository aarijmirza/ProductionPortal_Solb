using BAL.Repositories;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.SessionState;
using WebAPICode.Helpers;

namespace ProductionPortal_Solb.Controllers
{
    [SessionState(SessionStateBehavior.ReadOnly)]
    public class HomeController : BaseController
    {
        [Authorize(Roles = "Administrator,User,Rolling Mill User,QC User,Sher,Supply Chain,Utility,Steel Making")]
        [Route("dashboard")]
        public ActionResult Index()
        {
            var repo = new RollingMillRepository();

            DateTime selectedDate = Session["RollingMillSelectedDate"] != null
                ? Convert.ToDateTime(Session["RollingMillSelectedDate"])
                : DateTime.Today;

            var nextDate = selectedDate.AddDays(1);

            bool hasSelectedDateData = repo.RollingMillDetails()
                .Any(x => x.Date >= selectedDate && x.Date < nextDate);

            ViewBag.HasRollingMillTodayData = hasSelectedDateData;
            ViewBag.RollingMillSelectedDate = selectedDate.ToString("dd-MMM-yyyy");

            return View();
        }
    }
}