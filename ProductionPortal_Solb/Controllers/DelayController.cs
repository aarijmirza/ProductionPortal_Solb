using BAL.Repositories;
using DAL.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        public ActionResult list(
            DateTime? date,
            string shift,
            string plant)
        {
            DateTime selectedDate;

            /*
                Priority:
                1. User-selected filter date
                2. Initially selected RM Detail date
                3. Today's date
            */
            if (date.HasValue)
            {
                selectedDate =
                    date.Value.Date;
            }
            else
            {
                DateTime rmSelectedDate;

                bool hasRMSelectedDate =
                    DateTime.TryParse(
                        Convert.ToString(
                            Session["RM_SelectedDate"]
                        ),
                        out rmSelectedDate
                    );

                selectedDate =
                    hasRMSelectedDate
                        ? rmSelectedDate.Date
                        : DateTime.Today;
            }

            /*
                When the page is first opened, use the shift and plant
                of the initially selected RM Detail.
            */
            if (string.IsNullOrWhiteSpace(shift))
            {
                shift =
                    Convert.ToString(
                        Session["RM_SelectedShift"]
                    );
            }

            if (string.IsNullOrWhiteSpace(plant))
            {
                plant =
                    Convert.ToString(
                        Session["RM_SelectedPlant"]
                    );
            }

            List<PlantDelayBLL> data =
                repo.GetAllRMDelay(
                    selectedDate.Date,
                    selectedDate.Date,
                    shift
                ) ??
                new List<PlantDelayBLL>();

            if (!string.IsNullOrWhiteSpace(plant))
            {
                string selectedPlant =
                    plant.Trim();

                data =
                    data
                        .Where(x =>
                            !string.IsNullOrWhiteSpace(
                                x.Plant
                            ) &&
                            x.Plant
                                .Trim()
                                .Equals(
                                    selectedPlant,
                                    StringComparison.OrdinalIgnoreCase
                                )
                        )
                        .ToList();
            }

            ViewBag.SelectedDate =
                selectedDate.ToString(
                    "yyyy-MM-dd"
                );

            ViewBag.Shift =
                shift ?? "";

            ViewBag.Plant =
                plant ?? "";

            return View(
                data
            );
        }

        public ActionResult SMPdelaydetails()
        {
            return View();
        }
        [HttpGet]
        public ActionResult add(
                   int? id,
                   DateTime? date,
                   string shift,
                   string plant)
        {
            PrepareDelayDropdowns();

            /*
                EDIT MODE
            */
            if (id.HasValue && id.Value > 0)
            {
                PlantDelayBLL existing =
                    repo.GetDelayByID(
                        id.Value
                    );

                if (existing == null)
                {
                    TempData["ErrorMessage"] =
                        "Delay record was not found.";

                    return RedirectToAction(
                        "list"
                    );
                }

                string selectedEquipment =
                    ResolveEquipmentValue(
                        existing.Equipments
                    );

                var editVM =
                    new DelaysVM
                    {
                        ID =
                            existing.ID,

                        Date =
                            existing.Date,

                        Plant =
                            existing.Plant,

                        Area =
                            existing.Area,

                        Shift =
                            existing.Shift,

                        Team =
                            existing.Team,

                        ShiftIncharge =
                            existing.ShiftIncharge,

                        StartTime =
                            existing.StartTime,

                        EndTime =
                            existing.EndTime,

                        TotalDuration =
                            existing.TotalDuration,

                        Cobble =
                            existing.Cobble,

                        HotOut =
                            existing.HotOut,

                        DelayType =
                            existing.DelayType,

                        Delaycode =
                            existing.Delaycode,

                        Reason =
                            existing.Reason,

                        DelayDescription =
                            existing.DelayDescription,

                        ReasonForOccurence =
                            existing.ReasonForOccurence,

                        ActionTaken =
                            existing.ActionTaken,

                        AgencyName =
                            existing.AgencyName,

                        AgencyCode =
                            existing.AgencyCode,

                        EquipmentName =
                            selectedEquipment,

                        Component =
                            existing.Component
                    };

                /*
                    Recreate dropdowns with selected values.
                */
                PrepareDelayDropdowns(
                    selectedEquipment,
                    existing.Component
                );

                ViewBag.IsEdit =
                    true;

                return View(
                    editVM
                );
            }

            /*
                ADD MODE
            */
            DateTime selectedDate =
                date ??
                ParseSessionDate(
                    Session["RM_SelectedDate"]
                ) ??
                ParseSessionDate(
                    Session["RM_Date"]
                ) ??
                DateTime.Today;

            string selectedShift =
                !string.IsNullOrWhiteSpace(shift)
                    ? shift.Trim()
                    : FirstNotEmpty(
                        Convert.ToString(
                            Session["RM_SelectedShift"]
                        ),
                        Convert.ToString(
                            Session["RM_Shift"]
                        )
                    );

            string selectedPlant =
                !string.IsNullOrWhiteSpace(plant)
                    ? plant.Trim()
                    : FirstNotEmpty(
                        Convert.ToString(
                            Session["RM_SelectedPlant"]
                        ),
                        Convert.ToString(
                            Session["RM_Plant"]
                        )
                    );

            int selectedShiftDetailID =
                ParseInt(
                    Session["RM_ShiftDetailID"]
                );

            RMShiftDetailsBLL selectedRMDetail =
                null;

            if (selectedShiftDetailID > 0)
            {
                selectedRMDetail =
                    rm.RollingMillDetails()
                        .FirstOrDefault(x =>
                            x.ID ==
                                selectedShiftDetailID &&
                            x.StatusID == 1
                        );
            }

            /*
                Fallback by Date + Plant + Shift.
            */
            if (
                selectedRMDetail == null &&
                !string.IsNullOrWhiteSpace(
                    selectedPlant
                ) &&
                !string.IsNullOrWhiteSpace(
                    selectedShift
                )
            )
            {
                DateTime nextDate =
                    selectedDate.Date.AddDays(1);

                selectedRMDetail =
                    rm.RollingMillDetails()
                        .Where(x =>
                            x.Date >=
                                selectedDate.Date &&
                            x.Date <
                                nextDate &&
                            x.Plant ==
                                selectedPlant &&
                            x.Shift ==
                                selectedShift &&
                            x.StatusID == 1
                        )
                        .OrderByDescending(
                            x => x.ID
                        )
                        .FirstOrDefault();
            }

            if (selectedRMDetail == null)
            {
                TempData["ErrorMessage"] =
                    "Please select Rolling Mill Details first.";

                return RedirectToAction(
                    "AddDetails",
                    "RollingMill"
                );
            }

            /*
                Keep Rolling Mill session aligned.
            */
            Session["RM_ShiftDetailID"] =
                selectedRMDetail.ID;

            Session["RM_SelectedDate"] =
                selectedRMDetail.Date.ToString(
                    "yyyy-MM-dd"
                );

            Session["RM_SelectedShift"] =
                selectedRMDetail.Shift;

            Session["RM_SelectedPlant"] =
                selectedRMDetail.Plant;

            var vm =
                new DelaysVM
                {
                    Date =
                        selectedRMDetail.Date,

                    Shift =
                        selectedRMDetail.Shift,

                    Plant =
                        selectedRMDetail.Plant,

                    Team =
                        selectedRMDetail.Team,

                    ShiftIncharge =
                        selectedRMDetail.ShiftIncharge,

                    Area =
                        "Rolling Mill",

                    Cobble =
                        0,

                    HotOut =
                        0
                };

            ViewBag.IsEdit =
                false;

            return View(
                vm
            );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult add(
            PlantDelayBLL data)
        {
            if (data == null)
            {
                TempData["ErrorMessage"] =
                    "Invalid data submitted.";

                return RedirectToAction(
                    "list"
                );
            }

            try
            {
                /*
                    Resolve Agency Name, Code and Delay Type.
                */
                var agencies =
                    repo.GetAllAgency()
                        ?.ToList()
                    ?? new List<PlantDelayBLL>();

                PlantDelayBLL selectedAgency =
                    agencies.FirstOrDefault(a =>
                        string.Equals(
                            a.AgencyCode,
                            data.AgencyCode,
                            StringComparison.OrdinalIgnoreCase
                        )
                        ||
                        string.Equals(
                            a.AgencyName,
                            data.AgencyName,
                            StringComparison.OrdinalIgnoreCase
                        )
                    );

                if (selectedAgency != null)
                {
                    data.AgencyName =
                        selectedAgency.AgencyName;

                    data.AgencyCode =
                        selectedAgency.AgencyCode;

                    data.DelayType =
                        selectedAgency.DelayType;
                }

                /*
                    Recalculate duration for both Insert and Update.
                    Overnight delays are supported.
                */
                data.TotalDuration =
                    CalculateDurationMinutes(
                        data.StartTime,
                        data.EndTime
                    );

                data.StatusID =
                    1;

                int result;

                if (data.ID > 0)
                {
                    data.UpdatedDate =
                        DateTime.Now;

                    data.UpdatedBy =
                        User != null &&
                        User.Identity != null
                            ? User.Identity.Name
                            : "";

                    result =
                        repo.Update(
                            data
                        );

                    TempData[
                        result > 0
                            ? "SuccessMessage"
                            : "ErrorMessage"
                    ] =
                        result > 0
                            ? "Delay record updated successfully."
                            : "Delay record was not updated.";
                }
                else
                {
                    data.Delaycode =
                        repo.GenerateDelayCode();

                    data.CreatedDate =
                        DateTime.Now;

                    data.CreatedBy =
                        User != null &&
                        User.Identity != null
                            ? User.Identity.Name
                            : "";

                    result =
                        repo.Insert(
                            data
                        );

                    TempData[
                        result > 0
                            ? "SuccessMessage"
                            : "ErrorMessage"
                    ] =
                        result > 0
                            ? "Delay record saved successfully."
                            : "Delay record was not saved.";
                }

                /*
                    Return to the same selected date/shift/plant.
                */
                return RedirectToAction(
                    "list",
                    new
                    {
                        date =
                            data.Date.HasValue
                                ? data.Date.Value.ToString(
                                    "yyyy-MM-dd"
                                )
                                : DateTime.Today.ToString(
                                    "yyyy-MM-dd"
                                ),

                        shift =
                            data.Shift,

                        plant =
                            data.Plant
                    }
                );
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] =
                    "Unable to save delay record. " +
                    ex.Message;

                return RedirectToAction(
                    "add",
                    new
                    {
                        id =
                            data.ID > 0
                                ? (int?)data.ID
                                : null,

                        date =
                            data.Date.HasValue
                                ? data.Date.Value.ToString(
                                    "yyyy-MM-dd"
                                )
                                : null,

                        shift =
                            data.Shift,

                        plant =
                            data.Plant
                    }
                );
            }
        }

        private void PrepareDelayDropdowns(
            string selectedEquipment = null,
            string selectedComponent = null)
        {
            ViewBag.Agencies =
                repo.GetAllAgency()
                ?? new List<PlantDelayBLL>();

            var equipmentSource =
                repo.GetAllRMEquipments()
                ?? new List<DelayEquipmentBLL>();

            var equipmentItems =
                equipmentSource
                    .Where(x => x != null)
                    .Select(x => new SelectListItem
                    {
                        Value =
                            BuildEquipmentText(x),

                        Text =
                            BuildEquipmentText(x),

                        Selected =
                            EquipmentMatches(
                                x,
                                selectedEquipment
                            )
                    })
                    .ToList();

            ViewBag.EquipmentItems =
                equipmentItems;

            var componentSource =
                repo.GetAllComponent()
                ?? new List<DelayComponentBLL>();

            ViewBag.Component =
                new SelectList(
                    componentSource,
                    "Code",
                    "Description",
                    selectedComponent
                );
        }

        private string ResolveEquipmentValue(
            string storedValue)
        {
            string value =
                (storedValue ?? string.Empty)
                    .Trim();

            var equipmentSource =
                repo.GetAllRMEquipments()
                ?? new List<DelayEquipmentBLL>();

            DelayEquipmentBLL matched =
                equipmentSource.FirstOrDefault(x =>
                    EquipmentMatches(
                        x,
                        value
                    )
                );

            return matched != null
                ? BuildEquipmentText(
                    matched
                )
                : value;
        }

        private bool EquipmentMatches(
            DelayEquipmentBLL equipment,
            string storedValue)
        {
            if (equipment == null)
            {
                return false;
            }

            string value =
                (storedValue ?? string.Empty)
                    .Trim();

            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            string code =
                (equipment.Code ?? string.Empty)
                    .Trim();

            string equipmentText =
                BuildEquipmentText(
                    equipment
                );

            return
                string.Equals(
                    code,
                    value,
                    StringComparison.OrdinalIgnoreCase
                )
                ||
                string.Equals(
                    equipmentText,
                    value,
                    StringComparison.OrdinalIgnoreCase
                );
        }

        private string BuildEquipmentText(
            DelayEquipmentBLL equipment)
        {
            if (equipment == null)
            {
                return string.Empty;
            }

            string description =
                (equipment.Description ?? string.Empty)
                    .Trim();

            string location =
                (equipment.LocationName ?? string.Empty)
                    .Trim();

            if (string.IsNullOrWhiteSpace(description))
            {
                return location;
            }

            if (string.IsNullOrWhiteSpace(location))
            {
                return description;
            }

            return
                description +
                " - " +
                location;
        }

        private int CalculateDurationMinutes(
            TimeSpan? startTime,
            TimeSpan? endTime)
        {
            if (
                !startTime.HasValue ||
                !endTime.HasValue
            )
            {
                return 0;
            }

            TimeSpan start =
                startTime.Value;

            TimeSpan end =
                endTime.Value;

            if (end < start)
            {
                end =
                    end.Add(
                        TimeSpan.FromDays(1)
                    );
            }

            return Convert.ToInt32(
                (end - start).TotalMinutes
            );
        }

        private DateTime? ParseSessionDate(
            object value)
        {
            DateTime parsedDate;

            return DateTime.TryParse(
                Convert.ToString(value),
                out parsedDate
            )
                ? parsedDate.Date
                : (DateTime?)null;
        }

        private int ParseInt(
            object value)
        {
            int parsedValue;

            return int.TryParse(
                Convert.ToString(value),
                out parsedValue
            )
                ? parsedValue
                : 0;
        }

        private string FirstNotEmpty(
            params string[] values)
        {
            return values
                .FirstOrDefault(x =>
                    !string.IsNullOrWhiteSpace(x)
                )
                ?? "";
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

        [HttpGet]
        public ActionResult SMPadd()
        {
            var agencyList =
                repo.GetAllAgency()
                ?? new List<PlantDelayBLL>();

            ViewBag.Agencies =
                agencyList;

            // Equipment area select hone ke baad AJAX se load hoga.
            ViewBag.Equipment =
                new SelectList(
                    Enumerable.Empty<SelectListItem>(),
                    "Value",
                    "Text"
                );

            var component =
                repo.GetAllComponent()
                ?? new List<DelayComponentBLL>();

            ViewBag.Component =
                new SelectList(
                    component,
                    "Code",
                    "Description"
                );

            return View(
                "~/Views/Meltshop/Delay/add.cshtml",
                new DelaysVM()
            );
        }

        [HttpGet]
        public JsonResult GetEquipmentByArea(
    string areaId)
        {
            try
            {
                var equipmentSource =
                    repo.GetAllEquipments()
                    ?? new List<DelayEquipmentBLL>();

                var equipment =
                    equipmentSource
                        .Where(x =>
                            x != null &&
                            x.PlantArea == areaId
                        )
                        .Select(x => new
                        {
                            Value =
                                !string.IsNullOrWhiteSpace(x.Code)
                                    ? x.Code
                                    : (
                                        (x.Description ?? "") +
                                        " - " +
                                        (x.LocationName ?? "")
                                    ).Trim(' ', '-'),

                            Text =
                                (
                                    (x.Description ?? "") +
                                    (
                                        string.IsNullOrWhiteSpace(
                                            x.LocationName
                                        )
                                            ? ""
                                            : " - " +
                                              x.LocationName
                                    )
                                ).Trim()
                        })
                        .Where(x =>
                            !string.IsNullOrWhiteSpace(
                                x.Text
                            )
                        )
                        .OrderBy(x =>
                            x.Text
                        )
                        .ToList();

                return Json(
                    equipment,
                    JsonRequestBehavior.AllowGet
                );
            }
            catch (Exception ex)
            {
                Response.StatusCode =
                    500;

                return Json(
                    new
                    {
                        message =
                            "Unable to load equipment. " +
                            ex.Message
                    },
                    JsonRequestBehavior.AllowGet
                );
            }
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

        //[HttpGet]
        //public JsonResult GetEquipmentByArea(int areaId)
        //{
        //    var equipment = repo.GetEquipmentByArea(areaId)
        //        .Select(x => new
        //        {
        //            Value = x.Description,
        //            Text = x.Description
        //        })
        //        .ToList();

        //    return Json(equipment, JsonRequestBehavior.AllowGet);
        //}

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