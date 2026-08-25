using BAL.Repositories;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
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
        public ActionResult dashboard(
            DateTime? fromDate,
            DateTime? toDate)
        {
            DateTime selectedFromDate = (fromDate ?? DateTime.Today).Date;
            DateTime selectedToDate = (toDate ?? selectedFromDate).Date;

            if (selectedFromDate > selectedToDate)
            {
                DateTime swap = selectedFromDate;
                selectedFromDate = selectedToDate;
                selectedToDate = swap;
            }

            try
            {
                ExecutiveDashboardRepository repository =
                    new ExecutiveDashboardRepository();

                ExecutiveOperationsDashboardVM model =
                    repository.GetExecutiveDashboard(
                        selectedFromDate,
                        selectedToDate);

                return View("dashboard", model);
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    "Executive dashboard could not be loaded. " +
                    ex.Message;

                ExecutiveOperationsDashboardVM model =
                    new ExecutiveOperationsDashboardVM
                    {
                        FromDate = selectedFromDate,
                        ToDate = selectedToDate,
                        GeneratedOn = DateTime.Now
                    };

                return View("dashboard", model);
            }
        }
    }
}