using BAL.Repositories;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ProductionPortal_Solb.Controllers
{
    public class LimeReactivityController : Controller
    {
        private readonly LimeReactivityRepository repo = new LimeReactivityRepository();

        [HttpGet]
        public ActionResult List()
        {
            return View("~/Views/RawMaterial/LimeReactivity/List.cshtml", repo.GetAll());
        }

        [HttpGet]
        public ActionResult Add(int? id)
        {
            QCLimeReactivityReportBLL model;

            if (id.HasValue && id.Value > 0)
            {
                model = repo.GetByID(id.Value);
                if (model == null)
                {
                    TempData["ErrorMessage"] = "Lime Reactivity record not found.";
                    return RedirectToAction("List");
                }
            }
            else
            {
                model = new QCLimeReactivityReportBLL
                {
                    AnalysisDateQC = DateTime.Today,
                    FinalStatus = "Accepted",
                    Samples = new List<QCLimeReactivitySampleBLL>
                    {
                        new QCLimeReactivitySampleBLL { SrNo = 1 }
                    }
                };
            }

            if (model.Samples == null || model.Samples.Count == 0)
                model.Samples = new List<QCLimeReactivitySampleBLL> { new QCLimeReactivitySampleBLL { SrNo = 1 } };

            return View("~/Views/RawMaterial/LimeReactivity/Add.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(QCLimeReactivityReportBLL model)
        {
            if (model == null)
            {
                TempData["ErrorMessage"] = "Invalid Lime Reactivity data.";
                return RedirectToAction("List");
            }

            model.Samples = (model.Samples ?? new List<QCLimeReactivitySampleBLL>())
                .Where(x => x != null && !IsEmptySample(x))
                .ToList();

            if (!model.AnalysisDateQC.HasValue)
                return SaveError(model, "Analysis Date (QC) is required.");

            if (model.Samples.Count == 0)
                return SaveError(model, "At least one test row is required.");

            if (string.Equals(model.FinalStatus, "MRB", StringComparison.OrdinalIgnoreCase)
                && string.IsNullOrWhiteSpace(model.MRBNo))
                return SaveError(model, "MRB No. is required when Final Status is MRB.");

            try
            {
                string currentUser = GetCurrentUser();
                int reportID;

                if (model.ID <= 0)
                {
                    model.CreatedBy = currentUser;
                    reportID = repo.InsertMaster(model);
                    if (reportID <= 0)
                        return SaveError(model, "Lime Reactivity report could not be saved.");
                }
                else
                {
                    model.UpdatedBy = currentUser;
                    reportID = repo.UpdateMaster(model);
                    if (reportID <= 0)
                        return SaveError(model, "Lime Reactivity report could not be updated.");

                    repo.DeleteSamplesByReportID(model.ID, currentUser);
                    reportID = model.ID;
                }

                for (int i = 0; i < model.Samples.Count; i++)
                {
                    var sample = model.Samples[i];
                    sample.ReportID = reportID;
                    sample.SrNo = i + 1;
                    sample.CreatedBy = currentUser;

                    int sampleID = repo.InsertSample(sample);
                    if (sampleID <= 0)
                        throw new Exception("One or more test rows could not be saved.");
                }

                TempData["SuccessMessage"] = model.ID <= 0
                    ? "Lime Reactivity report saved successfully."
                    : "Lime Reactivity report updated successfully.";

                return RedirectToAction("List");
            }
            catch (Exception ex)
            {
                return SaveError(model, "Unable to save Lime Reactivity report. " + ex.Message);
            }
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            var model = repo.GetByID(id);
            if (model == null) return HttpNotFound();
            return View("~/Views/RawMaterial/LimeReactivity/Details.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                int affected = repo.DeleteReport(id, GetCurrentUser());
                TempData[affected > 0 ? "SuccessMessage" : "ErrorMessage"] =
                    affected > 0 ? "Lime Reactivity report deleted successfully." : "Record could not be deleted.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("List");
        }

        private ActionResult SaveError(QCLimeReactivityReportBLL model, string message)
        {
            ModelState.AddModelError(string.Empty, message);
            if (model.Samples == null || model.Samples.Count == 0)
                model.Samples = new List<QCLimeReactivitySampleBLL> { new QCLimeReactivitySampleBLL { SrNo = 1 } };
            return View("~/Views/RawMaterial/LimeReactivity/Add.cshtml", model);
        }

        private static bool IsEmptySample(QCLimeReactivitySampleBLL x)
        {
            return string.IsNullOrWhiteSpace(x.ShipmentCode)
                && !x.ReceivedDate.HasValue
                && string.IsNullOrWhiteSpace(x.Supplier)
                && !x.QuantityTons.HasValue
                && string.IsNullOrWhiteSpace(x.SampleNo)
                && !x.ActiveCaO.HasValue
                && !x.TotalCaO.HasValue
                && !x.MgO.HasValue
                && !x.LOI.HasValue
                && !x.Temp0Sec.HasValue
                && !x.Temp30Sec.HasValue
                && !x.Temp60Sec.HasValue
                && !x.Temp90Sec.HasValue
                && !x.Temp120Sec.HasValue
                && !x.Temp150Sec.HasValue
                && !x.Temp180Sec.HasValue
                && !x.Temp240Sec.HasValue
                && !x.Temp360Sec.HasValue
                && !x.Temp600Sec.HasValue
                && string.IsNullOrWhiteSpace(x.TestStatus);
        }

        private string GetCurrentUser()
        {
            string currentUser = Convert.ToString(Session["UserName"]);
            if (string.IsNullOrWhiteSpace(currentUser) && User != null && User.Identity != null)
                currentUser = User.Identity.Name;
            return string.IsNullOrWhiteSpace(currentUser) ? "System" : currentUser.Trim();
        }
    }
}
