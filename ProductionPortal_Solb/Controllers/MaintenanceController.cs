using BAL.Repositories;
using DAL.Models;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProductionPortal_Solb.Controllers
{
    public class MaintenanceController : Controller
    {
        DelayRespository repo = new DelayRespository();
        public MaintenanceController()
        {
            repo = new DelayRespository();
        }
        // GET: Maintenance
        public ActionResult list(
            DateTime? fromDate,
            DateTime? toDate,
            string plant)
        {
            DateTime startDate = fromDate ?? DateTime.Today;
            DateTime endDate = toDate ?? DateTime.Today;

            // Invalid date range protection
            if (startDate.Date > endDate.Date)
            {
                DateTime temp = startDate;
                startDate = endDate;
                endDate = temp;
            }

            plant = string.IsNullOrWhiteSpace(plant)
                ? null
                : plant.Trim();

            var model = repo.GetMaintenanceRecords(
                startDate.Date,
                endDate.Date,
                plant
            );

            ViewBag.FromDate = startDate.ToString("yyyy-MM-dd");
            ViewBag.ToDate = endDate.ToString("yyyy-MM-dd");
            ViewBag.Plant = plant ?? "";

            return View(model);
        }

        //public ActionResult detail()
        //{
        //    return View();
        //}

        public ActionResult detail(int id)
        {
            try
            {
                if (id <= 0)
                {
                    TempData["ErrorMessage"] = "Invalid record ID.";
                    return RedirectToAction("list");
                }

                var model = repo.GetDelayByID(id);

                if (model == null)
                {
                    TempData["ErrorMessage"] = "Record not found.";
                    return RedirectToAction("list");
                }

                // Existing analysis against this Delay ID
                //var analysis = repo.GetMaintenanceAnalysisByDelayID(id);

                //ViewBag.Analysis = analysis;
                ViewBag.Analysis = repo.GetMaintenanceAnalysisByDelayID(id);
                ViewBag.Actions = repo.GetFailureAnalysisActionsByDelayID(id);

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error: " + ex.Message;
                return RedirectToAction("list");
            }
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult InsertMaintenanceAnalysis(FailureAnalysisBLL model)
        //{
        //    try
        //    {
        //        if (model == null)
        //        {
        //            TempData["ErrorMessage"] = "Invalid data.";
        //            return RedirectToAction("list");
        //        }

        //        if (model.DelayID <= 0)
        //        {
        //            TempData["ErrorMessage"] = "Invalid Delay ID.";
        //            return RedirectToAction("list");
        //        }

        //        // Auto Generate Analysis Code from Controller
        //        model.AnalysisCode = repo.GenerateAnalysisCode();

        //        model.StatusID = 1;
        //        model.CreatedBy = User.Identity.Name;
        //        model.CreatedDate = DateTime.Now;

        //        int result = repo.InsertMaintenanceAnalysis(model);

        //        if (result < 0)
        //        {
        //            TempData["SuccessMessage"] = "Maintenance analysis saved successfully. Analysis Code: " + model.AnalysisCode;
        //        }
        //        else
        //        {
        //            TempData["ErrorMessage"] = "Maintenance analysis not saved.";
        //        }

        //        return RedirectToAction("detail", new { id = model.DelayID });
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["ErrorMessage"] = "Error: " + ex.Message;
        //        return RedirectToAction("detail", new { id = model.DelayID });
        //    }
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateDelayCorrection(int DelayID, string DelayDescription1, string ReasonforOccurence1, string ActionTaken1)
        {
            try
            {
                if (DelayID <= 0)
                {
                    TempData["ErrorMessage"] = "Invalid delay record.";
                    return RedirectToAction("list");
                }

                var model = new PlantDelayBLL
                {
                    ID = DelayID,
                    DelayDescription1 = DelayDescription1,
                    ReasonForOccurence1 = ReasonforOccurence1,
                    ActionTaken1 = ActionTaken1,
                    UpdatedBy = User.Identity.Name,
                    UpdatedDate = DateTime.Now
                };

                int result = repo.UpdateDelayCorrection(model);

                if (result < 0)
                {
                    TempData["SuccessMessage"] = "Delay remarks updated successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Delay remarks not updated.";
                }

                return RedirectToAction("detail", new { id = DelayID });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error: " + ex.Message;
                return RedirectToAction("detail", new { id = DelayID });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InsertMaintenanceAnalysis(FailureAnalysisBLL model)
        {
            try
            {
                if (model == null || model.DelayID <= 0)
                {
                    TempData["Error"] = "Invalid maintenance analysis request.";
                    return RedirectToAction("list");
                }

                string currentUser = Convert.ToString(Session["UserName"]);

                if (string.IsNullOrWhiteSpace(currentUser))
                    currentUser = User.Identity.Name;

                if (string.IsNullOrWhiteSpace(currentUser))
                    currentUser = "System";

                model.StatusID = 1;

                int result;

                if (model.ID > 0)
                {
                    // Existing record: only missing information update hogi
                    model.UpdatedBy = currentUser;
                    model.UpdatedDate = DateTime.Now;

                    result = repo.UpdateMissingMaintenanceAnalysis(model);

                    TempData["Success"] = result > 0
                        ? "Missing analysis information updated successfully."
                        : "No new information was available to update.";
                }
                else
                {
                    // New analysis record
                    model.CreatedBy = currentUser;
                    model.CreatedDate = DateTime.Now;

                    result = repo.InsertMaintenanceAnalysis(model);

                    TempData["Success"] = result > 0
                        ? "Maintenance analysis saved successfully."
                        : "Maintenance analysis could not be saved.";
                }

                return RedirectToAction("detail", new { ID = model.DelayID });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Unable to save maintenance analysis: " + ex.Message;

                return RedirectToAction("detail", new { ID = model.DelayID });
            }
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult InsertMaintenanceAnalysis(
        //    FailureAnalysisBLL model,
        //    string[] IncreaseMTBFAction,
        //    string[] DecreaseMTTRAction)
        //{
        //    try
        //    {
        //        if (model == null)
        //        {
        //            TempData["ErrorMessage"] = "Invalid data.";
        //            return RedirectToAction("list");
        //        }

        //        if (model.DelayID <= 0)
        //        {
        //            TempData["ErrorMessage"] = "Invalid Delay ID.";
        //            return RedirectToAction("list");
        //        }

        //        int analysisID = 0;
        //        int result = 0;

        //        // New analysis
        //        if (model.ID <= 0)
        //        {
        //            model.AnalysisCode = repo.GenerateAnalysisCode();
        //            model.StatusID = 1;
        //            model.CreatedBy = User.Identity.Name;
        //            model.CreatedDate = DateTime.Now;

        //            result = repo.InsertMaintenanceAnalysis(model);

        //            // Agar aapka insert SP ID return nahi karta to analysisID 0 rahega.
        //            // Actions DelayID ke against save ho jayengi.
        //            analysisID = model.ID;
        //        }
        //        else
        //        {
        //            // Existing analysis update case
        //            model.UpdatedBy = User.Identity.Name;
        //            model.UpdatedDate = DateTime.Now;

        //            //result = repo.UpdateMaintenanceAnalysis(model);
        //            analysisID = model.ID;
        //        }

        //        if (result < 0)
        //        {
        //            SaveMtbfMttrActions((int)model.DelayID, analysisID, IncreaseMTBFAction, DecreaseMTTRAction);

        //            TempData["SuccessMessage"] = "Maintenance analysis saved successfully.";
        //        }
        //        else
        //        {
        //            TempData["ErrorMessage"] = "Maintenance analysis not saved.";
        //        }

        //        return RedirectToAction("detail", new { id = model.DelayID });
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["ErrorMessage"] = "Error: " + ex.Message;

        //        int delayID = (int)(model != null ? model.DelayID : 0);

        //        if (delayID > 0)
        //            return RedirectToAction("detail", new { id = delayID });

        //        return RedirectToAction("list");
        //    }
        //}

        private void SaveMtbfMttrActions(
    int delayID,
    int analysisID,
    string[] increaseMTBFActions,
    string[] decreaseMTTRActions)
        {
            if (delayID <= 0)
                return;

            if (increaseMTBFActions != null)
            {
                foreach (string remarks in increaseMTBFActions)
                {
                    if (!string.IsNullOrWhiteSpace(remarks))
                    {
                        var action = new FailureAnalysisActionBLL
                        {
                            ActionCode = repo.GenerateFailureActionCode(),
                            DelayID = delayID,
                            AnalysisID = analysisID > 0 ? (int?)analysisID : null,
                            ActionType = "IncreaseMTBF",
                            ActionRemarks = remarks.Trim(),
                            StatusID = 1,
                            CreatedBy = User.Identity.Name,
                            CreatedDate = DateTime.Now
                        };

                        repo.InsertFailureAnalysisAction(action);
                    }
                }
            }

            if (decreaseMTTRActions != null)
            {
                foreach (string remarks in decreaseMTTRActions)
                {
                    if (!string.IsNullOrWhiteSpace(remarks))
                    {
                        var action = new FailureAnalysisActionBLL
                        {
                            ActionCode = repo.GenerateFailureActionCode(),
                            DelayID = delayID,
                            AnalysisID = analysisID > 0 ? (int?)analysisID : null,
                            ActionType = "DecreaseMTTR",
                            ActionRemarks = remarks.Trim(),
                            StatusID = 1,
                            CreatedBy = User.Identity.Name,
                            CreatedDate = DateTime.Now
                        };

                        repo.InsertFailureAnalysisAction(action);
                    }
                }
            }
        }

    }
}