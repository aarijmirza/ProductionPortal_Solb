using BAL.Repositories;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProductionPortal_Solb.Controllers
{
    public class SupplyChainController : Controller
    {
        SupplyChainRepository repo = new SupplyChainRepository();
        // GET: SupplyChain

        [HttpGet]
        public ActionResult List(DateTime? fromDate, DateTime? toDate)
        {
            DateTime from = fromDate ?? DateTime.Today;
            DateTime to = toDate ?? DateTime.Today;

            var data = repo.GetSupplyChainDailyList(from, to);

            ViewBag.FromDate = from.ToString("yyyy-MM-dd");
            ViewBag.ToDate = to.ToString("yyyy-MM-dd");

            return View(data);
        }

        [HttpGet]
        public ActionResult Add(int? id)
        {
            if (id.HasValue && id.Value > 0)
            {
                var model = repo.GetSupplyChainDailyByID(id.Value);

                if (model == null || model.ID <= 0)
                {
                    TempData["ErrorMessage"] = "Record not found.";
                    return RedirectToAction("List");
                }

                return View(model);
            }

            var newModel = new SupplyChainDailyBLL
            {
                ReportDate = DateTime.Today,
                ReportTime = DateTime.Now.TimeOfDay
            };

            return View(newModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult add(SupplyChainDailyBLL model)
        {
            try
            {
                if (model == null)
                {
                    TempData["ErrorMessage"] = "Invalid data submitted.";
                    return RedirectToAction("Add");
                }

                if (!model.ReportDate.HasValue)
                {
                    TempData["ErrorMessage"] = "Please select report date.";
                    return View(model);
                }

                if (!model.ReportTime.HasValue)
                {
                    model.ReportTime = DateTime.Now.TimeOfDay;
                }

                if (model.ID > 0)
                {
                    model.UpdatedBy = User.Identity.Name;
                    model.UpdatedDate = DateTime.Now;
                }
                else
                {
                    model.StatusID = 1;
                    model.CreatedBy = User.Identity.Name;
                    model.CreatedDate = DateTime.Now;
                }

                int result = repo.SaveSupplyChainDaily(model);

                if (result < 0)
                {
                    TempData["SuccessMessage"] = "Supply Chain dashboard data saved successfully.";
                    return RedirectToAction("Add");
                }

                TempData["ErrorMessage"] = "Data not saved. Please try again.";
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error: " + ex.Message;
                return View(model);
            }
        }
    }
}