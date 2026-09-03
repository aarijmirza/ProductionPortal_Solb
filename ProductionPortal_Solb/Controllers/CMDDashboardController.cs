using BAL.Repositories;
using DAL.Models;
using DAL.Repository;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProductionPortal_Solb.Controllers
{
    public class CMDDashboardController : Controller
    {
        private const decimal SMPYearlyProductionTarget = 700000M;

        DelayRespository repo;
        MaintenanceRepository mrepo;
        RollingMillTargetsRepository targetRepo;
        RollingMillDailyTargetRepository dailyTargetRepo;
        SMPReportsRepository smprepo;

        public CMDDashboardController()
        {
            repo = new DelayRespository();
            mrepo = new MaintenanceRepository();
            targetRepo = new RollingMillTargetsRepository();
            dailyTargetRepo = new RollingMillDailyTargetRepository();
            smprepo = new SMPReportsRepository();
        }

        // GET: CMDDashboard
        public ActionResult dashboard(
            DateTime? fromDate,
            DateTime? toDate)
        {
            DateTime selectedFromDate =
                (fromDate ?? DateTime.Today).Date;

            DateTime selectedToDate =
                (toDate ?? selectedFromDate).Date;

            if (selectedFromDate > selectedToDate)
            {
                DateTime temp = selectedFromDate;
                selectedFromDate = selectedToDate;
                selectedToDate = temp;
            }

            try
            {
                CMDPerformanceDashboardVM model =
                    mrepo.GetDashboard(
                        selectedFromDate,
                        selectedToDate
                    );

                if (model == null)
                {
                    model = new CMDPerformanceDashboardVM();
                }

                model.FromDate = selectedFromDate;
                model.ToDate = selectedToDate;

                model.DailyProduction =
                    model.DailyProduction ??
                    new ProductionSummaryVM();

                model.MTDProduction =
                    model.MTDProduction ??
                    new ProductionSummaryVM();

                model.YTDProduction =
                    model.YTDProduction ??
                    new ProductionSummaryVM();

                model.Downtime =
                    model.Downtime ??
                    new List<DowntimeSummaryVM>();

                model.EquipmentFailures =
                    model.EquipmentFailures ??
                    new List<TopFailureVM>();

                model.RCAFailures =
                    model.RCAFailures ??
                    new List<TopFailureVM>();

                // =====================================================
                // RCA NORMALIZATION
                // =====================================================
                // 1. All approved Root Cause categories are returned
                //    for SMP, RM1 and RM2.
                // 2. Duplicate Root Causes are merged.
                // 3. Duplicate DelayHours are summed.
                // 4. Missing Root Causes are returned with 0 hours.
                //
                // This keeps the View simple and avoids Dictionary<,>
                // usage in Razor on the older .NET Framework compiler.
                model.RCAFailures =
                    NormalizeRCAFailures(
                        model.RCAFailures
                    );

                model.ClosureRates =
                    model.ClosureRates ??
                    new List<ClosureRateVM>();

                model.TopDelays =
                    model.TopDelays ??
                    new List<CMDTopDelayVM>();

                // =====================================================
                // PRODUCTION TARGET PERIODS
                // =====================================================
                DateTime monthStartDate =
                    new DateTime(
                        selectedToDate.Year,
                        selectedToDate.Month,
                        1
                    );

                DateTime yearStartDate =
                    new DateTime(
                        selectedToDate.Year,
                        1,
                        1
                    );

                // =====================================================
                // SMP TARGET
                // Fixed yearly target = 700,000 MT.
                // Daily target = 700,000 / 365 (or 366 in leap year).
                // MTD/YTD targets are accumulated using that daily rate.
                // =====================================================
                ViewBag.SMPYearlyProductionTarget =
                    SMPYearlyProductionTarget;

                ViewBag.SMPDailyTarget =
                    GetSMPDailyTarget(selectedToDate.Year);

                // Daily / Selected Period
                ViewBag.SMPProductionPlan =
                    GetSMPProductionPlan(
                        selectedFromDate,
                        selectedToDate
                    );

                // Month To Date
                ViewBag.SMPMTDProductionPlan =
                    GetSMPProductionPlan(
                        monthStartDate,
                        selectedToDate
                    );

                // Year To Date
                ViewBag.SMPYTDProductionPlan =
                    GetSMPProductionPlan(
                        yearStartDate,
                        selectedToDate
                    );

                // =====================================================
                // RM1 / RM2 TARGETS
                // Source: RollingMillDailyTargetRepository.
                // Daily / selected period = sum of saved daily targets.
                // MTD = month start through selected date.
                // YTD = Jan-01 through selected date.
                // =====================================================

                // Daily / Selected Period
                ViewBag.RM1ProductionPlan =
                    GetRollingMillProductionPlan(
                        selectedFromDate,
                        selectedToDate,
                        "RM1"
                    );

                ViewBag.RM2ProductionPlan =
                    GetRollingMillProductionPlan(
                        selectedFromDate,
                        selectedToDate,
                        "RM2"
                    );

                // Month To Date
                ViewBag.RM1MTDProductionPlan =
                    GetRollingMillProductionPlan(
                        monthStartDate,
                        selectedToDate,
                        "RM1"
                    );

                ViewBag.RM2MTDProductionPlan =
                    GetRollingMillProductionPlan(
                        monthStartDate,
                        selectedToDate,
                        "RM2"
                    );

                // Year To Date
                ViewBag.RM1YTDProductionPlan =
                    GetRollingMillProductionPlan(
                        yearStartDate,
                        selectedToDate,
                        "RM1"
                    );

                ViewBag.RM2YTDProductionPlan =
                    GetRollingMillProductionPlan(
                        yearStartDate,
                        selectedToDate,
                        "RM2"
                    );

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    "CMD dashboard could not be loaded. " +
                    ex.Message;

                SetEmptyProductionPlans();

                CMDPerformanceDashboardVM model =
                    new CMDPerformanceDashboardVM
                    {
                        FromDate = selectedFromDate,
                        ToDate = selectedToDate,

                        DailyProduction =
                            new ProductionSummaryVM(),

                        MTDProduction =
                            new ProductionSummaryVM(),

                        YTDProduction =
                            new ProductionSummaryVM(),

                        Downtime =
                            new List<DowntimeSummaryVM>(),

                        EquipmentFailures =
                            new List<TopFailureVM>(),

                        RCAFailures =
                            new List<TopFailureVM>(),

                        ClosureRates =
                            new List<ClosureRateVM>(),

                        TopDelays =
                            new List<CMDTopDelayVM>()
                    };

                return View(model);
            }
        }

        // =============================================================
        // SMP DAILY TARGET FROM FIXED 700,000 MT ANNUAL TARGET
        // =============================================================
        private decimal GetSMPDailyTarget(int year)
        {
            int daysInYear =
                DateTime.IsLeapYear(year)
                    ? 366
                    : 365;

            return SMPYearlyProductionTarget /
                   Convert.ToDecimal(daysInYear);
        }

        // =============================================================
        // SMP TARGET FOR ANY DATE RANGE
        // Supports ranges crossing year boundaries correctly.
        // =============================================================
        private decimal GetSMPProductionPlan(
            DateTime fromDate,
            DateTime toDate)
        {
            fromDate = fromDate.Date;
            toDate = toDate.Date;

            if (fromDate > toDate)
            {
                DateTime temp = fromDate;
                fromDate = toDate;
                toDate = temp;
            }

            decimal totalPlan = 0M;
            DateTime currentDate = fromDate;

            while (currentDate <= toDate)
            {
                int currentYear = currentDate.Year;

                DateTime yearEnd =
                    new DateTime(
                        currentYear,
                        12,
                        31
                    );

                DateTime segmentEnd =
                    yearEnd < toDate
                        ? yearEnd
                        : toDate;

                int numberOfDays =
                    (segmentEnd - currentDate).Days + 1;

                decimal dailyTarget =
                    GetSMPDailyTarget(currentYear);

                totalPlan +=
                    dailyTarget * numberOfDays;

                currentDate = segmentEnd.AddDays(1);
            }

            return Math.Round(totalPlan, 2);
        }

        // =============================================================
        // SUM RM DAILY PRODUCTION TARGET FOR ANY DATE RANGE
        // =============================================================
        private decimal GetRollingMillProductionPlan(
            DateTime fromDate,
            DateTime toDate,
            string plant)
        {
            decimal totalPlan = 0M;

            for (
                DateTime targetDate = fromDate.Date;
                targetDate <= toDate.Date;
                targetDate = targetDate.AddDays(1)
            )
            {
                var dailyTarget =
                    dailyTargetRepo.GetByDate(
                        targetDate,
                        plant
                    );

                if (dailyTarget != null)
                {
                    totalPlan += Convert.ToDecimal(
                        dailyTarget.DailyProductionTarget
                    );
                }
            }

            return totalPlan;
        }

        // =============================================================
        // NORMALIZE FAILURE RCA DATA
        // =============================================================
        // Returns exactly one row per:
        //     Plant + Root Cause
        //
        // Duplicate rows are SUMMED and every approved Root Cause is
        // included even when no delay exists for that plant.
        // =============================================================
        private List<TopFailureVM> NormalizeRCAFailures(
            List<TopFailureVM> source)
        {
            string[] plants =
            {
                "SMP",
                "RM1",
                "RM2"
            };

            string[] rootCauses =
            {
                "Shortage of Spares",
                "Can Be avoided",
                "Machine Poor Condition",
                "Undetectable failure",
                "Poor Operation Condition",
                "Human Mistake",
                "Design issues"
            };

            List<TopFailureVM> normalized =
                new List<TopFailureVM>();

            source =
                source ??
                new List<TopFailureVM>();

            foreach (string plant in plants)
            {
                foreach (string rootCause in rootCauses)
                {
                    decimal totalDelayHours =
                        source
                            .Where(x =>
                                x != null &&
                                string.Equals(
                                    string.IsNullOrWhiteSpace(x.Plant)
                                        ? ""
                                        : x.Plant.Trim(),
                                    plant,
                                    StringComparison.OrdinalIgnoreCase
                                ) &&
                                string.Equals(
                                    string.IsNullOrWhiteSpace(x.Name)
                                        ? ""
                                        : x.Name.Trim(),
                                    rootCause,
                                    StringComparison.OrdinalIgnoreCase
                                )
                            )
                            .Sum(x => x.DelayHours);

                    /*
                     * Do not manufacture zero-value RCA rows.
                     * If a plant has no RCA data for this cause,
                     * nothing is added to the model.
                     */
                    if (totalDelayHours > 0m)
                    {
                        normalized.Add(
                            new TopFailureVM
                            {
                                Plant = plant,
                                Name = rootCause,
                                DelayHours = totalDelayHours
                            }
                        );
                    }
                }
            }

            return normalized;
        }


        private void SetEmptyProductionPlans()
        {
            ViewBag.SMPYearlyProductionTarget =
                SMPYearlyProductionTarget;

            ViewBag.SMPDailyTarget = 0M;
            ViewBag.SMPProductionPlan = 0M;
            ViewBag.SMPMTDProductionPlan = 0M;
            ViewBag.SMPYTDProductionPlan = 0M;

            ViewBag.RM1ProductionPlan = 0M;
            ViewBag.RM2ProductionPlan = 0M;
            ViewBag.RM1MTDProductionPlan = 0M;
            ViewBag.RM2MTDProductionPlan = 0M;
            ViewBag.RM1YTDProductionPlan = 0M;
            ViewBag.RM2YTDProductionPlan = 0M;
        }
    }
}
