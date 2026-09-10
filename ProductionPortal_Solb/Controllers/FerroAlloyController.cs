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
    public class FerroAlloyController : Controller
    {
        private readonly FerroAlloyRepository repo;

        public FerroAlloyController()
        {
            repo = new FerroAlloyRepository();
        }

        [HttpGet]
        public ActionResult List()
        {
            var data =
                repo.GetAll()
                ??
                new List<QCFerroAlloyReportBLL>();

            return View(
                "~/Views/RawMaterial/FerroAlloy/List.cshtml",
                data
            );
        }

        [HttpGet]
        public ActionResult Add(int? id)
        {
            QCFerroAlloyReportBLL model;

            if (
                id.HasValue &&
                id.Value > 0
            )
            {
                model =
                    repo.GetByID(id.Value);

                if (model == null)
                {
                    TempData["ErrorMessage"] =
                        "Ferro Alloy report not found.";

                    return RedirectToAction("List");
                }
            }
            else
            {
                model =
                    new QCFerroAlloyReportBLL
                    {
                        AnalysisDate = DateTime.Today,
                        ReceivingDate = DateTime.Today,
                        FinalStatus = "Accepted",
                        Samples = new List<QCFerroAlloySampleBLL>
                        {
                            new QCFerroAlloySampleBLL
                            {
                                SrNo = 1
                            }
                        }
                    };
            }

            return View(
                "~/Views/RawMaterial/FerroAlloy/Add.cshtml",
                model
            );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(
            QCFerroAlloyReportBLL model)
        {
            if (model == null)
            {
                TempData["ErrorMessage"] =
                    "Invalid Ferro Alloy report data.";

                return RedirectToAction("List");
            }

            model.Supplier = Clean(model.Supplier);
            model.ShipmentCodeNo = Clean(model.ShipmentCodeNo);
            model.Material = Clean(model.Material);
            model.ReferenceNo = Clean(model.ReferenceNo);
            model.DeliveryPO = Clean(model.DeliveryPO);
            model.LotSize = Clean(model.LotSize);
            model.CertificateNo = Clean(model.CertificateNo);
            model.FinalStatus = Clean(model.FinalStatus);
            model.MRBNo = Clean(model.MRBNo);
            model.Remarks = Clean(model.Remarks);
            model.Comments = Clean(model.Comments);

            if (!model.AnalysisDate.HasValue)
            {
                return SaveError(
                    model,
                    "Analysis Date is required."
                );
            }

            if (
                string.IsNullOrWhiteSpace(
                    model.Material
                )
            )
            {
                return SaveError(
                    model,
                    "Material is required."
                );
            }

            model.Samples =
                model.Samples
                ??
                new List<QCFerroAlloySampleBLL>();

            var activeSamples =
                model.Samples
                    .Where(
                        x =>
                            x != null &&
                            !IsEmptySample(x)
                    )
                    .ToList();

            if (!activeSamples.Any())
            {
                return SaveError(
                    model,
                    "At least one Sample row is required."
                );
            }

            for (
                int i = 0;
                i < activeSamples.Count;
                i++
            )
            {
                activeSamples[i].SrNo = i + 1;
                activeSamples[i].SampleNo =
                    Clean(activeSamples[i].SampleNo);
                activeSamples[i].Comment =
                    Clean(activeSamples[i].Comment);
            }

            var duplicateSampleNos =
                activeSamples
                    .Where(
                        x =>
                            !string.IsNullOrWhiteSpace(
                                x.SampleNo
                            )
                    )
                    .GroupBy(
                        x => x.SampleNo,
                        StringComparer.OrdinalIgnoreCase
                    )
                    .Where(
                        g => g.Count() > 1
                    )
                    .Select(
                        g => g.Key
                    )
                    .ToList();

            if (duplicateSampleNos.Any())
            {
                return SaveError(
                    model,
                    "Duplicate Sample No(s): "
                    +
                    string.Join(
                        ", ",
                        duplicateSampleNos
                    )
                );
            }

            string currentUser =
                GetCurrentUser();

            try
            {
                int reportID;

                if (model.ID <= 0)
                {
                    model.StatusID = 1;
                    model.CreatedDate = DateTime.Now;
                    model.CreatedBy = currentUser;

                    reportID =
                        repo.Insert(model);

                    if (reportID <= 0)
                    {
                        return SaveError(
                            model,
                            "Ferro Alloy report could not be saved."
                        );
                    }
                }
                else
                {
                    model.UpdatedDate = DateTime.Now;
                    model.UpdatedBy = currentUser;

                    reportID =
                        repo.Update(model);

                    if (reportID <= 0)
                    {
                        return SaveError(
                            model,
                            "Ferro Alloy report could not be updated."
                        );
                    }

                    repo.DeleteSamplesByReportID(
                        reportID,
                        currentUser
                    );
                }

                foreach (
                    var sample
                    in activeSamples
                )
                {
                    sample.ReportID = reportID;
                    sample.StatusID = 1;
                    sample.CreatedDate = DateTime.Now;
                    sample.CreatedBy = currentUser;

                    int sampleID =
                        repo.InsertSample(sample);

                    if (sampleID <= 0)
                    {
                        throw new Exception(
                            "One or more sample rows could not be saved."
                        );
                    }
                }

                TempData["SuccessMessage"] =
                    model.ID > 0
                        ? "Ferro Alloy report updated successfully."
                        : "Ferro Alloy report saved successfully.";

                return RedirectToAction("List");
            }
            catch (Exception ex)
            {
                return SaveError(
                    model,
                    "Unable to save Ferro Alloy report. "
                    + ex.Message
                );
            }
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            var model =
                repo.GetByID(id);

            if (model == null)
            {
                return HttpNotFound();
            }

            return View(
                "~/Views/RawMaterial/FerroAlloy/Details.cshtml",
                model
            );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                int affected =
                    repo.Delete(
                        id,
                        GetCurrentUser()
                    );

                TempData[
                    affected > 0
                        ? "SuccessMessage"
                        : "ErrorMessage"
                ] =
                    affected > 0
                        ? "Ferro Alloy report deleted successfully."
                        : "Ferro Alloy report was not deleted.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] =
                    "Unable to delete report. "
                    + ex.Message;
            }

            return RedirectToAction("List");
        }

        private ActionResult SaveError(
            QCFerroAlloyReportBLL model,
            string message)
        {
            ModelState.AddModelError(
                string.Empty,
                message
            );

            if (model == null)
            {
                model =
                    new QCFerroAlloyReportBLL();
            }

            if (
                model.Samples == null ||
                model.Samples.Count == 0
            )
            {
                model.Samples =
                    new List<QCFerroAlloySampleBLL>
                    {
                        new QCFerroAlloySampleBLL
                        {
                            SrNo = 1
                        }
                    };
            }

            return View(
                "~/Views/RawMaterial/FerroAlloy/Add.cshtml",
                model
            );
        }

        private static bool IsEmptySample(
            QCFerroAlloySampleBLL item)
        {
            return
                string.IsNullOrWhiteSpace(
                    item.SampleNo
                )
                &&
                item.Si == null
                &&
                item.Mn == null
                &&
                item.P == null
                &&
                item.S == null
                &&
                item.Al == null
                &&
                item.Ca == null
                &&
                item.V == null
                &&
                string.IsNullOrWhiteSpace(
                    item.Comment
                );
        }

        private static string Clean(string value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? string.Empty
                : value.Trim();
        }

        private string GetCurrentUser()
        {
            string currentUser =
                Convert.ToString(
                    Session["UserName"]
                );

            if (
                string.IsNullOrWhiteSpace(
                    currentUser
                )
                &&
                User != null
                &&
                User.Identity != null
            )
            {
                currentUser =
                    User.Identity.Name;
            }

            return string.IsNullOrWhiteSpace(
                currentUser
            )
                ? "System"
                : currentUser.Trim();
        }
    }
}
