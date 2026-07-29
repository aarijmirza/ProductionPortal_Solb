using BAL.Repositories;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ProductionPortal_Solb.Controllers
{
    public class DelayCounterMeasureController : Controller
    {
        private readonly DelayCounterMeasureRepository repo =
            new DelayCounterMeasureRepository();

        private readonly DelayRespository maintenanceRepo =
            new DelayRespository();

        [HttpGet]
        public ActionResult Add(int plantDelayID)
        {
            if (plantDelayID <= 0)
            {
                TempData["Error"] = "Invalid Plant Delay ID.";

                return RedirectToAction("list", "Maintenance");
            }

            DelayCounterMeasureVM model =
                repo.GetPageData(plantDelayID);

            if (model == null)
            {
                TempData["Error"] = "Delay record not found.";

                return RedirectToAction("list", "Maintenance");
            }

            model.PlantDelayID = plantDelayID;

            if (model.CounterMeasures == null)
            {
                model.CounterMeasures = new List<DelayCounterMeasureBLL>();
            }

            if (model.ExistingCounterMeasures == null)
            {
                model.ExistingCounterMeasures = new List<DelayCounterMeasureBLL>();
            }

            FailureAnalysisBLL analysis =
                repo.GetFailureAnalysisByDelayID(plantDelayID);

            if (analysis == null)
            {
                ViewBag.AnalysisMessage =
                    "Failure Analysis data not found against this delay.";
            }

            ViewBag.Analysis = analysis;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveMultiple(DelayCounterMeasureVM model)
        {
            try
            {
                if (model == null || model.PlantDelayID <= 0)
                {
                    TempData["Error"] = "Invalid countermeasure data.";

                    return RedirectToAction("list", "Maintenance");
                }

                if (model.CounterMeasures == null ||
                    !model.CounterMeasures.Any(x =>
                        !string.IsNullOrWhiteSpace(x.CounterMeasure)))
                {
                    TempData["Error"] =
                        "Please enter at least one countermeasure.";

                    return RedirectToAction(
                        "Add",
                        new
                        {
                            plantDelayID = model.PlantDelayID
                        }
                    );
                }

                List<DelayCounterMeasureBLL> validCounterMeasures =
                    model.CounterMeasures
                        .Where(x => !string.IsNullOrWhiteSpace(x.CounterMeasure))
                        .ToList();

                string createdBy =
                    Convert.ToString(Session["UserName"]);

                if (string.IsNullOrWhiteSpace(createdBy))
                {
                    createdBy = Convert.ToString(Session["UserID"]);
                }

                if (string.IsNullOrWhiteSpace(createdBy))
                {
                    createdBy = User.Identity.Name;
                }

                int savedRecords =
                    repo.SaveMultiple(
                        model.PlantDelayID,
                        validCounterMeasures,
                        createdBy
                    );

                TempData["Success"] =
                    savedRecords + " countermeasure(s) saved successfully.";

                return RedirectToAction(
                    "Add",
                    new
                    {
                        plantDelayID = model.PlantDelayID
                    }
                );
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;

                return RedirectToAction(
                    "Add",
                    new
                    {
                        plantDelayID =
                            model != null
                                ? model.PlantDelayID
                                : 0
                    }
                );
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(DelayCounterMeasureBLL model)
        {
            if (model == null || model.ID <= 0)
            {
                TempData["Error"] = "Invalid countermeasure record.";

                return RedirectToAction("list", "Maintenance");
            }

            model.UpdatedBy =
                Convert.ToString(Session["UserID"]);

            if (string.IsNullOrWhiteSpace(model.UpdatedBy))
            {
                model.UpdatedBy = User.Identity.Name;
            }

            bool result =
                repo.Update(model);

            TempData[result ? "Success" : "Error"] =
                result
                    ? "Countermeasure updated successfully."
                    : "Countermeasure could not be updated.";

            return RedirectToAction(
                "Add",
                new
                {
                    plantDelayID = model.PlantDelayID
                }
            );
        }
    }
}