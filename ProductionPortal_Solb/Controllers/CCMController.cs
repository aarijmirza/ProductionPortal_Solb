using BAL.Repositories;
using DAL.Models;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using static DAL.Models.ViewModel;

namespace ProductionPortal_Solb.Controllers
{
    public class CCMController : Controller
    {
        CCMRespository repo;
        MeltshopRepository mr;
        public CCMController()
        {
            repo = new CCMRespository();
            mr = new MeltshopRepository();
        }
        // GET: CCM
        public ActionResult list()
        {
            var CCM = repo.GetAllCCMHeat();
            return View(CCM);
        }

        public ActionResult details(int id)
        {
            var data = repo.GetCCMHeatByID(id);
            return View(data);
        }

        public ActionResult Add(int? id)
        {
            var vm = new DAL.Models.ViewModel.CCMMeltShopVM();

            var last24Hours = DateTime.Now.AddHours(-24);
            var heatData = mr.GetAllLFRecord();
            var grades = repo.GetAllGrade();

            if (id.HasValue)
            {
                // 🔹 EDIT
                var model = repo.GetCCMHeatByID(id);   // CCMBLL
                vm.Master = model ?? new CCMBLL();

                ViewBag.HeatNo = new SelectList(
                    heatData,
                    "HeatNo",
                    "HeatNo",
                    vm.Master.HeatNo   // ✅ CORRECT
                );

                ViewBag.Grade = new SelectList(
                    grades,
                    "GRADE_ID",
                    "GRADE_ID",
                    vm.Master.Grade    // ✅ CORRECT
                );
            }
            else
            {
                // 🔹 ADD
                vm.Master = new CCMBLL();

                ViewBag.HeatNo = new SelectList(heatData, "HeatNo", "HeatNo");
                ViewBag.Grade = new SelectList(grades, "GRADE_ID", "GRADE_ID");
            }

            return View(vm);
        }


