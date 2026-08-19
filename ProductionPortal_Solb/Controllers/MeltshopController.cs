using BAL.Repositories;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Web.SessionState;
using static DAL.Models.ViewModel;

namespace ProductionPortal_Solb.Controllers
{
    [SessionState(SessionStateBehavior.ReadOnly)]
    public class MeltshopController : Controller
    {
        private readonly MeltshopRepository repo;
        private readonly DelayRespository delay;

        public MeltshopController()
        {
            repo = new MeltshopRepository();
            delay = new DelayRespository();
        }

        // =========================================================
        // ELECTRIC ARC FURNACE
        // =========================================================

        public ActionResult EAFlist()
        {
            var requests =
                repo.GetAllEAFRecord() ??
                new List<ElectricArcFurnaceBLL>();

            return View(
                "~/Views/Meltshop/ElectricArcFurnace/EAFlist.cshtml",
                requests
            );
        }

        public ActionResult DailyHeatSummary(
            DateTime? from,
            DateTime? to)
        {
            DateTime fromDate =
                from ??
                new DateTime(
                    DateTime.Now.Year,
                    DateTime.Now.Month,
                    1
                );

            DateTime toDate =
                to ?? DateTime.Now;

            if (fromDate.Date > toDate.Date)
            {
                DateTime temp =
                    fromDate;

                fromDate =
                    toDate;

                toDate =
                    temp;
            }

            DateTime toExclusive =
                toDate.Date.AddDays(1);

            var list =
                repo.GetEAFListByDate(
                    fromDate.Date,
                    toExclusive
                ) ??
                new List<ElectricArcFurnaceBLL>();

            var vm =
                new EAFReportVM
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
            return View(
                "~/Views/Meltshop/ElectricArcFurnace/EAFadd.cshtml"
            );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EAFadd(
            ElectricArcFurnaceBLL model)
        {
            try
            {
                if (model == null)
                {
                    TempData["ErrorMessage"] =
                        "Invalid data submitted.";

                    return RedirectToAction(
                        "EAFlist"
                    );
                }

                model.StatusID =
                    1;

                model.CreatedDate =
                    DateTime.Now;

                model.CreatedBy =
                    User.Identity.Name;

                int result =
                    repo.InsertEAF(
                        model
                    );

                if (result > 0)
                {
                    TempData["SuccessMessage"] =
                        "Data saved successfully.";

                    return RedirectToAction(
                        "EAFlist"
                    );
                }

                TempData["ErrorMessage"] =
                    "Data not saved. Please try again.";

                return RedirectToAction(
                    "EAFlist"
                );
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] =
                    "Error: " +
                    ex.Message;

                return RedirectToAction(
                    "EAFlist"
                );
            }
        }

        public ActionResult EAFdetails(
            string heatNo)
        {
            if (string.IsNullOrWhiteSpace(
                heatNo
            ))
            {
                TempData["ErrorMessage"] =
                    "Heat number is required.";

                return RedirectToAction(
                    "EAFlist"
                );
            }

            var data =
                repo.GetEAFRecordByID(
                    heatNo.Trim()
                );

            if (data == null)
            {
                TempData["ErrorMessage"] =
                    "No records found for Heat # " +
                    heatNo.Trim() +
                    ".";

                return RedirectToAction(
                    "EAFlist"
                );
            }

            return View(
                "~/Views/Meltshop/ElectricArcFurnace/EAFdetails.cshtml",
                data
            );
        }

        public ActionResult EAFdelete(
            string heatNo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(
                    heatNo
                ))
                {
                    TempData["ErrorMessage"] =
                        "Heat number is required.";

                    return RedirectToAction(
                        "EAFlist"
                    );
                }

                string updatedBy =
                    User.Identity.Name;

                int result =
                    repo.DeleteEAFHeat(
                        heatNo.Trim(),
                        updatedBy
                    );

                if (result > 0)
                {
                    TempData["SuccessMessage"] =
                        "Data deleted successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] =
                        "Data could not be deleted.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] =
                    "Error: " +
                    ex.Message;
            }

            return RedirectToAction(
                "EAFlist"
            );
        }

