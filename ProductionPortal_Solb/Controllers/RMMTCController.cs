
using BAL.Repositories;
using BAL.Services;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ProductionPortal_Solb.Controllers
{
    public class RMMTCController : Controller
    {
        private readonly RMMTCRepository repo =
            new RMMTCRepository();

        [HttpGet]
        public ActionResult Index(
            int? id = null,
            string heatSearch = null,
            string mtcSearch = null)
        {
            RMMTCPageVM model =
                new RMMTCPageVM();

            model.EligibleHeats =
                repo.GetEligibleHeats(heatSearch)
                ?? new List<RMMTCEligibleHeatBLL>();

            model.MTCList =
                repo.GetMTCList(mtcSearch)
                ?? new List<RMMTCListRowBLL>();

            if (id.HasValue && id.Value > 0)
            {
                model.Header =
                    repo.GetHeader(id.Value)
                    ?? new RMMTCHeaderBLL();

                model.SelectedHeats =
                    repo.GetDetails(id.Value)
                    ?? new List<RMMTCDetailBLL>();
            }
            else
            {
                model.Header =
                    new RMMTCHeaderBLL
                    {
                        MTCNo =
                            repo.GetNextNumberPreview(),
                        IssueDate =
                            DateTime.Today
                    };
            }

            return View(
                "~/Views/RMMTC/Index.cshtml",
                model
            );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateNew()
        {
            try
            {
                RMMTCHeaderBLL header =
                    repo.CreateNew(CurrentUser());

                if (header == null || header.ID <= 0)
                    throw new Exception("Unable to create new MTC.");

                return RedirectToAction(
                    "Index",
                    new { id = header.ID }
                );
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelReset(int id)
        {
            try
            {
                if (id > 0)
                    repo.CancelDraft(id, CurrentUser());

                TempData.Remove("ErrorMessage");
                TempData.Remove("SuccessMessage");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveHeader(
            RMMTCHeaderBLL model)
        {
            try
            {
                if (model == null || model.ID <= 0)
                    throw new Exception("Create/select an MTC first.");

                repo.SaveHeader(
                    model,
                    CurrentUser()
                );

                TempData["SuccessMessage"] =
                    "MTC header saved successfully.";

                return RedirectToAction(
                    "Index",
                    new { id = model.ID }
                );
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;

                return RedirectToAction(
                    "Index",
                    new
                    {
                        id =
                            model != null && model.ID > 0
                                ? (int?)model.ID
                                : null
                    }
                );
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddHeats(
            int mtcID,
            int[] selectedHeatIDs)
        {
            try
            {
                if (mtcID <= 0)
                    throw new Exception("Create/select an MTC first.");

                if (selectedHeatIDs == null || selectedHeatIDs.Length == 0)
                    throw new Exception("Select at least one heat.");

                var ids = selectedHeatIDs
                    .Where(x => x > 0)
                    .Distinct()
                    .ToList();

                if (ids.Count == 0)
                    throw new Exception("Select at least one heat.");

                var currentDetails = repo.GetDetails(mtcID)
                    ?? new List<RMMTCDetailBLL>();

                int availableSlots = 10 - currentDetails.Count;
                if (availableSlots <= 0)
                    throw new Exception("Maximum 10 Heat IDs are allowed for every MTC.");

                if (ids.Count > availableSlots)
                    throw new Exception(
                        "You can select only " + availableSlots +
                        " more heat(s). Maximum 10 Heat IDs are allowed for every MTC.");

                var eligibleHeats = repo.GetEligibleHeats(null)
                    ?? new List<RMMTCEligibleHeatBLL>();

                var heatsToAdd = eligibleHeats
                    .Where(x => ids.Contains(x.QCInspectionRMID))
                    .ToList();

                if (heatsToAdd.Count != ids.Count)
                    throw new Exception("One or more selected heats are no longer available.");

                if (currentDetails.Any(x => ids.Contains(x.QCInspectionRMID)))
                    throw new Exception("One or more selected heats are already added to this MTC.");

                foreach (var heat in heatsToAdd)
                {
                    repo.AddHeat(
                        mtcID,
                        heat.QCInspectionRMID,
                        heat.NoOfBundles,
                        CurrentUser()
                    );
                }

                TempData["SuccessMessage"] =
                    ids.Count + " heat(s) added successfully.";

                return RedirectToAction(
                    "Index",
                    new { id = mtcID }
                );
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;

                return RedirectToAction(
                    "Index",
                    new { id = mtcID }
                );
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddHeat(
            int mtcID,
            int qcInspectionRMID,
            int? actualNoOfBundles)
        {
            try
            {
                repo.AddHeat(
                    mtcID,
                    qcInspectionRMID,
                    actualNoOfBundles,
                    CurrentUser()
                );

                return RedirectToAction(
                    "Index",
                    new { id = mtcID }
                );
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;

                return RedirectToAction(
                    "Index",
                    new { id = mtcID }
                );
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateDetail(
            int mtcID,
            int detailID,
            int? actualNoOfBundles)
        {
            try
            {
                repo.UpdateDetail(
                    detailID,
                    actualNoOfBundles
                );

                return RedirectToAction(
                    "Index",
                    new { id = mtcID }
                );
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;

                return RedirectToAction(
                    "Index",
                    new { id = mtcID }
                );
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveHeat(
            int mtcID,
            int detailID)
        {
            try
            {
                repo.RemoveHeat(detailID);

                return RedirectToAction(
                    "Index",
                    new { id = mtcID }
                );
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;

                return RedirectToAction(
                    "Index",
                    new { id = mtcID }
                );
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FinalizeMTC(
            int id)
        {
            try
            {
                repo.FinalizeMTC(
                    id,
                    CurrentUser()
                );

                TempData["SuccessMessage"] =
                    "MTC finalized successfully.";

                return RedirectToAction(
                    "Index",
                    new { id = id }
                );
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;

                return RedirectToAction(
                    "Index",
                    new { id = id }
                );
            }
        }

        [HttpGet]
        public ActionResult PrintMTC(
            int id)
        {
            try
            {
                RMMTCHeaderBLL header =
                    repo.GetHeader(id);

                List<RMMTCDetailBLL> details =
                    repo.GetDetails(id);

                if (header == null)
                    throw new Exception("Selected MTC was not found.");

                if (details == null || details.Count == 0)
                    throw new Exception("Add at least one heat before printing.");

                string templatePath =
                    Server.MapPath(
                        "~/Templates/MTC Form.xlsx"
                    );

                if (!System.IO.File.Exists(templatePath))
                {
                    templatePath =
                        Server.MapPath(
                            "~/Templates/1- MTC -TEMPELETE (1).xlsx"
                        );
                }

                string gradeLimitsPath =
                    Server.MapPath(
                        "~/Templates/Grades Limits.xlsx"
                    );

                byte[] bytes =
                    new RMMTCExcelBuilder()
                        .Build(
                            templatePath,
                            gradeLimitsPath,
                            header,
                            details,
                            CurrentUser()
                        );

                string fileName =
                    (header.MTCNo ?? "MTC")
                    + ".xlsx";

                return File(
                    bytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName
                );
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] =
                    "Unable to print MTC. " +
                    ex.Message;

                return RedirectToAction(
                    "Index",
                    new { id = id }
                );
            }
        }


        private static string Normalize(string value)
        {
            return (value ?? string.Empty).Trim().ToUpperInvariant();
        }

        private string CurrentUser()
        {
            return
                User != null &&
                User.Identity != null
                    ? User.Identity.Name
                    : "";
        }
    }
}
