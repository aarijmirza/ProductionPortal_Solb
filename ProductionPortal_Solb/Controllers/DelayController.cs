using BAL.Repositories;
using DAL.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using System.Xml.Linq;
using WebAPICode.Helpers;
using static DAL.Models.ViewModel;

namespace ProductionPortal_Solb.Controllers
{
    [SessionState(
    SessionStateBehavior.ReadOnly
    )]
    public class DelayController : BaseController
    {
        DelayRespository repo;
        RollingMillRepository rm;
        public DelayController()
        {
            repo = new DelayRespository();
            rm = new RollingMillRepository();
        }

        public ActionResult list(DateTime? date, string shift, string plant)
        {
            DateTime selectedDate = date ?? DateTime.Today;

            //var data = repo.GetAllRMDelay(selectedDate, selectedDate, shift);

            var data = repo.GetAllRMDelay(selectedDate, selectedDate, shift)
                                ?? new List<PlantDelayBLL>();

            if (!string.IsNullOrWhiteSpace(plant))
            {

                data = data
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(x.Plant) &&
                        x.Plant.Trim().Equals(plant, StringComparison.OrdinalIgnoreCase)
                    )
                    .ToList();
            }

            ViewBag.SelectedDate = selectedDate.ToString("yyyy-MM-dd");
            ViewBag.Shift = shift ?? "";

            return View(data);
        }

        public ActionResult SMPdelaydetails()
        {
            return View();
        }

        public ActionResult add()
        {
            var agencyList = repo.GetAllAgency().ToList();
            ViewBag.Agencies = agencyList;

            DateTime selectedDate = Session["RM_Date"] != null
                ? Convert.ToDateTime(Session["RM_Date"])
                : DateTime.Today;

            string selectedPlant = Session["RM_Plant"] != null
                ? Convert.ToString(Session["RM_Plant"])
                : "";

            string selectedShift = Session["RM_Shift"] != null
                ? Convert.ToString(Session["RM_Shift"])
                : "";

            var nextDate = selectedDate.AddDays(1);

            var todayShiftDetails = rm.RollingMillDetails()
                .Where(x =>
                    x.Date >= selectedDate &&
                    x.Date < nextDate &&
                    x.Plant == selectedPlant &&
                    x.Shift == selectedShift
                )
                .OrderByDescending(x => x.ID)
                .FirstOrDefault();

            if (todayShiftDetails == null)
            {
                TempData["ErrorMessage"] = "Please select Rolling Mill Details first.";
                return RedirectToAction("AddDetails", "RollingMill");
            }

            var vm = new DelaysVM
            {
                Date = todayShiftDetails.Date,
                Shift = todayShiftDetails.Shift,
                Plant = todayShiftDetails.Plant,
                Team = todayShiftDetails.Team,
                ShiftIncharge = todayShiftDetails.ShiftIncharge
            };

            var equipment = repo.GetAllRMEquipments()
                .Select(x => new
                {
                    Code = x.Code,
                    Text = x.Description + " - " + x.LocationName
                })
                .ToList();

            ViewBag.Equipment = new SelectList(equipment, "Text", "Text");

            var component = repo.GetAllComponent();
            ViewBag.Component = new SelectList(component, "Code", "Description");

            return View(vm);
        }

        //public ActionResult add()
        //{
        //    var agencyList = repo.GetAllAgency().ToList(); // ensure it's a List
        //    ViewBag.Agencies = agencyList;

        //    var today = DateTime.Today;
        //    var tomorrow = today.AddDays(1);

        //    // ✅ Aaj ki Shift Details uthao
        //    var todayShiftDetails = rm.RollingMillDetails()
        //        .Where(x => x.Date >= today && x.Date < tomorrow)
        //        .OrderByDescending(x => x.ID)
        //        .FirstOrDefault();

        //    if (todayShiftDetails == null)
        //    {
        //        TempData["ErrorMessage"] = "Please add Rolling Mill Details for today first.";
        //        return RedirectToAction("AddDetails", "RollingMill");
        //    }

        //    var vm = new DelaysVM
        //    {
        //        Date = todayShiftDetails.Date,
        //        Shift = todayShiftDetails.Shift,
        //        Plant = todayShiftDetails.Plant,
        //        Team = todayShiftDetails?.Team,
        //        ShiftIncharge = todayShiftDetails?.ShiftIncharge
        //    };

        //    var equipment = repo.GetAllEquipments()
        //      .Select(x => new
        //      {
        //          Code = x.Code,
        //          Text = x.Description + " - " + x.LocationName
        //      })
        //      .ToList();

        //    ViewBag.Equipment = new SelectList(equipment, "Text", "Text");
        //    //var equipment = repo.GetAllEquipments();
        //    //ViewBag.Equipment = new SelectList(equipment, "Code", "Description");

