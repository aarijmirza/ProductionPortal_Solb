using BAL.Repositories;
using DAL.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using Rotativa;
using Rotativa.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using System.Web.UI.WebControls;
using System.Windows.Media.Imaging;
using static DAL.Models.ViewModel;
using Image = iTextSharp.text.Image;


namespace ProductionPortal_Solb.Controllers
{
    [SessionState(
    SessionStateBehavior.ReadOnly
    )]
    public class MeltshopController : Controller
    {
        MeltshopRepository repo;
        DelayRespository delay;
        public MeltshopController()
        {
            repo = new MeltshopRepository();
            delay = new DelayRespository();
        }
        // Electric Arc Furnace
        public ActionResult EAFlist()
        {
            var requests = repo.GetAllEAFRecord();
            return View("~/Views/Meltshop/ElectricArcFurnace/EAFlist.cshtml", requests);
        }

        public ActionResult DailyHeatSummary(DateTime? from, DateTime? to)
        {
            DateTime fromDate = from ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime toDate = to ?? DateTime.Now;

            // ✅ include full day
            DateTime toInclusive = toDate.Date.AddDays(1);

            var list = repo.GetEAFListByDate(fromDate.Date, toInclusive);

            var vm = new EAFReportVM
            {
                EAFdata = list,
                FromDate = fromDate.Date,
                ToDate = toDate.Date,
                Shift = "ALL",
                Group = "ALL"
            };

            return View(
                "~/Views/Meltshop/ElectricArcFurnace/DailyHeatSummary.cshtml",
                vm
            );
        }

