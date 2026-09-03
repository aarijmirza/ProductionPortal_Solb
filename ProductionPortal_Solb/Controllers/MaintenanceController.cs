using BAL.Repositories;
using ClosedXML.Excel;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
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
            DateTime startDate =
                fromDate ?? DateTime.Today;

            DateTime endDate =
                toDate ?? DateTime.Today;


            if (startDate.Date > endDate.Date)
            {
                DateTime temp = startDate;
                startDate = endDate;
                endDate = temp;
            }

            var agencyList =
            repo.GetAllAgency()
            ?? new List<PlantDelayBLL>();

            ViewBag.Agencies =
                agencyList;


            string plantCsv =
                plant != null &&
                plant.Length > 0
                    ? string.Join(
                        ",",
                        plant
                            .Where(
                                x =>
                                    !string.IsNullOrWhiteSpace(x)
                            )
                            .Select(
                                x => x.Trim()
                            )
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
                            .Where(
                                x =>
                                    !string.IsNullOrWhiteSpace(x)
                            )
                            .Select(
                                x => x.Trim()
                            )
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
                            .Where(
                                x =>
                                    !string.IsNullOrWhiteSpace(x)
                            )
                            .Select(
                                x => x.Trim()
                            )
                            .Distinct(
                                StringComparer.OrdinalIgnoreCase
                            )
                      )
                    : null;


            ViewBag.FromDate =
                startDate.ToString(
                    "yyyy-MM-dd"
                );

            ViewBag.ToDate =
                endDate.ToString(
                    "yyyy-MM-dd"
                );

            ViewBag.Plant =
                plantCsv;

            ViewBag.Agency =
                agencyCsv;

            ViewBag.DelayType =
                delayTypeCsv;


            var records =
                repo.GetMaintenanceRecords(
                    startDate.Date,
                    endDate.Date,
                    plantCsv,
                    delayTypeCsv,
                    agencyCsv,
                    false
                );


            /*
             * Failure Analysis indicator for the main list.
             * A saved analysis record means Filled; no record means Pending.
             */
            Dictionary<int, bool> failureAnalysisStatus =
                new Dictionary<int, bool>();

            if (records != null)
            {
                foreach (PlantDelayBLL record in records)
                {
                    if (
                        record == null ||
                        record.ID <= 0
                    )
                    {
                        continue;
                    }

                    var analysis =
                        repo.GetMaintenanceAnalysisByDelayID(
                            record.ID
                        );

                    failureAnalysisStatus[record.ID] =
                        analysis != null &&
                        analysis.ID > 0;
                }
            }

            ViewBag.FailureAnalysisStatus =
                failureAnalysisStatus;


            return View(
                records
            );
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

                ViewBag.FailureAnalysisFile =
                    repo.GetFailureAnalysisFileByDelayID(
                        id
                    );

                // Equipment dropdown is required ONLY for Role = User.
                // Other roles can still view the saved equipment, but the
                // dropdown/save controls are not loaded for them.
                if (
                    User != null &&
                    User.Identity != null &&
                    User.Identity.IsAuthenticated &&
                    User.IsInRole("User")
                )
                {
                    ViewBag.EquipmentItems =
                        repo.GetMaintenanceEquipmentListByPlant(
                            model.Plant,
                            model.Equipments
                        );
                }
                else
                {
                    ViewBag.EquipmentItems =
                        new List<string>();
                }

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
        // UPDATE EQUIPMENT AGAINST THIS DELAY
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateDelayEquipment(
            int DelayID,
            string Equipments)
        {
            try
            {
                // Security check: hiding the dropdown in Razor is not enough.
                // Only Role = User is allowed to update equipment.
                if (
                    User == null ||
                    User.Identity == null ||
                    !User.Identity.IsAuthenticated ||
                    !User.IsInRole("User")
                )
                {
                    TempData["ErrorMessage"] =
                        "You are not authorized to update equipment.";

                    return RedirectToAction(
                        "detail",
                        new { id = DelayID }
                    );
                }

                if (DelayID <= 0)
                {
                    TempData["ErrorMessage"] =
                        "Invalid delay record.";

                    return RedirectToAction(
                        "list"
                    );
                }

                if (string.IsNullOrWhiteSpace(Equipments))
                {
                    TempData["ErrorMessage"] =
                        "Please select equipment.";

                    return RedirectToAction(
                        "detail",
                        new { id = DelayID }
                    );
                }

                PlantDelayBLL delay =
                    repo.GetDelayByID(
                        DelayID
                    );

                if (delay == null)
                {
                    TempData["ErrorMessage"] =
                        "Delay record was not found.";

                    return RedirectToAction(
                        "list"
                    );
                }

                // Server-side validation: selected equipment must belong
                // to the same Plant as this delay record.
                List<string> validEquipments =
                    repo.GetMaintenanceEquipmentListByPlant(
                        delay.Plant,
                        delay.Equipments
                    )
                    ?? new List<string>();

                bool isValidEquipment =
                    validEquipments.Any(
                        x =>
                            string.Equals(
                                x,
                                Equipments.Trim(),
                                StringComparison.OrdinalIgnoreCase
                            )
                    );

                if (!isValidEquipment)
                {
                    TempData["ErrorMessage"] =
                        "Selected equipment is not valid for plant " +
                        (delay.Plant ?? "-") + ".";

                    return RedirectToAction(
                        "detail",
                        new { id = DelayID }
                    );
                }

                int result =
                    repo.UpdateDelayEquipment(
                        DelayID,
                        Equipments.Trim(),
                        GetCurrentUser()
                    );

                if (result > 0)
                {
                    TempData["SuccessMessage"] =
                        "Equipment updated successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] =
                        "Equipment was not updated.";
                }

                return RedirectToAction(
                    "detail",
                    new { id = DelayID }
                );
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] =
                    "Unable to update equipment. Error: " +
                    ex.Message;

                return RedirectToAction(
                    "detail",
                    new { id = DelayID }
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
            FailureAnalysisBLL model,
            HttpPostedFileBase FailureAnalysisFile,
            string FailureAnalysisFileRemarks,
            string[] increaseMTBFActions,
            string[] decreaseMTTRActions)
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

                /*
                 * Keep the existing FailureAnalysis table and its existing
                 * insert/update procedures unchanged.
                 *
                 * The old controller relied on model.ID immediately after
                 * InsertMaintenanceAnalysis(). For a NEW analysis that ID
                 * may still be 0, so dynamic MTBF/MTTR rows could be saved
                 * without the correct AnalysisID.
                 *
                 * We save/update the main analysis first, then read the
                 * active FailureAnalysis row again by DelayID.
                 */
                if (model.ID > 0)
                {
                    model.UpdatedBy =
                        currentUser;

                    model.UpdatedDate =
                        DateTime.Now;

                    repo.UpdateMissingMaintenanceAnalysis(
                        model
                    );
                }
                else
                {
                    model.CreatedBy =
                        currentUser;

                    model.CreatedDate =
                        DateTime.Now;

                    repo.InsertMaintenanceAnalysis(
                        model
                    );
                }

                FailureAnalysisBLL savedAnalysis =
                    repo.GetMaintenanceAnalysisByDelayID(
                        delayID
                    );

                if (
                    savedAnalysis == null ||
                    savedAnalysis.ID <= 0
                )
                {
                    throw new Exception(
                        "Failure Analysis record was not found after save."
                    );
                }

                /*
                 * All dynamic rows are linked to this exact analysis.
                 * Each row receives its own ActionCode inside
                 * sp_InsertFailureAnalysisAction.
                 */
                int savedDynamicActions =
                    SaveMtbfMttrActions(
                        delayID,
                        savedAnalysis.ID,
                        increaseMTBFActions,
                        decreaseMTTRActions
                    );


                // =====================================================
                // FAILURE ANALYSIS ATTACHMENT
                // =====================================================
                if (
                    FailureAnalysisFile != null &&
                    FailureAnalysisFile.ContentLength > 0
                )
                {
                    string originalFileName =
                        Path.GetFileName(
                            FailureAnalysisFile.FileName
                        );

                    string savedRelativePath =
                        SaveFailureAnalysisFile(
                            FailureAnalysisFile,
                            delayID
                        );

                    int attachmentResult =
                        repo.SaveFailureAnalysisFile(
                            delayID,
                            savedRelativePath,
                            originalFileName,
                            FailureAnalysisFileRemarks,
                            currentUser
                        );

                    if (attachmentResult <= 0)
                    {
                        DeleteUploadedFile(
                            savedRelativePath
                        );

                        throw new Exception(
                            "Failure analysis attachment could not be saved."
                        );
                    }
                }


                TempData["Success"] =
                    savedDynamicActions > 0
                        ? "Maintenance analysis and " +
                          savedDynamicActions +
                          " dynamic MTBF/MTTR action(s) saved successfully."
                        : "Maintenance analysis saved successfully.";


                return RedirectToAction(
                    "detail",
                    new
                    {
                        id = delayID
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

        private int SaveMtbfMttrActions(
            int delayID,
            int analysisID,
            string[] increaseMTBFActions,
            string[] decreaseMTTRActions)
        {
            if (
                delayID <= 0 ||
                analysisID <= 0
            )
            {
                return 0;
            }

            int savedCount =
                0;

            string currentUser =
                GetCurrentUser();


            // =====================================================
            // MTBF - every box becomes a separate DB row/code
            // =====================================================
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

                    FailureAnalysisActionBLL action =
                        new FailureAnalysisActionBLL
                        {
                            DelayID =
                                delayID,

                            AnalysisID =
                                analysisID,

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

                    /*
                     * ActionCode is intentionally NOT generated here.
                     * SQL generates it after the identity ActionID exists,
                     * which makes every MTBF/MTTR code concurrency-safe
                     * and unique.
                     */
                    int actionID =
                        repo.InsertFailureAnalysisAction(
                            action
                        );

                    if (actionID <= 0)
                    {
                        throw new Exception(
                            "Dynamic MTBF action could not be saved."
                        );
                    }

                    savedCount++;
                }
            }


            // =====================================================
            // MTTR - every box becomes a separate DB row/code
            // =====================================================
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

                    FailureAnalysisActionBLL action =
                        new FailureAnalysisActionBLL
                        {
                            DelayID =
                                delayID,

                            AnalysisID =
                                analysisID,

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

                    int actionID =
                        repo.InsertFailureAnalysisAction(
                            action
                        );

                    if (actionID <= 0)
                    {
                        throw new Exception(
                            "Dynamic MTTR action could not be saved."
                        );
                    }

                    savedCount++;
                }
            }


            return savedCount;
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

        private string SaveFailureAnalysisFile(
            HttpPostedFileBase uploadedFile,
            int delayID)
        {
            const int maximumFileSize =
                10 * 1024 * 1024;

            if (
                uploadedFile == null ||
                uploadedFile.ContentLength <= 0
            )
            {
                return null;
            }

            if (
                uploadedFile.ContentLength >
                maximumFileSize
            )
            {
                throw new Exception(
                    "Attachment size cannot exceed 10 MB."
                );
            }

            string extension =
                Path.GetExtension(
                    uploadedFile.FileName
                );

            extension =
                string.IsNullOrWhiteSpace(extension)
                    ? ""
                    : extension.ToLowerInvariant();

            HashSet<string> allowedExtensions =
                new HashSet<string>(
                    StringComparer.OrdinalIgnoreCase
                )
                {
                    ".pdf",
                    ".doc",
                    ".docx",
                    ".xls",
                    ".xlsx",
                    ".jpg",
                    ".jpeg",
                    ".png"
                };

            if (!allowedExtensions.Contains(extension))
            {
                throw new Exception(
                    "Only PDF, Word, Excel, JPG, JPEG and PNG files are allowed."
                );
            }

            string uploadDirectory =
                Server.MapPath(
                    "~/Uploads/FailureAnalysis"
                );

            if (!Directory.Exists(uploadDirectory))
            {
                Directory.CreateDirectory(
                    uploadDirectory
                );
            }

            string storedFileName =
                "FA-" +
                delayID +
                "-" +
                DateTime.Now.ToString(
                    "yyyyMMddHHmmssfff"
                ) +
                "-" +
                Guid.NewGuid()
                    .ToString("N")
                    .Substring(0, 8) +
                extension;

            string physicalPath =
                Path.Combine(
                    uploadDirectory,
                    storedFileName
                );

            uploadedFile.SaveAs(
                physicalPath
            );

            return
                "~/Uploads/FailureAnalysis/" +
                storedFileName;
        }


        private void DeleteUploadedFile(
            string relativePath)
        {
            if (string.IsNullOrWhiteSpace(
                relativePath
            ))
            {
                return;
            }

            string physicalPath =
                Server.MapPath(
                    relativePath
                );

            if (System.IO.File.Exists(
                physicalPath
            ))
            {
                System.IO.File.Delete(
                    physicalPath
                );
            }
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


        [HttpGet]
        public ActionResult countermeasurelist(
        DateTime? fromDate,
        DateTime? toDate,
        string plant,
        string status)
        {
            try
            {
                /*
                 * No date supplied = show all countermeasures.
                 * If user supplies only one side of the range,
                 * the SP handles the open-ended filter.
                 */
                List<CounterMeasureFollowupVM> model =
                    repo.GetFollowupTracker(
                        fromDate,
                        toDate,
                        plant,
                        status
                    );

                ViewBag.FromDate =
                    fromDate.HasValue
                        ? fromDate.Value.ToString(
                            "yyyy-MM-dd"
                        )
                        : "";

                ViewBag.ToDate =
                    toDate.HasValue
                        ? toDate.Value.ToString(
                            "yyyy-MM-dd"
                        )
                        : "";

                ViewBag.Plant =
                    plant ?? "";

                ViewBag.Status =
                    status ?? "";

                return View(
                    model
                );
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    "Unable to load Counter Measure Follow-up Tracker. Error: " +
                    ex.Message;

                return View(
                    new List<CounterMeasureFollowupVM>()
                );
            }
        }

    }
}