        // =========================================================
        // LADDLE FURNACE
        // =========================================================

        public ActionResult LFlist(
            DateTime? fromDate,
            DateTime? toDate)
        {
            DateTime startDate =
                fromDate ??
                new DateTime(
                    DateTime.Today.Year,
                    DateTime.Today.Month,
                    1
                );

            DateTime endDate =
                toDate ??
                DateTime.Today;

            if (startDate.Date > endDate.Date)
            {
                DateTime temp =
                    startDate;

                startDate =
                    endDate;

                endDate =
                    temp;
            }

            List<LaddleFurnaceBLL> list =
                repo.GetAllLFRecord(
                    startDate.Date,
                    endDate.Date
                ) ??
                new List<LaddleFurnaceBLL>();

            ViewBag.FromDate =
                startDate.ToString("yyyy-MM-dd");

            ViewBag.ToDate =
                endDate.ToString("yyyy-MM-dd");

            return View(
                "~/Views/Meltshop/LaddleFurnace/LFlist.cshtml",
                list
            );
        }

        public ActionResult LFadd()
        {
            /*
                Existing repository currently returns all EAF records.
                For better performance, replace this with a filtered
                repository method that only returns recent/open Heat Nos.
            */
            var data =
                repo.GetAllEAFRecord() ??
                new List<ElectricArcFurnaceBLL>();

            ViewBag.HeatNo =
                new SelectList(
                    data,
                    "HeatNo",
                    "HeatNo"
                );

            return View(
                "~/Views/Meltshop/LaddleFurnace/LFadd.cshtml"
            );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LFadd(
            LaddleFurnaceBLL model)
        {
            try
            {
                if (model == null)
                {
                    TempData["ErrorMessage"] =
                        "Invalid data submitted.";

                    return RedirectToAction(
                        "LFadd"
                    );
                }

                model.StatusID =
                    1;

                model.CreatedDate =
                    DateTime.Now;

                model.CreatedBy =
                    User.Identity.Name;

                model.Date =
                    DateTime.Now;

                int result =
                    repo.InsertLF(
                        model
                    );

                if (result < 0)
                {
                    TempData["SuccessMessage"] =
                        "Data saved successfully.";

                    return RedirectToAction(
                        "LFlist"
                    );
                }

                TempData["ErrorMessage"] =
                    "Data not saved. Please try again.";

                return RedirectToAction(
                    "LFadd"
                );
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] =
                    "Error: " +
                    ex.Message;

                return RedirectToAction(
                    "LFadd"
                );
            }
        }

        public ActionResult LFdetails(
            string heatNo)
        {
            if (string.IsNullOrWhiteSpace(
                heatNo
            ))
            {
                TempData["ErrorMessage"] =
                    "Heat number is required.";

                return RedirectToAction(
                    "LFlist"
                );
            }

            var data =
                repo.GetLFRecordByID(
                    heatNo.Trim()
                );

            if (data == null)
            {
                TempData["ErrorMessage"] =
                    "No records found for Heat # " +
                    heatNo.Trim() +
                    ".";

                return RedirectToAction(
                    "LFlist"
                );
            }

            return View(
                "~/Views/Meltshop/LaddleFurnace/LFdetails.cshtml",
                data
            );
        }

        public ActionResult LFdelete(
            string heatNo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(
                    heatNo
                ))
                {
                    TempData["ErrorMessage"] =
                        "Heat number is required.";

                    return RedirectToAction(
                        "LFlist"
                    );
                }

                string updatedBy =
                    User.Identity.Name;

                int result =
                    repo.DeleteLFHeat(
                        heatNo.Trim(),
                        updatedBy
                    );

                if (result > 0)
                {
                    TempData["SuccessMessage"] =
                        "Data deleted successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] =
                        "Data could not be deleted.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] =
                    "Error: " +
                    ex.Message;
            }

