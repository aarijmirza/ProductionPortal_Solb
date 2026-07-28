using BAL.Repositories;
using DAL.Models;
using DAL.Repository;
using iTextSharp.text;
using iTextSharp.text.pdf;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using Rotativa;
using Rotativa.Options;
using Spire.Xls;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using System.Windows.Media.Imaging;
using WebAPICode.Helpers;
using static DAL.Models.ViewModel;

namespace ProductionPortal_Solb.Controllers
{
    [SessionState(
    SessionStateBehavior.ReadOnly
    )]
    public class ReportingController : Controller
    {
        DelayRespository repo;
        RollingMillRepository rm;
        ConsumptionRepository crepo;
        SupplyChainRepository srepo;
        MaintenanceRepository mrepo;
        RollingMillTargetsRepository targetRepo;
        RollingMillDailyTargetRepository dailyTargetRepo;
        public ReportingController()
        {
            repo = new DelayRespository();
            rm = new RollingMillRepository();
            crepo = new ConsumptionRepository();
            srepo = new SupplyChainRepository();
            mrepo = new MaintenanceRepository();
            targetRepo = new RollingMillTargetsRepository();
            dailyTargetRepo = new RollingMillDailyTargetRepository();
        }

        // GET: Reporting
        public ActionResult DownloadSMPIntegratedPDF()
        {
            DataTable dt = GetSMPDailyReportData();

            MemoryStream ms = new MemoryStream();
            Document doc = new Document(PageSize.A4.Rotate(), 20, 20, 20, 20);
            PdfWriter.GetInstance(doc, ms);
            doc.Open();

            Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 13);
            Font normal = FontFactory.GetFont(FontFactory.HELVETICA, 8);
            Font bold = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8);

            // ------------------------
            // HEADER
            // ------------------------
            PdfPTable header = new PdfPTable(3);
            header.WidthPercentage = 100;
            header.SetWidths(new float[] { 15, 70, 15 });

            Image logo = Image.GetInstance(Server.MapPath("~/assets/images/logo.png"));
            logo.ScaleAbsolute(80, 40);
            header.AddCell(new PdfPCell(logo) { Border = 0 });

