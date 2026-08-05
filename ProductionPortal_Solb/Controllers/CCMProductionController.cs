using BAL.Repositories;
using DAL.Models;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.SessionState;

namespace ProductionPortal_Solb.Controllers
{
    [SessionState(
        SessionStateBehavior.ReadOnly
    )]
    public class CCMProductionController :
        Controller
    {
        private readonly
            CCMDailyProductionRepository repo;

        public CCMProductionController()
        {
            repo =
                new CCMDailyProductionRepository();
        }

        #region Add / Edit

        [HttpGet]
        public ActionResult Add(
            int? id)
        {
            CCMDailyProductionReportBLL model;

            if (id.HasValue &&
                id.Value > 0)
            {
                model =
                    repo.GetByID(
                        id.Value
                    );

                if (model == null)
                {
                    TempData["Error"] =
                        "Billet Yard report not found.";

                    return RedirectToAction(
                        "List"
                    );
                }
            }
            else
            {
                model =
                    new CCMDailyProductionReportBLL();
            }

            EnsureAtLeastOneDetailRow(
                model
            );

            LoadDropdowns(
                model.Shift
            );

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(
            CCMDailyProductionReportBLL model)
        {
            if (model == null)
            {
                model =
                    new CCMDailyProductionReportBLL();
            }

            LoadDropdowns(
                model.Shift
            );

            try
            {
                NormalizeDetailRows(model);
                ValidateReport(model);

                if (!ModelState.IsValid)
                {
                    EnsureAtLeastOneDetailRow(
                        model
                    );

                    return View(model);
                }

                model.CreatedBy =
                    GetCurrentUser();

                int savedID =
                    repo.Save(model);

                TempData["Success"] =
                    model.ID > 0
                        ? "Billet Yard report updated successfully."
                        : "Billet Yard report saved successfully.";

                return RedirectToAction(
                    "Add",
                    new
                    {
                        id = savedID
                    }
                );
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(
                    "",
                    ex.Message
                );

                EnsureAtLeastOneDetailRow(
                    model
                );

                LoadDropdowns(
                    model.Shift
                );

                return View(model);
            }
        }

        #endregion

        #region List

        [HttpGet]
        public ActionResult List(
            DateTime? fromDate,
            DateTime? toDate,
            string shift)
        {
            DateTime startDate =
                fromDate ?? DateTime.Today;

            DateTime endDate =
                toDate ?? DateTime.Today;

            if (endDate < startDate)
            {
                endDate = startDate;
            }

            ViewBag.FromDate =
                startDate.ToString(
                    "yyyy-MM-dd"
                );

            ViewBag.ToDate =
                endDate.ToString(
                    "yyyy-MM-dd"
                );

            ViewBag.Shift =
                shift;

            List<
                CCMDailyProductionReportBLL
            > list =
                repo.GetAll(
                    startDate,
                    endDate,
                    shift
                );

            return View(list);
        }

        #endregion

        #region Details

        [HttpGet]
        public ActionResult Details(
            int id)
        {
            CCMDailyProductionReportBLL model =
                repo.GetByID(id);

            if (model == null)
            {
                return HttpNotFound();
            }

            return View(model);
        }

        #endregion

        #region Delete

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(
            int id)
        {
            try
            {
                bool deleted =
                    repo.Delete(
                        id,
                        GetCurrentUser()
                    );

                return Json(
                    new
                    {
                        success = deleted,

                        message =
                            deleted
                                ? "Report deleted successfully."
                                : "Unable to delete report."
                    }
                );
            }
            catch (Exception ex)
            {
                return Json(
                    new
                    {
                        success = false,
                        message = ex.Message
                    }
                );
            }
        }

        #endregion

        #region PDF

        [HttpGet]
        public ActionResult DownloadPdf(
            int id)
        {
            CCMDailyProductionReportBLL model =
                repo.GetByID(id);

            if (model == null)
            {
                return HttpNotFound();
            }

            string safeShift =
                string.IsNullOrWhiteSpace(
                    model.Shift
                )
                    ? "NoShift"
                    : model.Shift.Replace(
                        " ",
                        "_"
                    );

            string fileName =
                "CCM_Daily_Production_" +
                model.ReportDate.ToString(
                    "yyyyMMdd"
                ) +
                "_" +
                safeShift +
                ".pdf";

            return new ViewAsPdf(
                "PdfReport",
                model
            )
            {
                FileName =
                    fileName,

                PageSize =
                    Rotativa.Options.Size.A4,

                PageOrientation =
                    Rotativa.Options
                        .Orientation
                        .Landscape,

                PageMargins =
                    new Rotativa.Options.Margins
                    {
                        Top = 8,
                        Bottom = 8,
                        Left = 8,
                        Right = 8
                    },

                CustomSwitches =
                    "--print-media-type " +
                    "--disable-smart-shrinking " +
                    "--encoding utf-8"
            };
        }

        #endregion

        #region Private Methods

        private void NormalizeDetailRows(
            CCMDailyProductionReportBLL model)
        {
            if (model.Details == null)
            {
                model.Details =
                    new List<
                        CCMDailyProductionReportDetailBLL
                    >();

                return;
            }

            model.Details =
                model.Details
                    .Where(
                        x =>
                            x != null &&
                            IsDetailRowEntered(x)
                    )
                    .ToList();

            for (
                int index = 0;
                index < model.Details.Count;
                index++)
            {
                model.Details[index]
                    .SequenceNo =
                        index + 1;
            }
        }

        private bool IsDetailRowEntered(
            CCMDailyProductionReportDetailBLL row)
        {
            return
                !string.IsNullOrWhiteSpace(
                    row.HeatNo
                ) ||
                !string.IsNullOrWhiteSpace(
                    row.Grade
                ) ||
                row.Billet14M > 0 ||
                row.Billet13M > 0 ||
                row.Billet12M > 0 ||
                row.Billet11M > 0 ||
                row.GoodBillets > 0 ||
                row.ShortBillets > 0 ||
                row.Bend > 0 ||
                row.TotalBillets > 0 ||
                (row.TotalLength ?? 0M) > 0 ||
                (row.ShortBilletTotalLength ?? 0M) > 0 ||
                (row.ShortBilletAvgLength ?? 0M) > 0 ||
                (row.PerCoilBundleWeight ?? 0M) > 0 ||
                (row.PrimeBilletWeight ?? 0M) > 0 ||
                (row.ShortBilletWeight ?? 0M) > 0 ||
                (row.TotalWeight ?? 0M) > 0 ||
                !string.IsNullOrWhiteSpace(
                    row.Remarks
                );
        }

        private void ValidateReport(
            CCMDailyProductionReportBLL model)
        {
            if (model.ReportDate ==
                DateTime.MinValue)
            {
                ModelState.AddModelError(
                    "ReportDate",
                    "Report date is required."
                );
            }

            if (string.IsNullOrWhiteSpace(
                model.ReportNo))
            {
                ModelState.AddModelError(
                    "ReportNo",
                    "Report number is required."
                );
            }

            if (string.IsNullOrWhiteSpace(
                model.Shift))
            {
                ModelState.AddModelError(
                    "Shift",
                    "Shift is required."
                );
            }

            if (model.Details == null ||
                model.Details.Count == 0)
            {
                ModelState.AddModelError(
                    "",
                    "Please enter at least one production row."
                );

                return;
            }

            for (
                int index = 0;
                index < model.Details.Count;
                index++)
            {
                CCMDailyProductionReportDetailBLL row =
                    model.Details[index];

                if (row.GoodBillets < 0)
                {
                    ModelState.AddModelError(
                        "Details[" +
                        index +
                        "].GoodBillets",

                        "Good billets cannot be negative."
                    );
                }

                if (row.ShortBillets < 0)
                {
                    ModelState.AddModelError(
                        "Details[" +
                        index +
                        "].ShortBillets",

                        "Short billets cannot be negative."
                    );
                }

                if (row.TotalBillets < 0)
                {
                    ModelState.AddModelError(
                        "Details[" +
                        index +
                        "].TotalBillets",

                        "Total billets cannot be negative."
                    );
                }

                if (row.GoodBillets >
                    row.TotalBillets)
                {
                    ModelState.AddModelError(
                        "Details[" +
                        index +
                        "].GoodBillets",

                        "Good billets cannot exceed total billets."
                    );
                }

                if (
                    row.ShortBillets > 0 &&
                    (row.ShortBilletTotalLength ?? 0M) <= 0)
                {
                    ModelState.AddModelError(
                        "Details[" +
                        index +
                        "].ShortBilletTotalLength",

                        "Short billet total length is required when short billet quantity is entered."
                    );
                }

                if (
                    (row.PerCoilBundleWeight ?? 0M) < 0)
                {
                    ModelState.AddModelError(
                        "Details[" +
                        index +
                        "].PerCoilBundleWeight",

                        "Per unit weight cannot be negative."
                    );
                }
            }
        }

        private void EnsureAtLeastOneDetailRow(
            CCMDailyProductionReportBLL model)
        {
            if (model.Details == null)
            {
                model.Details =
                    new List<
                        CCMDailyProductionReportDetailBLL
                    >();
            }

            if (model.Details.Count == 0)
            {
                model.Details.Add(
                    new CCMDailyProductionReportDetailBLL
                    {
                        SequenceNo = 1
                    }
                );
            }
        }

        private void LoadDropdowns(
            string selectedShift)
        {
            List<string> shifts =
                new List<string>
                {
                    "Morning",
                    "Evening",
                    "Night",
                    "Long Morning",
                    "Long Night"
                };

            ViewBag.Shifts =
                new SelectList(
                    shifts,
                    selectedShift
                );
        }

        private string GetCurrentUser()
        {
            string userName =
                Convert.ToString(
                    Session["UserName"]
                );

            return string.IsNullOrWhiteSpace(
                userName)
                    ? User.Identity.Name
                    : userName;
        }

        #endregion

        [HttpGet]
        public ActionResult DownloadAllPdf(
    DateTime? fromDate,
    DateTime? toDate,
    string shift)
        {
            DateTime selectedFromDate =
                fromDate ?? DateTime.Today;

            DateTime selectedToDate =
                toDate ?? selectedFromDate;

            string selectedShift =
                string.IsNullOrWhiteSpace(shift)
                    ? null
                    : shift.Trim();

            /*
                IMPORTANT:
                Yahan wahi repository method use karo jo List action mein
                records return kar raha hai.
            */
            List<CCMDailyProductionReportBLL> headerRecords =
                repo.GetAll(
                        selectedFromDate,
                        selectedToDate,
                        selectedShift
                    )
                    ?.ToList()
                ?? new List<CCMDailyProductionReportBLL>();

            if (headerRecords.Count == 0)
            {
                TempData["Error"] =
                    "No CCM reports found for the selected filters.";

                return RedirectToAction(
                    "List",
                    new
                    {
                        fromDate =
                            selectedFromDate.ToString(
                                "yyyy-MM-dd"
                            ),

                        toDate =
                            selectedToDate.ToString(
                                "yyyy-MM-dd"
                            ),

                        shift =
                            selectedShift
                    }
                );
            }

            /*
                List action aksar sirf report headers return karta hai.
                Isliye har ID ka complete report dobara load kar rahe hain,
                including Details.
            */
            var completeRecords =
                new List<CCMDailyProductionReportBLL>();

            foreach (var header in headerRecords)
            {
                if (header == null || header.ID <= 0)
                {
                    continue;
                }

                CCMDailyProductionReportBLL fullRecord =
                    repo.GetByID(
                        header.ID
                    );

                if (fullRecord == null)
                {
                    continue;
                }

                /*
                    Safety:
                    Details null hon to empty list set kar do.
                */
                if (fullRecord.Details == null)
                {
                    fullRecord.Details =
                        new List<CCMDailyProductionReportDetailBLL>();
                }

                completeRecords.Add(
                    fullRecord
                );
            }

            if (completeRecords.Count == 0)
            {
                TempData["Error"] =
                    "Report headers were found, but complete report data could not be loaded.";

                return RedirectToAction(
                    "List",
                    new
                    {
                        fromDate =
                            selectedFromDate.ToString(
                                "yyyy-MM-dd"
                            ),

                        toDate =
                            selectedToDate.ToString(
                                "yyyy-MM-dd"
                            ),

                        shift =
                            selectedShift
                    }
                );
            }

            return new Rotativa.ViewAsPdf(
                "CCM_GetAllPdf_Template_NoLinq",
                completeRecords
            )
            {
                FileName =
        "Billet_Yard_All_Heats_" +
        selectedFromDate.ToString("yyyyMMdd") +
        "_" +
        selectedToDate.ToString("yyyyMMdd") +
        ".pdf",

                PageSize =
        Rotativa.Options.Size.A3,

                PageOrientation =
        Rotativa.Options.Orientation.Landscape,

                PageMargins =
        new Rotativa.Options.Margins(
            5,
            5,
            5,
            5
        ),

                CustomSwitches =
        "--print-media-type " +
        "--encoding utf-8 " +
        "--zoom 1.0"
            };
        }
    }
}