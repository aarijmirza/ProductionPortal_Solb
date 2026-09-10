using BAL.Repositories;
using DAL.Models;
using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProductionPortal_Solb.Controllers
{
    public class SMPProductionDelayController : Controller
    {
        private const string UploadSheetName = "SMP Delay Upload";
        private const int HeaderRowIndex = 2;     // Excel row 3
        private const int DataStartRowIndex = 3;  // Excel row 4

        private readonly SMPProductionDelayRepository repo =
            new SMPProductionDelayRepository();

        private static readonly string[] ExpectedHeaders =
        {
            "ID",
            "RowNo",
            "DelayCode",
            "Plant ID",
            "ShiftGroup",
            "ProductionDate",
            "DelayStart",
            "DelayFinish",
            "TotalMinutes",
            "Agency",
            "AgencyCode",
            "Area",
            "Equipment",
            "DelayDescription",
            "ReasonForOccurrence",
            "ActionTaken",
            "LastPMDate",
            "FailureReportStatus",
            "IncreaseMTBF",
            "DecreaseMTTR",
            "SAPBreakdownOrder",
            "FailureCategory1Component",
            "FailureCategory2RootCause",
            "StatusID",
            "CreatedBy",
            "CreatedDate",
            "UpdatedBy",
            "UpdatedDate"
        };


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
                selectedFromDate.Value > selectedToDate.Value
            )
            {
                DateTime temp = selectedFromDate.Value;
                selectedFromDate = selectedToDate;
                selectedToDate = temp;
            }

            ViewBag.FromDate =
                selectedFromDate.HasValue
                    ? selectedFromDate.Value.ToString("yyyy-MM-dd")
                    : "";

            ViewBag.ToDate =
                selectedToDate.HasValue
                    ? selectedToDate.Value.ToString("yyyy-MM-dd")
                    : "";

            var records =
                repo.GetAll(
                    selectedFromDate,
                    selectedToDate
                );

            return View(records);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadExcel(
            HttpPostedFileBase excelFile)
        {
            try
            {
                ValidateUploadedFile(excelFile);

                List<SMPProductionDelayUploadBLL> rows =
                    ReadDelayRows(
                        excelFile.InputStream
                    );

                if (rows.Count == 0)
                {
                    throw new InvalidOperationException(
                        "No delay records were found in the '" +
                        UploadSheetName +
                        "' sheet."
                    );
                }

                ValidateDuplicateRows(rows);

                string createdBy =
                    GetCurrentUser();

                SMPProductionDelayImportResultBLL result =
                    repo.ImportExcelRows(
                        rows,
                        createdBy
                    );

                string codeMessage =
                    result.GeneratedDelayCodes > 0
                        ? " Delay codes generated: " +
                          result.FirstGeneratedDelayCode +
                          " to " +
                          result.LastGeneratedDelayCode +
                          "."
                        : " Existing delay codes were retained.";

                TempData["Success"] =
                    result.ProcessedRows +
                    " Excel row(s) processed. " +
                    result.InsertedSMPProductionDelays +
                    " new row(s) inserted, " +
                    result.UpdatedSMPProductionDelays +
                    " existing row(s) updated, " +
                    result.DeactivatedSMPProductionDelays +
                    " old row(s) closed." +
                    codeMessage;

                return RedirectToAction(
                    "Index",
                    new
                    {
                        fromDate = rows
                            .Min(x => x.ProductionDate)
                            .ToString("yyyy-MM-dd"),

                        toDate = rows
                            .Max(x => x.ProductionDate)
                            .ToString("yyyy-MM-dd")
                    }
                );
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    ex.Message;

                return RedirectToAction(
                    "Index"
                );
            }
        }


        private static void ValidateUploadedFile(
            HttpPostedFileBase excelFile)
        {
            if (
                excelFile == null ||
                excelFile.ContentLength <= 0
            )
            {
                throw new InvalidOperationException(
                    "Please select the SMP Delay Excel file."
                );
            }

            string extension =
                Path.GetExtension(
                    excelFile.FileName
                );

            if (
                !string.Equals(
                    extension,
                    ".xlsx",
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                throw new InvalidOperationException(
                    "Only .xlsx files are allowed."
                );
            }

            const int maximumFileSize =
                10 * 1024 * 1024;

            if (
                excelFile.ContentLength >
                maximumFileSize
            )
            {
                throw new InvalidOperationException(
                    "Excel file size cannot exceed 10 MB."
                );
            }
        }


        private static List<SMPProductionDelayUploadBLL> ReadDelayRows(
            Stream excelStream)
        {
            if (excelStream.CanSeek)
            {
                excelStream.Position = 0;
            }

            using (
                IExcelDataReader reader =
                    ExcelReaderFactory.CreateReader(
                        excelStream
                    )
            )
            {
                DataSet dataSet =
                    reader.AsDataSet(
                        new ExcelDataSetConfiguration
                        {
                            UseColumnDataType = false,

                            ConfigureDataTable = _ =>
                                new ExcelDataTableConfiguration
                                {
                                    UseHeaderRow = false
                                }
                        }
                    );

                DataTable sheet =
                    dataSet.Tables
                        .Cast<DataTable>()
                        .FirstOrDefault(
                            x => string.Equals(
                                (x.TableName ?? "").Trim(),
                                UploadSheetName,
                                StringComparison.OrdinalIgnoreCase
                            )
                        );

                if (sheet == null)
                {
                    throw new InvalidOperationException(
                        "Required sheet '" +
                        UploadSheetName +
                        "' was not found."
                    );
                }

                ValidateExcelHeaders(sheet);

                var rows =
                    new List<SMPProductionDelayUploadBLL>();

                /*
                 * Blank template rows contain formulas/default StatusID.
                 * HasBusinessInput intentionally ignores system/formula-only
                 * cells so unused template rows are skipped.
                 */
                for (
                    int rowIndex = DataStartRowIndex;
                    rowIndex < sheet.Rows.Count;
                    rowIndex++
                )
                {
                    DataRow row =
                        sheet.Rows[rowIndex];

                    int excelRowNo =
                        rowIndex + 1;

                    if (!HasBusinessInput(row))
                    {
                        continue;
                    }

                    string plant =
                        RequiredPlantID(
                            row[3],
                            excelRowNo
                        );

                    int rowNo =
                        OptionalPositiveInteger(
                            row[1],
                            excelRowNo,
                            "RowNo"
                        )
                        ??
                        (
                            rowIndex -
                            DataStartRowIndex +
                            1
                        );

                    TimeSpan? delayStart =
                        OptionalTime(
                            row[6],
                            excelRowNo,
                            "DelayStart"
                        );

                    TimeSpan? delayFinish =
                        OptionalTime(
                            row[7],
                            excelRowNo,
                            "DelayFinish"
                        );

                    int totalMinutes =
                        ReadTotalMinutes(
                            row[8],
                            delayStart,
                            delayFinish,
                            excelRowNo
                        );

                    string agency =
                        RequiredText(
                            row[9],
                            excelRowNo,
                            "Agency"
                        );

                    string agencyCode =
                        ResolveAgencyCode(
                            agency,
                            OptionalText(row[10]),
                            excelRowNo
                        );

                    var uploadRow =
                        new SMPProductionDelayUploadBLL
                        {
                            ExcelRowNo = excelRowNo,

                            ID =
                                OptionalInteger(
                                    row[0],
                                    excelRowNo,
                                    "ID"
                                ),

                            RowNo = rowNo,

                            DelayCode =
                                OptionalText(
                                    row[2]
                                ),

                            Plant = plant,

                            ShiftGroup =
                                RequiredText(
                                    row[4],
                                    excelRowNo,
                                    "ShiftGroup"
                                ),

                            ProductionDate =
                                RequiredDate(
                                    row[5],
                                    excelRowNo,
                                    "ProductionDate"
                                ),

                            DelayStart = delayStart,

                            DelayFinish = delayFinish,

                            TotalMinutes = totalMinutes,

                            Agency =
                                agency,

                            AgencyCode =
                                agencyCode,

                            Area =
                                OptionalText(
                                    row[11]
                                ),

                            Equipment =
                                OptionalText(
                                    row[12]
                                ),

                            DelayDescription =
                                OptionalText(
                                    row[13]
                                ),

                            ReasonForOccurrence =
                                OptionalText(
                                    row[14]
                                ),

                            ActionTaken =
                                OptionalText(
                                    row[15]
                                ),

                            LastPMDate =
                                OptionalDate(
                                    row[16],
                                    excelRowNo,
                                    "LastPMDate"
                                ),

                            FailureReportStatus =
                                OptionalText(
                                    row[17]
                                ),

                            IncreaseMTBF =
                                OptionalText(
                                    row[18]
                                ),

                            DecreaseMTTR =
                                OptionalText(
                                    row[19]
                                ),

                            SAPBreakdownOrder =
                                OptionalText(
                                    row[20]
                                ),

                            FailureCategory1Component =
                                OptionalText(
                                    row[21]
                                ),

                            FailureCategory2RootCause =
                                OptionalText(
                                    row[22]
                                ),

                            StatusID =
                                OptionalInteger(
                                    row[23],
                                    excelRowNo,
                                    "StatusID"
                                )
                                ??
                                1,

                            CreatedBy =
                                OptionalText(
                                    row[24]
                                ),

                            CreatedDate =
                                OptionalDateTime(
                                    row[25],
                                    excelRowNo,
                                    "CreatedDate"
                                ),

                            UpdatedBy =
                                OptionalText(
                                    row[26]
                                ),

                            UpdatedDate =
                                OptionalDateTime(
                                    row[27],
                                    excelRowNo,
                                    "UpdatedDate"
                                )
                        };

                    rows.Add(uploadRow);
                }

                return rows;
            }
        }


        private static void ValidateExcelHeaders(
            DataTable sheet)
        {
            if (
                sheet.Rows.Count <= HeaderRowIndex ||
                sheet.Columns.Count < ExpectedHeaders.Length
            )
            {
                throw new InvalidOperationException(
                    "The '" +
                    UploadSheetName +
                    "' sheet does not contain the expected " +
                    ExpectedHeaders.Length +
                    " columns."
                );
            }

            DataRow headerRow =
                sheet.Rows[HeaderRowIndex];

            for (
                int columnIndex = 0;
                columnIndex < ExpectedHeaders.Length;
                columnIndex++
            )
            {
                string expected =
                    NormalizeHeader(
                        ExpectedHeaders[columnIndex]
                    );

                string actual =
                    NormalizeHeader(
                        Convert.ToString(
                            headerRow[columnIndex]
                        )
                    );

                if (
                    !string.Equals(
                        expected,
                        actual,
                        StringComparison.OrdinalIgnoreCase
                    )
                )
                {
                    throw new InvalidOperationException(
                        "Invalid Excel column " +
                        (columnIndex + 1) +
                        ". Expected '" +
                        ExpectedHeaders[columnIndex] +
                        "' but found '" +
                        Convert.ToString(
                            headerRow[columnIndex]
                        ) +
                        "'."
                    );
                }
            }
        }


        private static bool HasBusinessInput(
            DataRow row)
        {
            int[] indexes =
            {
                3, 4, 5, 6, 7,
                9, 11, 12, 13, 14, 15,
                16, 17, 18, 19, 20, 21, 22
            };

            foreach (int index in indexes)
            {
                object value =
                    row[index];

                if (
                    value != null &&
                    value != DBNull.Value &&
                    !string.IsNullOrWhiteSpace(
                        Convert.ToString(value)
                    )
                )
                {
                    return true;
                }
            }

            return false;
        }



        private static string ResolveAgencyCode(
            string agency,
            string excelAgencyCode,
            int excelRowNo)
        {
            string normalized =
                (agency ?? "")
                    .Trim()
                    .Replace(" ", "")
                    .Replace("-", "")
                    .Replace("_", "")
                    .Replace("/", "")
                    .Replace("\\", "")
                    .Replace("(", "")
                    .Replace(")", "")
                    .ToUpperInvariant();

            string code = null;

            switch (normalized)
            {
                case "EAF":
                    code = "EF";
                    break;

                case "LF":
                    code = "LF";
                    break;

                case "CCM":
                    code = "CC";
                    break;

                case "REFRACTORY":
                    code = "RF";
                    break;

                case "ELECTRICAL":
                    code = "EM";
                    break;

                case "MECHANICAL":
                    code = "MM";
                    break;

                case "CRANE":
                    code = "CR";
                    break;

                case "MATERIALHANDLING":
                    code = "MH";
                    break;

                case "QUALITY":
                    code = "QU";
                    break;

                case "UTILITY":
                    code = "UT";
                    break;

                case "OTHERS":
                case "OTHER":
                    code = "OT";
                    break;

                case "SHELLCHANGEEAF":
                    code = "SH";
                    break;

                case "TUNDISHCHANGECCM":
                    code = "TC";
                    break;

                case "SECTIONCHANGECCM":
                    code = "SC";
                    break;

                case "MAINTAINENCEDOWNDAY":
                case "MAINTENANCEDOWNDAY":
                    code = "MD";
                    break;

                case "POWERFAILURESEC":
                    code = "PF";
                    break;

                case "NORAWMATERIAL":
                    code = "NR";
                    break;

                case "ANNUALSHUTDOWN":
                    code = "AS";
                    break;
            }

            if (!string.IsNullOrWhiteSpace(code))
            {
                return code;
            }

            if (!string.IsNullOrWhiteSpace(excelAgencyCode))
            {
                return excelAgencyCode.Trim();
            }

            throw new InvalidOperationException(
                "Excel row " +
                excelRowNo +
                ": AgencyCode could not be resolved for Agency '" +
                agency +
                "'."
            );
        }


        private static string RequiredPlantID(
            object value,
            int excelRowNo)
        {
            string text =
                RequiredText(
                    value,
                    excelRowNo,
                    "Plant ID"
                );

            int plantID;

            if (
                !int.TryParse(
                    text,
                    NumberStyles.Integer,
                    CultureInfo.InvariantCulture,
                    out plantID
                )
                ||
                plantID <= 0
            )
            {
                throw new InvalidOperationException(
                    "Excel row " +
                    excelRowNo +
                    ": Plant ID must be a valid positive number from SMPPlantArea."
                );
            }

            return plantID.ToString(
                CultureInfo.InvariantCulture
            );
        }


        private static int ReadTotalMinutes(
            object value,
            TimeSpan? start,
            TimeSpan? finish,
            int excelRowNo)
        {
            int? excelMinutes =
                OptionalInteger(
                    value,
                    excelRowNo,
                    "TotalMinutes"
                );

            if (excelMinutes.HasValue)
            {
                if (excelMinutes.Value < 0)
                {
                    throw new InvalidOperationException(
                        "Excel row " +
                        excelRowNo +
                        ": TotalMinutes cannot be negative."
                    );
                }

                return excelMinutes.Value;
            }

            if (
                start.HasValue &&
                finish.HasValue
            )
            {
                TimeSpan duration =
                    finish.Value -
                    start.Value;

                if (duration.TotalMinutes < 0)
                {
                    duration =
                        duration.Add(
                            TimeSpan.FromDays(1)
                        );
                }

                return Convert.ToInt32(
                    Math.Round(
                        duration.TotalMinutes,
                        MidpointRounding.AwayFromZero
                    )
                );
            }

            throw new InvalidOperationException(
                "Excel row " +
                excelRowNo +
                ": TotalMinutes is required when DelayStart/DelayFinish are not both provided."
            );
        }


        private static void ValidateDuplicateRows(
            List<SMPProductionDelayUploadBLL> rows)
        {
            var duplicateRowNo =
                rows
                    .GroupBy(x => x.RowNo)
                    .FirstOrDefault(
                        x => x.Count() > 1
                    );

            if (duplicateRowNo != null)
            {
                throw new InvalidOperationException(
                    "Duplicate RowNo '" +
                    duplicateRowNo.Key +
                    "' was found in the Excel upload."
                );
            }

            var duplicate =
                rows
                    .GroupBy(BuildDuplicateKey)
                    .FirstOrDefault(
                        x => x.Count() > 1
                    );

            if (duplicate == null)
            {
                return;
            }

            string rowNumbers =
                string.Join(
                    ", ",
                    duplicate.Select(
                        x => x.ExcelRowNo
                    )
                );

            throw new InvalidOperationException(
                "Duplicate delay entries were found in Excel row(s): " +
                rowNumbers +
                "."
            );
        }


        private static string BuildDuplicateKey(
            SMPProductionDelayUploadBLL row)
        {
            return string.Join(
                "|",
                new[]
                {
                    KeyText(row.Plant),
                    KeyText(row.ShiftGroup),
                    row.ProductionDate.ToString("yyyyMMdd"),
                    row.DelayStart.HasValue
                        ? row.DelayStart.Value.Ticks.ToString()
                        : "",
                    row.DelayFinish.HasValue
                        ? row.DelayFinish.Value.Ticks.ToString()
                        : "",
                    row.TotalMinutes.ToString(
                        CultureInfo.InvariantCulture
                    ),
                    KeyText(row.Agency),
                    KeyText(row.Area),
                    KeyText(row.Equipment),
                    KeyText(row.DelayDescription),
                    KeyText(row.ReasonForOccurrence),
                    KeyText(row.ActionTaken)
                }
            );
        }


        private static string KeyText(string value)
        {
            return (value ?? "")
                .Trim()
                .ToUpperInvariant();
        }


        private static string RequiredText(
            object value,
            int excelRowNo,
            string columnName)
        {
            string text =
                OptionalText(value);

            if (string.IsNullOrWhiteSpace(text))
            {
                throw new InvalidOperationException(
                    "Excel row " +
                    excelRowNo +
                    ": " +
                    columnName +
                    " is required."
                );
            }

            return text;
        }


        private static string OptionalText(object value)
        {
            if (
                value == null ||
                value == DBNull.Value
            )
            {
                return null;
            }

            string text =
                Convert.ToString(
                    value,
                    CultureInfo.CurrentCulture
                );

            return string.IsNullOrWhiteSpace(text)
                ? null
                : text.Trim();
        }


        private static int? OptionalPositiveInteger(
            object value,
            int excelRowNo,
            string columnName)
        {
            int? result =
                OptionalInteger(
                    value,
                    excelRowNo,
                    columnName
                );

            if (
                result.HasValue &&
                result.Value <= 0
            )
            {
                throw new InvalidOperationException(
                    "Excel row " +
                    excelRowNo +
                    ": " +
                    columnName +
                    " must be greater than zero."
                );
            }

            return result;
        }


        private static int? OptionalInteger(
            object value,
            int excelRowNo,
            string columnName)
        {
            if (
                value == null ||
                value == DBNull.Value ||
                string.IsNullOrWhiteSpace(
                    Convert.ToString(value)
                )
            )
            {
                return null;
            }

            decimal numericValue;

            if (IsNumeric(value))
            {
                numericValue =
                    Convert.ToDecimal(
                        value,
                        CultureInfo.InvariantCulture
                    );
            }
            else
            {
                decimal parsedValue;
                string text =
                    Convert.ToString(value).Trim();

                if (
                    decimal.TryParse(
                        text,
                        NumberStyles.Any,
                        CultureInfo.CurrentCulture,
                        out parsedValue
                    )
                    ||
                    decimal.TryParse(
                        text,
                        NumberStyles.Any,
                        CultureInfo.InvariantCulture,
                        out parsedValue
                    )
                )
                {
                    numericValue = parsedValue;
                }
                else
                {
                    throw InvalidCell(
                        excelRowNo,
                        columnName
                    );
                }
            }

            if (
                numericValue != decimal.Truncate(numericValue) ||
                numericValue > int.MaxValue ||
                numericValue < int.MinValue
            )
            {
                throw new InvalidOperationException(
                    "Excel row " +
                    excelRowNo +
                    ": " +
                    columnName +
                    " must be a whole number."
                );
            }

            return Convert.ToInt32(
                numericValue
            );
        }


        private static DateTime RequiredDate(
            object value,
            int excelRowNo,
            string columnName)
        {
            DateTime? date =
                OptionalDate(
                    value,
                    excelRowNo,
                    columnName
                );

            if (!date.HasValue)
            {
                throw new InvalidOperationException(
                    "Excel row " +
                    excelRowNo +
                    ": " +
                    columnName +
                    " is required."
                );
            }

            return date.Value.Date;
        }


        private static DateTime? OptionalDate(
            object value,
            int excelRowNo,
            string columnName)
        {
            DateTime? dateTime =
                OptionalDateTime(
                    value,
                    excelRowNo,
                    columnName
                );

            return dateTime.HasValue
                ? (DateTime?)dateTime.Value.Date
                : null;
        }


        private static DateTime? OptionalDateTime(
            object value,
            int excelRowNo,
            string columnName)
        {
            if (
                value == null ||
                value == DBNull.Value ||
                string.IsNullOrWhiteSpace(
                    Convert.ToString(value)
                )
            )
            {
                return null;
            }

            if (value is DateTime)
            {
                return (DateTime)value;
            }

            if (IsNumeric(value))
            {
                double serial =
                    Convert.ToDouble(
                        value,
                        CultureInfo.InvariantCulture
                    );

                try
                {
                    return DateTime.FromOADate(
                        serial
                    );
                }
                catch (ArgumentException)
                {
                    throw InvalidCell(
                        excelRowNo,
                        columnName
                    );
                }
            }

            string text =
                Convert.ToString(value).Trim();

            double serialFromText;

            if (
                double.TryParse(
                    text,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out serialFromText
                )
            )
            {
                try
                {
                    return DateTime.FromOADate(
                        serialFromText
                    );
                }
                catch (ArgumentException)
                {
                    throw InvalidCell(
                        excelRowNo,
                        columnName
                    );
                }
            }

            DateTime parsedDate;

            if (
                DateTime.TryParse(
                    text,
                    CultureInfo.CurrentCulture,
                    DateTimeStyles.AllowWhiteSpaces,
                    out parsedDate
                )
                ||
                DateTime.TryParse(
                    text,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AllowWhiteSpaces,
                    out parsedDate
                )
            )
            {
                return parsedDate;
            }

            throw InvalidCell(
                excelRowNo,
                columnName
            );
        }


        private static TimeSpan? OptionalTime(
            object value,
            int excelRowNo,
            string columnName)
        {
            if (
                value == null ||
                value == DBNull.Value ||
                string.IsNullOrWhiteSpace(
                    Convert.ToString(value)
                )
            )
            {
                return null;
            }

            if (value is TimeSpan)
            {
                return (TimeSpan)value;
            }

            if (value is DateTime)
            {
                return ((DateTime)value).TimeOfDay;
            }

            if (IsNumeric(value))
            {
                double serial =
                    Convert.ToDouble(
                        value,
                        CultureInfo.InvariantCulture
                    );

                double timeFraction =
                    serial -
                    Math.Floor(serial);

                if (timeFraction < 0d)
                {
                    timeFraction += 1d;
                }

                return TimeSpan.FromDays(
                    timeFraction
                );
            }

            string text =
                Convert.ToString(value).Trim();

            double serialFromText;

            if (
                double.TryParse(
                    text,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out serialFromText
                )
            )
            {
                double timeFraction =
                    serialFromText -
                    Math.Floor(serialFromText);

                if (timeFraction < 0d)
                {
                    timeFraction += 1d;
                }

                return TimeSpan.FromDays(
                    timeFraction
                );
            }

            TimeSpan parsedTime;

            if (
                TimeSpan.TryParse(
                    text,
                    CultureInfo.CurrentCulture,
                    out parsedTime
                )
                ||
                TimeSpan.TryParse(
                    text,
                    CultureInfo.InvariantCulture,
                    out parsedTime
                )
            )
            {
                return parsedTime;
            }

            DateTime parsedDateTime;

            if (
                DateTime.TryParse(
                    text,
                    CultureInfo.CurrentCulture,
                    DateTimeStyles.AllowWhiteSpaces,
                    out parsedDateTime
                )
            )
            {
                return parsedDateTime.TimeOfDay;
            }

            throw InvalidCell(
                excelRowNo,
                columnName
            );
        }


        private static bool IsNumeric(object value)
        {
            return value is byte ||
                   value is sbyte ||
                   value is short ||
                   value is ushort ||
                   value is int ||
                   value is uint ||
                   value is long ||
                   value is ulong ||
                   value is float ||
                   value is double ||
                   value is decimal;
        }


        private static InvalidOperationException InvalidCell(
            int excelRowNo,
            string columnName)
        {
            return new InvalidOperationException(
                "Excel row " +
                excelRowNo +
                ": " +
                columnName +
                " contains an invalid value."
            );
        }


        private static string NormalizeHeader(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "";
            }

            return new string(
                value
                    .Where(char.IsLetterOrDigit)
                    .Select(char.ToLowerInvariant)
                    .ToArray()
            );
        }


        private string GetCurrentUser()
        {
            string createdBy =
                Convert.ToString(
                    Session["UserName"]
                );

            if (string.IsNullOrWhiteSpace(createdBy))
            {
                createdBy =
                    Convert.ToString(
                        Session["UserID"]
                    );
            }

            if (
                string.IsNullOrWhiteSpace(createdBy) &&
                User != null &&
                User.Identity != null
            )
            {
                createdBy =
                    User.Identity.Name;
            }

            return string.IsNullOrWhiteSpace(createdBy)
                ? "System"
                : createdBy.Trim();
        }
    }
}