        //    var component = repo.GetAllComponent();
        //    ViewBag.Component = new SelectList(component, "Code", "Description");

        //    return View(vm);
        //}

        [HttpPost]
        public ActionResult add(PlantDelayBLL data)
        {
            if (data != null)
            {
                var agencies = repo.GetAllAgency().ToList();

                var agencyname = repo.GetAllAgency().ToList(); // ensure it's a List

                var selectedAgency = agencies
                    .Where(a => a.AgencyCode == data.AgencyCode)
                    .FirstOrDefault();

                if (selectedAgency != null)
                {
                    data.AgencyName = selectedAgency.AgencyName;  // Agency ka Name
                    data.AgencyCode = selectedAgency.AgencyCode;  // Agency ka Code
                    data.DelayType = selectedAgency.DelayType;
                }

                // Delay Code Auto Generate
                data.Delaycode = repo.GenerateDelayCode();

                //data.Date = DateTime.Now;
                data.StatusID = 1;
                data.CreatedDate = DateTime.Now;
                data.CreatedBy = User.Identity.Name;
                // int rtn1 = AddEntry(data);
                int rtn = repo.Insert(data);
                if (rtn > 0)
                {
                    TempData["SuccessMessage"] = "Data saved successfully";
                }
                //else
                //{
                //    TempData["ErrorMessage"] = "Data not saved. Please try again.";
                //    return RedirectToAction("list"); // 👈 back to form
                //}
            }
            else
            {
                TempData["ErrorMessage"] = "Invalid data submitted.";
                return RedirectToAction("list");
            }

            return RedirectToAction("list");
        }



        public ActionResult AddEntry()
        {
            ShiftEntryBLL model = new ShiftEntryBLL();

            // 🔹 get today's first entry
            var first = repo.GetTodayFirstEntry(DateTime.Now);

            if (first != null)
            {
                model.Plant = first.Plant;
                model.Shift = first.Shift;
                model.Team = first.Team;
                model.ShiftIncharge = first.ShiftIncharge;
            }

            return View(model);
        }

        public ActionResult delete(int ID)
        {
            var UpdatedBy = User.Identity.Name;
            int rtn = repo.Delete(ID, UpdatedBy);
            TempData["SuccessMessage"] = "Data Delete Successfully";

            return RedirectToAction("list");
        }
        public ActionResult GenerateDelayReportPDF(DateTime? startdate, DateTime? enddate)
        {
            var list = repo.GetDelayReport(startdate, enddate);

            DataTable dt = BuildDelayDataTable(list);

            MemoryStream ms = new MemoryStream();
            Document doc = new Document(PageSize.A4.Rotate(), 20, 20, 20, 20);
            PdfWriter.GetInstance(doc, ms);

            doc.Open();

            AddHeader(doc, $"Plant Delay Report");

            PdfPTable table = BuildDelayPdfTable(dt);
            doc.Add(table);

            doc.Close();

            return File(ms.ToArray(),
                "application/pdf",
                $"Delay_Report.pdf");
        }
        private DataTable BuildDelayDataTable(List<PlantDelayBLL> list)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("Area");
            dt.Columns.Add("Start Time");
            dt.Columns.Add("End Time");
            dt.Columns.Add("Duration (min)");
            dt.Columns.Add("Cobble");
            dt.Columns.Add("Hot Out");
            dt.Columns.Add("Delay Type");
            dt.Columns.Add("Delay Code");
            dt.Columns.Add("Component");
            dt.Columns.Add("Equipment");
            dt.Columns.Add("Reason");
            dt.Columns.Add("Delay Description");
            dt.Columns.Add("Reason For Occurrence");
            dt.Columns.Add("Action Taken");

            foreach (var x in list)
            {
                int duration = 0;
                if (x.StartTime.HasValue && x.EndTime.HasValue)
                    duration = (int)(x.EndTime.Value - x.StartTime.Value).TotalMinutes;

                dt.Rows.Add(
                    x.Area,
                    x.StartTime?.ToString(@"hh\:mm"),
                    x.EndTime?.ToString(@"hh\:mm"),
                    duration,
                    x.Cobble,
                    x.HotOut,
                    x.DelayType,
                    x.AgencyName,
                    x.Component,
                    x.Equipments,
                    x.Reason,
                    x.DelayDescription,
                    x.ReasonForOccurence,
                    x.ActionTaken
                );
            }