        [HttpPost]
        public ActionResult Add(CCMMeltShopVM model, decimal[] SteelTemperature, string[] Strand)
        {
            var data = model.Master;

            if (model.Master.ID == 0 || model.Master.ID == null)
            {
                model.Master.StatusID = 1;
                model.Master.CreatedDate = DateTime.Now;
                model.Master.CreatedBy = User.Identity.Name;
                model.Master.Date = DateTime.Now;

                // Assign Steel Temperatures dynamically
                if (SteelTemperature != null && SteelTemperature.Length > 0)
                {
                    // The conditional check can be simplified and relies on the length check.
                    model.Master.SteelTemperature1 = SteelTemperature.Length > 0 ? (decimal?)SteelTemperature[0] : null;
                    model.Master.SteelTemperature2 = SteelTemperature.Length > 1 ? (decimal?)SteelTemperature[1] : null;
                    model.Master.SteelTemperature3 = SteelTemperature.Length > 2 ? (decimal?)SteelTemperature[2] : null;
                    model.Master.SteelTemperature4 = SteelTemperature.Length > 3 ? (decimal?)SteelTemperature[3] : null;
                }

                // Assign Steel Temperatures dynamically
                if (Strand != null && Strand.Length > 0)
                {
                    // The conditional check can be simplified and relies on the length check.
                    model.Master.Strand1 = Strand.Length > 0 ? (string)Strand[0] : null;
                    model.Master.Strand2 = Strand.Length > 1 ? (string)Strand[1] : null;
                    model.Master.Strand3 = Strand.Length > 2 ? (string)Strand[2] : null;
                    model.Master.Strand4 = Strand.Length > 3 ? (string)Strand[3] : null;
                    model.Master.Strand5 = Strand.Length > 4 ? (string)Strand[4] : null;
                }

                if (model.Master.Analysis != null && model.Master.Analysis.Any())
                {
                    foreach (var Analysis in model.Master.Analysis)
                    {
                        repo.AddChemicalAnalysis(new CCMChemicalAnalysisBLL
                        {
                            HeatNo = data.HeatNo,
                            Sample = Analysis.Sample,
                            C = Analysis.C,
                            Si = Analysis.Si,
                            Mn = Analysis.Mn,
                            P = Analysis.P,
                            S = Analysis.S,
                            TE = Analysis.TE,
                            CreatedBy = User.Identity.Name,
                            CreatedDate = DateTime.Now,
                            StatusID = 1
                        });
                    }
                }

                int rtn = repo.InsertCCMHeat(data);
            }
            else
            {
                // ===============================
                // 🔹 FETCH EXISTING RECORD
                // ===============================
                var existing = repo.GetCCMHeatByID(model.Master.ID);

                if (existing == null)
                    return HttpNotFound();

                // ===============================
                // 🔹 UPDATE BASIC FIELDS
                // ===============================
                existing.Date = model.Master.Date;
                existing.Shift = model.Master.Shift;
                existing.Grade = model.Master.Grade;
                existing.HeatNo = model.Master.HeatNo;
                existing.SequenceHeat = model.Master.SequenceHeat;
                existing.LadleNo = model.Master.LadleNo;
                existing.LadleTemperature = model.Master.LadleTemperature;
                existing.MoltenSteel = model.Master.MoltenSteel;
                existing.TimeOnTurret = model.Master.TimeOnTurret;
                existing.PlateLife = model.Master.PlateLife;
                existing.OpenTime = model.Master.OpenTime;
                existing.CloseTime = model.Master.CloseTime;

                existing.TundishNo = model.Master.TundishNo;
                existing.TundishLife = model.Master.TundishLife;
                existing.ShroudLife = model.Master.ShroudLife;

                existing.BilletSize = model.Master.BilletSize;
                existing.BilletLength = model.Master.BilletLength;
                existing.CastingTime = model.Master.CastingTime;
                existing.BilletNumber = model.Master.BilletNumber;
                existing.BilletTotalWeight = model.Master.BilletTotalWeight;
                existing.Productivitytonhr = model.Master.Productivitytonhr;
                existing.Yeild = model.Master.Yeild;

                // LF Analysis
                existing.LF_C = model.Master.LF_C;
                existing.LF_Si = model.Master.LF_Si;
                existing.LF_Mn = model.Master.LF_Mn;
                existing.LF_S = model.Master.LF_S;
                existing.LF_TE = model.Master.LF_TE;
                existing.LF_MnSi = model.Master.LF_MnSi;
                existing.LF_MnS = model.Master.LF_MnS;

                // ===============================
                // 🔹 META FIELDS
                // ===============================
                existing.StatusID = 1;
                existing.UpdatedDate = DateTime.Now;
                existing.UpdatedBy = User.Identity.Name;

                // ===============================
                // 🔹 STEEL TEMPERATURES (RESET + ASSIGN)
                // ===============================
                existing.SteelTemperature1 = null;
                existing.SteelTemperature2 = null;
                existing.SteelTemperature3 = null;
                existing.SteelTemperature4 = null;

                if (SteelTemperature != null)
                {
                    if (SteelTemperature.Length > 0) existing.SteelTemperature1 = (decimal?)SteelTemperature[0];
                    if (SteelTemperature.Length > 1) existing.SteelTemperature2 = (decimal?)SteelTemperature[1];
                    if (SteelTemperature.Length > 2) existing.SteelTemperature3 = (decimal?)SteelTemperature[2];
                    if (SteelTemperature.Length > 3) existing.SteelTemperature4 = (decimal?)SteelTemperature[3];
                }

                // ===============================
                // 🔹 STRANDS (RESET + ASSIGN)
                // ===============================
                existing.Strand1 = null;
                existing.Strand2 = null;
                existing.Strand3 = null;
                existing.Strand4 = null;
                existing.Strand5 = null;

                if (Strand != null)
                {
                    if (Strand.Length > 0) existing.Strand1 = Strand[0];
                    if (Strand.Length > 1) existing.Strand2 = Strand[1];
                    if (Strand.Length > 2) existing.Strand3 = Strand[2];
                    if (Strand.Length > 3) existing.Strand4 = Strand[3];
                    if (Strand.Length > 4) existing.Strand5 = Strand[4];
                }

                // ===============================
                // 🔹 CHEMICAL ANALYSIS (REPLACE)
                // ===============================
                existing.UpdatedBy = User.Identity.Name;
                repo.Delete(existing.HeatNo, existing.UpdatedBy);

                if (data.Analysis != null && data.Analysis.Any())
                {
                    foreach (var a in data.Analysis)
                    {
                        repo.AddChemicalAnalysis(new CCMChemicalAnalysisBLL
                        {
                            HeatNo = existing.HeatNo,
                            Sample = a.Sample,
                            C = a.C,
                            Si = a.Si,
                            Mn = a.Mn,
                            P = a.P,
                            S = a.S,
                            TE = a.TE,
                            CreatedBy = User.Identity.Name,
                            CreatedDate = DateTime.Now,
                            StatusID = 1
                        });
                    }
                }

                //===============================
                //🔹 SAVE UPDATE
                //===============================
                repo.UpdateCCMHeat(existing);
            }

            TempData["SuccessMessage"] = "Data Saved Successfully";
            return RedirectToAction("list");
        }
        public ActionResult delete(string heatNo)
        {
            var UpdatedBy = User.Identity.Name;
            int rtn = repo.Delete(heatNo, UpdatedBy);
            TempData["SuccessMessage"] = "Data Delete Successfully";

            return RedirectToAction("list");
        }

