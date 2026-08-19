using BAL.Repositories;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProductionPortal_Solb.Controllers
{
    public class RMMechanicalController : Controller
    {
        RMMechanicalRepository repo = new RMMechanicalRepository();

        [HttpGet]
        public ActionResult QCInspectionRMadd(
            string rm = "RM1",
            int? inspectionID = null,
            int? boardingID = null,
            string mtcHeatNo = null)
        {
            var model =
                new QCInspectionRMPageVM();

            model.SelectedRM =
                NormalizeRM(rm);

            model.BilletBoardingRows =
                repo.GetBilletBoardingRows(
                    model.SelectedRM
                )
                ?? new List<QCBilletBoardingRowBLL>();

            model.MTCRows =
                repo.GetMTCRows(
                    mtcHeatNo
                )
                ?? new List<QCMTCRowBLL>();


            if (
                inspectionID.HasValue &&
                inspectionID.Value > 0
            )
            {
                model.Detail =
                    repo.GetQCInspectionRMByID(
                        inspectionID.Value
                    )
                    ?? new QCInspectionRMDetailBLL();

                model.Detail.ID =
                    inspectionID.Value;
            }
            else if (
                boardingID.HasValue &&
                boardingID.Value > 0
            )
            {
                model.Detail =
                    repo.GetQCInspectionRMFromBoarding(
                        boardingID.Value
                    )
                    ?? new QCInspectionRMDetailBLL();
            }
            else
            {
                model.Detail =
                    new QCInspectionRMDetailBLL
                    {
                        ProductionDate =
                            DateTime.Today.ToString(
                                "dd-MM-yyyy"
                            ),

                        ProductionDateValue =
                            DateTime.Today,

                        ProductionShift =
                            "Morning",

                        GaugeLength =
                            "200",

                        GaugeLengthValue =
                            200M,

                        YieldStrength =
                            "0.0",

                        YieldStrengthValue =
                            0M,

                        TensileStrength =
                            "0.0",

                        TensileStrengthValue =
                            0M,

                        TensileYieldRatio =
                            "0.0",

                        TensileYieldRatioValue =
                            0M,

                        Elongation =
                            "0.0",

                        ElongationValue =
                            0M
                    };
            }


            return View(
                "~/Views/Quality/RMMechanical/add.cshtml",
                model
            );
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult QCInspectionRMadd(
            QCInspectionRMDetailBLL model)
        {
            if (model == null)
            {
                TempData["ErrorMessage"] =
                    "Invalid QC inspection data.";

                return RedirectToAction(
                    "QCInspectionRMadd"
                );
            }

            try
            {
                model.HeatNo =
                    (model.HeatNo ?? string.Empty).Trim();

                if (string.IsNullOrWhiteSpace(model.HeatNo))
                {
                    TempData["ErrorMessage"] =
                        "Please select a Billet Boarding or MTC heat before saving.";

                    return RedirectToAction(
                        "QCInspectionRMadd",
                        new
                        {
                            rm = NormalizeRM(model.Site)
                        }
                    );
                }

                model.Site =
                    NormalizeRM(model.Site);

                /*
                 * When an MTC row is selected, its ID is the saved
                 * QCInspectionRM identity and must be updated, not inserted.
                 */
                if (model.ID <= 0 && model.MTCID > 0)
                {
                    model.ID = model.MTCID;
                }

                bool isUpdate =
                    model.ID > 0;

                /*
                 * Chemistry fields are readonly in View.
                 * They are loaded from RMChemicalAnalysis and are NOT
                 * treated as manually entered QCInspectionRM chemistry.
                 */

                model.CreatedBy =
                    User != null &&
                    User.Identity != null
                        ? User.Identity.Name
                        : "";

                model.CreatedDate =
                    DateTime.Now;


                int savedID =
                    repo.SaveQCInspectionRM(
                        model
                    );


                if (savedID <= 0)
                {
                    TempData["ErrorMessage"] =
                        "QC inspection record was not saved.";

                    return RedirectToAction(
                        "QCInspectionRMadd",
                        new
                        {
                            rm = NormalizeRM(model.Site)
                        }
                    );
                }


                TempData["SuccessMessage"] =
                    isUpdate
                        ? "QC inspection data updated successfully."
                        : "QC inspection data saved successfully.";


                return RedirectToAction(
                    "QCInspectionRMadd",
                    new
                    {
                        inspectionID =
                            savedID,

                        rm = NormalizeRM(model.Site)
                    }
                );
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] =
                    "Unable to save QC inspection record. "
                    +
                    ex.Message;

                return RedirectToAction(
                    "QCInspectionRMadd",
                    new
                    {
                        inspectionID =
                            model.ID > 0
                                ? (int?)model.ID
                                : null,

                        rm = NormalizeRM(model.Site)
                    }
                );
            }
        }



        [HttpGet]
        public JsonResult GetBoardingDetails(
            int boardingID)
        {
            try
            {
                if (boardingID <= 0)
                {
                    Response.StatusCode =
                        400;

                    return Json(
                        new
                        {
                            success =
                                false,

                            message =
                                "A valid billet boarding record must be selected."
                        },
                        JsonRequestBehavior.AllowGet
                    );
                }


                var data =
                    repo.GetQCInspectionRMFromBoarding(
                        boardingID
                    );


                return Json(
                    new
                    {
                        success =
                            data != null,

                        data =
                            data,

                        message =
                            data == null
                                ? "Billet boarding details were not found."
                                : ""
                    },
                    JsonRequestBehavior.AllowGet
                );
            }
            catch (Exception ex)
            {
                Response.StatusCode =
                    500;

                return Json(
                    new
                    {
                        success =
                            false,

                        message =
                            "Billet boarding details could not be loaded. "
                            +
                            ex.Message
                    },
                    JsonRequestBehavior.AllowGet
                );
            }
        }



        [HttpGet]
        public JsonResult GetMTCRowsJson(
            string heatNo = null)
        {
            try
            {
                var rows =
                    repo.GetMTCRows(
                        heatNo
                    )
                    ?? new List<QCMTCRowBLL>();


                /*
                 * IMPORTANT:
                 * Same HeatNo must appear only once in MTC grid.
                 * Latest QCInspectionRM.ID wins.
                 */
                rows =
                    rows
                        .Where(
                            x =>
                                x != null
                                &&
                                !string.IsNullOrWhiteSpace(
                                    x.HeatNo
                                )
                        )
                        .GroupBy(
                            x =>
                                x.HeatNo.Trim(),
                            StringComparer.OrdinalIgnoreCase
                        )
                        .Select(
                            g =>
                                g
                                    .OrderByDescending(
                                        x => x.ID
                                    )
                                    .First()
                        )
                        .OrderByDescending(
                            x => x.ID
                        )
                        .ToList();


                return Json(
                    new
                    {
                        success =
                            true,

                        data =
                            rows
                    },
                    JsonRequestBehavior.AllowGet
                );
            }
            catch (Exception ex)
            {
                Response.StatusCode =
                    500;

                return Json(
                    new
                    {
                        success =
                            false,

                        message =
                            "MTC data could not be loaded. "
                            +
                            ex.Message
                    },
                    JsonRequestBehavior.AllowGet
                );
            }
        }



        [HttpGet]
        public JsonResult GetMTCDetails(
            int mtcID)
        {
            try
            {
                if (mtcID <= 0)
                {
                    Response.StatusCode =
                        400;

                    return Json(
                        new
                        {
                            success =
                                false,

                            message =
                                "A valid MTC record must be selected."
                        },
                        JsonRequestBehavior.AllowGet
                    );
                }


                /*
                 * One repository call:
                 *
                 * QCInspectionRM
                 *   -> Product + Mechanical
                 *
                 * RMChemicalAnalysis
                 *   -> C, Si, Mn, P, S, N, Ceq
                 */
                var data =
                    repo.GetMTCDetails(
                        mtcID
                    );


                if (data == null)
                {
                    Response.StatusCode =
                        404;

                    return Json(
                        new
                        {
                            success =
                                false,

                            message =
                                "The selected MTC record was not found."
                        },
                        JsonRequestBehavior.AllowGet
                    );
                }


                return Json(
                    new
                    {
                        success =
                            true,

                        data =
                            data
                    },
                    JsonRequestBehavior.AllowGet
                );
            }
            catch (Exception ex)
            {
                Response.StatusCode =
                    500;

                return Json(
                    new
                    {
                        success =
                            false,

                        message =
                            "The selected MTC data could not be loaded. "
                            +
                            ex.Message
                    },
                    JsonRequestBehavior.AllowGet
                );
            }
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteQCInspectionRM(
            int id)
        {
            try
            {
                if (id <= 0)
                {
                    TempData["ErrorMessage"] =
                        "A valid QC inspection record must be selected.";

                    return RedirectToAction(
                        "QCInspectionRMadd"
                    );
                }


                int affected =
                    repo.DeleteQCInspectionRM(
                        id,
                        User != null &&
                        User.Identity != null
                            ? User.Identity.Name
                            : ""
                    );


                TempData[
                    affected > 0
                        ? "SuccessMessage"
                        : "ErrorMessage"
                ] =
                    affected > 0
                        ? "QC inspection record deleted successfully."
                        : "QC inspection record was not deleted.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] =
                    "Unable to delete QC inspection record. "
                    +
                    ex.Message;
            }


            return RedirectToAction(
                "QCInspectionRMadd"
            );
        }


        private static string NormalizeRM(
            string rm)
        {
            string value =
                (rm ?? string.Empty).Trim();

            return
                string.Equals(
                    value,
                    "RM2",
                    StringComparison.OrdinalIgnoreCase
                )
                ||
                string.Equals(
                    value,
                    "Rolling Mill 2",
                    StringComparison.OrdinalIgnoreCase
                )
                ? "RM2"
                : "RM1";
        }
    }
}