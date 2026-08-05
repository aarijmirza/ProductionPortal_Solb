using BAL.Repositories;
using ClosedXML.Excel;
using DAL.Models;
using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.SessionState;

namespace ProductionPortal_Solb.Controllers
{
    [SessionState(SessionStateBehavior.ReadOnly)]
    public class MaintenanceController : Controller
    {
        private readonly DelayRespository repo;

        public MaintenanceController()
        {
            repo = new DelayRespository();
        }

        // =========================================================
        // LIST
        // =========================================================

        public ActionResult list(
            DateTime? fromDate,
            DateTime? toDate,
            string[] plant,
            string[] agency,
            string[] delayType)
        {
            DateTime startDate = fromDate ?? DateTime.Today;
            DateTime endDate = toDate ?? DateTime.Today;

            if (startDate.Date > endDate.Date)
            {
                DateTime temp = startDate;
                startDate = endDate;
                endDate = temp;
            }

            string plantCsv = plant != null && plant.Length > 0
                ? string.Join(",", plant.Where(x => !string.IsNullOrWhiteSpace(x))
                                        .Select(x => x.Trim())
                                        .Distinct(StringComparer.OrdinalIgnoreCase))
                : null;

            string agencyCsv = agency != null && agency.Length > 0
                ? string.Join(",", agency.Where(x => !string.IsNullOrWhiteSpace(x))
                                         .Select(x => x.Trim())
                                         .Distinct(StringComparer.OrdinalIgnoreCase))
                : null;

            string delayTypeCsv = delayType != null && delayType.Length > 0
                ? string.Join(",", delayType.Where(x => !string.IsNullOrWhiteSpace(x))
                                            .Select(x => x.Trim())
                                            .Distinct(StringComparer.OrdinalIgnoreCase))
                : null;

            ViewBag.FromDate = startDate.ToString("yyyy-MM-dd");
            ViewBag.ToDate = endDate.ToString("yyyy-MM-dd");
            ViewBag.Plant = plantCsv;
            ViewBag.Agency = agencyCsv;
            ViewBag.DelayType = delayTypeCsv;

            var records = repo.GetMaintenanceRecords(
                startDate.Date,
                endDate.Date,
                plantCsv,
                delayTypeCsv,
                agencyCsv,
                false
            );

            return View(records);
        }

        // =========================================================
        // DETAIL
        // =========================================================

        public ActionResult detail(
            int id)
        {
            try
            {
                if (id <= 0)
                {
                    TempData["ErrorMessage"] =
                        "Invalid record ID.";

                    return RedirectToAction(
                        "list"
                    );
                }

                PlantDelayBLL model =
                    repo.GetDelayByID(
                        id
                    );

                if (model == null)
                {
                    TempData["ErrorMessage"] =
                        "Record not found.";

                    return RedirectToAction(
                        "list"
                    );
                }

                ViewBag.Analysis =
                    repo.GetMaintenanceAnalysisByDelayID(
                        id
                    );

                ViewBag.Actions =
                    repo.GetFailureAnalysisActionsByDelayID(
                        id
                    );

                return View(
                    model
                );
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] =
                    "Unable to load maintenance detail. Error: " +
                    ex.Message;

                return RedirectToAction(
                    "list"
                );
            }
        }

