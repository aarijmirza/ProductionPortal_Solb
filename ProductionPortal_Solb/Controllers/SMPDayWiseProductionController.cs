using ClosedXML.Excel;
using DAL.Models;
using DAL.Repository;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProductionPortal_Solb.Controllers
{
    public class SMPDayWiseProductionController
        : Controller
    {
        private readonly
            SMPDayWiseProductionRepository repo =
                new SMPDayWiseProductionRepository();


        [HttpGet]
        public ActionResult Index(
            DateTime? fromDate,
            DateTime? toDate)
        {
            DateTime? selectedFromDate =
                fromDate.HasValue
                    ? fromDate.Value.Date
                    : (DateTime?)null;

            DateTime? selectedToDate =
                toDate.HasValue
                    ? toDate.Value.Date
                    : (DateTime?)null;


            if (
                selectedFromDate.HasValue &&
                selectedToDate.HasValue &&
                selectedFromDate.Value >
                selectedToDate.Value
            )
            {
                DateTime temp =
                    selectedFromDate.Value;

                selectedFromDate =
                    selectedToDate;

                selectedToDate =
                    temp;
            }


            ViewBag.FromDate =
                selectedFromDate.HasValue
                    ? selectedFromDate.Value
                        .ToString("yyyy-MM-dd")
                    : "";

            ViewBag.ToDate =
                selectedToDate.HasValue
                    ? selectedToDate.Value
                        .ToString("yyyy-MM-dd")
                    : "";


            var records =
                repo.GetAll(
                    selectedFromDate,
                    selectedToDate
                );


            return View(records);
        }


        [HttpGet]
        public ActionResult Details(
            int id)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] =
                    "Invalid day-wise production record.";

                return RedirectToAction("Index");
            }

            var model =
                repo.GetByID(id);

            if (model == null)
            {
                TempData["ErrorMessage"] =
                    "Day-wise production record was not found.";

                return RedirectToAction("Index");
            }

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadExcel(
            HttpPostedFileBase excelFile)
        {
            try
            {
                if (
                    excelFile == null ||
                    excelFile.ContentLength <= 0
                )
                {
                    TempData["ErrorMessage"] =
                        "Please select an Excel file.";

                    return RedirectToAction(
                        "Index"
                    );
                }


                string extension =
                    Path.GetExtension(
                        excelFile.FileName
                    );

                if (
                    !extension.Equals(
                        ".xlsx",
                        StringComparison.OrdinalIgnoreCase
                    )
                    &&
                    !extension.Equals(
                        ".xlsm",
                        StringComparison.OrdinalIgnoreCase
                    )
                )
                {
                    TempData["ErrorMessage"] =
                        "Only .xlsx or .xlsm Excel files are allowed.";

                    return RedirectToAction(
                        "Index"
                    );
                }


                List<SMPDayWiseProductionBLL> rows;


                using (
                    XLWorkbook workbook =
                        new XLWorkbook(
                            excelFile.InputStream
                        )
                )
                {
                    IXLWorksheet worksheet =
                        FindDayWiseProductionSheet(
                            workbook
                        );


                    if (worksheet == null)
                    {
                        TempData["ErrorMessage"] =
                            "Day Wise Production sheet was not found. "
                            +
                            "Please upload the standard production template.";

                        return RedirectToAction(
                            "Index"
                        );
                    }


                    ValidateTemplate(
                        worksheet
                    );


                    rows =
                        ReadProductionRows(
                            worksheet
                        );
                }


                if (
                    rows == null ||
                    rows.Count == 0
                )
                {
                    TempData["ErrorMessage"] =
                        "No valid day-wise production rows were found in the Excel file.";

                    return RedirectToAction(
                        "Index"
                    );
                }


                string currentUser =
                    User != null &&
                    User.Identity != null
                        ? User.Identity.Name
                        : "";


                int insertedRows =
                    repo.ReplaceAll(
                        rows,
                        currentUser
                    );


                TempData["SuccessMessage"] =
                    insertedRows
                    +
                    " day-wise production record(s) uploaded successfully. "
                    +
                    "Previous uploaded data was replaced.";


                return RedirectToAction(
                    "Index"
                );
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] =
                    "Excel upload failed: "
                    +
                    ex.Message;

                return RedirectToAction(
                    "Index"
                );
            }
        }


        /* =========================================================
           EXCEL READER

           Attached template column order:

           A  Date
           B  Month
           C  No of Heats
           D  Production Plan
           E  Total Casted (Ton)
           F  TLS (Ton)
           G  Scrap
           H  DRI + OLD DRI
           I  HBI
           J  Average Heat weight
           K  CCM Productivity
           L  Performance Rate
           M  SMP Material Yield
           N  Availability
           O  Quality Yield
           P  Power On Time
           Q  Net tap to tap
           R  Average Casting Time
           S  Length of Sequence
           T  Electrical
           U  EAF-LF
           V  LPG
           W  O2
           X  Argon
           Y  N2
           Z  DRI / HBI
           AA Scrap
           AB FeSi
           AC SiMn
           AD EAF Electrode
           AE LRF Electrode
           AF Flourspar
           AG Calcined Carbon
           AH Charge coal
           AI Rice Husk
           AJ Lime
           AK LF Lime
           AL Dolo Lime
           AM Electrical EM
           AN Mechanical MM
           AO Refractory RF
           AP Operation O
           AQ Utility U
           AR Cranes CR
           AS Material Handling RMH
           AT Procurement PR
           AU CCM Operation O
           AV Outside OS
           AW Planned Maintenance
           AX Schedule Time
           AY Utilized Time
           AZ Total Delay Time
           ========================================================= */


        private IXLWorksheet
            FindDayWiseProductionSheet(
                XLWorkbook workbook)
        {
            /*
             * Prefer the actual attached template sheet name.
             */
            IXLWorksheet exact =
                workbook.Worksheets
                    .FirstOrDefault(
                        x =>
                            x.Name.Equals(
                                "KPI'S 2026",
                                StringComparison.OrdinalIgnoreCase
                            )
                    );


            if (exact != null)
                return exact;


            /*
             * Future-proof fallback:
             * any sheet whose first row looks like the same
             * Day Wise Production template.
             */
            foreach (
                IXLWorksheet sheet
                in workbook.Worksheets
            )
            {
                string h1 =
                    NormalizeHeader(
                        sheet.Cell(1, 1)
                            .GetFormattedString()
                    );

                string h3 =
                    NormalizeHeader(
                        sheet.Cell(1, 3)
                            .GetFormattedString()
                    );

                string h5 =
                    NormalizeHeader(
                        sheet.Cell(1, 5)
                            .GetFormattedString()
                    );


                if (
                    h1 == "DATE" &&
                    h3.Contains("NOOFHEATS") &&
                    h5.Contains("TOTALCASTED")
                )
                {
                    return sheet;
                }
            }


            return null;
        }


        private void ValidateTemplate(
            IXLWorksheet sheet)
        {
            string dateHeader =
                NormalizeHeader(
                    sheet.Cell(1, 1)
                        .GetFormattedString()
                );

            string heatHeader =
                NormalizeHeader(
                    sheet.Cell(1, 3)
                        .GetFormattedString()
                );

            string castedHeader =
                NormalizeHeader(
                    sheet.Cell(1, 5)
                        .GetFormattedString()
                );

            string powerOnTimeHeader =
                NormalizeHeader(
                    sheet.Cell(1, 16)
                        .GetFormattedString()
                );

            string totalDelayHeader =
                NormalizeHeader(
                    sheet.Cell(1, 52)
                        .GetFormattedString()
                );


            if (
                dateHeader != "DATE" ||
                !heatHeader.Contains(
                    "NOOFHEATS"
                ) ||
                !castedHeader.Contains(
                    "TOTALCASTED"
                ) ||
                !powerOnTimeHeader.Contains(
                    "POWERONTIME"
                ) ||
                !totalDelayHeader.Contains(
                    "TOTALDELAYTIME"
                )
            )
            {
                throw new Exception(
                    "Excel format is not valid. "
                    +
                    "Please use the standard Day Wise Production template."
                );
            }
        }


        private List<SMPDayWiseProductionBLL>
            ReadProductionRows(
                IXLWorksheet sheet)
        {
            var result =
                new List<
                    SMPDayWiseProductionBLL
                >();


            int lastRow =
                sheet.LastRowUsed() != null
                    ? sheet.LastRowUsed()
                        .RowNumber()
                    : 1;


            for (
                int rowNo = 2;
                rowNo <= lastRow;
                rowNo++
            )
            {
                DateTime? productionDate =
                    GetDate(
                        sheet.Cell(
                            rowNo,
                            1
                        )
                    );


                /*
                 * Empty / invalid date row is ignored.
                 */
                if (
                    !productionDate.HasValue
                )
                {
                    continue;
                }


                var item =
                    new SMPDayWiseProductionBLL
                    {
                        Date =
                            productionDate.Value.Date,

                        Month =
                            GetText(
                                sheet.Cell(
                                    rowNo,
                                    2
                                )
                            ),

                        NoOfHeats =
                            GetInt(
                                sheet.Cell(
                                    rowNo,
                                    3
                                )
                            ),

                        ProductionPlan =
                            GetDecimal(
                                sheet.Cell(
                                    rowNo,
                                    4
                                )
                            ),

                        TotalCastedTon =
                            GetDecimal(
                                sheet.Cell(
                                    rowNo,
                                    5
                                )
                            ),

                        TLSTon =
                            GetDecimal(
                                sheet.Cell(
                                    rowNo,
                                    6
                                )
                            ),

                        ScrapCharge =
                            GetDecimal(
                                sheet.Cell(
                                    rowNo,
                                    7
                                )
                            ),

                        DRI_OLD_DRI =
                            GetDecimal(
                                sheet.Cell(
                                    rowNo,
                                    8
                                )
                            ),

                        HBI =
                            GetDecimal(
                                sheet.Cell(
                                    rowNo,
                                    9
                                )
                            ),

                        AverageHeatWeight =
                            GetDecimal(
                                sheet.Cell(
                                    rowNo,
                                    10
                                )
                            ),

                        CCMProductivity =
                            GetDecimal(
                                sheet.Cell(
                                    rowNo,
                                    11
                                )
                            ),

                        PerformanceRate =
                            GetDecimal(
                                sheet.Cell(
                                    rowNo,
                                    12
                                )
                            ),

                        SMPMaterialYield =
                            GetDecimal(
                                sheet.Cell(
                                    rowNo,
                                    13
                                )
                            ),

                        Availability =
                            GetDecimal(
                                sheet.Cell(
                                    rowNo,
                                    14
                                )
                            ),

                        QualityYield =
                            GetDecimal(
                                sheet.Cell(
                                    rowNo,
                                    15
                                )
                            ),

                        PowerOnTime =
                            GetDecimal(
                                sheet.Cell(
                                    rowNo,
                                    16
                                )
                            ),

                        NetTapToTap =
                            GetDecimal(
                                sheet.Cell(
                                    rowNo,
                                    17
                                )
                            ),

                        AverageCastingTime =
                            GetDecimal(
                                sheet.Cell(
                                    rowNo,
                                    18
                                )
                            ),

                        LengthOfSequence =
                            GetText(
                                sheet.Cell(
                                    rowNo,
                                    19
                                )
                            ),

                        Electrical =
                            GetDecimal(sheet.Cell(rowNo, 20)),

                        EAFLF =
                            GetDecimal(sheet.Cell(rowNo, 21)),

                        LPG =
                            GetDecimal(sheet.Cell(rowNo, 22)),

                        O2 =
                            GetDecimal(sheet.Cell(rowNo, 23)),

                        Argon =
                            GetDecimal(sheet.Cell(rowNo, 24)),

                        N2 =
                            GetDecimal(sheet.Cell(rowNo, 25)),

                        DRI_HBI =
                            GetDecimal(sheet.Cell(rowNo, 26)),

                        ScrapConsumption =
                            GetDecimal(sheet.Cell(rowNo, 27)),

                        FeSi =
                            GetDecimal(sheet.Cell(rowNo, 28)),

                        SiMn =
                            GetDecimal(sheet.Cell(rowNo, 29)),

                        EAFElectrode =
                            GetDecimal(sheet.Cell(rowNo, 30)),

                        LRFElectrode =
                            GetDecimal(sheet.Cell(rowNo, 31)),

                        Flourspar =
                            GetDecimal(sheet.Cell(rowNo, 32)),

                        CalcinedCarbon =
                            GetDecimal(sheet.Cell(rowNo, 33)),

                        ChargeCoal =
                            GetDecimal(sheet.Cell(rowNo, 34)),

                        RiceHusk =
                            GetDecimal(sheet.Cell(rowNo, 35)),

                        Lime =
                            GetDecimal(sheet.Cell(rowNo, 36)),

                        LFLime =
                            GetDecimal(sheet.Cell(rowNo, 37)),

                        DoloLime =
                            GetDecimal(sheet.Cell(rowNo, 38)),

                        ElectricalDelayEM =
                            GetDecimal(sheet.Cell(rowNo, 39)),

                        MechanicalDelayMM =
                            GetDecimal(sheet.Cell(rowNo, 40)),

                        RefractoryDelayRF =
                            GetDecimal(sheet.Cell(rowNo, 41)),

                        OperationDelayO =
                            GetDecimal(sheet.Cell(rowNo, 42)),

                        UtilityDelayU =
                            GetDecimal(sheet.Cell(rowNo, 43)),

                        CranesDelayCR =
                            GetDecimal(sheet.Cell(rowNo, 44)),

                        MaterialHandlingRMH =
                            GetDecimal(sheet.Cell(rowNo, 45)),

                        ProcurementPR =
                            GetDecimal(sheet.Cell(rowNo, 46)),

                        CCMOperationO =
                            GetDecimal(sheet.Cell(rowNo, 47)),

                        OutsideOS =
                            GetDecimal(sheet.Cell(rowNo, 48)),

                        PlannedMaintenance =
                            GetDecimal(sheet.Cell(rowNo, 49)),

                        ScheduleTime =
                            GetDecimal(sheet.Cell(rowNo, 50)),

                        UtilizedTime =
                            GetDecimal(sheet.Cell(rowNo, 51)),

                        TotalDelayTime =
                            GetDecimal(sheet.Cell(rowNo, 52))
                    };


                /*
                 * Average Heat Weight in the uploaded workbook may be
                 * a formula. If Excel does not provide a cached formula
                 * result, calculate it here.
                 */
                if (
                    !item.AverageHeatWeight.HasValue &&
                    item.NoOfHeats.HasValue &&
                    item.NoOfHeats.Value > 0 &&
                    item.TotalCastedTon.HasValue
                )
                {
                    item.AverageHeatWeight =
                        item.TotalCastedTon.Value
                        /
                        item.NoOfHeats.Value;
                }


                /*
                 * If Month is blank, derive it from Date.
                 */
                if (
                    string.IsNullOrWhiteSpace(
                        item.Month
                    )
                )
                {
                    item.Month =
                        item.Date.ToString(
                            "MMMM",
                            CultureInfo.InvariantCulture
                        );
                }


                result.Add(
                    item
                );
            }


            /*
             * One record per date.
             * If the Excel file contains the same date more than once,
             * keep the last occurrence from the uploaded file.
             */
            return
                result
                    .GroupBy(
                        x => x.Date.Date
                    )
                    .Select(
                        x => x.Last()
                    )
                    .OrderBy(
                        x => x.Date
                    )
                    .ToList();
        }


        private string NormalizeHeader(
            string value)
        {
            if (
                string.IsNullOrWhiteSpace(
                    value
                )
            )
            {
                return "";
            }


            return
                new string(
                    value
                        .ToUpperInvariant()
                        .Where(
                            char.IsLetterOrDigit
                        )
                        .ToArray()
                );
        }


        private string GetText(
            IXLCell cell)
        {
            if (cell == null)
                return null;

            string value =
                cell.GetFormattedString();

            return
                string.IsNullOrWhiteSpace(
                    value
                )
                    ? null
                    : value.Trim();
        }


        private DateTime? GetDate(
            IXLCell cell)
        {
            if (cell == null)
                return null;


            DateTime dateValue;

            if (
                cell.TryGetValue<DateTime>(
                    out dateValue
                )
            )
            {
                return
                    dateValue.Date;
            }


            string text =
                cell.GetFormattedString()
                    .Trim();


            DateTime parsed;

            if (
                DateTime.TryParse(
                    text,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out parsed
                )
                ||
                DateTime.TryParse(
                    text,
                    CultureInfo.CurrentCulture,
                    DateTimeStyles.None,
                    out parsed
                )
            )
            {
                return
                    parsed.Date;
            }


            return null;
        }


        private int? GetInt(
            IXLCell cell)
        {
            decimal? value =
                GetDecimal(
                    cell
                );

            if (!value.HasValue)
                return null;

            return
                Convert.ToInt32(
                    value.Value
                );
        }


        private decimal? GetDecimal(
            IXLCell cell)
        {
            if (cell == null)
                return null;


            decimal decimalValue;

            if (
                cell.TryGetValue<decimal>(
                    out decimalValue
                )
            )
            {
                return
                    decimalValue;
            }


            double doubleValue;

            if (
                cell.TryGetValue<double>(
                    out doubleValue
                )
            )
            {
                return
                    Convert.ToDecimal(
                        doubleValue
                    );
            }


            string text =
                cell.GetFormattedString()
                    .Trim()
                    .Replace(
                        ",",
                        ""
                    );


            if (
                string.IsNullOrWhiteSpace(
                    text
                )
                ||
                text == "-"
            )
            {
                return null;
            }


            if (
                decimal.TryParse(
                    text,
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out decimalValue
                )
                ||
                decimal.TryParse(
                    text,
                    NumberStyles.Any,
                    CultureInfo.CurrentCulture,
                    out decimalValue
                )
            )
            {
                return
                    decimalValue;
            }


            return null;
        }
    }
}