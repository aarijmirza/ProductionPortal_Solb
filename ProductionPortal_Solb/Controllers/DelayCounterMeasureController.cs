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

        public ActionResult Add(
            int plantDelayID)
        {
            if (plantDelayID <= 0)
            {
                TempData["Error"] =
                    "Invalid Plant Delay ID.";

                return RedirectToAction(
                    "list",
                    "Maintenance"
                );
            }

            DelayCounterMeasureVM model =
                repo.GetPageData(
                    plantDelayID
                );

            if (model == null)
            {
                TempData["Error"] =
                    "Failure Analysis record not found.";

                return RedirectToAction(
                    "list",
                    "Maintenance"
                );
            }

            FailureAnalysisBLL analysis =
                repo
                    .GetFailureAnalysisByDelayID(
                        plantDelayID
                    );

            ViewBag.Analysis =
                analysis;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveMultiple(
            DelayCounterMeasureVM model)
        {
            try
            {
                string createdBy =
                    Convert.ToString(
                        Session["UserName"]
                    );

                int savedRecords =
                    repo.SaveMultiple(
                        model.PlantDelayID,
                        model.CounterMeasures,
                        createdBy
                    );

                TempData["Success"] =
                    savedRecords +
                    " countermeasure(s) saved successfully.";

                return RedirectToAction(
                    "Add",
                    new
                    {
                        plantDelayID =
                            model.PlantDelayID
                    }
                );
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    ex.Message;

                return RedirectToAction(
                    "Add",
                    new
                    {
                        plantDelayID =
                            model.PlantDelayID
                    }
                );
            }
        }

        //[HttpGet]
        //public JsonResult GetByID(
        //    int id)
        //{
        //    DelayCounterMeasureBLL model =
        //        repo.GetByID(id);

        //    if (model == null)
        //    {
        //        return Json(
        //            new
        //            {
        //                success = false,
        //                message =
        //                    "Record not found."
        //            },
        //            JsonRequestBehavior.AllowGet
        //        );
        //    }

        //    return Json(
        //        new
        //        {
        //            success = true,
        //            data = new
        //            {
        //                model.ID,
        //                model.PlantDelayID,
        //                model.CounterMeasure,
        //                model.SAPOrderNo,
        //                model.Responsible,

        //                TargetDate =
        //                    model.TargetDate
        //                        .HasValue
        //                            ? model.TargetDate
        //                                .Value
        //                                .ToString(
        //                                    "yyyy-MM-dd"
        //                                )
        //                            : "",

        //                model.EvidenceForCompletion,
        //                model.CounterMeasureStatus,
        //                model.ReasonForNotClosing
        //            }
        //        },
        //        JsonRequestBehavior.AllowGet
        //    );
        //}

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

        //[HttpPost]
        //public JsonResult Delete(
        //    int id)
        //{
        //    string updatedBy =
        //        Convert.ToString(
        //            Session["UserID"]
        //        );

        //    bool result =
        //        repo.Delete(
        //            id,
        //            updatedBy
        //        );

        //    return Json(
        //        new
        //        {
        //            success = result,
        //            message = result
        //                ? "Countermeasure deleted."
        //                : "Record could not be deleted."
        //        }
        //    );
        //}
    }
}