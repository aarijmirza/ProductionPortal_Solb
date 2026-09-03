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
        private readonly SMPProductionDelayRepository repo =
            new SMPProductionDelayRepository();

        private static readonly string[] ExpectedHeaders =
        {
            "Plant",
            "Shift Group",
            "Prod Date",
            "Delay Start",
            "Delay Finish",
            "Total mins",
            "Agency",
            "Area",
            "Equipment",
            "Delay Description",
            "Reason for Occurence",
            "Action Taken",
            "Last P.M Date",
            "Failure report Status",
            "Long term Action To increase MTBF",
            "Long term Action To decrease MTTR",
            "SAP Breakdown Order",
            "Failure Category1 ( Component)",
            "Failure Category 2 (Root Cause)"
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

            var records = repo.GetAll(
                selectedFromDate,
                selectedToDate
            );

            return View(records);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadExcel(HttpPostedFileBase excelFile)
        {
            try
            {
                ValidateUploadedFile(excelFile);

                List<SMPProductionDelayUploadBLL> rows =
                    ReadDelayRows(excelFile.InputStream);

                if (rows.Count == 0)
                {
                    throw new InvalidOperationException(
                        "No delay records were found in the 'Delays analysis' sheet."
                    );
                }

                ValidateDuplicateRows(rows);

                string createdBy = GetCurrentUser();

                SMPProductionDelayImportResultBLL result =
                    repo.ImportExcelRows(rows, createdBy);

                string codeMessage = result.GeneratedDelayCodes > 0
                    ? " Delay codes generated: " +
                      result.FirstGeneratedDelayCode + " to " +
                      result.LastGeneratedDelayCode + "."
                    : " Existing delay codes were retained.";

                TempData["Success"] =
                    result.ProcessedRows + " Excel row(s) processed. " +
                    result.InsertedSMPProductionDelays + " new row(s) inserted, " +
                    result.UpdatedSMPProductionDelays + " existing row(s) updated, " +
                    result.DeactivatedSMPProductionDelays + " old row(s) closed." +
                    codeMessage;

                return RedirectToAction(
                    "Index",
                    new
                    {
                        fromDate = rows.Min(x => x.ProductionDate)
                            .ToString("yyyy-MM-dd"),
                        toDate = rows.Max(x => x.ProductionDate)
                            .ToString("yyyy-MM-dd")
                    }
                );
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        private static void ValidateUploadedFile(
            HttpPostedFileBase excelFile)
        {
            if (excelFile == null || excelFile.ContentLength <= 0)
            {
                throw new InvalidOperationException(
                    "Please select the Failure Analysis Excel file."
                );
            }

            string extension = Path.GetExtension(excelFile.FileName);

            if (!string.Equals(
                extension,
                ".xlsx",
                StringComparison.OrdinalIgnoreCase
            ))
            {
                throw new InvalidOperationException(
                    "Only .xlsx files are allowed."
                );
            }

            const int maximumFileSize = 10 * 1024 * 1024;

            if (excelFile.ContentLength > maximumFileSize)
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

            using (IExcelDataReader reader =
                ExcelReaderFactory.CreateReader(excelStream))
            {
                DataSet dataSet = reader.AsDataSet(
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

                DataTable sheet = dataSet.Tables
                    .Cast<DataTable>()
                    .FirstOrDefault(x => string.Equals(
                        (x.TableName ?? "").Trim(),
                        "Delays analysis",
                        StringComparison.OrdinalIgnoreCase
                    ));

                if (sheet == null)
                {
                    throw new InvalidOperationException(
                        "Required sheet 'Delays analysis' was not found."
                    );
                }

                ValidateExcelHeaders(sheet);

                var rows = new List<SMPProductionDelayUploadBLL>();

                // Excel row 1 is the title, row 2 contains headers,
                // therefore data starts at zero-based DataTable row 2.
                for (int rowIndex = 2; rowIndex < sheet.Rows.Count; rowIndex++)
                {
                    DataRow row = sheet.Rows[rowIndex];
                    int excelRowNo = rowIndex + 1;

                    if (!HasAnyValue(row, ExpectedHeaders.Length))
                    {
                        continue;
                    }

                    string plant = RequiredText(
                        row[0],
                        excelRowNo,
                        "Plant"
                    );

                    if (!string.Equals(
                        plant,
                        "SMP",
                        StringComparison.OrdinalIgnoreCase
                    ))
                    {
                        throw new InvalidOperationException(
                            "Excel row " + excelRowNo +
                            ": Plant must be SMP."
                        );
                    }

                    int totalMinutes = RequiredInteger(
                        row[5],
                        excelRowNo,
                        "Total mins"
                    );

                    if (totalMinutes < 0)
                    {
                        throw new InvalidOperationException(
                            "Excel row " + excelRowNo +
                            ": Total mins cannot be negative."
                        );
                    }

                    rows.Add(
                        new SMPProductionDelayUploadBLL
                        {
                            ExcelRowNo = excelRowNo,
                            Plant = "SMP",
                            ShiftGroup = RequiredText(
                                row[1],
                                excelRowNo,
                                "Shift Group"
                            ),
                            ProductionDate = RequiredDate(
                                row[2],
                                excelRowNo,
                                "Prod Date"
                            ),
                            DelayStart = OptionalTime(
                                row[3],
                                excelRowNo,
                                "Delay Start"
                            ),
                            DelayFinish = OptionalTime(
                                row[4],
                                excelRowNo,
                                "Delay Finish"
                            ),
                            TotalMinutes = totalMinutes,
                            Agency = RequiredText(
                                row[6],
                                excelRowNo,
                                "Agency"
                            ),
                            Area = OptionalText(row[7]),
                            Equipment = OptionalText(row[8]),
                            DelayDescription = OptionalText(row[9]),
                            ReasonForOccurrence = OptionalText(row[10]),
                            ActionTaken = OptionalText(row[11]),
                            LastPMDate = OptionalDate(
                                row[12],
                                excelRowNo,
                                "Last P.M Date"
                            ),
                            FailureReportStatus = OptionalText(row[13]),
                            IncreaseMTBF = OptionalText(row[14]),
                            DecreaseMTTR = OptionalText(row[15]),
                            SAPBreakdownOrder = OptionalText(row[16]),
                            FailureCategory1Component = OptionalText(row[17]),
                            FailureCategory2RootCause = OptionalText(row[18])
                        }
                    );
                }

                return rows;
            }
        }

        private static void ValidateExcelHeaders(DataTable sheet)
        {
            if (sheet.Rows.Count < 2 ||
                sheet.Columns.Count < ExpectedHeaders.Length)
            {
                throw new InvalidOperationException(
                    "The 'Delays analysis' sheet does not contain the expected 19 columns."
                );
            }

            DataRow headerRow = sheet.Rows[1];

            for (int columnIndex = 0;
                 columnIndex < ExpectedHeaders.Length;
                 columnIndex++)
            {
                string expected = NormalizeHeader(
                    ExpectedHeaders[columnIndex]
                );

                string actual = NormalizeHeader(
                    Convert.ToString(headerRow[columnIndex])
                );

                if (!string.Equals(
                    expected,
                    actual,
                    StringComparison.OrdinalIgnoreCase
                ))
                {
                    throw new InvalidOperationException(
                        "Invalid Excel column " + (columnIndex + 1) +
                        ". Expected '" + ExpectedHeaders[columnIndex] +
                        "' but found '" +
                        Convert.ToString(headerRow[columnIndex]) + "'."
                    );
                }
            }
        }

        private static void ValidateDuplicateRows(
            List<SMPProductionDelayUploadBLL> rows)
        {
            var duplicate = rows
                .GroupBy(BuildDuplicateKey)
                .FirstOrDefault(x => x.Count() > 1);

            if (duplicate == null)
            {
                return;
            }

            string rowNumbers = string.Join(
                ", ",
                duplicate.Select(x => x.ExcelRowNo)
            );

            throw new InvalidOperationException(
                "Duplicate delay entries were found in Excel row(s): " +
                rowNumbers + "."
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
            return (value ?? "").Trim().ToUpperInvariant();
        }

        private static bool HasAnyValue(
            DataRow row,
            int columnCount)
        {
            for (int columnIndex = 0;
                 columnIndex < columnCount;
                 columnIndex++)
            {
                object value = row[columnIndex];

                if (value != null &&
                    value != DBNull.Value &&
                    !string.IsNullOrWhiteSpace(Convert.ToString(value)))
                {
                    return true;
                }
            }

            return false;
        }

        private static string RequiredText(
            object value,
            int excelRowNo,
            string columnName)
        {
            string text = OptionalText(value);

            if (string.IsNullOrWhiteSpace(text))
            {
                throw new InvalidOperationException(
                    "Excel row " + excelRowNo + ": " +
                    columnName + " is required."
                );
            }

            return text;
        }

        private static string OptionalText(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return null;
            }

            string text = Convert.ToString(
                value,
                CultureInfo.CurrentCulture
            );

            return string.IsNullOrWhiteSpace(text)
                ? null
                : text.Trim();
        }

        private static DateTime RequiredDate(
            object value,
            int excelRowNo,
            string columnName)
        {
            DateTime? date = OptionalDate(
                value,
                excelRowNo,
                columnName
            );

            if (!date.HasValue)
            {
                throw new InvalidOperationException(
                    "Excel row " + excelRowNo + ": " +
                    columnName + " is required."
                );
            }

            return date.Value.Date;
        }

        private static DateTime? OptionalDate(
            object value,
            int excelRowNo,
            string columnName)
        {
            if (value == null ||
                value == DBNull.Value ||
                string.IsNullOrWhiteSpace(Convert.ToString(value)))
            {
                return null;
            }

            if (value is DateTime)
            {
                return ((DateTime)value).Date;
            }

            if (IsNumeric(value))
            {
                double serial = Convert.ToDouble(
                    value,
                    CultureInfo.InvariantCulture
                );

                try
                {
                    return DateTime.FromOADate(serial).Date;
                }
                catch (ArgumentException)
                {
                    throw InvalidCell(excelRowNo, columnName);
                }
            }

            string text = Convert.ToString(value).Trim();
            double serialFromText;

            if (double.TryParse(
                text,
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out serialFromText
            ))
            {
                try
                {
                    return DateTime.FromOADate(serialFromText).Date;
                }
                catch (ArgumentException)
                {
                    throw InvalidCell(excelRowNo, columnName);
                }
            }

            DateTime parsedDate;

            if (DateTime.TryParse(
                    text,
                    CultureInfo.CurrentCulture,
                    DateTimeStyles.AllowWhiteSpaces,
                    out parsedDate
                ) ||
                DateTime.TryParse(
                    text,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AllowWhiteSpaces,
                    out parsedDate
                ))
            {
                return parsedDate.Date;
            }

            throw InvalidCell(excelRowNo, columnName);
        }

        private static TimeSpan? OptionalTime(
            object value,
            int excelRowNo,
            string columnName)
        {
            if (value == null ||
                value == DBNull.Value ||
                string.IsNullOrWhiteSpace(Convert.ToString(value)))
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
                double serial = Convert.ToDouble(
                    value,
                    CultureInfo.InvariantCulture
                );

                double timeFraction = serial - Math.Floor(serial);

                if (timeFraction < 0d)
                {
                    timeFraction += 1d;
                }

                return TimeSpan.FromDays(timeFraction);
            }

            string text = Convert.ToString(value).Trim();
            double serialFromText;

            if (double.TryParse(
                text,
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out serialFromText
            ))
            {
                double timeFraction =
                    serialFromText - Math.Floor(serialFromText);

                if (timeFraction < 0d)
                {
                    timeFraction += 1d;
                }

                return TimeSpan.FromDays(timeFraction);
            }

            TimeSpan parsedTime;

            if (TimeSpan.TryParse(
                    text,
                    CultureInfo.CurrentCulture,
                    out parsedTime
                ) ||
                TimeSpan.TryParse(
                    text,
                    CultureInfo.InvariantCulture,
                    out parsedTime
                ))
            {
                return parsedTime;
            }

            DateTime parsedDateTime;

            if (DateTime.TryParse(
                text,
                CultureInfo.CurrentCulture,
                DateTimeStyles.AllowWhiteSpaces,
                out parsedDateTime
            ))
            {
                return parsedDateTime.TimeOfDay;
            }

            throw InvalidCell(excelRowNo, columnName);
        }

        private static int RequiredInteger(
            object value,
            int excelRowNo,
            string columnName)
        {
            decimal numericValue;

            if (value != null &&
                value != DBNull.Value &&
                IsNumeric(value))
            {
                numericValue = Convert.ToDecimal(
                    value,
                    CultureInfo.InvariantCulture
                );
            }
            else
            {
                decimal parsedValue;
                string text = Convert.ToString(value).Trim();

                if (decimal.TryParse(
                        text,
                        NumberStyles.Any,
                        CultureInfo.CurrentCulture,
                        out parsedValue
                    ) ||
                    decimal.TryParse(
                        text,
                        NumberStyles.Any,
                        CultureInfo.InvariantCulture,
                        out parsedValue
                    ))
                {
                    numericValue = parsedValue;
                }
                else
                {
                    throw InvalidCell(excelRowNo, columnName);
                }
            }

            if (numericValue != decimal.Truncate(numericValue) ||
                numericValue > int.MaxValue ||
                numericValue < int.MinValue)
            {
                throw new InvalidOperationException(
                    "Excel row " + excelRowNo + ": " +
                    columnName + " must be a whole number."
                );
            }

            return Convert.ToInt32(numericValue);
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
                "Excel row " + excelRowNo + ": " +
                columnName + " contains an invalid value."
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
            string createdBy = Convert.ToString(Session["UserName"]);

            if (string.IsNullOrWhiteSpace(createdBy))
            {
                createdBy = Convert.ToString(Session["UserID"]);
            }

            if (string.IsNullOrWhiteSpace(createdBy) &&
                User != null &&
                User.Identity != null)
            {
                createdBy = User.Identity.Name;
            }

            return string.IsNullOrWhiteSpace(createdBy)
                ? "System"
                : createdBy.Trim();
        }
    }
}
