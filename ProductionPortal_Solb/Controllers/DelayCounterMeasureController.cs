using BAL.Repositories;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static DAL.Models.UtilityDailyReportVM;

namespace ProductionPortal_Solb.Controllers
{
    public class DelayCounterMeasureController : Controller
    {
        private readonly
            DelayCounterMeasureRepository repo =
                new DelayCounterMeasureRepository();

        private readonly
            DelayRespository maintenanceRepo =
                new DelayRespository();

        [HttpGet]
        public ActionResult add(int plantDelayID)
        {
            var delay =
                maintenanceRepo
                    .GetDelayByID(
                        plantDelayID
                    );

            if (delay == null)
            {
                return HttpNotFound();
            }

            var model =
                new DelayCounterMeasureVM
                {
                    PlantDelayID =
                        plantDelayID,

                    DelayDetail =
                        delay,

                    ExistingCounterMeasures =
                        repo.GetByPlantDelayID(
                            plantDelayID
                        ),

                    CounterMeasures =
                        new List<DelayCounterMeasureBLL>
                        {
                    new DelayCounterMeasureBLL
                    {
                        PlantDelayID =
                            plantDelayID,

                        CounterMeasureStatus =
                            "Open"
                    }
                        }
                };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveMultiple(
            DelayCounterMeasureVM model)
        {
            if (model == null ||
                model.PlantDelayID <= 0)
            {
                TempData["Error"] =
                    "Invalid delay record.";

                return RedirectToAction(
                    "Index",
                    new
                    {
                        plantDelayID =
                            model?.PlantDelayID ?? 0
                    }
                );
            }

            if (model.CounterMeasures == null ||
                model.CounterMeasures.Count == 0)
            {
                TempData["Error"] =
                    "Please add at least one countermeasure.";

                return RedirectToAction(
                    "Index",
                    new
                    {
                        plantDelayID =
                            model.PlantDelayID
                    }
                );
            }

            string createdBy =
                Convert.ToString(
                    Session["UserID"]
                );

            int savedCount = 0;

            foreach (var item in
                model.CounterMeasures)
            {
                if (string.IsNullOrWhiteSpace(
                    item.CounterMeasure))
                {
                    continue;
                }

                item.PlantDelayID =
                    model.PlantDelayID;

                item.CreatedBy =
                    createdBy;

                item.CounterMeasureStatus =
                    string.IsNullOrWhiteSpace(
                        item.CounterMeasureStatus
                    )
                        ? "Open"
                        : item.CounterMeasureStatus;

                int newID =
                    repo.Insert(item);

                if (newID > 0)
                {
                    savedCount++;
                }
            }

            TempData[savedCount > 0
                ? "Success"
                : "Error"] =
                savedCount > 0
                    ? savedCount +
                      " countermeasure(s) saved successfully."
                    : "No countermeasure was saved.";

            return RedirectToAction(
                "Index",
                new
                {
                    plantDelayID =
                        model.PlantDelayID
                }
            );
        }

        [HttpGet]
        public JsonResult GetByID(
            int id)
        {
            DelayCounterMeasureBLL model =
                repo.GetByID(id);

            if (model == null)
            {
                return Json(
                    new
                    {
                        success = false,
                        message =
                            "Record not found."
                    },
                    JsonRequestBehavior.AllowGet
                );
            }

            return Json(
                new
                {
                    success = true,
                    data = new
                    {
                        model.ID,
                        model.PlantDelayID,
                        model.CounterMeasure,
                        model.SAPOrderNo,
                        model.Responsible,

                        TargetDate =
                            model.TargetDate
                                .HasValue
                                    ? model.TargetDate
                                        .Value
                                        .ToString(
                                            "yyyy-MM-dd"
                                        )
                                    : "",

                        model.EvidenceForCompletion,
                        model.CounterMeasureStatus,
                        model.ReasonForNotClosing
                    }
                },
                JsonRequestBehavior.AllowGet
            );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(
            DelayCounterMeasureBLL model)
        {
            model.UpdatedBy =
                Convert.ToString(
                    Session["UserID"]
                );

            bool result =
                repo.Update(model);

            TempData[result
                ? "Success"
                : "Error"] =
                result
                    ? "Countermeasure updated successfully."
                    : "Countermeasure could not be updated.";

            return RedirectToAction(
                "add",
                new
                {
                    plantDelayID =
                        model.PlantDelayID
                }
            );
        }

        [HttpPost]
        public JsonResult Delete(
            int id)
        {
            string updatedBy =
                Convert.ToString(
                    Session["UserID"]
                );

            bool result =
                repo.Delete(
                    id,
                    updatedBy
                );

            return Json(
                new
                {
                    success = result,
                    message = result
                        ? "Countermeasure deleted."
                        : "Record could not be deleted."
                }
            );
        }
    }
}