        // ---------------------------------------------------------------------
        // CORRECTED PDF REPORT GENERATION METHOD
        // ---------------------------------------------------------------------

        public ActionResult CCMReportPDF()
        {
            // 1) Get data
            DataTable dt = BuildCCMDataTable();   // MUST return 52 columns in correct order

            // ALSO get raw list to read Shift & Date for header
            var heats = repo.GetAllCCMHeat();     // SAME source used in BuildCCMDataTable()

            // ---- Production Date from first record ----
            string productionDate = "";
            var firstHeat = heats.FirstOrDefault();
            //if (firstHeat != null && firstHeat.Date.ToString("dd-MM-yyyy").HasValue)   // <-- adjust property name if needed
            //{
            //    productionDate = firstHeat.Date.Value.ToString("dd-MM-yyyy");
            //}

            // ---- Distinct shift labels from DB ----
            // assumes property `Shift` exists in your CCM model
            var shiftGroups = heats
                .Where(h => !string.IsNullOrEmpty(h.Shift))     // <-- adjust property name if needed
                .GroupBy(h => h.Shift)
                .OrderBy(g => g.Key)
                .Take(3)
                .Select(g => g.Key)
                .ToList();

            string shift1Text = shiftGroups.Count > 0 ? shiftGroups[0] : "";
            string shift2Text = shiftGroups.Count > 1 ? shiftGroups[1] : "";
            string shift3Text = shiftGroups.Count > 2 ? shiftGroups[2] : "";

            // 2) PDF setup
            MemoryStream ms = new MemoryStream();
            Document pdf = new Document(PageSize.A3.Rotate(), 10, 10, 10, 10);
            PdfWriter.GetInstance(pdf, ms);
            pdf.Open();

            // 3) Fonts
            Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
            Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8);
            Font normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 8);
            BaseColor headerBg = BaseColor.WHITE;
            float borderWidth = 0.5f;

