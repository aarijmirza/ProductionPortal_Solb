using BAL.Repositories;
using DAL.Models;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebAPICode.Helpers;
using static DAL.Models.ViewModel;

namespace ProductionPortal_Solb.Controllers
{
    public class RollingMillController : BaseController
    {
        QualityRepository repo;
        RollingMillRepository rm;
        DelayRespository delay;

        public RollingMillController()
        {
            repo = new QualityRepository();
            rm = new RollingMillRepository();
            delay = new DelayRespository();
        }
        [Route("Charging")]
        // GET: RollingMill
        public ActionResult Charging()
        {
            var data = repo.GetAllCharging();
            return View("~/Views/RollingMill/Charging/list.cshtml", data);
        }


        [HttpGet]
        public JsonResult GetBilletBoardingByHeat(string heatNo)
        {
            var data = repo.GetAllBoarding()
                           .Where(x => x.HeatNo == heatNo)
                           .Select(x => new
                           {
                               BoardingNo = x.BilletBoarding,
                               SteelGrade = x.Grade,
                               BilletSize = x.CrossSection,
                               NoOfBillet = x.NoOfBillets,
                               BilletWeight = x.BilletWeight,
                               BilletLength = x.BilletLength
                               //Weight = x.Weight,
                               //TotalBillet = x.TotalBillet,
                               //TotalWeight = x.TotalWeight
                           })
                           .FirstOrDefault();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        private string GetDefaultShift()
        {
            var hour = DateTime.Now.Hour;

            if (hour >= 7 && hour < 15)
                return "Morning";

            if (hour >= 15 && hour < 23)
                return "Afternoon";

            // 23:00 – 07:00
            return "Night";
        }

        public ActionResult AddCharge()
        {
            var heats = repo.GetAllBoarding();
            var submittedAll = repo.GetAllCharging();

            // 🔥 SESSION VALUES
            if (Session["RM_Date"] == null || Session["RM_Shift"] == null)
            {
                TempData["ErrorMessage"] = "Please select Rolling Mill Shift first.";
                return RedirectToAction("AddDetails", "RollingMill");
            }

            DateTime selectedDate = Convert.ToDateTime(Session["RM_Date"]);
            string selectedShift = Session["RM_Shift"].ToString().Trim();

            var start = selectedDate.Date;
            var end = start.AddDays(1);

            // 🔥 DEBUG (REMOVE AFTER TEST)
            Console.WriteLine("SESSION DATE: " + selectedDate);
            Console.WriteLine("SESSION SHIFT: " + selectedShift);

            foreach (var x in rm.RollingMillDetails())
            {
                Console.WriteLine($"DB => {x.Date} | '{x.Shift}'");
            }

            // 🔥 FINAL MATCH (SAFE VERSION)
            var shiftDetails = rm.RollingMillDetails()
                .Where(x =>
                    x.Date >= start &&
                    x.Date < end &&
                    !string.IsNullOrEmpty(x.Shift) &&
                    x.Shift.Trim().ToLower().Contains(selectedShift.ToLower())
                )
                .OrderByDescending(x => x.ID)
                .FirstOrDefault();

            if (shiftDetails == null)
            {
                TempData["ErrorMessage"] = "Shift not found. Debug mismatch.";
                return RedirectToAction("AddDetails", "RollingMill");
            }

            // 🔥 FILTER SUBMITTED HEATS
            var submittedToday = submittedAll
                .Where(x =>
                    x.Date >= start &&
                    x.Date < end &&
                    !string.IsNullOrEmpty(x.Shift) &&
                    x.Shift.Trim().ToLower().Contains(selectedShift.ToLower())
                )
                .OrderBy(x => x.CreatedDate)
                .ToList();

            // 🔥 DISABLE USED HEATS
            var submittedHeatSet = new HashSet<string>(
                submittedAll
                    .Where(s => !string.IsNullOrWhiteSpace(s.HeatNo))
                    .Select(s => s.HeatNo.Trim()),
                StringComparer.OrdinalIgnoreCase
            );

            ViewBag.HeatNo = heats.Select(h => new SelectListItem
            {
                Text = h.HeatNo,
                Value = h.HeatNo,
                Disabled = submittedHeatSet.Contains(h.HeatNo.Trim())
            }).ToList();

            // 🔥 VIEW MODEL
            var vm = new RMChargingVM
            {
                Date = shiftDetails.Date,
                Shift = shiftDetails.Shift,
                Plant = shiftDetails.Plant,
                Team = shiftDetails.Team,
                ShiftIncharge = shiftDetails.ShiftIncharge,
                SubmittedHeat = submittedToday
            };

            return View("~/Views/RollingMill/Charging/Add.cshtml", vm);
        }

        [HttpPost]
        public ActionResult AddCharge(BilletChargingBLL model)
        {
            // 🔥 Get selected shift from SESSION
            DateTime selectedDate = Session["RM_Date"] != null
                ? Convert.ToDateTime(Session["RM_Date"])
                : DateTime.Today;

            string selectedShift = Session["RM_Shift"]?.ToString();

            if (string.IsNullOrEmpty(selectedShift))
            {
                TempData["ErrorMessage"] = "Please select Rolling Mill Shift first.";
                return RedirectToAction("AddDetails", "RollingMill");
            }

            // 🔥 Fetch correct shift record
            var shiftDetails = rm.RollingMillDetails()
                .Where(x => x.Date == selectedDate && x.Shift == selectedShift)
                .OrderByDescending(x => x.ID)
                .FirstOrDefault();

            if (shiftDetails == null)
            {
                TempData["ErrorMessage"] = "Please add Rolling Mill Details for selected date & shift.";
                return RedirectToAction("AddDetails", "RollingMill");
            }

            // 🔥 Assign values
            model.Date = shiftDetails.Date;
            model.Shift = shiftDetails.Shift;

            model.CreatedDate = DateTime.Now;
            model.CreatedBy = User.Identity.Name;
            model.StatusID = 1;

            int rtn = rm.InsertBilletCharging(model);

            if (rtn < 0)
            {
                TempData["SuccessMessage"] = "Heat charged successfully.";
                return View("~/Views/RollingMill/Charging/list.cshtml", TempData);
            }
            else
            {
                TempData["ErrorMessage"] = "Data not saved.";
                return View("~/Views/RollingMill/Charging/list.cshtml", TempData);
            }
        }

        [Route("Discharging")]
        public ActionResult Discharging(DateTime? from, DateTime? to)
        {
            // 🔑 Default = TODAY
            DateTime startDate = from ?? DateTime.Today;
            DateTime endDate = to ?? DateTime.Today;

            var data = rm.GetDichargedHeat(startDate, endDate);

            return View("~/Views/RollingMill/Discharging/list.cshtml", data);
        }
        [HttpGet]
        public JsonResult GetChargingByHeat(string heatNo)
        {
            var data = repo.GetAllCharging()
                           .Where(x => x.HeatNo == heatNo)
                           .Select(x => new
                           {
                               HeatNo = x.HeatNo,
                               BoardingNo = x.BoardingNo,
                               SteelGrade = x.SteelGrade,
                               Profile = x.Profile,
                               TotalWeight = x.TotalWeight,
                               TotalBillet = x.TotalBillet,
                               ProductSpecs = x.ProductSpecs,
                               //NoOfBillet = x.NoOfBillets,
                               //BilletWeight = x.BilletWeight,
                               //BilletLength = x.BilletLength

                           })
                           .FirstOrDefault();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddDischarge()
        {
            var heats = repo.GetAllCharging();
            var submittedAll = rm.GetDichargedHeat2();

            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            // ✅ Get today's shift details from RM Shift Details table
            var todayShiftDetails = rm.RollingMillDetails()
                .Where(x => x.Date >= today && x.Date < tomorrow)
                .OrderByDescending(x => x.ID)
                .FirstOrDefault();

            if (todayShiftDetails == null)
            {
                TempData["ErrorMessage"] = "Please add Rolling Mill Details for today first.";
                return RedirectToAction("AddDetails", "RollingMill");
            }

            var submittedToday = submittedAll
                .Where(x => x.CreatedOn >= today && x.CreatedOn < tomorrow)
                .OrderBy(x => x.CreatedOn)
                .ToList();

            // 🔹 Disable already discharged heats
            var submittedHeatSet = new HashSet<string>(
                submittedAll
                    .Where(s => !string.IsNullOrWhiteSpace(s.HeatNo))
                    .Select(s => s.HeatNo.Trim()),
                StringComparer.OrdinalIgnoreCase
            );

            ViewBag.HeatNo = heats.Select(h => new SelectListItem
            {
                Text = h.HeatNo,
                Value = h.HeatNo,
                Disabled = submittedHeatSet.Contains(h.HeatNo.Trim())
            }).ToList();

            var vm = new RMDischargingVM
            {
                Date = todayShiftDetails.Date,
                Shift = todayShiftDetails.Shift,
                Plant = todayShiftDetails.Plant,
                Team = todayShiftDetails?.Team,
                ShiftIncharge = todayShiftDetails?.ShiftIncharge,
                SubmittedHeat = submittedToday
            };

            var BilletGradeList = repo.GetBilletGrade();
            ViewBag.BilletGrade = new SelectList(BilletGradeList, "ProductID", "SpecGrade");
            ViewBag.GradeDataJson = JsonConvert.SerializeObject(BilletGradeList);

            return View("~/Views/RollingMill/Discharging/Add.cshtml", vm);
        }

        [HttpPost]
        public ActionResult AddDischarge(BilletDischargingBLL data)
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            // ✅ Get today's shift details from RM Shift Details table
            var todayShiftDetails = rm.RollingMillDetails()
                .Where(x => x.Date >= today && x.Date < tomorrow)
                .OrderByDescending(x => x.ID)
                .FirstOrDefault();

            if (todayShiftDetails == null)
            {
                TempData["ErrorMessage"] = "Please add Rolling Mill Details for today first.";
                return RedirectToAction("AddDetails", "RollingMill");
            }

            // dropdown reload
            var heatList = repo.GetAllCharging();
            ViewBag.HeatNo = new SelectList(heatList, "HeatNo", "HeatNo", data.HeatNo);

            var BilletGradeList = repo.GetBilletGrade();
            ViewBag.BilletGrade = new SelectList(BilletGradeList, "ProductID", "SpecGrade");
            ViewBag.GradeDataJson = JsonConvert.SerializeObject(BilletGradeList);

            var model = rm.GetDichargedByHeatNo(data.HeatNo);

            if (model != null && !string.IsNullOrWhiteSpace(model.HeatNo))
            {
                ModelState.AddModelError("HeatNo", "Heat is already discharged");
                return View("~/Views/RollingMill/Discharging/Add.cshtml", data);
            }

            // ✅ force Date and Shift from today's RM Shift Details
            data.Date = todayShiftDetails.Date;
            data.Shift = todayShiftDetails.Shift;

            data.StatusID = 1;
            data.CreatedOn = DateTime.Now;
            data.CreatedBy = User.Identity.Name;

            int rtn = rm.InsertDischarging(data);

            if (rtn < 0)
            {
                TempData["SuccessMessage"] = "Data saved successfully";
                return RedirectToAction("AddDischarge");
            }

            TempData["ErrorMessage"] = "Data not saved";
            return View("~/Views/RollingMill/Discharging/Add.cshtml", data);
        }

        //[Route("HeatDischarge")]
        //[HttpPost]
        //public ActionResult AddDischarge(BilletDischargingBLL data)
        //{
        //    // dropdown reload
        //    var heatList = repo.GetAllCharging();
        //    ViewBag.HeatNo = new SelectList(heatList, "HeatNo", "HeatNo", data.HeatNo);

        //    var model = rm.GetDichargedByHeatNo(data.HeatNo);

        //    if (model.HeatNo != null)
        //    {
        //        ModelState.AddModelError("HeatNo", "Heat is already discharged");
        //        return View("~/Views/RollingMill/Discharging/Add.cshtml", data);
        //    }

        //    // ✅ insert
        //    data.StatusID = 1;   // discharged
        //    data.CreatedOn = DateTime.Now;
        //    data.CreatedBy = User.Identity.Name;

        //    int rtn = rm.InsertDischarging(data);

        //    if (rtn < 0)
        //    {
        //        TempData["SuccessMessage"] = "Data saved successfully";
        //        return RedirectToAction("AddDischarge");
        //    }

        //    TempData["ErrorMessage"] = "Data not saved";
        //    return View("~/Views/RollingMill/Discharging/Add.cshtml", data);
        //}


        public ActionResult ShiftProduction()
        {
            return View("~/Views/RollingMill/ShiftProduction/ShiftProduction.cshtml");
        }

        public ActionResult AddShiftProduction()
        {
            return View("~/Views/RollingMill/ShiftProduction/addShiftProduction.cshtml");
        }

        public ActionResult RMConsumptionlist()
        {
            return View("~/Views/RollingMill/RMConsumption/RMConsumptionlist");
        }

        public ActionResult RMConsumptionAdd()
        {
            return View("~/Views/RollingMill/RMConsumption/RMConsumptionAdd");
        }

        //public ActionResult GenerateDelayReportPDF(DateTime? startdate, DateTime? enddate, string shift)
        //{
        //    // Force Gregorian
        //    var greg = new CultureInfo("en-US");
        //    greg.DateTimeFormat.Calendar = new GregorianCalendar();

        //    DateTime? sDate = null;
        //    DateTime? eDate = null;

        //    if (startdate.HasValue)
        //        sDate = startdate.Value.Date;

        //    if (enddate.HasValue)
        //        eDate = enddate.Value.Date.AddDays(1).AddTicks(-1); // inclusive

        //    //var data = delay.GetAllRMDelay(sDate, eDate).AsQueryable();

        //    // ✅ Rolling Mill (case + space safe)
        //    data = data.Where(x => x.Area != null &&
        //                           x.Area.Trim().ToLower() == "rolling mill");

        //    // ✅ Date filters
        //    if (sDate.HasValue)
        //        data = data.Where(x => x.Date >= sDate.Value);

        //    if (eDate.HasValue)
        //        data = data.Where(x => x.Date <= eDate.Value);

        //    // ✅ NEW: Shift filter
        //    if (!string.IsNullOrEmpty(shift))
        //    {
        //        data = data.Where(x => x.Shift != null &&
        //                               x.Shift.Trim().ToLower() == shift.Trim().ToLower());
        //    }

        //    var result = data.ToList();

        //    // 🔎 DEBUG CHECK (remove later)
        //    if (!result.Any())
        //    {
        //        ViewBag.Debug = "No data after filter";
        //    }

        //    return View("GenerateDelayReportPDF", result);
        //}

        [Route("BundleSection")]
        public ActionResult BundleSection()
        {
            var submitted = rm.GetBundlesHeats();
            return View("~/Views/RollingMill/BundleSection/BundleSection.cshtml", submitted);
        }

        [Route("AddBundleSection")]
        public ActionResult AddBundleSection()
        {
            var heats = repo.GetAllCharging();
            var submitted = rm.GetBundlesHeats();

            var items = heats.Select(h => new SelectListItem
            {
                Text = h.HeatNo,
                Value = h.HeatNo,
                Disabled = submitted.Any(s => s.HeatNo == h.HeatNo)
            }).ToList();

            ViewBag.HeatNo = items;

            //ViewBag.HeatNo = new SelectList(availableHeats, "HeatNo", "HeatNo");

            // table ke liye
            var vm = new RollingMillChargeVM();
            vm.SubmittedHeat = rm.GetBundlesHeats(); // grid data

            return View("~/Views/RollingMill/BundleSection/AddBundleSection.cshtml", vm);
        }
        //public ActionResult AddBundleSection()
        //{
        //    var data = repo.GetAllCharging();
        //    ViewBag.HeatNo = new SelectList(data, "HeatNo", "HeatNo");
        //    return View("~/Views/RollingMill/BundleSection/AddBundleSection.cshtml");
        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InsertBundleSection(BundlingSectionBLL data)
        {

            data.StatusID = 1;   // discharged
            data.CreatedDate = DateTime.Now;
            data.CreatedBy = User.Identity.Name;

            int rtn = rm.AddBundlingSection(data);

            if (rtn == -1)
            {
                TempData["SuccessMessage"] = "Data saved successfully";
                return RedirectToAction("AddBundleSection");
            }

            TempData["ErrorMessage"] = "Data not saved";
            return RedirectToAction("AddBundleSection");
            //return View("~/Views/RollingMill/BundleSection/AddBundleSection.cshtml");
        }
        public ActionResult ShiftSummaryReportPDF(DateTime? from, DateTime? to)
        {
            // 1️⃣ Default dates = Today
            DateTime startDate = from?.Date ?? DateTime.Today;
            DateTime endDate = (to?.Date ?? DateTime.Today).AddDays(1); // next day 00:00

            // 2️⃣ Fetch raw data
            var delays = delay.GetAllRMDelay(startDate, endDate);            // IQueryable or List
            var heats = rm.GetDichargedHeat(startDate, endDate);          // IQueryable or List

            // 3️⃣ Apply date filters SAFELY
            delays = delays
                        .Where(x => x.Date >= startDate && x.Date < endDate)
                        .ToList();

            heats = heats
                        .Where(x => x.Date >= startDate && x.Date < endDate)
                        .ToList();

            // 4️⃣ Build ViewModel
            var vm = new ShiftProductionReportVM
            {
                Delays = delays ?? new List<PlantDelayBLL>(),
                DischargedHeats = heats ?? new List<BilletDischargingBLL>()
            };

            return View(vm);
        }

        [Route("ProductionDetails")]
        public ActionResult AddDetails()
        {
            var vm = new RollingMillPageVM
            {
                Form = new RMShiftDetailsBLL(),
                List = rm.RMShiftDetailAll()
            };

            return View(vm);
        }
        [HttpPost]
        public ActionResult AddDetails(RMShiftDetailsBLL data)
        {

            data.StatusID = 1;
            data.CreatedDate = DateTime.Now;
            data.CreatedBy = User.Identity.Name;

            int rtn;

            if (data.ID > 0)
            {
                // 🔥 UPDATE
                rtn = rm.UpdateRMShiftDetails(data);
                TempData["SuccessMessage"] = "Record updated successfully";
            }
            else
            {
                // 🔥 DUPLICATE CHECK
                bool exists = rm.IsShiftExist(data.Date, data.Plant, data.Shift);

                if (exists)
                {
                    TempData["ErrorMessage"] = "Record already exists!";
                    return RedirectToAction("AddDetails");
                }

                // 🔥 INSERT
                rtn = rm.AddRMShiftDetails(data);
                TempData["SuccessMessage"] = "Data saved successfully";
            }

            return RedirectToAction("AddDetails");
        }

        [HttpPost]
        public JsonResult SetSelectedShift(string date, string shift, string plant)
        {
            Session["RM_Date"] = Convert.ToDateTime(date);
            Session["RM_Shift"] = shift;
            Session["RM_Plant"] = plant;

            return Json(new { success = true });
        }
        //public ActionResult AddDetails(RMShiftDetailsBLL data)
        //{
        //    data.StatusID = 1;
        //    data.CreatedDate = DateTime.Now;
        //    data.CreatedBy = User.Identity.Name;

        //    int rtn = rm.AddRMShiftDetails(data);

        //    if (rtn == -1)
        //    {
        //        TempData["SuccessMessage"] = "Data saved successfully";
        //        return RedirectToAction("Index", "Home");
        //    }

        //    TempData["ErrorMessage"] = "Data not saved";
        //    return RedirectToAction("AddDetails");
        //}

        [HttpPost]
        public JsonResult SetSelectedDateAjax(DateTime date)
        {
            Session["RollingMillSelectedDate"] = date.Date;

            return Json(new
            {
                success = true
            });
        }


    }
}