            header.AddCell(new PdfPCell(new Phrase("SMP DAILY INTEGRATED REPORT", titleFont))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            });

            var gregorianDate = DateTime.Now.ToString("dd-MMM-yyyy", System.Globalization.CultureInfo.InvariantCulture);

            header.AddCell(new PdfPCell(new Phrase("Production Date: " + gregorianDate, bold))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_RIGHT
            });

            doc.Add(header);
            doc.Add(new Paragraph("\n"));

            // ------------------------
            // MAIN TABLE ONLY
            // ------------------------
            string[] headers =
            {
        "Heat No","Steel Grade","Total Charge","Power On","Tap to Tap",
        "Turn Around","Tapping Weight","EAF Yield","Total Delay",
        "EAF Productivity","LF Treatment","Casting Time",
        "Running Strand","Restranding","CCM Productivity",
        "Prime Billet","Short Billet","Total Casted",
        "CCM Yield","SMP Yield"
    };

            PdfPTable table = new PdfPTable(headers.Length);
            table.WidthPercentage = 100;

            foreach (string h in headers)
            {
                PdfPCell cell = new PdfPCell(new Phrase(h, bold));
                cell.Rotation = 90;
                cell.FixedHeight = 75;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                //cell.BackgroundColor = new BaseColor(221, 235, 247);
                cell.Padding = 5;
                table.AddCell(cell);
            }

            foreach (DataRow row in dt.Rows)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    PdfPCell c = new PdfPCell(new Phrase(row[i].ToString(), normal));
                    c.HorizontalAlignment = Element.ALIGN_CENTER;
                    c.Padding = 4;
                    table.AddCell(c);
                }
            }

            doc.Add(table);

            doc.Close();

            return File(ms.ToArray(), "application/pdf", "SMP_Daily_Integrated.pdf");
        }

        private DataTable GetSMPDailyReportData()
        {
            DataTable dt = new DataTable();

            dt.Columns.AddRange(new DataColumn[] {
        new DataColumn("Heat No"),
        new DataColumn("Steel Grade"),
        new DataColumn("Total Charge"),
        new DataColumn("Power On"),
        new DataColumn("Tap to Tap"),
        new DataColumn("Turn Around"),
        new DataColumn("Tapping Weight"),
        new DataColumn("EAF Yield"),
        new DataColumn("Total Delay"),
        new DataColumn("EAF Productivity"),
        new DataColumn("LF Treatment"),
        new DataColumn("Casting Time"),
        new DataColumn("Running Strand"),
        new DataColumn("Restranding"),
        new DataColumn("CCM Productivity"),
        new DataColumn("Prime Billet"),
        new DataColumn("Short Billet"),
        new DataColumn("Total Casted"),
        new DataColumn("CCM Yield"),
        new DataColumn("SMP Yield")
    });

            // SAMPLE ROW (Remove, replace with DB data)
            dt.Rows.Add("H1023", "A36", "98", "45", "65", "15", "96",
                        "97%", "12", "145", "40", "55", "4", "8", "130",
                        "88", "5", "93", "96%", "94%");

            return dt;
        }

        public ActionResult DownloadDailyProductionPDF()
        {
            string templatePath = Server.MapPath("~/Templates/Daily Production Report.xlsx");
            string excelPath = Server.MapPath("~/Temp/Report_" + DateTime.Now.Ticks + ".xlsx");
            string pdfPath = Server.MapPath("~/Temp/Report_" + DateTime.Now.Ticks + ".pdf");

            if (!System.IO.File.Exists(templatePath))
                return Content("Excel template file missing!");

            // ✅ DBHelper use
            DBHelper db = new DBHelper();

            DataTable dt = db.GetTableFromSP("sp_GetAllElectricArcFurnaceRecord");
            DataTable size = db.GetTableFromSP("sp_GetAllElectricArcFurnaceRecord");

            using (var package = new ExcelPackage(new FileInfo(templatePath)))
            {
                var ws = package.Workbook.Worksheets["Daily Report"];

                if (ws == null)
                    return Content("Excel sheet 'Daily Report' nahi mili!");

                // ✅ Date
                ws.Cells["H2"].Value = DateTime.Now.ToString("dd MMM yyyy");

                // ✅ Summary data
                ws.Cells["D7"].Value = Safe(dt, "ActualProduction");
                ws.Cells["E7"].Value = Safe(dt, "Diff");
                ws.Cells["F7"].Value = Safe(dt, "Fuel");
                ws.Cells["G7"].Value = Safe(dt, "Power");
                ws.Cells["H7"].Value = Safe(dt, "Water");
                ws.Cells["I7"].Value = Safe(dt, "Misroll");
                ws.Cells["J7"].Value = Safe(dt, "Chopping");
                ws.Cells["K7"].Value = Safe(dt, "Yield");
                ws.Cells["L7"].Value = Safe(dt, "TheoryYield");
                ws.Cells["M7"].Value = Safe(dt, "RRR_Day");
                ws.Cells["N7"].Value = Safe(dt, "RRR_YTD");
                ws.Cells["O7"].Value = Safe(dt, "Productivity_Day");
                ws.Cells["P7"].Value = Safe(dt, "Productivity_YTD");

                // ✅ Monthly
                ws.Cells["C11"].Value = Safe(dt, "MonthPlan");
                ws.Cells["D11"].Value = Safe(dt, "MonthActual");
                ws.Cells["E11"].Value = Safe(dt, "MonthDiff");

                // ✅ Yearly
                ws.Cells["C14"].Value = Safe(dt, "YearPlan");
                ws.Cells["D14"].Value = Safe(dt, "YearActual");

                // ✅ Size wise table
                //int row = 20;
                //foreach (DataRow r in size.Rows)
                //{
                //    ws.Cells[row, 1].Value = r["Size"];
                //    ws.Cells[row, 2].Value = r["Plan"];
                //    ws.Cells[row, 3].Value = r["Actual"];
                //    ws.Cells[row, 4].Value = r["Diff"];
                //    ws.Cells[row, 5].Value = r["Mix"];
                //    ws.Cells[row, 6].Value = r["Yield"];
                //    ws.Cells[row, 7].Value = r["RRR"];
                //    ws.Cells[row, 8].Value = r["Productivity"];
                //    row++;
                //}

                package.SaveAs(new FileInfo(excelPath));
            }

            // ✅ Excel → PDF
            Workbook book = new Workbook();
            book.LoadFromFile(excelPath);
            book.SaveToFile(pdfPath, FileFormat.PDF);

            // ✅ Download PDF
            byte[] file = System.IO.File.ReadAllBytes(pdfPath);

            System.IO.File.Delete(excelPath);
            System.IO.File.Delete(pdfPath);

            return File(file, "application/pdf", "DailyProductionReport.pdf");
        }

        public string Safe(DataTable dt, string col)
        {
            if (dt == null || dt.Rows.Count == 0) return "0";
            if (!dt.Columns.Contains(col)) return "0";
            return dt.Rows[0][col].ToString();
        }

        public ActionResult DownloadSMPDailyReport()
        {
            // ==========================
            // FORCE GREGORIAN DATE
            // ==========================
            var greg = new CultureInfo("en-US");
            greg.DateTimeFormat.Calendar = new GregorianCalendar();
            string reportDate = DateTime.Now.ToString("dd-MMM-yyyy", greg);

            // ==========================
            // PDF SETUP
            // ==========================
            MemoryStream ms = new MemoryStream();
            Document pdf = new Document(PageSize.A4, 20, 20, 25, 25);
            PdfWriter.GetInstance(pdf, ms);
            pdf.Open();

            Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14);
            Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8);
            Font normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 8);
            Font boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8);
            BaseColor headerBg = new BaseColor(240, 240, 240);

            // ==========================
            // HEADER
            // ==========================
            PdfPTable header = new PdfPTable(2) { WidthPercentage = 100 };
            header.SetWidths(new float[] { 10, 90 });

            Image logo = Image.GetInstance(Server.MapPath("~/assets/images/logo.png"));
            logo.ScaleToFit(90, 40);
            header.AddCell(new PdfPCell(logo) { Border = Rectangle.NO_BORDER });

            header.AddCell(new PdfPCell(new Phrase("SMP Daily Summary Report", titleFont))
            {
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            });

            pdf.Add(header);
            pdf.Add(new Paragraph("Date: " + reportDate, normalFont));
            pdf.Add(Chunk.NEWLINE);

            // ==========================
            // SUMMARY
            // ==========================
            PdfPTable summary = new PdfPTable(6) { WidthPercentage = 100 };
            string[] sumTitles =
            { "Number of Heats", "DRI (Ton)", "Scrap (Ton)", "Liquid Steel (Ton)", "Casted Weight", "Tap To Tap (Min)" };

            string[] sumValues = { "25", "2500", "300", "2620", "2570", "60.3" };

            for (int i = 0; i < sumTitles.Length; i++)
                summary.AddCell(ValueBox(sumTitles[i], sumValues[i]));

            pdf.Add(summary);
            pdf.Add(Chunk.NEWLINE);

            // ==========================
            // PERFORMANCE
            // ==========================
            BuildMatrix(pdf,
                new[] { "Availability", "Performance", "Yield", "Quality Yield", "EAF Productivity", "CCM Productivity" },
                new[] { "87%", "85%", "87%", "99%", "140", "140" },      // Target
                new[] { "80", "69", "85", "100", "135", "130" }          // Actual
            );

            pdf.Add(Chunk.NEWLINE);

            // ==========================
            // CONSUMPTION
            // ==========================
            BuildMatrix(pdf,
                new[] { "DRI Kg/T", "Scrap Kg/T", "FeSi", "Fe-SiMn", "Fe-Mn", "Rice Husk" },
                new[] { "909", "225", "2", "15", "2", "0.25" },          // Target
                new[] { "1060", "125", "2.1", "12.3", "1.3", "0.30" }    // Actual
            );

            pdf.Add(Chunk.NEWLINE);

            // ==========================
            // ADDITIVES / FLUXES
            // ==========================
            BuildMatrix(pdf,
                new[] { "Lime Kg/T", "Dolo Lime Kg/T", "Charged Coal Kg/T", "Fluorspar Kg/T", "Calcined Carbon Kg/T" },
                new[] { "30", "30", "5", "1", "17" },         // Target
                new[] { "25", "25", "2.1", "0.56", "15.3" }   // Actual
            );

            pdf.Add(Chunk.NEWLINE);

            // ==========================
            // UTILITIES
            // ==========================
            BuildMatrix(pdf,
                new[] { "Power KWH/T", "LPG Nm3/T", "Oxygen Nm3/T", "Argon Nm3/T", "Nitrogen Nm3/T", "Water m3" },
                new[] { "590", "10", "33", "0.16", "1", "1.8" },   // Target
                new[] { "", "", "", "", "", "" }                  // Actual
            );

            pdf.Add(Chunk.NEWLINE);

            // ==========================
            // DELAYS
            // ==========================
            BuildDelayRow(pdf,
                new[] { "Mechanical (Min)", "Electrical (Min)", "Operation (Min)", "Refractory (Min)", "Utility (Min)", "Crane (Min)" },
                new[] { "30", "29", "35", "31", "0", "2" }
            );

            pdf.Add(Chunk.NEWLINE);

            // ==========================
            // REMARKS
            // ==========================
            PdfPTable remarks = new PdfPTable(1) { WidthPercentage = 100 };
            remarks.AddCell(new PdfPCell(new Phrase("Remarks:", boldFont))
            {
                MinimumHeight = 80,
                Padding = 6
            });
            pdf.Add(remarks);

            pdf.Close();
            return File(ms.ToArray(), "application/pdf", "SMP_DailySummaryReport.pdf");



            // ========================================================
            // HELPER METHODS
            // ========================================================

            PdfPCell ValueBox(string title, string value)
            {
                PdfPTable t = new PdfPTable(1);

                t.AddCell(new PdfPCell(new Phrase(title, headerFont))
                {
                    Border = Rectangle.NO_BORDER,
                    HorizontalAlignment = Element.ALIGN_CENTER
                });

                t.AddCell(new PdfPCell(new Phrase(value ?? "", boldFont))
                {
                    Border = Rectangle.BOX,
                    Padding = 4,
                    HorizontalAlignment = Element.ALIGN_CENTER
                });

                return new PdfPCell(t) { Border = Rectangle.NO_BORDER };
            }

            void BuildMatrix(Document d, string[] columns, string[] target, string[] actual)
            {
                PdfPTable tbl = new PdfPTable(columns.Length + 1) { WidthPercentage = 100 };

                tbl.AddCell("");
                foreach (string c in columns)
                    tbl.AddCell(HeaderCell(c));

                tbl.AddCell(LabelCell("Target"));
                foreach (string v in target)
                    tbl.AddCell(ValueCell(v));

                tbl.AddCell(LabelCell("Actual"));
                foreach (string v in actual)
                    tbl.AddCell(ValueCell(v));

                d.Add(tbl);
            }

            void BuildDelayRow(Document d, string[] titles, string[] values)
            {
                PdfPTable tbl = new PdfPTable(titles.Length) { WidthPercentage = 100 };

                foreach (string t in titles)
                    tbl.AddCell(HeaderCell(t));

                foreach (string v in values)
                    tbl.AddCell(ValueCell(v));

                d.Add(tbl);
            }

            PdfPCell HeaderCell(string text)
                => new PdfPCell(new Phrase(text, headerFont))
                { BackgroundColor = headerBg, HorizontalAlignment = Element.ALIGN_CENTER };

            PdfPCell LabelCell(string text)
                => new PdfPCell(new Phrase(text, boldFont))
                { HorizontalAlignment = Element.ALIGN_CENTER };

            PdfPCell ValueCell(string text)
                => new PdfPCell(new Phrase(text ?? "", boldFont))
                { HorizontalAlignment = Element.ALIGN_CENTER };
        }

        public ActionResult DownloadDailyPlantStatistics()
        {
            MemoryStream ms = new MemoryStream();
            Document pdf = new Document(PageSize.A3.Rotate(), 10, 10, 20, 20);
            PdfWriter.GetInstance(pdf, ms);
            pdf.Open();

            Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14);
            Font groupFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8);
            Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 7);
            Font dataFont = FontFactory.GetFont(FontFactory.HELVETICA, 7);

            BaseColor headerBg = new BaseColor(255, 230, 170);
            BaseColor grid = BaseColor.BLACK;

            string monthYear = DateTime.Now.ToString("MMMM, yyyy");

            // ------------------ TITLE ------------------
            Paragraph title = new Paragraph(
                "STEEL PLANT DAILY PRODUCTION STATISTICS\nFOR THE MONTH " + monthYear.ToUpper(),
                titleFont
            );
            title.Alignment = Element.ALIGN_CENTER;
            title.SpacingAfter = 10;
            pdf.Add(title);

            // ------------------ TABLE ------------------
            int COLS = 36;
            PdfPTable table = new PdfPTable(COLS);
            table.WidthPercentage = 100;

            float[] widths = {
        1.2f,1.2f,1.2f,1.2f,1.4f,1.2f,1.2f,        // Charge
        1.4f,1.4f,1.2f,1.4f,                      // Tapped Steel
        1.2f,1.2f,1.2f,1.4f,                      // Billets
        1.2f,                                     // Flux
        1.0f,                                     // GM %
        1.2f,                                     // Prod Rate
        1.2f,                                     // Delay
        1.2f,                                     // TAT
        1.2f,                                     // Tap
        1.2f,                                     // Prod Rate
        1.2f,1.2f,                                // KWH
        1.2f,1.2f,1.2f,                           // Lime & Mg
        1.2f,1.2f,                                // EAF SAF
        1.2f,                                     // Tilting
        1.2f,1.2f,1.2f,1.2f,1.2f,                  // Quality %
        1.2f                                      // Melt Time
    };
            table.SetWidths(widths);

            // ------------------ HELPERS ------------------
            PdfPCell GCell(string t, int colspan) => new PdfPCell(new Phrase(t, groupFont))
            {
                Colspan = colspan,
                HorizontalAlignment = Element.ALIGN_CENTER,
                BackgroundColor = headerBg,
                BorderColor = grid
            };

            PdfPCell HCell(string t) => new PdfPCell(new Phrase(t, headerFont))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                BackgroundColor = headerBg,
                BorderColor = grid,
                Padding = 3
            };

            PdfPCell DCell(string t = "") => new PdfPCell(new Phrase(t, dataFont))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                BorderColor = grid
            };

            // ------------------ GROUP HEADER ROW ------------------
            table.AddCell(GCell("Charge", 7));
            table.AddCell(GCell("Tapped Steel", 4));
            table.AddCell(GCell("Billets", 4));
            table.AddCell(GCell("Flux", 1));
            table.AddCell(GCell("Power", 1));
            table.AddCell(GCell("Delay", 1));
            table.AddCell(GCell("TAT", 1));
            table.AddCell(GCell("Tap-to-tap", 1));
            table.AddCell(GCell("Prod Rate", 1));
            table.AddCell(GCell("KWH", 2));
            table.AddCell(GCell("Additives", 3));
            table.AddCell(GCell("Oxygen", 1));
            table.AddCell(GCell("Productivity", 1));
            table.AddCell(GCell("Quality", 5));
            table.AddCell(GCell("Melt Time", 1));

            // ------------------ COLUMN HEADER ROW ------------------
            string[] headers = {
        "Scrap\nTon/H","HBI\nTon/H","DRI\nTon/H","Fe Alloy","Total Metal",
        "Baskets","Heats",
        "Tapped","Total","Pig","Net Steel",
        "150 mm","130 mm","Reject","Total",
        "Additives",
        "GM %",
        "Ton/Hr",
        "Delay",
        "TAT",
        "Tap",
        "Ton/Hr",
        "Per Ton","EAF",
        "Lime EAF","Lime LF","Mag.",
        "EAF","SAF",
        "Tilting",
        "Scrap %","HBI %","DRI %","Loss %","Liquid %",
        "Melt %"
    };

            foreach (var h in headers)
                table.AddCell(HCell(h));

            // ------------------ DUMMY DATA ------------------
            for (int d = 1; d <= 30; d++)
            {
                table.AddCell(DCell(d.ToString()));
                for (int i = 1; i < COLS; i++)
                    table.AddCell(DCell((i * 1.2).ToString("0.0")));
            }

            // ------------------ FOOTER ------------------
            for (int i = 0; i < COLS; i++)
                table.AddCell(HCell("AVG"));

            for (int i = 0; i < COLS; i++)
                table.AddCell(HCell("SUM"));

            pdf.Add(table);
            pdf.Close();
            return File(ms.ToArray(), "application/pdf", "Steel_Plant_Daily_Report.pdf");
        }

        public ActionResult ShiftProductionReport(DateTime? date, string plant, string shift, bool download = false)
        {
            try
            {
                DateTime selectedDate = date ?? DateTime.Today;

                string selectedPlant = string.IsNullOrWhiteSpace(plant) ? "" : plant.Trim();
                string selectedShift = string.IsNullOrWhiteSpace(shift) ? "" : shift.Trim();

                var dischargedData = rm.GetDichargedHeat(selectedDate, selectedDate, selectedShift)
                                    ?? new List<BilletDischargingBLL>();

                var delayData = repo.GetAllRMDelay(selectedDate, selectedDate, selectedShift)
                                ?? new List<PlantDelayBLL>();

                if (!string.IsNullOrWhiteSpace(selectedPlant))
                {
                    dischargedData = dischargedData
                        .Where(x =>
                            !string.IsNullOrWhiteSpace(x.Plant) &&
                            x.Plant.Trim().Equals(selectedPlant, StringComparison.OrdinalIgnoreCase)
                        )
                        .ToList();

                    delayData = delayData
                        .Where(x =>
                            !string.IsNullOrWhiteSpace(x.Plant) &&
                            x.Plant.Trim().Equals(selectedPlant, StringComparison.OrdinalIgnoreCase)
                        )
                        .ToList();
                }

                var shiftDetails = rm.RollingMillDetails()
                    .Where(x =>
                        x.Date >= selectedDate.Date &&
                        x.Date < selectedDate.Date.AddDays(1) &&
                        x.StatusID == 1 &&
                        (
                            string.IsNullOrWhiteSpace(selectedPlant) ||
                            (
                                !string.IsNullOrWhiteSpace(x.Plant) &&
                                x.Plant.Trim().Equals(selectedPlant, StringComparison.OrdinalIgnoreCase)
                            )
                        ) &&
                        (
                            string.IsNullOrWhiteSpace(selectedShift) ||
                            (
                                !string.IsNullOrWhiteSpace(x.Shift) &&
                                x.Shift.Trim().Equals(selectedShift, StringComparison.OrdinalIgnoreCase)
                            )
                        )
                    )
                    .OrderByDescending(x => x.ID)
                    .FirstOrDefault();

                var vm = new ShiftProductionReportVM
                {
                    Delays = delayData,
                    DischargedHeats = dischargedData
                };

                ViewBag.TotalBundles = vm.DischargedHeats.Count;

                ViewBag.Cobble = vm.Delays
                    .Where(x => !string.IsNullOrWhiteSpace(x.DelayType))
                    .Count(x => x.DelayType.Trim().Equals("Cobble", StringComparison.OrdinalIgnoreCase));

                ViewBag.HotOut = vm.Delays
                    .Where(x => !string.IsNullOrWhiteSpace(x.DelayType))
                    .Count(x => x.DelayType.Trim().Equals("HotOut", StringComparison.OrdinalIgnoreCase));

                ViewBag.Date = selectedDate;
                ViewBag.From = selectedDate;
                ViewBag.To = selectedDate;

                ViewBag.Plant = string.IsNullOrWhiteSpace(selectedPlant) ? "All" : selectedPlant;
                ViewBag.Shift = string.IsNullOrWhiteSpace(selectedShift) ? "All" : selectedShift;

                ViewBag.ReportDate = (shiftDetails?.Date ?? selectedDate).ToString("dd/MM/yyyy");
                ViewBag.ReportPlant = shiftDetails?.Plant ?? selectedPlant;
                ViewBag.ReportShift = shiftDetails?.Shift ?? selectedShift;
                ViewBag.ReportTeam = shiftDetails?.Team ?? "";
                ViewBag.ReportShiftIncharge = shiftDetails?.ShiftIncharge ?? "";

                if (download)
                {
                    string fileName = "Shift_Production_Report_" + selectedDate.ToString("yyyyMMdd") + ".pdf";

                    return new ViewAsPdf("ShiftProductionReport", vm)
                    {
                        FileName = fileName,
                        PageSize = Rotativa.Options.Size.A4,
                        PageOrientation = Rotativa.Options.Orientation.Landscape,
                        PageMargins = new Rotativa.Options.Margins
                        {
                            Top = 5,
                            Bottom = 5,
                            Left = 5,
                            Right = 5
                        }
                    };
                }

                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error: " + ex.Message;
                return RedirectToAction("RMReports");
            }
        }

        public ActionResult DailyProductionRM()
        {
            return View();
        }

        public ActionResult RMMonthlyPerformanceReport()
        {
            return View();
        }

        public ActionResult ReportingSection()
        {
            return View();
        }

        public ActionResult SMPDailySummary()
        {
            return View();
        }

        public ActionResult SMPProductionSummary()
        {
            return View();
        }

        //    public ActionResult ShiftProductionDashboard(DateTime? date, string plant, string shift)
        //    {
        //        DateTime selectedDate = date ?? DateTime.Today;

        //        string selectedPlant = string.IsNullOrWhiteSpace(plant) ? "" : plant.Trim();
        //        string selectedShift = string.IsNullOrWhiteSpace(shift) ? "" : shift.Trim();

        //        var dischargedData = rm.GetDichargedHeat(selectedDate, selectedDate, selectedShift)
        //                            ?? new List<BilletDischargingBLL>();

        //        var delayData = repo.GetAllRMDelay(selectedDate, selectedDate, selectedShift)
        //                        ?? new List<PlantDelayBLL>();

        //        if (!string.IsNullOrWhiteSpace(selectedPlant))
        //        {
        //            dischargedData = dischargedData
        //                .Where(x =>
        //                    !string.IsNullOrWhiteSpace(x.Plant) &&
        //                    x.Plant.Trim().Equals(selectedPlant, StringComparison.OrdinalIgnoreCase)
        //                )
        //                .ToList();

        //            delayData = delayData
        //                .Where(x =>
        //                    !string.IsNullOrWhiteSpace(x.Plant) &&
        //                    x.Plant.Trim().Equals(selectedPlant, StringComparison.OrdinalIgnoreCase)
        //                )
        //                .ToList();
        //        }

        //        var shiftDetails = rm.RollingMillDetails()
        //            .Where(x =>
        //                x.Date >= selectedDate.Date &&
        //                x.Date < selectedDate.Date.AddDays(1) &&
        //                x.StatusID == 1 &&
        //                (
        //                    string.IsNullOrWhiteSpace(selectedPlant) ||
        //                    (
        //                        !string.IsNullOrWhiteSpace(x.Plant) &&
        //                        x.Plant.Trim().Equals(selectedPlant, StringComparison.OrdinalIgnoreCase)
        //                    )
        //                ) &&
        //                (
        //                    string.IsNullOrWhiteSpace(selectedShift) ||
        //                    (
        //                        !string.IsNullOrWhiteSpace(x.Shift) &&
        //                        x.Shift.Trim().Equals(selectedShift, StringComparison.OrdinalIgnoreCase)
        //                    )
        //                )
        //            )
        //            .OrderByDescending(x => x.ID)
        //            .FirstOrDefault();

        //        var vm = new ShiftProductionReportVM
        //        {
        //            Delays = delayData,
        //            DischargedHeats = dischargedData
        //        };

        //        ViewBag.ReportDate = (shiftDetails?.Date ?? selectedDate).ToString("dd/MM/yyyy");
        //        ViewBag.ReportPlant = shiftDetails?.Plant ?? selectedPlant;
        //        //ViewBag.ReportShift = shiftDetails?.Shift ?? selectedShift;

        //        var reportShifts = vm.DischargedHeats != null
        //? vm.DischargedHeats
        //    .Where(x => !string.IsNullOrWhiteSpace(x.Shift))
        //    .Select(x => x.Shift.Trim())
        //    .Distinct(StringComparer.OrdinalIgnoreCase)
        //    .ToList()
        //: new List<string>();

        //        ViewBag.ReportShift = reportShifts.Any()
        //            ? string.Join(", ", reportShifts)
        //            : selectedShift;

        //        ViewBag.ReportTeam = shiftDetails?.Team ?? "";
        //        ViewBag.ReportShiftIncharge = shiftDetails?.ShiftIncharge ?? "";

        //        return View(vm);
        //    }

        public ActionResult ShiftProductionDashboard(
    DateTime? fromdate,
    DateTime? todate,
    string plant,
    string shift)
        {
            // =========================================================
            // DATE FILTER
            // =========================================================

            DateTime fromDate = (fromdate ?? DateTime.Today).Date;
            DateTime toDate = (todate ?? fromDate).Date;

            if (fromDate > toDate)
            {
                DateTime tempDate = fromDate;
                fromDate = toDate;
                toDate = tempDate;
            }

            string selectedPlant = string.IsNullOrWhiteSpace(plant)
                ? string.Empty
                : plant.Trim();

            string selectedShift = string.IsNullOrWhiteSpace(shift)
                ? string.Empty
                : shift.Trim();

            // =========================================================
            // MONTHLY TARGET
            // Selected To Date ke month/year ka target load hoga
            // =========================================================

            string targetMonth = toDate.ToString(
                "MMMM",
                System.Globalization.CultureInfo.InvariantCulture
            );

            string targetYear = toDate.Year.ToString();

            RollingMillTargetsBLL monthlyTarget =
                targetRepo.GetByMonthYear(
                    targetMonth,
                    targetYear
                );

            // =========================================================
            // SELECTED FILTER PRODUCTION DATA
            // KPI aur detail table ke liye
            // =========================================================

            var dischargedData = rm.GetDichargedHeats(
                fromDate,
                toDate,
                selectedPlant,
                selectedShift
            ) ?? new List<BilletDischargingBLL>();

            // =========================================================
            // ACCUMULATED MONTH-TO-DATE PRODUCTION DATA
            // Is data mein koi downtime deduction nahi hogi
            // =========================================================

            DateTime monthStartDate = new DateTime(
                toDate.Year,
                toDate.Month,
                1
            );

            var dailyTarget =
    dailyTargetRepo.GetByDate(toDate);

            ViewBag.DailyProductionTarget =
                dailyTarget != null
                    ? dailyTarget.DailyProductionTarget
                    : 0;

            ViewBag.DailyFuelConsumption =
                dailyTarget != null
                    ? dailyTarget.FuelConsumption
                    : 0;

            var monthlyChartData = rm.GetDichargedHeats(
                monthStartDate,
                toDate,
                selectedPlant,
                selectedShift
            ) ?? new List<BilletDischargingBLL>();

            // =========================================================
            // DELAY DATA
            // Sirf downtime calculations/chart ke liye
            // =========================================================

            var delayData = repo.GetAllRMDelay(
                fromDate,
                toDate,
                selectedShift
            ) ?? new List<PlantDelayBLL>();

            // =========================================================
            // SAFE PLANT FILTER
            // =========================================================

            if (!string.IsNullOrWhiteSpace(selectedPlant))
            {
                dischargedData = dischargedData
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(x.Plant) &&
                        x.Plant.Trim().Equals(
                            selectedPlant,
                            StringComparison.OrdinalIgnoreCase
                        )
                    )
                    .ToList();

                monthlyChartData = monthlyChartData
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(x.Plant) &&
                        x.Plant.Trim().Equals(
                            selectedPlant,
                            StringComparison.OrdinalIgnoreCase
                        )
                    )
                    .ToList();

                delayData = delayData
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(x.Plant) &&
                        x.Plant.Trim().Equals(
                            selectedPlant,
                            StringComparison.OrdinalIgnoreCase
                        )
                    )
                    .ToList();
            }

            // =========================================================
            // SAFE SHIFT FILTER
            // =========================================================

            if (!string.IsNullOrWhiteSpace(selectedShift))
            {
                dischargedData = dischargedData
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(x.Shift) &&
                        x.Shift.Trim().Equals(
                            selectedShift,
                            StringComparison.OrdinalIgnoreCase
                        )
                    )
                    .ToList();

                monthlyChartData = monthlyChartData
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(x.Shift) &&
                        x.Shift.Trim().Equals(
                            selectedShift,
                            StringComparison.OrdinalIgnoreCase
                        )
                    )
                    .ToList();

                delayData = delayData
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(x.Shift) &&
                        x.Shift.Trim().Equals(
                            selectedShift,
                            StringComparison.OrdinalIgnoreCase
                        )
                    )
                    .ToList();
            }

            // =========================================================
            // SHIFT DETAILS
            // =========================================================

            var shiftDetailsList = rm.RollingMillDetails()
                .Where(x =>
                    x.StatusID == 1 &&
                    x.Date >= fromDate &&
                    x.Date < toDate.AddDays(1)
                )
                .ToList();

            if (!string.IsNullOrWhiteSpace(selectedPlant))
            {
                shiftDetailsList = shiftDetailsList
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(x.Plant) &&
                        x.Plant.Trim().Equals(
                            selectedPlant,
                            StringComparison.OrdinalIgnoreCase
                        )
                    )
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(selectedShift))
            {
                shiftDetailsList = shiftDetailsList
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(x.Shift) &&
                        x.Shift.Trim().Equals(
                            selectedShift,
                            StringComparison.OrdinalIgnoreCase
                        )
                    )
                    .ToList();
            }

            var shiftDetails = shiftDetailsList
                .OrderByDescending(x => x.Date)
                .ThenByDescending(x => x.ID)
                .FirstOrDefault();

            // Total scheduled shift minutes for the complete selected period.
            // Each shift-detail record is counted separately, so a monthly report
            // does not incorrectly use only one day's 24 hours.
            Func<string, decimal> getShiftHours = shiftName =>
            {
                if (string.IsNullOrWhiteSpace(shiftName))
                    return 0;

                string value = shiftName.Trim();

                if (value.Equals("Morning", StringComparison.OrdinalIgnoreCase) ||
                    value.Equals("Evening", StringComparison.OrdinalIgnoreCase) ||
                    value.Equals("Night", StringComparison.OrdinalIgnoreCase))
                {
                    return 8;
                }

                if (value.Equals("Long Morning", StringComparison.OrdinalIgnoreCase) ||
                    value.Equals("Long Night", StringComparison.OrdinalIgnoreCase))
                {
                    return 12;
                }

                return 0;
            };

            decimal totalShiftMinutes = shiftDetailsList
                .GroupBy(x => new
                {
                    ShiftDate = x.Date.Date,
                    ShiftName = (x.Shift ?? string.Empty).Trim().ToUpper()
                })
                .Select(g => g.First())
                .Sum(x => getShiftHours(x.Shift) * 60);

            // Fallback only when no shift-detail records are available.
            if (totalShiftMinutes <= 0)
            {
                int totalDays = (toDate - fromDate).Days + 1;

                if (!string.IsNullOrWhiteSpace(selectedShift))
                {
                    totalShiftMinutes = getShiftHours(selectedShift) * 60 * totalDays;
                }
                else
                {
                    totalShiftMinutes = 24 * 60 * totalDays;
                }
            }

            // =========================================================
            // VIEW MODEL
            // =========================================================

            var vm = new ShiftProductionReportVM
            {
                Delays = delayData,
                DischargedHeats = dischargedData,

                // Accumulated chart isi separate month-to-date list se banega
                MonthlyProductionData = monthlyChartData,

                RollingMillTarget = monthlyTarget
            };

            // =========================================================
            // VIEWBAGS
            // =========================================================

            ViewBag.FromDate = fromDate.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate.ToString("yyyy-MM-dd");

            ViewBag.SelectedPlant = selectedPlant;
            ViewBag.SelectedShift = selectedShift;

            ViewBag.TargetMonth = targetMonth;
            ViewBag.TargetYear = targetYear;
            ViewBag.TotalShiftMinutes = totalShiftMinutes;

            ViewBag.ReportDate = fromDate == toDate
                ? fromDate.ToString("dd/MM/yyyy")
                : fromDate.ToString("dd/MM/yyyy") +
                  " - " +
                  toDate.ToString("dd/MM/yyyy");

            ViewBag.ReportPlant =
                !string.IsNullOrWhiteSpace(selectedPlant)
                    ? selectedPlant
                    : "All Plants";

            var reportShifts = dischargedData
                .Where(x => !string.IsNullOrWhiteSpace(x.Shift))
                .Select(x => x.Shift.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            ViewBag.ReportShift = reportShifts.Any()
                ? string.Join(", ", reportShifts)
                : !string.IsNullOrWhiteSpace(selectedShift)
                    ? selectedShift
                    : "All Shifts";

            ViewBag.ReportTeam =
                shiftDetails != null
                    ? shiftDetails.Team ?? string.Empty
                    : string.Empty;

            ViewBag.ReportShiftIncharge =
                shiftDetails != null
                    ? shiftDetails.ShiftIncharge ?? string.Empty
                    : string.Empty;

            return View(vm);
        }

        public ActionResult UtilityDailyReport(DateTime? date)
        {
            try
            {
                DateTime reportDate = date ?? DateTime.Today;

                var vm = crepo.GetUtilityDailyReport(reportDate);

                if (vm == null)
                {
                    vm = new UtilityDailyReportVM();
                    vm.ReportDate = reportDate;
                }

                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error: " + ex.Message;
                return RedirectToAction("UtilityDailyReport", "Reporting");
            }
        }


        public ActionResult CMDDailyReport(
            DateTime? fromDate,
            DateTime? toDate)
        {
            DateTime selectedFromDate = fromDate?.Date ?? DateTime.Today;
            DateTime selectedToDate = toDate?.Date ?? DateTime.Today;

            if (selectedFromDate > selectedToDate)
            {
                TempData["Error"] = "From Date cannot be greater than To Date.";

                selectedFromDate = DateTime.Today;
                selectedToDate = DateTime.Today;
            }

            var model = mrepo.GetDashboard(
                selectedFromDate,
                selectedToDate
            );

            return View(model);
        }

        //public ActionResult CMDDailyReport()
        //{
        //    return View();
        //}

        public ActionResult SupplyChainReport(DateTime? from, DateTime? to, bool download = false)
        {
            DateTime fromDate = from ?? DateTime.Today;
            DateTime toDate = to ?? fromDate;

            var model = srepo.GetSupplyChainDailyReport(fromDate, toDate);

            ViewBag.FromDate = fromDate.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate.ToString("yyyy-MM-dd");

            return View(model);
        }

        public ActionResult SupplyChainPrint(DateTime? from, DateTime? to)
        {
            DateTime fromDate = from ?? DateTime.Today;
            DateTime toDate = to ?? fromDate;

            var model = srepo.GetSupplyChainDailyReport(fromDate, toDate);

            return View(model);
        }
    }
}
