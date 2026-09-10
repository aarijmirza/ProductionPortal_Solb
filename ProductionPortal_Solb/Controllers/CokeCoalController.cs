using BAL.Repositories;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.SessionState;

namespace ProductionPortal_Solb.Controllers
{
    [SessionState(SessionStateBehavior.Required)]
    public class CokeCoalController : Controller
    {
        private readonly CokeCoalRepository repo;

        public CokeCoalController()
        {
            repo = new CokeCoalRepository();
        }

        [HttpGet]
        public ActionResult Index()
        {
            var data = repo.GetAll() ?? new List<QCCokeCoalReportBLL>();

            return View(
                "~/Views/RawMaterial/CokeCoal/Index.cshtml",
                data
            );
        }

        [HttpGet]
        public ActionResult Add(int? id)
        {
            QCCokeCoalReportBLL model;

            if (id.HasValue && id.Value > 0)
            {
                model = repo.GetByID(id.Value);

                if (model == null)
                {
                    TempData["ErrorMessage"] = "Coke & Coal report not found.";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                model = new QCCokeCoalReportBLL
                {
                    ReceivingDate = DateTime.Today,
                    AnalysisDate = DateTime.Today,
                    FinalStatus = "Accepted",
                    Samples = new List<QCCokeCoalSampleBLL>
                    {
                        new QCCokeCoalSampleBLL { SrNo = 1 }
                    }
                };
            }

            return View(
                "~/Views/RawMaterial/CokeCoal/Add.cshtml",
                model
            );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(QCCokeCoalReportBLL model)
        {
            if (model == null)
            {
                TempData["ErrorMessage"] = "Invalid Coke & Coal report data.";
                return RedirectToAction("Index");
            }

            if (!model.AnalysisDate.HasValue)
            {
                return SaveError(model, "Analysis Date is required.");
            }

            if (string.IsNullOrWhiteSpace(model.Material))
            {
                return SaveError(model, "Material is required.");
            }

            model.Samples = model.Samples ?? new List<QCCokeCoalSampleBLL>();

            var rows = model.Samples
                .Where(x => x != null && !IsEmptySample(x))
                .ToList();

            if (!rows.Any())
            {
                return SaveError(model, "At least one sample row is required.");
            }

            for (int i = 0; i < rows.Count; i++)
            {
                rows[i].SrNo = i + 1;
            }

            string currentUser = GetCurrentUser();

            try
            {
                int reportID;

                if (model.ID <= 0)
                {
                    model.CreatedBy = currentUser;
                    reportID = repo.Insert(model);

                    if (reportID <= 0)
                        return SaveError(model, "Report could not be saved.");
                }
                else
                {
                    model.UpdatedBy = currentUser;
                    reportID = repo.Update(model);

                    if (reportID <= 0)
                        return SaveError(model, "Report could not be updated.");

                    repo.DeleteSamplesByReportID(reportID, currentUser);
                }

                foreach (var row in rows)
                {
                    row.ReportID = reportID;
                    row.CreatedBy = currentUser;

                    int rowID = repo.InsertSample(row);

                    if (rowID <= 0)
                    {
                        throw new Exception(
                            "One or more sample rows could not be saved."
                        );
                    }
                }

                TempData["SuccessMessage"] =
                    model.ID > 0
                        ? "Coke & Coal report updated successfully."
                        : "Coke & Coal report saved successfully.";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return SaveError(
                    model,
                    "Unable to save Coke & Coal report. " + ex.Message
                );
            }
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            var model = repo.GetByID(id);

            if (model == null)
                return HttpNotFound();

            return View(
                "~/Views/RawMaterial/CokeCoal/Details.cshtml",
                model
            );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                int result = repo.Delete(id, GetCurrentUser());

                TempData[result > 0 ? "SuccessMessage" : "ErrorMessage"] =
                    result > 0
                        ? "Coke & Coal report deleted successfully."
                        : "Report was not deleted.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] =
                    "Unable to delete report. " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        private ActionResult SaveError(
            QCCokeCoalReportBLL model,
            string message)
        {
            ModelState.AddModelError(string.Empty, message);

            if (model.Samples == null || model.Samples.Count == 0)
            {
                model.Samples = new List<QCCokeCoalSampleBLL>
                {
                    new QCCokeCoalSampleBLL { SrNo = 1 }
                };
            }

            return View(
                "~/Views/RawMaterial/CokeCoal/Add.cshtml",
                model
            );
        }

        private static bool IsEmptySample(QCCokeCoalSampleBLL x)
        {
            return
                string.IsNullOrWhiteSpace(x.SampleCode) &&
                x.Ash == null &&
                x.Moisture == null &&
                x.Volatile == null &&
                x.S == null &&
                x.FixedCarbon == null &&
                string.IsNullOrWhiteSpace(x.GrainSizeMM) &&
                string.IsNullOrWhiteSpace(x.Remark);
        }

        private string GetCurrentUser()
        {
            string user = Convert.ToString(Session["UserName"]);

            if (
                string.IsNullOrWhiteSpace(user) &&
                User != null &&
                User.Identity != null
            )
            {
                user = User.Identity.Name;
            }

            return string.IsNullOrWhiteSpace(user)
                ? "System"
                : user.Trim();
        }
    }
}
