using BAL.Repositories;
using DAL.Models;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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

        //[Route("HeatCharge")]
        //public ActionResult AddCharge(string shift = null)
        //{
        //    var heats = repo.GetAllBoarding();
        //    var submittedAll = repo.GetAllCharging();

        //    var today = DateTime.Today;

        //    var submittedToday = submittedAll
        //        .Where(x => x.CreatedDate.Date == today)
        //        .OrderBy(x => x.CreatedDate)
        //        .ToList();

        //    string shiftValue = submittedToday.FirstOrDefault()?.Shift ?? "Morning";

        //    var shiftList = new List<string>
        //    {
        //        "Morning",
        //        "Afternoon",
        //        "Night",
        //        "Long Morning",
        //        "Long Night"
        //    };

        //    ViewBag.Shift = new SelectList(shiftList);


        //    // 🔹 Disable already submitted heats
        //    var submittedHeatSet = new HashSet<string>(
        //        submittedAll
        //            .Where(s => !string.IsNullOrWhiteSpace(s.HeatNo))
        //            .Select(s => s.HeatNo.Trim()),
        //        StringComparer.OrdinalIgnoreCase
        //    );

        //    ViewBag.HeatNo = heats.Select(h => new SelectListItem
        //    {
        //        Text = h.HeatNo,
        //        Value = h.HeatNo,
        //        Disabled = submittedHeatSet.Contains(h.HeatNo.Trim())
        //    }).ToList();

        //    var vm = new RMChargingVM
        //    {
        //        Shift = shiftValue,        // 🔥 THIS IS IMPORTANT
        //        SubmittedHeat = submittedToday
        //    };

        //    return View("~/Views/RollingMill/Charging/Add.cshtml", vm);
        //}

        [Route("HeatCharge")]
        public ActionResult AddCharge()
        {
            var heats = repo.GetAllBoarding();
            var submittedAll = repo.GetAllCharging();

            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            // ✅ Aaj ki Shift Details uthao
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
                .Where(x => x.CreatedDate >= today && x.CreatedDate < tomorrow)
                .OrderBy(x => x.CreatedDate)
                .ToList();

            // 🔹 Disable already submitted heats
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

            var vm = new RMChargingVM
            {
                Date = todayShiftDetails.Date,
                Shift = todayShiftDetails.Shift,
                Plant = todayShiftDetails.Plant,
                Team = todayShiftDetails?.Team,
                ShiftIncharge = todayShiftDetails?.ShiftIncharge,
                SubmittedHeat = submittedToday
            };

            return View("~/Views/RollingMill/Charging/Add.cshtml", vm);
        }

        [HttpPost]
        public ActionResult AddCharge(BilletChargingBLL model)
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var todayShiftDetails = rm.RollingMillDetails()
                .Where(x => x.Date >= today && x.Date < tomorrow)
                .OrderByDescending(x => x.ID)
                .FirstOrDefault();

            if (todayShiftDetails == null)
            {
                TempData["ErrorMessage"] = "Please add Rolling Mill Details for today first.";
                return RedirectToAction("AddDetails", "RollingMill");
            }

            model.Date = todayShiftDetails.Date;
            model.Shift = todayShiftDetails.Shift;
            model.CreatedDate = DateTime.Now;
            model.CreatedBy = User.Identity.Name;

            int rtn = rm.InsertBilletCharging(model);

            if (rtn > 0)
            {
                TempData["SuccessMessage"] = "Heat charged successfully.";
                return RedirectToAction("AddCharge");
            }

            TempData["ErrorMessage"] = "Data not saved.";
            return View("~/Views/RollingMill/Charging/Add.cshtml", model);
        }

        //[HttpPost]
        //[Route("HeatCharge")]
        //public ActionResult AddCharge(BilletChargingBLL data)
        //{
        //    // 🔁 ALWAYS reload dropdown before returning View
        //    var heatList = repo.GetAllBoarding();
        //    ViewBag.HeatNo = new SelectList(heatList, "HeatNo", "HeatNo", data.HeatNo);

        //    if (!ModelState.IsValid)
        //        return View("~/Views/RollingMill/Charging/Add.cshtml", data);

        //    // ✅ Check if Heat is already on charging
        //    if (!string.IsNullOrEmpty(data.HeatNo) && rm.IsHeatOnCharging(data.HeatNo))
        //    {
        //        ModelState.AddModelError("HeatNo", "Heat is already on charging");
        //        return View("~/Views/RollingMill/Charging/Add.cshtml", data);
        //    }

        //    // ✅ Set audit & status
        //    data.StatusID = 1; // Charging (use same status everywhere)
        //    data.CreatedDate = DateTime.Now;
        //    data.CreatedBy = User.Identity.Name;

        //    int rtn = rm.InsertBilletCharging(data);

        //    if (rtn < 0)
        //    {
        //        TempData["SuccessMessage"] = "Data saved successfully";
        //        return RedirectToAction("AddCharge");
        //    }

        //    TempData["ErrorMessage"] = "Data not saved. Please try again.";
        //    //return RedirectToAction("AddCharge");
        //    return View("~/Views/RollingMill/Charging/add.cshtml");
        //}

        [Route("Discharging")]
        public ActionResult Discharging()
        {
            var data = rm.GetDichargedHeat();

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
                               //BilletSize = x.CrossSection,
                               //NoOfBillet = x.NoOfBillets,
                               //BilletWeight = x.BilletWeight,
                               //BilletLength = x.BilletLength

                           })
                           .FirstOrDefault();

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        //[Route("HeatDischarge")]
        //public ActionResult AddDischarge(string shift = null)
        //{
        //    var heats = repo.GetAllCharging();
        //    var submittedAll = rm.GetDichargedHeat();

        //    var today = DateTime.Today;

        //    var submittedToday = submittedAll
        //        .Where(x => x.CreatedOn.Date == today)
        //        .OrderBy(x => x.CreatedOn)
        //        .ToList();

        //    string shiftValue = submittedToday.FirstOrDefault()?.Shift ?? "Morning";

        //    var shiftList = new List<string>
        //    {
        //        "Morning",
        //        "Afternoon",
        //        "Night",
        //        "Long Morning",
        //        "Long Night"
        //    };

        //    ViewBag.Shift = new SelectList(shiftList);


        //    // 🔹 Disable already submitted heats
        //    var submittedHeatSet = new HashSet<string>(
        //        submittedAll
        //            .Where(s => !string.IsNullOrWhiteSpace(s.HeatNo))
        //            .Select(s => s.HeatNo.Trim()),
        //        StringComparer.OrdinalIgnoreCase
        //    );

        //    ViewBag.HeatNo = heats.Select(h => new SelectListItem
        //    {
        //        Text = h.HeatNo,
        //        Value = h.HeatNo,
        //        Disabled = submittedHeatSet.Contains(h.HeatNo.Trim())
        //    }).ToList();

        //    var vm = new RMDischargingVM
        //    {
        //        Shift = shiftValue,        // 🔥 THIS IS IMPORTANT
        //        SubmittedHeat = submittedToday
        //    };

        //    var BilletGradeList = repo.GetBilletGrade();
        //    ViewBag.BilletGrade = new SelectList(BilletGradeList, "ProductID", "SpecGrade");
        //    ViewBag.GradeDataJson = JsonConvert.SerializeObject(BilletGradeList);

        //    return View("~/Views/RollingMill/Discharging/add.cshtml", vm);
        //}

        [Route("HeatDischarge")]
        public ActionResult AddDischarge()
        {
            var heats = repo.GetAllCharging();
            var submittedAll = rm.GetDichargedHeat();

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

        [Route("HeatDischarge")]
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

        public ActionResult GenerateDelayReportPDF(DateTime? startdate, DateTime? enddate, string shift)
        {
            // Force Gregorian
            var greg = new CultureInfo("en-US");
            greg.DateTimeFormat.Calendar = new GregorianCalendar();

            DateTime? sDate = null;
            DateTime? eDate = null;

            if (startdate.HasValue)
                sDate = startdate.Value.Date;

            if (enddate.HasValue)
                eDate = enddate.Value.Date.AddDays(1).AddTicks(-1); // inclusive

            var data = delay.GetAllRMDelay().AsQueryable();

            // ✅ Rolling Mill (case + space safe)
            data = data.Where(x => x.Area != null &&
                                   x.Area.Trim().ToLower() == "rolling mill");

            // ✅ Date filters
            if (sDate.HasValue)
                data = data.Where(x => x.Date >= sDate.Value);

            if (eDate.HasValue)
                data = data.Where(x => x.Date <= eDate.Value);

            // ✅ NEW: Shift filter
            if (!string.IsNullOrEmpty(shift))
            {
                data = data.Where(x => x.Shift != null &&
                                       x.Shift.Trim().ToLower() == shift.Trim().ToLower());
            }

            var result = data.ToList();

            // 🔎 DEBUG CHECK (remove later)
            if (!result.Any())
            {
                ViewBag.Debug = "No data after filter";
            }

            return View("GenerateDelayReportPDF", result);
        }

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
            var delays = delay.GetAllRMDelay();            // IQueryable or List
            var heats = rm.GetDichargedHeat();          // IQueryable or List

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
            return View();
        }

        [HttpPost]
        public ActionResult AddDetails(RMShiftDetailsBLL data)
        {
            data.StatusID = 1;
            data.CreatedDate = DateTime.Now;
            data.CreatedBy = User.Identity.Name;

            int rtn = rm.AddRMShiftDetails(data);

            if (rtn == -1)
            {
                TempData["SuccessMessage"] = "Data saved successfully";
                return RedirectToAction("Index", "Home");
            }

            TempData["ErrorMessage"] = "Data not saved";
            return RedirectToAction("AddDetails");
        }
    }
}