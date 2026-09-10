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
    public class FluorsparController : Controller
    {
        private readonly FluorsparRepository repo = new FluorsparRepository();

        [HttpGet]
        public ActionResult List()
        {
            return View("~/Views/RawMaterial/Fluorspar/List.cshtml", repo.GetAll() ?? new List<QCFluorsparReportBLL>());
        }

        [HttpGet]
        public ActionResult Add(int? id)
        {
            QCFluorsparReportBLL model;
            if (id.HasValue && id.Value > 0)
            {
                model = repo.GetByID(id.Value);
                if (model == null)
                {
                    TempData["ErrorMessage"] = "Fluorspar report not found.";
                    return RedirectToAction("List");
                }
            }
            else
            {
                model = new QCFluorsparReportBLL
                {
                    ReceivingDate = DateTime.Today,
                    AnalysisDate = DateTime.Today,
                    Material = "Fluorspar",
                    FinalStatus = "Accepted",
                    Samples = new List<QCFluorsparSampleBLL> { new QCFluorsparSampleBLL { SrNo = 1 } }
                };
            }
            return View("~/Views/RawMaterial/Fluorspar/Add.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(QCFluorsparReportBLL model)
        {
            if (model == null) return RedirectToAction("List");
            model.Supplier = Clean(model.Supplier); model.ShipmentCodeNo = Clean(model.ShipmentCodeNo); model.Material = Clean(model.Material);
            model.ReferenceNo = Clean(model.ReferenceNo); model.DeliveryPO = Clean(model.DeliveryPO); model.LotSize = Clean(model.LotSize);
            model.CertificateNo = Clean(model.CertificateNo); model.FinalStatus = Clean(model.FinalStatus); model.MRBNo = Clean(model.MRBNo);
            model.Remarks = Clean(model.Remarks); model.Comments = Clean(model.Comments);

            if (!model.AnalysisDate.HasValue) return SaveError(model, "Analysis Date is required.");
            if (string.IsNullOrWhiteSpace(model.Material)) return SaveError(model, "Material is required.");

            model.Samples = model.Samples ?? new List<QCFluorsparSampleBLL>();
            var rows = model.Samples.Where(x => x != null && !IsEmpty(x)).ToList();
            if (!rows.Any()) return SaveError(model, "At least one sample row is required.");

            for (int i = 0; i < rows.Count; i++) { rows[i].SrNo = i + 1; rows[i].SampleNo = Clean(rows[i].SampleNo); rows[i].Comment = Clean(rows[i].Comment); }
            var duplicates = rows.Where(x => !string.IsNullOrWhiteSpace(x.SampleNo)).GroupBy(x => x.SampleNo, StringComparer.OrdinalIgnoreCase).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
            if (duplicates.Any()) return SaveError(model, "Duplicate Sample No(s): " + string.Join(", ", duplicates));

            string user = GetCurrentUser();
            try
            {
                int reportID;
                bool isEdit = model.ID > 0;
                if (!isEdit)
                {
                    model.StatusID = 1; model.CreatedDate = DateTime.Now; model.CreatedBy = user;
                    reportID = repo.Insert(model);
                }
                else
                {
                    model.UpdatedDate = DateTime.Now; model.UpdatedBy = user;
                    reportID = repo.Update(model);
                    if (reportID > 0) repo.DeleteSamplesByReportID(reportID, user);
                }

                if (reportID <= 0) return SaveError(model, isEdit ? "Fluorspar report could not be updated." : "Fluorspar report could not be saved.");

                foreach (var row in rows)
                {
                    row.ReportID = reportID; row.StatusID = 1; row.CreatedDate = DateTime.Now; row.CreatedBy = user;
                    if (repo.InsertSample(row) <= 0) throw new Exception("One or more sample rows could not be saved.");
                }

                TempData["SuccessMessage"] = isEdit ? "Fluorspar report updated successfully." : "Fluorspar report saved successfully.";
                return RedirectToAction("List");
            }
            catch (Exception ex) { return SaveError(model, "Unable to save Fluorspar report. " + ex.Message); }
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            var model = repo.GetByID(id);
            if (model == null) return HttpNotFound();
            return View("~/Views/RawMaterial/Fluorspar/Details.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                int affected = repo.Delete(id, GetCurrentUser());
                TempData[affected > 0 ? "SuccessMessage" : "ErrorMessage"] = affected > 0 ? "Fluorspar report deleted successfully." : "Fluorspar report was not deleted.";
            }
            catch (Exception ex) { TempData["ErrorMessage"] = "Unable to delete Fluorspar report. " + ex.Message; }
            return RedirectToAction("List");
        }

        private ActionResult SaveError(QCFluorsparReportBLL model, string message)
        {
            ModelState.AddModelError(string.Empty, message);
            if (model.Samples == null || model.Samples.Count == 0) model.Samples = new List<QCFluorsparSampleBLL> { new QCFluorsparSampleBLL { SrNo = 1 } };
            return View("~/Views/RawMaterial/Fluorspar/Add.cshtml", model);
        }

        private static bool IsEmpty(QCFluorsparSampleBLL x)
        {
            return string.IsNullOrWhiteSpace(x.SampleNo) && x.CaF2 == null && x.SiO2 == null && x.P == null && x.S == null && x.Fe2O3 == null && x.Al2O3 == null && x.Na2O == null && x.K2O == null && x.BaO == null && x.Pb == null && string.IsNullOrWhiteSpace(x.Comment);
        }
        private static string Clean(string s) => string.IsNullOrWhiteSpace(s) ? string.Empty : s.Trim();
        private string GetCurrentUser()
        {
            string u = Convert.ToString(Session["UserName"]);
            if (string.IsNullOrWhiteSpace(u) && User != null && User.Identity != null) u = User.Identity.Name;
            return string.IsNullOrWhiteSpace(u) ? "System" : u.Trim();
        }
    }
}