            return RedirectToAction(
                "LFlist"
            );
        }

        public ActionResult LFDailyHeatSummary(
            DateTime? from,
            DateTime? to)
        {
            DateTime fromDate =
                from ??
                new DateTime(
                    DateTime.Now.Year,
                    DateTime.Now.Month,
                    1
                );

            DateTime toDate =
                to ?? DateTime.Now;

            if (fromDate.Date > toDate.Date)
            {
                DateTime temp =
                    fromDate;

                fromDate =
                    toDate;

                toDate =
                    temp;
            }

            DateTime toExclusive =
                toDate.Date.AddDays(1);

            var list =
                repo.GetLFListByDate(
                    fromDate.Date,
                    toExclusive
                ) ??
                new List<LaddleFurnaceBLL>();

            var vm =
                new LFReportVM
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

        // =========================================================
        // SMP DELAY REPORT
        // =========================================================

        //public ActionResult SMPDelayReport(
        //    DateTime? startdate,
        //    DateTime? enddate)
        //{
        //    try
        //    {
        //        DateTime fromDate =
        //            startdate ??
        //            DateTime.Today;

        //        DateTime toDate =
        //            enddate ??
        //            DateTime.Today;

        //        if (fromDate.Date > toDate.Date)
        //        {
        //            DateTime temp =
        //                fromDate;

        //            fromDate =
        //                toDate;

        //            toDate =
        //                temp;
        //        }

        //        DateTime toExclusive =
        //            toDate.Date.AddDays(1);

        //        /*
        //            IMPORTANT:
        //            Filtering must happen inside SQL/repository.
        //            Do not call GetAllDelay() and then apply LINQ,
        //            because it loads the entire delay table first.
        //        */
        //        var result =
        //            delay.GetSMPDelayByDate(
        //                fromDate.Date,
        //                toExclusive
        //            ) ??
        //            new List<PlantDelayBLL>();

        //        ViewBag.FromDate =
        //            fromDate.ToString(
        //                "yyyy-MM-dd"
        //            );

        //        ViewBag.ToDate =
        //            toDate.ToString(
        //                "yyyy-MM-dd"
        //            );

        //        return View(
        //            "SMPDelayReport",
        //            result
        //        );
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["ErrorMessage"] =
        //            "Unable to load SMP delay report. Error: " +
        //            ex.Message;

        //        ViewBag.FromDate =
        //            (
        //                startdate ??
        //                DateTime.Today
        //            ).ToString(
        //                "yyyy-MM-dd"
        //            );

        //        ViewBag.ToDate =
        //            (
        //                enddate ??
        //                DateTime.Today
        //            ).ToString(
        //                "yyyy-MM-dd"
        //            );

        //        return View(
        //            "SMPDelayReport",
        //            new List<PlantDelayBLL>()
        //        );
        //    }
        //}

        // =========================================================
        // PRIVATE HELPERS
        // =========================================================

        private List<ElectricArcFurnaceBLL>
            GetFilteredData(
                string from,
                string to)
        {
            DateTime? fromDate =
                null;

            DateTime? toDateExclusive =
                null;

            const string format =
                "yyyy-MM-dd";

            CultureInfo culture =
                CultureInfo.InvariantCulture;

            DateTime parsedFrom;

            if (DateTime.TryParseExact(
                from,
                format,
                culture,
                DateTimeStyles.None,
                out parsedFrom
            ))
            {
                fromDate =
                    parsedFrom.Date;
            }

            DateTime parsedTo;

            if (DateTime.TryParseExact(
                to,
                format,
                culture,
                DateTimeStyles.None,
                out parsedTo
            ))
            {
                toDateExclusive =
                    parsedTo.Date.AddDays(1);
            }

            /*
                This helper is retained for compatibility.
                For best performance, replace it with a repository method
                that accepts FromDate and ToDate and filters in SQL.
            */
            var allRecords =
                repo.GetAllEAFRecord() ??
                new List<ElectricArcFurnaceBLL>();

            return allRecords
                .Where(x =>
                    x.CreatedDate.HasValue &&
                    (
                        !fromDate.HasValue ||
                        x.CreatedDate.Value >=
                        fromDate.Value
                    ) &&
                    (
                        !toDateExclusive.HasValue ||
                        x.CreatedDate.Value <
                        toDateExclusive.Value
                    )
                )
                .OrderByDescending(
                    x => x.CreatedDate
                )
                .ToList();
        }
    }
}