            // Helpers
            PdfPCell HCell(string text, int colSpan = 1, int rowSpan = 1, int rotation = 0)
            {
                PdfPCell c = new PdfPCell(new Phrase(text, headerFont));
                c.Colspan = colSpan;
                c.Rowspan = rowSpan;
                c.HorizontalAlignment = Element.ALIGN_CENTER;
                c.VerticalAlignment = Element.ALIGN_MIDDLE;
                c.Rotation = rotation;
                c.BackgroundColor = headerBg;
                c.BorderWidth = borderWidth;
                c.Padding = 2f;
                return c;
            }

            PdfPCell VCell(string text)
            {
                return HCell(text, 1, 1, 90);
            }

            PdfPCell DCell(string text)
            {
                PdfPCell c = new PdfPCell(new Phrase(text, normalFont));
                c.HorizontalAlignment = Element.ALIGN_CENTER;
                c.VerticalAlignment = Element.ALIGN_MIDDLE;
                c.BorderWidth = borderWidth;
                c.Padding = 2f;
                return c;
            }

            // -------------------------------------------------
            // HEADER: LOGO + CENTER HEADING
            // -------------------------------------------------
            PdfPTable headerTable = new PdfPTable(2);
            headerTable.WidthPercentage = 100;
            headerTable.SetWidths(new float[] { 15f, 85f });

            string logoUrl = "http://10.1.10.202:8888/Content/images/logo.png";
            iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(new Uri(logoUrl));
            logo.ScaleAbsolute(100, 40);

            PdfPCell logoCell = new PdfPCell(logo);
            logoCell.Border = Rectangle.NO_BORDER;
            logoCell.HorizontalAlignment = Element.ALIGN_LEFT;
            logoCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            headerTable.AddCell(logoCell);

