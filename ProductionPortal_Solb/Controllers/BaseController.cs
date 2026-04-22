using BAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProductionPortal_Solb.Controllers
{
    public class BaseController : Controller
    {
        // GET: Base
        protected void SetRollingMillMenuFlag()
        {
            var repo = new RollingMillRepository();

            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            ViewBag.HasRollingMillTodayData = repo.RollingMillDetails()
                .Any(x => x.Date >= today && x.Date < tomorrow);
        }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            SetRollingMillMenuFlag(); // 🔥 yahan call hoga har request pe
            base.OnActionExecuting(filterContext);
        }
    }
}