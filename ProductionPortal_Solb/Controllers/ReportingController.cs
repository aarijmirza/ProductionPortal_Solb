using iTextSharp.text.pdf;
using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OfficeOpenXml;
using Spire.Xls;
using System.Configuration;
using System.Data.SqlClient;
using WebAPICode.Helpers;
using DAL.Models;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System.Globalization;
using BAL.Repositories;
using static DAL.Models.ViewModel;
using System.Windows.Media.Imaging;
using Rotativa;
using Rotativa.Options;

namespace ProductionPortal_Solb.Controllers
{
    public class ReportingController : Controller
    {
        DelayRespository repo;
        RollingMillRepository rm;
        public ReportingController()
        {
            repo = new DelayRespository();
            rm = new RollingMillRepository();
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

        //public ActionResult DownloadDailyProductionPDF()
        //{
        //    string templatePath = Server.MapPath("~/Templates/DailyProductionReport.xlsx");
        //    string excelPath = Server.MapPath("~/Temp/DPR_" + DateTime.Now.Ticks + ".xlsx");
        //    string pdfPath = Server.MapPath("~/Temp/DPR_" + DateTime.Now.Ticks + ".pdf");

        //    if (!System.IO.File.Exists(templatePath))
        //        return Content("Excel template not found.");

        //    // 1) LOAD DATA FROM DATABASE
        //    DataTable dt = GetSMPDailyReportData();   // YOUR DATABASE METHOD

        //    // 2) FILL EXCEL
        //    using (var package = new OfficeOpenXml.ExcelPackage(new FileInfo(templatePath)))
        //    {
        //        var ws = package.Workbook.Worksheets["Daily Report"];

        //        if (ws == null)
        //            return Content("Sheet 'Daily Report' not found.");

        //        // ============== HEADER DATE =================
        //        ws.Cells["H2"].Value = DateTime.Now.ToString("dd MMM yyyy");

        //        // ============== PRODUCTION SUMMARY ==========
        //        ws.Cells["D7"].Value = GetVal(dt, "ActualProduction");
        //        ws.Cells["E7"].Value = GetVal(dt, "Diff");
        //        ws.Cells["F7"].Value = GetVal(dt, "FuelConsumption");
        //        ws.Cells["G7"].Value = GetVal(dt, "PowerConsumption");
        //        ws.Cells["H7"].Value = GetVal(dt, "WaterConsumption");
        //        ws.Cells["I7"].Value = GetVal(dt, "Misroll");
        //        ws.Cells["J7"].Value = GetVal(dt, "Chopping");
        //        ws.Cells["K7"].Value = GetVal(dt, "ActualYield");
        //        ws.Cells["L7"].Value = GetVal(dt, "TheoreticalYield");
        //        ws.Cells["M7"].Value = GetVal(dt, "RRR_Day");
        //        ws.Cells["N7"].Value = GetVal(dt, "RRR_YTD");
        //        ws.Cells["O7"].Value = GetVal(dt, "Prod_Day");
        //        ws.Cells["P7"].Value = GetVal(dt, "Prod_YTD");

        //        // ============== MONTHLY =====================
        //        ws.Cells["C11"].Value = GetVal(dt, "MonthPlan");
        //        ws.Cells["D11"].Value = GetVal(dt, "MonthActual");
        //        ws.Cells["E11"].Value = GetVal(dt, "MonthDiff");

        //        // ============== YEARLY ======================
        //        ws.Cells["C14"].Value = GetVal(dt, "YearPlan");
        //        ws.Cells["D14"].Value = GetVal(dt, "YearActual");

        //        package.SaveAs(new FileInfo(excelPath));
        //    }

        //    // 3) CONVERT EXCEL TO PDF (PRESERVES CHARTS & COLORS)
        //    Spire.Xls.Workbook book = new Spire.Xls.Workbook();
        //    book.LoadFromFile(excelPath);
        //    book.SaveToFile(pdfPath, Spire.Xls.FileFormat.PDF);

        //    // 4) RETURN PDF DOWNLOAD
        //    byte[] pdfBytes = System.IO.File.ReadAllBytes(pdfPath);

        //    // Clean temp files
        //    System.IO.File.Delete(excelPath);
        //    System.IO.File.Delete(pdfPath);

        //    return File(pdfBytes, "application/pdf", "DailyProductionReport.pdf");
        //}

        //private object GetVal(DataTable dt1, string v)
        //{
        //    DataTable dt = new DataTable();
        //    dt.Columns.Add("ActualProduction");
        //    dt.Columns.Add("Diff");
        //    dt.Columns.Add("FuelConsumption");
        //    dt.Columns.Add("PowerConsumption");
        //    dt.Columns.Add("WaterConsumption");
        //    dt.Columns.Add("Misroll");
        //    dt.Columns.Add("Chopping");
        //    dt.Columns.Add("ActualYield");
        //    dt.Columns.Add("TheoreticalYield");
        //    dt.Columns.Add("RRR_Day");
        //    dt.Columns.Add("RRR_YTD");
        //    dt.Columns.Add("Prod_Day");
        //    dt.Columns.Add("Prod_YTD");
        //    dt.Columns.Add("MonthPlan");
        //    dt.Columns.Add("MonthActual");
        //    dt.Columns.Add("MonthDiff");
        //    dt.Columns.Add("YearPlan");
        //    dt.Columns.Add("YearActual");

        //    dt.Rows.Add(972.25, 230, 32.2, 131.43, 0.16, 4.201, 0, 97.16, 99.11, 75.79, 77.34, 73.66, 80.67, 26469, 26699, 230, 125371, 127569);

        //    return dt;
        //}

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

        // ==========================
        // SAFE FUNCTION (DEFAULT 0)
        // ==========================
        public string Safe(DataTable dt, string col)
        {
            if (dt == null || dt.Rows.Count == 0) return "0";
            if (!dt.Columns.Contains(col)) return "0";
            return dt.Rows[0][col].ToString();
        }

        //public ActionResult DownloadSMPDailyReport()
        //{
        //    // ===== DATA (replace with DB later) =====
        //    string reportDate = DateTime.Now.ToString("dd-MMM-yyyy");

        //    // SUMMARY
        //    string heats = "25", dri = "2500", scrap = "300", liquid = "2620", casted = "2570", ttt = "60.3";

        //    // KPI
        //    string availT = "87%", perfT = "85%", yieldT = "87%", qYieldT = "99%", eafT = "140", ccmT = "140";
        //    string availA = "80", perfA = "69", yieldA = "85", qYieldA = "100", eafA = "135", ccmA = "130";

        //    // CONSUMPTION
        //    string driKg = "1060", scrapKg = "125", fesi = "2.1", fesmn = "12.3", femn = "1.3", rice = "0.3";
        //    string lime = "25", dolo = "25", coal = "2.1", fluorspar = "0.56", carbon = "15.3", blank = "";

        //    // POWER
        //    string power = "590", lpg = "10", o2 = "33", ar = "0.16", n2 = "1", water = "1.8";

        //    // DELAY
        //    string mech = "30", elec = "29", opr = "35", refra = "31", utility = "0", crane = "2";

        //    // ===== PDF =====
        //    MemoryStream ms = new MemoryStream();
        //    Document pdf = new Document(PageSize.A4, 20, 20, 20, 20);
        //    PdfWriter.GetInstance(pdf, ms);
        //    pdf.Open();

        //    Font title = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
        //    Font bold = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9);
        //    Font normal = FontFactory.GetFont(FontFactory.HELVETICA, 9);

        //    BaseColor boxGray = new BaseColor(245, 245, 245);
        //    float bw = 0.7f;

        //    // ===== HEADER =====
        //    PdfPTable head = new PdfPTable(2);
        //    head.WidthPercentage = 100;
        //    head.SetWidths(new float[] { 20, 80 });

        //    Image logo = Image.GetInstance(Server.MapPath("~/assets/images/logo.png"));
        //    logo.ScaleAbsolute(60, 30);
        //    head.AddCell(new PdfPCell(logo) { Border = Rectangle.NO_BORDER });

        //    head.AddCell(new PdfPCell(new Phrase("SMP Daily Performance Report", title))
        //    {
        //        VerticalAlignment = Element.ALIGN_CENTER,
        //        Border = Rectangle.NO_BORDER,
        //        PaddingTop = 10
        //    });

        //    pdf.Add(head);
        //    pdf.Add(new Paragraph($"Date: {reportDate}", bold));
        //    pdf.Add(Chunk.NEWLINE);

        //    // ===== HELPER =====
        //    PdfPCell Box(string h, string v, string u)
        //    {
        //        PdfPTable t = new PdfPTable(1);
        //        t.AddCell(new PdfPCell(new Phrase(h, normal)));
        //        t.AddCell(new PdfPCell(new Phrase(v + " " + u, bold)) { HorizontalAlignment = Element.ALIGN_CENTER });

        //        return new PdfPCell(t)
        //        {
        //            BorderWidth = bw,
        //            Padding = 4,
        //            BackgroundColor = boxGray
        //        };
        //    }

        //    // ===== KPI ROW =====
        //    PdfPTable row1 = new PdfPTable(6); row1.WidthPercentage = 100;
        //    row1.AddCell(Box("Number of Heats", heats, "Total"));
        //    row1.AddCell(Box("DRI", dri, "Ton"));
        //    row1.AddCell(Box("Scrap", scrap, "Ton"));
        //    row1.AddCell(Box("Liquid Steel", liquid, "Ton"));
        //    row1.AddCell(Box("Casted Weight", casted, "Ton"));
        //    row1.AddCell(Box("Tap to Tap", ttt, "Min"));
        //    pdf.Add(row1);

        //    pdf.Add(Chunk.NEWLINE);

        //    // ===== TARGET / ACTUAL =====
        //    PdfPTable perf = new PdfPTable(6); perf.WidthPercentage = 100;
        //    perf.AddCell(Box("Availability", availT, ""));
        //    perf.AddCell(Box("Performance", perfT, ""));
        //    perf.AddCell(Box("Yield", yieldT, ""));
        //    perf.AddCell(Box("Quality Yield", qYieldT, ""));
        //    perf.AddCell(Box("EAF Productivity", eafT, "TPH"));
        //    perf.AddCell(Box("CCM Productivity", ccmT, "TPH"));

        //    perf.AddCell(Box("Actual", availA, ""));
        //    perf.AddCell(Box("Actual", perfA, ""));
        //    perf.AddCell(Box("Actual", yieldA, ""));
        //    perf.AddCell(Box("Actual", qYieldA, ""));
        //    perf.AddCell(Box("Actual", eafA, "TPH"));
        //    perf.AddCell(Box("Actual", ccmA, "TPH"));
        //    pdf.Add(perf);

        //    pdf.Add(Chunk.NEWLINE);

        //    // ===== CONSUMPTION =====
        //    PdfPTable cons = new PdfPTable(6); cons.WidthPercentage = 100;
        //    cons.AddCell(Box("DRI", driKg, "kg/t"));
        //    cons.AddCell(Box("Scrap", scrapKg, "kg/t"));
        //    cons.AddCell(Box("Fe-Si", fesi, "kg/t"));
        //    cons.AddCell(Box("Fe-SiMn", fesmn, "kg/t"));
        //    cons.AddCell(Box("Fe-Mn", femn, "kg/t"));
        //    cons.AddCell(Box("Rice Husk", rice, "kg/t"));

        //    cons.AddCell(Box("Lime", lime, "kg/t"));
        //    cons.AddCell(Box("Dolo Lime", dolo, "kg/t"));
        //    cons.AddCell(Box("Charge Coal", coal, "kg/t"));
        //    cons.AddCell(Box("Fluorspar", fluorspar, "kg/t"));
        //    cons.AddCell(Box("Clacined Carbon", carbon, "kg/t"));
        //    cons.AddCell(Box("", blank, ""));
        //    pdf.Add(cons);

        //    pdf.Add(Chunk.NEWLINE);

        //    // ===== POWER =====
        //    PdfPTable util = new PdfPTable(6); util.WidthPercentage = 100;
        //    util.AddCell(Box("Power", power, "kWh/t"));
        //    util.AddCell(Box("LPG", lpg, "Nm3/t"));
        //    util.AddCell(Box("Oxygen", o2, "Nm3/t"));
        //    util.AddCell(Box("Argon", ar, "Nm3/t"));
        //    util.AddCell(Box("Nitrogen", n2, "Nm3/t"));
        //    util.AddCell(Box("Water", water, "m3"));
        //    pdf.Add(util);

        //    pdf.Add(Chunk.NEWLINE);

        //    // ===== DELAYS =====
        //    PdfPTable delay = new PdfPTable(6); delay.WidthPercentage = 100;
        //    delay.AddCell(Box("Mechanical", mech, "min"));
        //    delay.AddCell(Box("Electrical", elec, "min"));
        //    delay.AddCell(Box("Operation", opr, "min"));
        //    delay.AddCell(Box("Refractory", refra, "min"));
        //    delay.AddCell(Box("Utility", utility, "min"));
        //    delay.AddCell(Box("Crane", crane, "min"));
        //    pdf.Add(delay);

        //    // ===== REMARKS =====
        //    pdf.Add(Chunk.NEWLINE);
        //    PdfPCell remarks = new PdfPCell(new Phrase("Remarks:", bold))
        //    {
        //        MinimumHeight = 80,
        //        BorderWidth = bw,
        //        Padding = 6
        //    };

        //    PdfPTable rem = new PdfPTable(1); rem.WidthPercentage = 100;
        //    rem.AddCell(remarks);
        //    pdf.Add(rem);

        //    pdf.Close();

        //    Response.AppendHeader("Content-Disposition", "attachment; filename=SMP_Daily_Report.pdf");
        //    return File(ms.ToArray(), "application/pdf");
        //}

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


        //    public ActionResult DownloadSteelPlantProductionPDF()
        //    {
        //        // ------------ current month / year ------------
        //        DateTime now = DateTime.Now;
        //        string monthName = now.ToString("MMMM").ToUpper();    // e.g. NOVEMBER
        //        int year = now.Year;
        //        int daysInMonth = DateTime.DaysInMonth(year, now.Month);

        //        // ------------ pdf setup ------------
        //        MemoryStream ms = new MemoryStream();
        //        Document pdf = new Document(PageSize.A3.Rotate(), 8, 8, 10, 10);
        //        PdfWriter.GetInstance(pdf, ms);
        //        pdf.Open();

        //        Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14);
        //        Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 7);
        //        Font normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 7);
        //        Font boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 7);

        //        BaseColor yellow = new BaseColor(255, 223, 128);
        //        BaseColor gray = new BaseColor(230, 230, 230);
        //        float bw = 0.6f;

        //        // ------------ helpers ------------
        //        PdfPCell GroupHeader(string text, int colspan)
        //        {
        //            return new PdfPCell(new Phrase(text, headerFont))
        //            {
        //                Colspan = colspan,
        //                BackgroundColor = yellow,
        //                HorizontalAlignment = Element.ALIGN_CENTER,
        //                VerticalAlignment = Element.ALIGN_MIDDLE,
        //                BorderWidth = bw
        //            };
        //        }

        //        PdfPCell VerticalHeader(string text)
        //        {
        //            return new PdfPCell(new Phrase(text, headerFont))
        //            {
        //                Rotation = 90,
        //                BackgroundColor = yellow,
        //                HorizontalAlignment = Element.ALIGN_CENTER,
        //                VerticalAlignment = Element.ALIGN_MIDDLE,
        //                Padding = 4,
        //                BorderWidth = bw
        //            };
        //        }

        //        PdfPCell DataCell(string text = "")
        //        {
        //            return new PdfPCell(new Phrase(text ?? "", normalFont))
        //            {
        //                HorizontalAlignment = Element.ALIGN_CENTER,
        //                VerticalAlignment = Element.ALIGN_MIDDLE,
        //                BorderWidth = bw
        //            };
        //        }

        //        PdfPCell BoldCell(string text)
        //        {
        //            return new PdfPCell(new Phrase(text ?? "", boldFont))
        //            {
        //                HorizontalAlignment = Element.ALIGN_CENTER,
        //                VerticalAlignment = Element.ALIGN_MIDDLE,
        //                BorderWidth = bw,
        //                BackgroundColor = gray
        //            };
        //        }

        //        // ------------ title ------------
        //        pdf.Add(new Paragraph("STEEL PLANT DAILY PRODUCTION STATISTICS", titleFont)
        //        {
        //            Alignment = Element.ALIGN_CENTER
        //        });

        //        pdf.Add(new Paragraph($"FOR THE MONTH {monthName}, {year}", boldFont)
        //        {
        //            Alignment = Element.ALIGN_CENTER
        //        });

        //        pdf.Add(Chunk.NEWLINE);

        //        // ------------ main table (32 columns) ------------
        //        const int COLS = 32;
        //        PdfPTable table = new PdfPTable(COLS)
        //        {
        //            WidthPercentage = 100,
        //            HeaderRows = 2
        //        };
        //        table.SetWidths(Enumerable.Repeat(1f, COLS).ToArray());

        //        // row 1 – group headers (colspans)
        //        table.AddCell(GroupHeader("Charge", 5)); // Scrap, DRI, HBI, Total, Yield
        //        table.AddCell(GroupHeader("Tapped Steel", 3)); // Tap Wt, Liquid, Casting
        //        table.AddCell(GroupHeader("Energy", 4)); // Total, Prod, Spec, Power On
        //        table.AddCell(GroupHeader("Temperature", 3)); // T1, T2, T3
        //        table.AddCell(GroupHeader("Carbon Injection", 2)); // Kg, Coal
        //        table.AddCell(GroupHeader("Additives", 3)); // Lime, Dolo, Fluor
        //        table.AddCell(GroupHeader("Oxygen", 2)); // Nm3, Spec
        //        table.AddCell(GroupHeader("Productivity", 3)); // TPH, Yield, Tap-Tap
        //        table.AddCell(GroupHeader("Quality", 6)); // R, T, LCC, C, S, TE
        //        table.AddCell(GroupHeader("Day", 1)); // Day

        //        // row 2 – leaf headers (32 exactly)
        //        string[] leafHeaders =
        //        {
        //    // Charge (5)
        //    "Scrap", "DRI", "HBI", "Total", "Yield",

        //    // Tapped Steel (3)
        //    "Tap Wt", "Liquid", "Casting",

        //    // Energy (4)
        //    "Total kWh", "Prod kWh", "Spec kWh", "Power On",

        //    // Temperature (3)
        //    "T1", "T2", "T3",

        //    // Carbon Injection (2)
        //    "Kg", "Coal",

        //    // Additives (3)
        //    "Lime", "Dolo", "Fluor",

        //    // Oxygen (2)
        //    "Nm3", "Spec",

        //    // Productivity (3)
        //    "TPH", "Yield", "Tap–Tap",

        //    // Quality (6)
        //    "R", "T", "LCC", "C", "S", "TE",

        //    // Day (1)
        //    "Day"
        //};

        //        if (leafHeaders.Length != COLS)
        //            throw new Exception("Header definition must have exactly 32 columns.");

        //        foreach (var h in leafHeaders)
        //            table.AddCell(VerticalHeader(h));

        //        // ------------ data rows (dummy data – replace with DB) ------------
        //        // 31 numeric columns + 1 Day column
        //        for (int d = 1; d <= daysInMonth; d++)
        //        {
        //            for (int c = 0; c < COLS - 1; c++)
        //            {
        //                // dummy value – you will bind real values from DB instead
        //                double value = (d * (c + 1)) / 10.0;
        //                table.AddCell(DataCell(value.ToString("0.0")));
        //            }

        //            // Day number in last column
        //            table.AddCell(BoldCell(d.ToString()));
        //        }

        //        // ------------ AVG row ------------
        //        table.AddCell(BoldCell("AVG"));
        //        for (int c = 1; c < COLS - 1; c++)
        //            table.AddCell(BoldCell("0.00"));   // later you can compute real averages

        //        table.AddCell(BoldCell(""));           // Day column blank

        //        // ------------ SUM row ------------
        //        table.AddCell(BoldCell("SUM"));
        //        for (int c = 1; c < COLS - 1; c++)
        //            table.AddCell(BoldCell("0.00"));   // later you can compute real sums

        //        table.AddCell(BoldCell(""));

        //        // add table to pdf
        //        pdf.Add(table);
        //        pdf.Close();

        //        return File(ms.ToArray(),
        //                    "application/pdf",
        //                    $"SteelPlantDailyProduction_{monthName}_{year}.pdf");
        //    }

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

        //public ActionResult ShiftProductionReport(DateTime? from, DateTime? to)
        //{
        //    var vm = new ShiftProductionReportVM
        //    {
        //        Delays = repo.GetAllDelay().ToList(),
        //        DischargedHeats = rm.GetDichargedHeat().ToList()
        //    };

        //    return View(vm);
        //}

        //public ActionResult ShiftProductionReport(DateTime? from, DateTime? to)
        //{
        //    // 🔑 Default = TODAY
        //    DateTime startDate = from ?? DateTime.Today;
        //    DateTime endDate = to ?? DateTime.Today;

        //    var vm = new ShiftProductionReportVM
        //    {
        //        Delays = repo.GetAllRMDelay(startDate, endDate),
        //        DischargedHeats = rm.GetDichargedHeat(startDate, endDate)
        //    };

        //    return View(vm);
        //}

        public ActionResult ShiftProductionReport(DateTime? from, DateTime? to)
        {
            DateTime startDate = from ?? DateTime.Today;
            DateTime endDate = to ?? DateTime.Today;

            var dischargedData = rm.GetDichargedHeat(startDate, endDate);
            var delayData = repo.GetAllRMDelay(startDate, endDate);

            var vm = new ShiftProductionReportVM
            {
                Delays = delayData ?? new List<PlantDelayBLL>(),
                DischargedHeats = dischargedData ?? new List<BilletDischargingBLL>()
            };

            // ===== OPTIONAL (ViewBag stuff same rakho) =====
            ViewBag.TotalBundles = vm.DischargedHeats.Count;
            ViewBag.Cobble = vm.Delays.Count(x => x.DelayType == "Cobble");
            ViewBag.HotOut = vm.Delays.Count(x => x.DelayType == "HotOut");

            ViewBag.From = startDate;
            ViewBag.To = endDate;

            return View(vm);   // 🔥 MOST IMPORTANT
        }

        //public ActionResult ShiftProductionReportPDF(DateTime? from, DateTime? to)
        //{
        //    var vm = new ShiftProductionReportVM
        //    {
        //        Delays = repo.GetAllDelay().ToList(),
        //        DischargedHeats = rm.GetDichargedHeat().ToList()
        //    };

        //    return new ViewAsPdf("ShiftProductionReport", vm)
        //    {
        //        FileName = "Shift_Production_Report.pdf",
        //        PageSize = Size.A4,
        //        PageOrientation = Orientation.Landscape,
        //        CustomSwitches = "--disable-smart-shrinking"
        //    };
        //}

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
    }
}
