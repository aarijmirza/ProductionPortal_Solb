using BAL.Repositories;
using ClosedXML.Excel;
using DAL.Models;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;

namespace ProductionPortal_Solb.Controllers
{
    [SessionState(
    SessionStateBehavior.ReadOnly
    )]
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
            string plant,
            string[] agency,
            string[] delayType)
        {
            DateTime startDate =
                fromDate ?? DateTime.Today;

            DateTime endDate =
                toDate ?? DateTime.Today;

            string agencyCsv =
                agency != null &&
                agency.Length > 0
                    ? string.Join(",", agency)
                    : null;

            string delayTypeCsv =
                delayType != null &&
                delayType.Length > 0
                    ? string.Join(",", delayType)
                    : null;

            ViewBag.FromDate =
                startDate.ToString("yyyy-MM-dd");

            ViewBag.ToDate =
                endDate.ToString("yyyy-MM-dd");

            ViewBag.Plant = plant;
            ViewBag.Agency = agencyCsv;
            ViewBag.DelayType = delayTypeCsv;

            var records =
                repo.GetMaintenanceRecords(
                    startDate,
                    endDate,
                    plant,
                    delayTypeCsv,
                    agencyCsv,
                    false
                );

            return View(records);
        }

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

        public ActionResult ExportFailureAnalysisExcel(
         DateTime? fromDate,
         DateTime? toDate,
         string plant,
         string[] agency,
         string[] delayType)
        {
            DateTime startDate =
                fromDate ?? DateTime.Today;

            DateTime endDate =
                toDate ?? DateTime.Today;

            string agencyCsv =
                agency != null
                    ? string.Join(",", agency)
                    : null;

            string delayTypeCsv =
                delayType != null
                    ? string.Join(",", delayType)
                    : null;

            var records =
                repo.GetMaintenanceRecords(
                    fromDate ?? DateTime.Today,
                    toDate ?? DateTime.Today,
                    plant,
                    delayTypeCsv,
                    agencyCsv,
                    true
                );

            using (var workbook = new XLWorkbook())
            {
                var sheet =
                    workbook.Worksheets.Add(
                        "Failure Analysis"
                    );

                string[] headers =
                {
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
            "Long Term Action to Decrease MTTR",
            "SAP Breakdown No.",
            "Failure Category 1 (Component)",
            "Failure Category 2 (Root Cause)"
        };

                for (int column = 0;
                     column < headers.Length;
                     column++)
                {
                    sheet.Cell(1, column + 1).Value =
                        headers[column];
                }

                int rowNumber = 2;

                foreach (var item in records)
                {
                    sheet.Cell(rowNumber, 1).Value =
                        item.Plant;

                    sheet.Cell(rowNumber, 2).Value =
                        item.ProductSize;

                    sheet.Cell(rowNumber, 3).Value =
                        item.Date;

                    sheet.Cell(rowNumber, 4).Value =
                        item.StartTime.HasValue
                            ? item.StartTime.Value
                                .ToString(@"hh\:mm")
                            : "";

                    sheet.Cell(rowNumber, 5).Value =
                        item.EndTime.HasValue
                            ? item.EndTime.Value
                                .ToString(@"hh\:mm")
                            : "";

                    sheet.Cell(rowNumber, 6).Value =
                        item.TotalDuration;

                    sheet.Cell(rowNumber, 7).Value =
                        item.AgencyName;

                    sheet.Cell(rowNumber, 8).Value =
                        item.Area;

                    sheet.Cell(rowNumber, 9).Value =
                        item.Equipments;

                    sheet.Cell(rowNumber, 10).Value =
                        item.DelayDescription;

                    sheet.Cell(rowNumber, 11).Value =
                        item.ReasonForOccurence;

                    sheet.Cell(rowNumber, 12).Value =
                        item.ActionTaken;

                    sheet.Cell(rowNumber, 13).Value =
                        item.LastPMDate;

                    sheet.Cell(rowNumber, 14).Value =
                        item.FailureReportStatus;

                    sheet.Cell(rowNumber, 15).Value =
                        item.IncreaseMTBF;

                    sheet.Cell(rowNumber, 16).Value =
                        item.DecreaseMTTR;

                    sheet.Cell(rowNumber, 17).Value =
                        item.SAPBreakdownOrder;

                    sheet.Cell(rowNumber, 18).Value =
                        item.FailureCategory1Component;

                    sheet.Cell(rowNumber, 19).Value =
                        item.FailureCategory2RootCause;

                    rowNumber++;
                }

                var headerRange =
                    sheet.Range(
                        1,
                        1,
                        1,
                        headers.Length
                    );

                headerRange.Style.Fill.BackgroundColor =
                    XLColor.FromHtml("#0B7285");

                headerRange.Style.Font.FontColor =
                    XLColor.White;

                headerRange.Style.Font.Bold = true;

                headerRange.Style.Alignment
                    .Horizontal =
                    XLAlignmentHorizontalValues.Center;

                headerRange.Style.Alignment
                    .Vertical =
                    XLAlignmentVerticalValues.Center;

                headerRange.Style.Alignment
                    .WrapText = true;

                headerRange.Style.Border
                    .OutsideBorder =
                    XLBorderStyleValues.Thin;

                headerRange.Style.Border
                    .InsideBorder =
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

                    dataRange.Style.Alignment
                        .Vertical =
                        XLAlignmentVerticalValues.Center;

                    dataRange.Style.Alignment
                        .WrapText = true;

                    dataRange.Style.Border
                        .OutsideBorder =
                        XLBorderStyleValues.Thin;

                    dataRange.Style.Border
                        .InsideBorder =
                        XLBorderStyleValues.Thin;

                    for (int row = 2;
                         row < rowNumber;
                         row++)
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

                sheet.Column(3)
                    .Style.DateFormat.Format =
                    "dd-MMM-yyyy";

                sheet.Column(13)
                    .Style.DateFormat.Format =
                    "dd-MMM-yyyy";

                sheet.SheetView.FreezeRows(1);
                sheet.RangeUsed()?.SetAutoFilter();

                sheet.Columns(1, 9)
                    .AdjustToContents();

                sheet.Columns(10, 19)
                    .Width = 30;

                sheet.Row(1).Height = 38;

                using (var stream =
                    new MemoryStream())
                {
                    workbook.SaveAs(stream);

                    string fileName =
                        "Failure_Analysis_" +
                        startDate.ToString("yyyyMMdd") +
                        "_to_" +
                        endDate.ToString("yyyyMMdd") +
                        ".xlsx";

                    return File(
                        stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        fileName
                    );
                }
            }
        }

    }
}