        // =========================================================
        // DELAY CORRECTION
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateDelayCorrection(
            int DelayID,
            string DelayDescription1,
            string ReasonforOccurence1,
            string ActionTaken1)
        {
            try
            {
                if (DelayID <= 0)
                {
                    TempData["ErrorMessage"] =
                        "Invalid delay record.";

                    return RedirectToAction(
                        "list"
                    );
                }

                var model =
                    new PlantDelayBLL
                    {
                        ID =
                            DelayID,

                        DelayDescription1 =
                            string.IsNullOrWhiteSpace(
                                DelayDescription1
                            )
                                ? null
                                : DelayDescription1.Trim(),

                        ReasonForOccurence1 =
                            string.IsNullOrWhiteSpace(
                                ReasonforOccurence1
                            )
                                ? null
                                : ReasonforOccurence1.Trim(),

                        ActionTaken1 =
                            string.IsNullOrWhiteSpace(
                                ActionTaken1
                            )
                                ? null
                                : ActionTaken1.Trim(),

                        UpdatedBy =
                            GetCurrentUser(),

                        UpdatedDate =
                            DateTime.Now
                    };

                int result =
                    repo.UpdateDelayCorrection(
                        model
                    );

                if (result < 0)
                {
                    TempData["SuccessMessage"] =
                        "Delay remarks updated successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] =
                        "Delay remarks were not updated.";
                }

                return RedirectToAction(
                    "detail",
                    new
                    {
                        id = DelayID
                    }
                );
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] =
                    "Unable to update delay remarks. Error: " +
                    ex.Message;

                return RedirectToAction(
                    "detail",
                    new
                    {
                        id = DelayID
                    }
                );
            }
        }

        // =========================================================
        // MAINTENANCE ANALYSIS
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InsertMaintenanceAnalysis(
            FailureAnalysisBLL model)
        {
            int delayID =
            model != null
                ? model.DelayID ?? 0
                : 0;

            try
            {
                if (
                    model == null ||
                    model.DelayID <= 0
                )
                {
                    TempData["Error"] =
                        "Invalid maintenance analysis request.";

                    return RedirectToAction(
                        "list"
                    );
                }

                string currentUser =
                    GetCurrentUser();

                model.StatusID =
                    1;

                int result;

                if (model.ID > 0)
                {
                    model.UpdatedBy =
                        currentUser;

                    model.UpdatedDate =
                        DateTime.Now;

                    result =
                        repo.UpdateMissingMaintenanceAnalysis(
                            model
                        );

                    TempData["Success"] =
                        result > 0
                            ? "Missing analysis information updated successfully."
                            : "No new information was available to update.";
                }
                else
                {
                    model.CreatedBy =
                        currentUser;

                    model.CreatedDate =
                        DateTime.Now;

                    result =
                        repo.InsertMaintenanceAnalysis(
                            model
                        );

                    TempData["Success"] =
                        result > 0
                            ? "Maintenance analysis saved successfully."
                            : "Maintenance analysis could not be saved.";
                }

                return RedirectToAction(
                    "detail",
                    new
                    {
                        id = model.DelayID
                    }
                );
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    "Unable to save maintenance analysis. Error: " +
                    ex.Message;

                if (delayID > 0)
                {
                    return RedirectToAction(
                        "detail",
                        new
                        {
                            id = delayID
                        }
                    );
                }

                return RedirectToAction(
                    "list"
                );
            }
        }

        // =========================================================
        // MTBF / MTTR ACTIONS
        // =========================================================

        private void SaveMtbfMttrActions(
            int delayID,
            int analysisID,
            string[] increaseMTBFActions,
            string[] decreaseMTTRActions)
        {
            if (delayID <= 0)
            {
                return;
            }

            string currentUser =
                GetCurrentUser();

            if (increaseMTBFActions != null)
            {
                foreach (
                    string remarks
                    in increaseMTBFActions
                )
                {
                    if (string.IsNullOrWhiteSpace(
                        remarks
                    ))
                    {
                        continue;
                    }

                    var action =
                        new FailureAnalysisActionBLL
                        {
                            ActionCode =
                                repo.GenerateFailureActionCode(),

                            DelayID =
                                delayID,

                            AnalysisID =
                                analysisID > 0
                                    ? (int?)analysisID
                                    : null,

                            ActionType =
                                "IncreaseMTBF",

                            ActionRemarks =
                                remarks.Trim(),

                            StatusID =
                                1,

                            CreatedBy =
                                currentUser,

                            CreatedDate =
                                DateTime.Now
                        };

                    repo.InsertFailureAnalysisAction(
                        action
                    );
                }
            }

            if (decreaseMTTRActions != null)
            {
                foreach (
                    string remarks
                    in decreaseMTTRActions
                )
                {
                    if (string.IsNullOrWhiteSpace(
                        remarks
                    ))
                    {
                        continue;
                    }

                    var action =
                        new FailureAnalysisActionBLL
                        {
                            ActionCode =
                                repo.GenerateFailureActionCode(),

                            DelayID =
                                delayID,

                            AnalysisID =
                                analysisID > 0
                                    ? (int?)analysisID
                                    : null,

                            ActionType =
                                "DecreaseMTTR",

                            ActionRemarks =
                                remarks.Trim(),

                            StatusID =
                                1,

                            CreatedBy =
                                currentUser,

                            CreatedDate =
                                DateTime.Now
                        };

                    repo.InsertFailureAnalysisAction(
                        action
                    );
                }
            }
        }

        // =========================================================
        // EXCEL EXPORT
        // =========================================================

        public ActionResult ExportFailureAnalysisExcel(
            DateTime? fromDate,
            DateTime? toDate,
            string[] plant,
            string[] agency,
            string[] delayType)
        {
            DateTime startDate =
                fromDate ?? DateTime.Today;

            DateTime endDate =
                toDate ?? DateTime.Today;

            if (startDate.Date > endDate.Date)
            {
                DateTime temp =
                    startDate;

                startDate =
                    endDate;

                endDate =
                    temp;
            }

            string plantCsv =
                plant != null &&
                plant.Length > 0
                    ? string.Join(
                        ",",
                        plant
                            .Where(x =>
                                !string.IsNullOrWhiteSpace(x)
                            )
                            .Select(x => x.Trim())
                            .Distinct(
                                StringComparer.OrdinalIgnoreCase
                            )
                    )
                    : null;

            string agencyCsv =
                agency != null &&
                agency.Length > 0
                    ? string.Join(
                        ",",
                        agency
                            .Where(x =>
                                !string.IsNullOrWhiteSpace(x)
                            )
                            .Select(x => x.Trim())
                            .Distinct(
                                StringComparer.OrdinalIgnoreCase
                            )
                    )
                    : null;

            string delayTypeCsv =
                delayType != null &&
                delayType.Length > 0
                    ? string.Join(
                        ",",
                        delayType
                            .Where(x =>
                                !string.IsNullOrWhiteSpace(x)
                            )
                            .Select(x => x.Trim())
                            .Distinct(
                                StringComparer.OrdinalIgnoreCase
                            )
                    )
                    : null;

            var records =
                repo.GetMaintenanceRecords(
                    startDate.Date,
                    endDate.Date,
                    plantCsv,
                    delayTypeCsv,
                    agencyCsv,
                    true
                );

            using (
                var workbook =
                    new XLWorkbook()
            )
            {
                var sheet =
                    workbook.Worksheets.Add(
                        "Failure Analysis"
                    );

                string[] headers =
                {
                    "Delay ID",
                    "Delay Code",
                    "Plant",
                    "Product Size",
                    "Production Date",
                    "Delay Start",
                    "Delay End",
                    "Total Minutes",
                    "Agency",
                    "Area",
                    "Equipment",
                    "Delay Description",
                    "Reason for Occurrence",
                    "Action Taken",
                    "Last PM Date",
                    "Failure Report Status",
                    "Long Term Action to Increase MTBF",
                    "Additional Action to Increase MTBF",
                    "Long Term Action to Decrease MTTR",
                    "Additional Action to Decrease MTTR",
                    "SAP Breakdown No.",
                    "Failure Category 1 (Component)",
                    "Failure Category 2 (Root Cause)"
                };

                for (
                    int column = 0;
                    column < headers.Length;
                    column++
                )
                {
                    sheet.Cell(
                        1,
                        column + 1
                    ).Value =
                        headers[column];
                }

                int rowNumber =
                    2;

                if (records != null)
                {
                    foreach (
                        var item
                        in records
                    )
                    {
                        sheet.Cell(
                            rowNumber,
                            1
                        ).Value =
                            item.ID;

                        sheet.Cell(
                            rowNumber,
                            2
                        ).Value =
                            item.Delaycode;

                        sheet.Cell(
                            rowNumber,
                            3
                        ).Value =
                            item.Plant;

                        sheet.Cell(
                            rowNumber,
                            4
                        ).Value =
                            item.ProductSize;

                        sheet.Cell(
                            rowNumber,
                            5
                        ).Value =
                            item.Date;

                        sheet.Cell(
                            rowNumber,
                            6
                        ).Value =
                            item.StartTime.HasValue
                                ? item.StartTime.Value
                                    .ToString(@"hh\:mm")
                                : "";

                        sheet.Cell(
                            rowNumber,
                            7
                        ).Value =
                            item.EndTime.HasValue
                                ? item.EndTime.Value
                                    .ToString(@"hh\:mm")
                                : "";

                        sheet.Cell(
                            rowNumber,
                            8
                        ).Value =
                            item.TotalDuration;

                        sheet.Cell(
                            rowNumber,
                            9
                        ).Value =
                            item.AgencyName;

                        sheet.Cell(
                            rowNumber,
                            10
                        ).Value =
                            item.Area;

                        sheet.Cell(
                            rowNumber,
                            11
                        ).Value =
                            item.Equipments;

                        sheet.Cell(
                            rowNumber,
                            12
                        ).Value =
                            item.DelayDescription;

                        sheet.Cell(
                            rowNumber,
                            13
                        ).Value =
                            item.ReasonForOccurence;

                        sheet.Cell(
                            rowNumber,
                            14
                        ).Value =
                            item.ActionTaken;

                        sheet.Cell(
                            rowNumber,
                            15
                        ).Value =
                            item.LastPMDate;

                        sheet.Cell(
                            rowNumber,
                            16
                        ).Value =
                            item.FailureReportStatus;

                        sheet.Cell(
                            rowNumber,
                            17
                        ).Value =
                            item.IncreaseMTBF;

                        sheet.Cell(
                            rowNumber,
                            18
                        ).Value =
                            item.IncreaseMTBF1;

                        sheet.Cell(
                            rowNumber,
                            19
                        ).Value =
                            item.DecreaseMTTR;

                        sheet.Cell(
                            rowNumber,
                            20
                        ).Value =
                            item.DecreaseMTTR1;

                        sheet.Cell(
                            rowNumber,
                            21
                        ).Value =
                            item.SAPBreakdownOrder;

                        sheet.Cell(
                            rowNumber,
                            22
                        ).Value =
                            item.FailureCategory1Component;

                        sheet.Cell(
                            rowNumber,
                            23
                        ).Value =
                            item.FailureCategory2RootCause;

                        rowNumber++;
                    }
                }

                var headerRange =
                    sheet.Range(
                        1,
                        1,
                        1,
                        headers.Length
                    );

                headerRange.Style.Fill.BackgroundColor =
                    XLColor.FromHtml(
                        "#0B7285"
                    );

                headerRange.Style.Font.FontColor =
                    XLColor.White;

                headerRange.Style.Font.Bold =
                    true;

                headerRange.Style.Alignment.Horizontal =
                    XLAlignmentHorizontalValues.Center;

                headerRange.Style.Alignment.Vertical =
                    XLAlignmentVerticalValues.Center;

                headerRange.Style.Alignment.WrapText =
                    true;

                headerRange.Style.Border.OutsideBorder =
                    XLBorderStyleValues.Thin;

                headerRange.Style.Border.InsideBorder =
                    XLBorderStyleValues.Thin;

                if (rowNumber > 2)
                {
                    var dataRange =
                        sheet.Range(
                            2,
                            1,
                            rowNumber - 1,
                            headers.Length
                        );

                    dataRange.Style.Alignment.Vertical =
                        XLAlignmentVerticalValues.Center;

                    dataRange.Style.Alignment.WrapText =
                        true;

                    dataRange.Style.Border.OutsideBorder =
                        XLBorderStyleValues.Thin;

                    dataRange.Style.Border.InsideBorder =
                        XLBorderStyleValues.Thin;

                    for (
                        int row = 2;
                        row < rowNumber;
                        row++
                    )
                    {
                        if (row % 2 == 0)
                        {
                            sheet.Range(
                                row,
                                1,
                                row,
                                headers.Length
                            )
                            .Style.Fill.BackgroundColor =
                                XLColor.FromHtml(
                                    "#CDEBF3"
                                );
                        }
                    }
                }

                sheet.Column(
                    5
                ).Style.DateFormat.Format =
                    "dd-MMM-yyyy";

                sheet.Column(
                    15
                ).Style.DateFormat.Format =
                    "dd-MMM-yyyy";

                sheet.SheetView.FreezeRows(
                    1
                );

                if (sheet.RangeUsed() != null)
                {
                    sheet.RangeUsed()
                        .SetAutoFilter();
                }

                sheet.Columns(
                    1,
                    11
                ).AdjustToContents();

                sheet.Columns(
                    12,
                    headers.Length
                ).Width =
                    30;

                sheet.Row(
                    1
                ).Height =
                    42;

                using (
                    var stream =
                        new MemoryStream()
                )
                {
                    workbook.SaveAs(
                        stream
                    );

                    string fileName =
                        "Failure_Analysis_" +
                        startDate.ToString(
                            "yyyyMMdd"
                        ) +
                        "_to_" +
                        endDate.ToString(
                            "yyyyMMdd"
                        ) +
                        ".xlsx";

                    return File(
                        stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        fileName
                    );
                }
            }
        }

        // =========================================================
        // HELPERS
        // =========================================================

        private string GetCurrentUser()
        {
            string currentUser =
                Convert.ToString(
                    Session["UserName"]
                );

            if (
                string.IsNullOrWhiteSpace(
                    currentUser
                ) &&
                User != null &&
                User.Identity != null
            )
            {
                currentUser =
                    User.Identity.Name;
            }

            return
                string.IsNullOrWhiteSpace(
                    currentUser
                )
                    ? "System"
                    : currentUser.Trim();
        }
    }
}