        public ActionResult EAFadd()
        {
            return View("~/Views/Meltshop/ElectricArcFurnace/EAFadd.cshtml");
        }
        [HttpPost]
        public ActionResult EAFadd(ElectricArcFurnaceBLL model)
        {
            if (model != null)
            {
                model.StatusID = 1;
                model.CreatedDate = DateTime.Now;
                model.CreatedBy = User.Identity.Name;
                int rtn = repo.InsertEAF(model);
                if (rtn > 0)
                {
                    TempData["SuccessMessage"] = "Data saved successfully";
                }
                else
                {
                    TempData["ErrorMessage"] = "Data not saved. Please try again.";
                    return RedirectToAction("EAFlist"); // 👈 back to form
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Invalid data submitted.";
                return RedirectToAction("EAFlist");
            }

            return RedirectToAction("EAFlist");
        }
        public ActionResult EAFdetails(string heatNo)
        {
            if (string.IsNullOrEmpty(heatNo))
            {
                // Handle case where no Heat Number is provided
                return RedirectToAction("list");
            }

            // Call the corrected repository method to fetch all bucket records for the heat
            var data = repo.GetEAFRecordByID(heatNo);

            if (data == null)
            {
                TempData["Error"] = $"No records found for Heat # {heatNo}.";
                return RedirectToAction("list");
            }

            // Pass the list of ScrapyardBLL objects to the view
            return View("~/Views/Meltshop/ElectricArcFurnace/EAFdetails.cshtml", data);
        }
        // Electric Arc Furnace

        // Laddle Furnace
        public ActionResult LFlist()
        {
            var LFlist = repo.GetAllLFRecord();
            return View("~/Views/Meltshop/LaddleFurnace/LFlist.cshtml", LFlist);
        }
        public ActionResult LFadd()
        {
            var last24Hours = DateTime.Now.AddHours(-24);
            var data = repo.GetAllEAFRecord();

            //var data = repo.GetAllEAFRecord().ToList();
            ViewBag.HeatNo = new SelectList(data, "HeatNo", "HeatNo");

            return View("~/Views/Meltshop/LaddleFurnace/LFadd.cshtml");
        }
        [HttpPost]
        public ActionResult LFadd(LaddleFurnaceBLL model)
        {
            if (model != null)
            {
                model.StatusID = 1;
                model.CreatedDate = DateTime.Now;
                model.CreatedBy = User.Identity.Name;
                model.Date = DateTime.Now;
                int rtn = repo.InsertLF(model);
                if (rtn > 0)
                {
                    TempData["SuccessMessage"] = "Data saved successfully";
                }
                else
                {
                    TempData["ErrorMessage"] = "Data not saved. Please try again.";
                    return RedirectToAction("LFlist"); // 👈 back to form
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Invalid data submitted.";
                return RedirectToAction("LFlist");
            }

            return RedirectToAction("LFlist");
        }

        public ActionResult LFdetails(string heatNo)
        {
            if (string.IsNullOrEmpty(heatNo))
            {
                // Handle case where no Heat Number is provided
                return RedirectToAction("LFlist");
            }

            // Call the corrected repository method to fetch all bucket records for the heat
            var data = repo.GetLFRecordByID(heatNo);

            if (data == null)
            {
                TempData["Error"] = $"No records found for Heat # {heatNo}.";
                return RedirectToAction("LFlist");
            }

            // Pass the list of ScrapyardBLL objects to the view
            return View("~/Views/Meltshop/LaddleFurnace/LFdetails.cshtml", data);
        }
        private List<ElectricArcFurnaceBLL> GetFilteredData(string from, string to)
        {
            DateTime? fromDate = null;
            DateTime? toDate = null;
            // The view uses 'yyyy-MM-dd' format for date inputs
            const string format = "yyyy-MM-dd";
            var culture = CultureInfo.InvariantCulture;

            // 1. Parse From Date
            if (DateTime.TryParseExact(from, format, culture, DateTimeStyles.None, out DateTime tempFrom))
            {
                fromDate = tempFrom.Date;
            }

            // 2. Parse To Date
            if (DateTime.TryParseExact(to, format, culture, DateTimeStyles.None, out DateTime tempTo))
            {
                // Set the ToDate to the end of the day (23:59:59.999) to include all records created on that date
                toDate = tempTo.Date.AddDays(1).AddTicks(-1);
            }

            var allRecords = repo.GetAllEAFRecord() ?? new List<ElectricArcFurnaceBLL>();

            // 3. Apply Filtering using LINQ
            var filteredRecords = allRecords.Where(r =>
                r.CreatedDate.HasValue && // Ensure date is not null
                (!fromDate.HasValue || r.CreatedDate.Value.Date >= fromDate.Value.Date) && // Filter by start date
                (!toDate.HasValue || r.CreatedDate.Value <= toDate.Value) // Filter by end date (inclusive of the whole day)
            )
            .OrderByDescending(r => r.CreatedDate) // Sort by date descending
            .ToList();
            return filteredRecords;
        }

        public ActionResult EAFdelete(string heatNo)
        {
            var UpdatedBy = User.Identity.Name;
            int rtn = repo.DeleteEAFHeat(heatNo, UpdatedBy);
            TempData["SuccessMessage"] = "Data Delete Successfully";

            return RedirectToAction("EAFlist");
        }
        public ActionResult LFdelete(string heatNo)
        {
            var UpdatedBy = User.Identity.Name;
            int rtn = repo.DeleteLFHeat(heatNo, UpdatedBy);
            TempData["SuccessMessage"] = "Data Delete Successfully";

            return RedirectToAction("LFlist");
        }

        public ActionResult LFDailyHeatSummary(DateTime? from, DateTime? to)
        {
            DateTime fromDate = from ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime toDate = to ?? DateTime.Now;

            // ✅ include full day
            DateTime toInclusive = toDate.Date.AddDays(1);

            var list = repo.GetLFListByDate(fromDate.Date, toInclusive);

            var vm = new LFReportVM
            {
                LFdata = list,
                FromDate = fromDate.Date,
                ToDate = toDate.Date,
                Shift = "ALL",
                Group = "ALL"
            };

            return View(
                "~/Views/Meltshop/LaddleFurnace/LFDailyHeatSummary.cshtml",
                vm
            );
        }

        public ActionResult SMPDelayReport(DateTime? startdate, DateTime? enddate)
        {
            DateTime? sDate = startdate?.Date;
            DateTime? eDate = enddate?.Date.AddDays(1).AddTicks(-1); // inclusive

            var data = delay.GetAllDelay().AsQueryable();

            if (sDate.HasValue)
                data = data.Where(x => x.Date >= sDate.Value);

            if (eDate.HasValue)
                data = data.Where(x => x.Date <= eDate.Value);

            var result = data.ToList();

            // ✅ PASS FILTER DATES TO VIEW
            ViewBag.FromDate = startdate;
            ViewBag.ToDate = enddate;

            return View("SMPDelayReport", result);
        }

    }
}