            PdfPCell headingCell = new PdfPCell(new Phrase(
                "Daily Casting Machine Production Data",
                FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20)))
            {
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                PaddingTop = 10f,
                PaddingBottom = 10f
            };
            headerTable.AddCell(headingCell);

            pdf.Add(headerTable);
            pdf.Add(Chunk.NEWLINE);

            // -------------------------------------------------
            // Shift / Date strip
            // -------------------------------------------------
            PdfPTable shiftTable = new PdfPTable(4);
            shiftTable.WidthPercentage = 100;
            shiftTable.SetWidths(new float[] { 10, 40, 10, 40 });

            var greg = new CultureInfo("en-US");
            greg.DateTimeFormat.Calendar = new GregorianCalendar();

            // Row 1: Shift1 + Date
            shiftTable.AddCell(new PdfPCell(new Phrase("Shift1:", headerFont)) { BorderWidth = borderWidth });
            shiftTable.AddCell(new PdfPCell(new Phrase(shift1Text, headerFont)) { BorderWidth = borderWidth });
            shiftTable.AddCell(new PdfPCell(new Phrase("Date:", headerFont)) { BorderWidth = borderWidth });
            //shiftTable.AddCell(new PdfPCell(new Phrase(DateTime.Now.ToString("dd-MM-yyyy"), headerFont)) { BorderWidth = borderWidth });
            shiftTable.AddCell(
                new PdfPCell(
                    new Phrase(DateTime.Now.ToString("dd-MM-yyyy", greg), headerFont)
                )
                {
                    BorderWidth = borderWidth
                }
            );

            // Row 2: Shift2
            shiftTable.AddCell(new PdfPCell(new Phrase("Shift2:", headerFont)) { BorderWidth = borderWidth });
            shiftTable.AddCell(new PdfPCell(new Phrase(shift2Text, headerFont)) { BorderWidth = borderWidth, Colspan = 3 });

            // Row 3: Shift3
            shiftTable.AddCell(new PdfPCell(new Phrase("Shift3:", headerFont)) { BorderWidth = borderWidth });
            shiftTable.AddCell(new PdfPCell(new Phrase(shift3Text, headerFont)) { BorderWidth = borderWidth, Colspan = 3 });

            pdf.Add(shiftTable);
            pdf.Add(Chunk.NEWLINE);

            // -------------------------------------------------
            // Main 53-column table
            // -------------------------------------------------
            PdfPTable table = new PdfPTable(53);
            table.WidthPercentage = 100;
            table.HeaderRows = 3;

            float[] widths = new float[53];
            for (int i = 0; i < 52; i++) widths[i] = 1f;
            table.SetWidths(widths);

            // =========================
            // HEADER ROW 1 – Big Groups
            // =========================
            table.AddCell(HCell("LF data", 13, 1));
            table.AddCell(HCell("CCM ladle data", 5, 1));
            table.AddCell(HCell("CCM tundish data", 8, 1));
            table.AddCell(HCell("CCM Productivity", 10, 1));
            table.AddCell(HCell("CCM chemical analysis", 12, 1));
            table.AddCell(HCell("Quality", 5, 1));  // now 5 only

            // =========================
            // HEADER ROW 2 – Subgroups
            // =========================
            // LF data
            table.AddCell(HCell("Ladle furnace", 6, 1));
            table.AddCell(HCell("LF leave analysis", 7, 1));

            // CCM ladle data
            table.AddCell(HCell("slide gate", 4, 1));

            // CCM tundish data
            table.AddCell(HCell("refractory", 4, 1));
            table.AddCell(HCell("steel temperature", 4, 1));

            // Productivity
            table.AddCell(HCell("Casting lines", 5, 1));
            table.AddCell(HCell("Billet data", 6, 1));  // was 5 -> now 6

            // Chemical elements
            table.AddCell(HCell("C%", 2, 1));
            table.AddCell(HCell("Si%", 2, 1));
            table.AddCell(HCell("Mn%", 2, 1));
            table.AddCell(HCell("P%", 2, 1));
            table.AddCell(HCell("S%", 2, 1));
            table.AddCell(HCell("T.E%", 2, 1));

            // Quality legend – now 5 columns
            table.AddCell(HCell("R-rhomboidity  T-twist\nLCC-longitudinal cracks\nC-camber", 5, 1));

            // =========================
            // HEADER ROW 3 – 52 leaf labels
            // =========================
            string[] leafHeaders = new[]
            {
        // LF: Ladle furnace (6)
        "heat no.",
        "sequence no.",
        "steel grade",
        "ladle no.",
        "leave temperature",
        "molten steel wt (ton)",

        // LF leave analysis (7)
        "C%",
        "Si%",
        "Mn%",
        "S%",
        "T.E.%",
        "Mn/Si",
        "Mn/S",

        // CCM ladle data (4)
        "time on turret",
        "Plates life",
        "open time",
        "close time",

        // CCM tundish data (7)
        "Tundish no.",
        "Tundish life (heats)",
        "shroud life (heats)",
        "Ladle Open",
        "meas no. 1",
        "meas no. 2",
        "meas no. 3",
        "meas no. 4",

        // Casting lines (5)
        "1",
        "2",
        "3",
        "4",
        "5",

        // Billet data (6)  ⭐ NEW DESIGN
        "billet size (mm2)",
        "billet length (m)",
        "hot charging",
        "TOCB",
        "total billets",
        "yield %",

        // Chemical 1st / 2nd (12)
        "C% 1st",
        "C% 2nd",
        "Si% 1st",
        "Si% 2nd",
        "Mn% 1st",
        "Mn% 2nd",
        "P% 1st",
        "P% 2nd",
        "S% 1st",
        "S% 2nd",
        "T.E% 1st",
        "T.E% 2nd",

        // Quality (5) – Quality 6 REMOVED
        "1",
        "2",
        "3",
        "4",
        "5"
    };

            if (leafHeaders.Length != 53)
                throw new Exception("Leaf header count must be 53.");

            foreach (var h in leafHeaders)
                table.AddCell(VCell(h));

            // =========================
            // DATA ROWS
            // =========================
            foreach (DataRow row in dt.Rows)
            {
                foreach (var obj in row.ItemArray)
                {
                    string txt = obj == null ? "" : obj.ToString();
                    table.AddCell(DCell(txt));
                }
            }

            pdf.Add(table);
            pdf.Close();

            return File(ms.ToArray(),
                "application/pdf",
                "CCM_DailyCastingMachine.pdf");
        }

        private DataTable BuildCCMDataTable()
        {
            var list = repo.GetAllCCMHeat();

            DataTable dt = new DataTable();

            // 52 columns in EXACT order as headers
            // LF data
            dt.Columns.Add("HeatNo");           // 1
            dt.Columns.Add("SequenceNo");       // 2
            dt.Columns.Add("SteelGrade");       // 3
            dt.Columns.Add("LadleNo");          // 4
            dt.Columns.Add("LeaveTemperature"); // 5
            dt.Columns.Add("MoltenSteelWt");    // 6

            // LF leave analysis
            dt.Columns.Add("C_LF");             // 7
            dt.Columns.Add("Si_LF");            // 8
            dt.Columns.Add("Mn_LF");            // 9
            dt.Columns.Add("S_LF");             //10
            dt.Columns.Add("TE_LF");            //11
            dt.Columns.Add("MnBySi");           //12
            dt.Columns.Add("MnByS");            //13

            // CCM ladle data
            dt.Columns.Add("TimeOnTurret");     //14
            dt.Columns.Add("PlatesLife");       //15
            dt.Columns.Add("OpenTime");         //16
            dt.Columns.Add("CloseTime");        //17

            // CCM tundish data
            dt.Columns.Add("TundishNo");        //18
            dt.Columns.Add("TundishLife");      //19
            dt.Columns.Add("ShroudLife");       //20
            dt.Columns.Add("LadleOpen");       //20
            dt.Columns.Add("SteelTemperature1");//21
            dt.Columns.Add("SteelTemperature2");//22
            dt.Columns.Add("SteelTemperature3");//23
            dt.Columns.Add("SteelTemperature4");//24

            // Casting lines
            dt.Columns.Add("Strand1");          //25
            dt.Columns.Add("Strand2");          //26
            dt.Columns.Add("Strand3");          //27
            dt.Columns.Add("Strand4");          //28
            dt.Columns.Add("Strand5");          //29

            // Billet data (NEW layout)
            dt.Columns.Add("BilletSize");       //30
            dt.Columns.Add("BilletLength");     //31
            dt.Columns.Add("HotCharging");      //32
            dt.Columns.Add("TOCB");             //33
            dt.Columns.Add("TotalBillets");     //34  = Hot + TOCB
            dt.Columns.Add("Yeild");            //35

            // Chemical 1st / 2nd
            dt.Columns.Add("C_1st");            //36
            dt.Columns.Add("C_2nd");            //37
            dt.Columns.Add("Si_1st");           //38
            dt.Columns.Add("Si_2nd");           //39
            dt.Columns.Add("Mn_1st");           //40
            dt.Columns.Add("Mn_2nd");           //41
            dt.Columns.Add("P_1st");            //42
            dt.Columns.Add("P_2nd");            //43
            dt.Columns.Add("S_1st");            //44
            dt.Columns.Add("S_2nd");            //45
            dt.Columns.Add("TE_1st");           //46
            dt.Columns.Add("TE_2nd");           //47

            // Quality – only 5 columns (no 6)
            dt.Columns.Add("Quality1");         //48
            dt.Columns.Add("Quality2");         //49
            dt.Columns.Add("Quality3");         //50
            dt.Columns.Add("Quality4");         //51
            dt.Columns.Add("Quality5");         //52

            // Fill rows
            foreach (var item in list)
            {
                DataRow row = dt.NewRow();

                // LF data
                row["HeatNo"] = item.HeatNo;
                row["SequenceNo"] = item.SequenceHeat;
                row["SteelGrade"] = item.Grade;
                row["LadleNo"] = item.LadleNo;
                row["LeaveTemperature"] = item.LadleTemperature;
                row["MoltenSteelWt"] = item.MoltenSteel;

                // LF leave analysis
                row["C_LF"] = item.LF_C;
                row["Si_LF"] = item.LF_Si;
                row["Mn_LF"] = item.LF_Mn;
                row["S_LF"] = item.LF_S;
                row["TE_LF"] = item.LF_TE;
                row["MnBySi"] = item.LF_MnSi;
                row["MnByS"] = item.LF_MnS;

                // CCM ladle
                row["TimeOnTurret"] = item.TimeOnTurret;
                row["PlatesLife"] = item.PlateLife;
                row["OpenTime"] = item.OpenTime;
                row["CloseTime"] = item.CloseTime;

                // CCM tundish
                row["TundishNo"] = item.TundishNo;
                row["TundishLife"] = item.TundishLife;
                row["ShroudLife"] = item.ShroudLife;
                row["LadleOpen"] = item.LadleOpen;
                row["SteelTemperature1"] = item.SteelTemperature1;
                row["SteelTemperature2"] = item.SteelTemperature2;
                row["SteelTemperature3"] = item.SteelTemperature3;
                row["SteelTemperature4"] = item.SteelTemperature4;

                // Casting lines
                row["Strand1"] = item.Strand1;
                row["Strand2"] = item.Strand2;
                row["Strand3"] = item.Strand3;
                row["Strand4"] = item.Strand4;
                row["Strand5"] = item.Strand5;

                // Billet data
                row["BilletSize"] = item.BilletSize;
                row["BilletLength"] = item.BilletLength;
                row["HotCharging"] = 10;   // <-- adjust property name if different
                row["TOCB"] = 15;          // <-- adjust property name
                                           // Total billets = Hot + TOCB
                decimal hot = item.HotCharging ?? 10;
                decimal tocb = item.TOCB ?? 15;
                row["TotalBillets"] = hot + tocb;
                row["Yeild"] = item.Yeild;        // <-- adjust property name

                // Chemical analysis 1st / 2nd
                //row["C_1st"] = item.C_1st;
                //row["C_2nd"] = item.C_2nd;
                //row["Si_1st"] = item.Si_1st;
                //row["Si_2nd"] = item.Si_2nd;
                //row["Mn_1st"] = item.Mn_1st;
                //row["Mn_2nd"] = item.Mn_2nd;
                //row["P_1st"] = item.P_1st;
                //row["P_2nd"] = item.P_2nd;
                //row["S_1st"] = item.S_1st;
                //row["S_2nd"] = item.S_2nd;
                //row["TE_1st"] = item.TE_1st;
                //row["TE_2nd"] = item.TE_2nd;

                // Quality (only 5)
                //row["Quality1"] = item.Quality1;
                //row["Quality2"] = item.Quality2;
                //row["Quality3"] = item.Quality3;
                //row["Quality4"] = item.Quality4;
                //row["Quality5"] = item.Quality5;

                dt.Rows.Add(row);
            }

            return dt;
        }

        public ActionResult CCMYieldlist()
        {
            var data = repo.GetAllCCMBreakdown();
            return View(data);
        }

        public ActionResult AddYieldRecord()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddYieldRecord(CCMYeildBLL data)
        {
            if (data.ID == null || data.ID == 0)
            {
                data.StatusID = 1;
                data.CreatedDate = DateTime.Now;
                data.CreatedBy = User.Identity.Name;
                data.Date = DateTime.Now;
                int rtn = repo.InsertCCMYeild(data);
            }
            TempData["SuccessMessage"] = "Data Saved Successfully";
            return RedirectToAction("CCMYieldlist");
        }

        public ActionResult CCMYeildPDF()
        {
            var data = repo.GetAllCCMBreakdown();    // DB data source

            using (var ms = new MemoryStream())
            {
                // A4 Landscape
                var doc = new Document(PageSize.A4.Rotate(), 20, 20, 20, 20);
                PdfWriter.GetInstance(doc, ms);
                doc.Open();

                // =========================================================
                // HEADER TABLE (LOGO + TITLE)
                // =========================================================
                PdfPTable header = new PdfPTable(2);
                header.WidthPercentage = 100;
                header.SetWidths(new float[] { 10f, 90f });

                // ---------- LOGO ----------
                string logoPath = Server.MapPath("~/assets/images/logo.png");

                if (System.IO.File.Exists(logoPath))
                {
                    Image logo = Image.GetInstance(logoPath);
                    logo.ScaleAbsolute(90f, 50f);
                    PdfPCell logoCell = new PdfPCell(logo);
                    logoCell.Border = Rectangle.NO_BORDER;
                    logoCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    header.AddCell(logoCell);
                }
                else
                {
                    header.AddCell(new PdfPCell() { Border = Rectangle.NO_BORDER });
                }

                // ---------- TITLE ----------
                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14);
                PdfPCell titleCell = new PdfPCell(new Phrase("CCM DAILY YEILD BREAK DOWN", titleFont));
                titleCell.Border = Rectangle.NO_BORDER;
                titleCell.HorizontalAlignment = Element.ALIGN_CENTER;
                titleCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                header.AddCell(titleCell);

                doc.Add(header);
                doc.Add(new Paragraph("\n"));

                // =========================================================
                // MAIN TABLE (7 COLUMNS)
                // =========================================================
                PdfPTable table = new PdfPTable(7);
                table.WidthPercentage = 100;
                table.SetWidths(new float[]
                {
                13f, // Date
                10f, // Heat #
                13f, // Tundish Skull
                20f, // Process Rejected Billet
                18f, // Short Billet < 6m
                13f, // Head / Tail
                35f  // Comment
                });

                // ---------- HEADER ROW ----------
                table.AddCell(HeaderCell("Date"));
                table.AddCell(HeaderCell("Heat #"));
                table.AddCell(HeaderCell("Tundish Skull"));
                table.AddCell(HeaderCell("Process Rejected Billet"));
                table.AddCell(HeaderCell("Short Billet < 6m"));
                table.AddCell(HeaderCell("Head / Tail"));
                table.AddCell(HeaderCell("Comment"));

                // ---------- DATA ROWS ----------

                var greg = new CultureInfo("en-US");
                greg.DateTimeFormat.Calendar = new GregorianCalendar();

                foreach (var r in data)
                {
                    DateTime dt = Convert.ToDateTime(r.Date, greg);

                    table.AddCell(DataCell(dt.ToString("dd-MM-yyyy", greg)));
                    table.AddCell(DataCell(r.HeatNo));
                    table.AddCell(DataCell(r.TundishSkull.ToString()));
                    table.AddCell(DataCell(r.ProcessRejectedBillet.ToString()));
                    table.AddCell(DataCell(r.ShortBillet6m.ToString()));
                    table.AddCell(DataCell(r.HeadTail.ToString()));
                    table.AddCell(DataCell(r.Comment));
                }

                doc.Add(table);
                doc.Close();

                byte[] bytes = ms.ToArray();
                return File(bytes, "application/pdf", "CCM_Yeild_Report.pdf");
            }
        }

        // =========================================================
        // Helper Cells
        // =========================================================

        private PdfPCell HeaderCell(string text)
        {
            var font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9, BaseColor.BLACK);
            PdfPCell cell = new PdfPCell(new Phrase(text, font));
            cell.BackgroundColor = new BaseColor(240, 240, 240);
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Padding = 4f;
            return cell;
        }

        private PdfPCell DataCell(string text)
        {
            var font = FontFactory.GetFont(FontFactory.HELVETICA, 9, BaseColor.BLACK);
            PdfPCell cell = new PdfPCell(new Phrase(text ?? "", font));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Padding = 3f;
            return cell;
        }
    }
}