            return dt;
        }
        private void AddHeader(Document doc, string heading)
        {
            PdfPTable header = new PdfPTable(2);
            header.WidthPercentage = 100;

            // Define widths to avoid pushing heading
            float[] widths = new float[] { 10f, 90f };
            header.SetWidths(widths);

            // Left : LOGO
            string logoUrl = "http://10.1.10.202:8888/Content/images/logo.png";
            Image logo = Image.GetInstance(new Uri(logoUrl));
            logo.ScaleAbsolute(80, 40);

            PdfPCell logoCell = new PdfPCell(logo);
            logoCell.Border = Rectangle.NO_BORDER;
            header.AddCell(logoCell);

            // Right : Heading
            PdfPCell headCell = new PdfPCell(new Phrase(
                heading,
                FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20)))
            {
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            header.AddCell(headCell);

            doc.Add(header);
            doc.Add(new Paragraph("\n"));
        }
        private PdfPTable BuildDelayPdfTable(DataTable dt)
        {
            // base columns ALWAYS shown
            List<string> baseCols = new List<string>
    {
        "Date",
        "Area",
        "Start Time",
        "End Time",
        "Duration (min)",
        "Delay Code",
        "Equipment",
        "Reason",
        "Delay Description",
        "Reason For Occurence",
        "Action Taken"
    };

            // dynamic columns
            bool hasCobble = dt.AsEnumerable().Any(r => !string.IsNullOrEmpty(r["Cobble"].ToString()) && r["Cobble"].ToString() != "0");
            bool hasHotOut = dt.AsEnumerable().Any(r => !string.IsNullOrEmpty(r["HotOut"].ToString()) && r["HotOut"].ToString() != "0");

            if (hasCobble)
                baseCols.Insert(5, "Cobble"); // insert before Delay Code

            if (hasHotOut)
                baseCols.Insert(6, "HotOut"); // adjust index if needed

            PdfPTable table = new PdfPTable(baseCols.Count);
            table.WidthPercentage = 100;

            // HEADER ROW
            foreach (var col in baseCols)
            {
                PdfPCell headerCell = new PdfPCell(new Phrase(
                    col, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9)))
                {
                    BackgroundColor = BaseColor.LIGHT_GRAY,
                    Padding = 5,
                    HorizontalAlignment = Element.ALIGN_CENTER
                };
                table.AddCell(headerCell);
            }

            // DATA ROWS
            foreach (DataRow row in dt.Rows)
            {
                // DATE
                table.AddCell(new PdfPCell(new Phrase(
                    Convert.ToDateTime(row["CreatedDate"]).ToString("dd-MM-yyyy"),
                    FontFactory.GetFont(FontFactory.HELVETICA, 8)))
                { HorizontalAlignment = Element.ALIGN_CENTER });

                // AREA
                table.AddCell(new PdfPCell(new Phrase(
                    row["Area"].ToString(),
                    FontFactory.GetFont(FontFactory.HELVETICA, 8)))
                { HorizontalAlignment = Element.ALIGN_CENTER });

                // START TIME
                table.AddCell(new PdfPCell(new Phrase(
                    row["StartTime"].ToString(),
                    FontFactory.GetFont(FontFactory.HELVETICA, 8)))
                { HorizontalAlignment = Element.ALIGN_CENTER });

                // END TIME
                table.AddCell(new PdfPCell(new Phrase(
                    row["EndTime"].ToString(),
                    FontFactory.GetFont(FontFactory.HELVETICA, 8)))
                { HorizontalAlignment = Element.ALIGN_CENTER });

                // AUTO DURATION
                table.AddCell(new PdfPCell(new Phrase(
                    row["TotalDuration"].ToString(),
                    FontFactory.GetFont(FontFactory.HELVETICA, 8)))
                { HorizontalAlignment = Element.ALIGN_CENTER });

                // CONDITIONAL Cobble
                if (hasCobble)
                {
                    var cob = row["Cobble"]?.ToString();
                    table.AddCell(new PdfPCell(new Phrase(
                        string.IsNullOrEmpty(cob) || cob == "0" ? "-" : cob,
                        FontFactory.GetFont(FontFactory.HELVETICA, 8)))
                    { HorizontalAlignment = Element.ALIGN_CENTER });
                }

                // CONDITIONAL HotOut
                if (hasHotOut)
                {
                    var hot = row["HotOut"]?.ToString();
                    table.AddCell(new PdfPCell(new Phrase(
                        string.IsNullOrEmpty(hot) || hot == "0" ? "-" : hot,
                        FontFactory.GetFont(FontFactory.HELVETICA, 8)))
                    { HorizontalAlignment = Element.ALIGN_CENTER });
                }

                // DELAY CODE
                table.AddCell(new PdfPCell(new Phrase(
                    row["DelayCode"].ToString(),
                    FontFactory.GetFont(FontFactory.HELVETICA, 8)))
                { HorizontalAlignment = Element.ALIGN_CENTER });

                // EQUIPMENT
                table.AddCell(new PdfPCell(new Phrase(
                    row["Equipment"].ToString(),
                    FontFactory.GetFont(FontFactory.HELVETICA, 8)))
                { HorizontalAlignment = Element.ALIGN_CENTER });

                // REASON
                table.AddCell(new PdfPCell(new Phrase(
                    row["Reason"].ToString(),
                    FontFactory.GetFont(FontFactory.HELVETICA, 8)))
                { HorizontalAlignment = Element.ALIGN_LEFT });

                // DESCRIPTION
                table.AddCell(new PdfPCell(new Phrase(
                    row["DelayDescription"].ToString(),
                    FontFactory.GetFont(FontFactory.HELVETICA, 8)))
                { HorizontalAlignment = Element.ALIGN_LEFT });

                // OCCURRENCE
                table.AddCell(new PdfPCell(new Phrase(
                    row["ReasonForOccurence"].ToString(),
                    FontFactory.GetFont(FontFactory.HELVETICA, 8)))
                { HorizontalAlignment = Element.ALIGN_LEFT });

                // ACTION TAKEN
                table.AddCell(new PdfPCell(new Phrase(
                    row["ActionTaken"].ToString(),
                    FontFactory.GetFont(FontFactory.HELVETICA, 8)))
                { HorizontalAlignment = Element.ALIGN_LEFT });
            }

            return table;
        }

        public ActionResult SMPlist()
        {
            var data = repo.GetAllDelay();
            return View("~/Views/Meltshop/Delay/list.cshtml", data);
        }

        public ActionResult SMPadd()
        {
            var agencyList = repo.GetAllAgency().ToList(); // ensure it's a List
            ViewBag.Agencies = agencyList;

            var equipment = repo.GetAllEquipments()
              .Select(x => new
              {
                  Code = x.Code,
                  Text = x.Description + " - " + x.LocationName
              })
              .ToList();

            ViewBag.Equipment = new SelectList(equipment, "Text", "Text");
            //var equipment = repo.GetAllEquipments();
            //ViewBag.Equipment = new SelectList(equipment, "Code", "Description");

            var component = repo.GetAllComponent();
            ViewBag.Component = new SelectList(component, "Code", "Description");

            return View("~/Views/Meltshop/Delay/add.cshtml");
        }

        [HttpPost]
        public ActionResult SMPadd(PlantDelayBLL data)
        {
            if (data != null)
            {
                var agencies = repo.GetAllAgency().ToList();

                var agencyname = repo.GetAllAgency().ToList(); // ensure it's a List

                var selectedAgency = agencies
                    .Where(a => a.AgencyCode == data.AgencyCode)
                    .FirstOrDefault();

                if (selectedAgency != null)
                {
                    data.AgencyName = selectedAgency.AgencyName;  // Agency ka Name
                    data.AgencyCode = selectedAgency.AgencyCode;  // Agency ka Code
                    data.DelayType = selectedAgency.DelayType;
                }

                // Delay Code Auto Generate
                data.Delaycode = repo.GenerateSMPDelayCode();

                data.Date = DateTime.Now;
                data.StatusID = 1;
                data.CreatedDate = DateTime.Now;
                data.CreatedBy = User.Identity.Name;
                // int rtn1 = AddEntry(data);
                int rtn = repo.Insert(data);
                if (rtn > 0)
                {
                    TempData["SuccessMessage"] = "Data saved successfully";
                }
                else
                {
                    TempData["ErrorMessage"] = "Data not saved. Please try again.";
                    return RedirectToAction("SMPlist"); // 👈 back to form
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Invalid data submitted.";
                return RedirectToAction("SMPlist");
            }

            return RedirectToAction("SMPlist");
        }

        [HttpPost]
        public ActionResult AddDelayPopupDetail(FailureAnalysisBLL data)
        {
            try
            {
                if (data == null)
                {
                    TempData["ErrorMessage"] = "Invalid data.";
                    return RedirectToAction("SMPList");
                }

                data.StatusID = 1;
                data.CreatedDate = DateTime.Now;
                data.CreatedBy = User.Identity.Name;

                repo.InsertFailureAnalysis(data);

                TempData["SuccessMessage"] = "Delay detail added successfully.";
                return RedirectToAction("SMPList");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error while saving delay detail: " + ex.Message;
                return RedirectToAction("SMPList");
            }
        }

        [HttpGet]
        public JsonResult GetEquipmentByArea(int areaId)
        {
            var equipment = repo.GetEquipmentByArea(areaId)
                .Select(x => new
                {
                    Value = x.Description,
                    Text = x.Description
                })
                .ToList();

            return Json(equipment, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SMPdetails(int id)
        {
            var model = repo.GetDelayByID(id);

            if (model == null)
            {
                TempData["ErrorMessage"] = "Record not found.";
                return RedirectToAction("Index");
            }

            return View("~/Views/Meltshop/Delay/detail.cshtml", model);
        }
    }
}