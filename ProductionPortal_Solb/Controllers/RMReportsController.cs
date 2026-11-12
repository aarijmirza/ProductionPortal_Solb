using BAL.Repositories;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ProductionPortal_Solb.Controllers
{
    public class RMReportsController : Controller
    {
        private readonly RMReportsRepository repo =
            new RMReportsRepository();
        private readonly RollingMillRepository rmrepo =
            new RollingMillRepository();

        public ActionResult list()
        {
            return View();
        }

        public ActionResult FinishedProductShiftSummaryReport(
            DateTime? fromDate,
            DateTime? toDate,
            string plant,
            string shift,
            bool download = false)
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

            string selectedPlant =
                string.IsNullOrWhiteSpace(plant)
                    ? ""
                    : plant.Trim();

            string selectedShift =
                string.IsNullOrWhiteSpace(shift)
                    ? ""
                    : shift.Trim();

            List<FinishedProductShiftSummaryRowBLL> rows =
                rmrepo.GetFinishedProductShiftSummary(
                    selectedFromDate,
                    selectedToDate,
                    selectedPlant,
                    selectedShift
                )
                ?? new List<FinishedProductShiftSummaryRowBLL>();

            RMShiftDetailsBLL shiftDetail =
                rmrepo.GetRollingMillDetailsByRange(
                    selectedFromDate,
                    selectedToDate,
                    selectedPlant,
                    selectedShift
                )
                .OrderByDescending(x => x.Date)
                .ThenByDescending(x => x.ID)
                .FirstOrDefault();

            string group =
                shiftDetail == null
                    ? ""
                    : Convert.ToString(
                        shiftDetail.Team ?? ""
                    ).Trim();

            ViewBag.Rows = rows;

            ViewBag.ReportDate =
                selectedFromDate == selectedToDate
                    ? selectedFromDate.ToString("dd-MMM-yyyy")
                    : selectedFromDate.ToString("dd-MMM-yyyy")
                      + " to "
                      + selectedToDate.ToString("dd-MMM-yyyy");

            ViewBag.Shift =
                string.IsNullOrWhiteSpace(selectedShift)
                    ? "All"
                    : selectedShift;

            ViewBag.Group = group;

            ViewBag.Plant =
                string.IsNullOrWhiteSpace(selectedPlant)
                    ? "All"
                    : selectedPlant;

            ViewBag.DocumentNo =
                selectedPlant.Equals(
                    "Rolling Mill 1",
                    StringComparison.OrdinalIgnoreCase
                )
                ||
                selectedPlant.Equals(
                    "RM1",
                    StringComparison.OrdinalIgnoreCase
                )
                    ? "RM1F-FPS-1016"
                    : "RM2F-FPS-1016";

            ViewBag.RevisionNo = "02";
            ViewBag.AutoPrint = download;

            return View(
                "~/Views/RMReports/FinishedProductShiftSummaryReport.cshtml"
            );
        }

        public ActionResult FurnaceChargingDischargingSummary(
            DateTime? fromDate,
            DateTime? toDate,
            string plant,
            string shift,
            bool download = false)
        {
            DateTime selectedFromDate =
                (fromDate ?? DateTime.Today).Date;

            DateTime selectedToDate =
                (toDate ?? selectedFromDate).Date;

            if (selectedFromDate > selectedToDate)
            {
                DateTime temp =
                    selectedFromDate;

                selectedFromDate =
                    selectedToDate;

                selectedToDate =
                    temp;
            }

            string selectedPlant =
                string.IsNullOrWhiteSpace(plant)
                    ? ""
                    : plant.Trim();

            string selectedShift =
                string.IsNullOrWhiteSpace(shift)
                    ? ""
                    : shift.Trim();

            List<FurnaceChargingDischargingSummaryRowBLL> rows =
                rmrepo.GetFurnaceChargingDischargingSummary(
                    selectedFromDate,
                    selectedToDate,
                    selectedPlant,
                    selectedShift
                );

            ViewBag.Rows =
                rows
                ??
                new List<
                    FurnaceChargingDischargingSummaryRowBLL
                >();

            ViewBag.ReportDate =
                selectedFromDate == selectedToDate
                    ? selectedFromDate.ToString(
                        "dd-MMM-yyyy"
                    )
                    : selectedFromDate.ToString(
                        "dd-MMM-yyyy"
                    )
                    + " to "
                    + selectedToDate.ToString(
                        "dd-MMM-yyyy"
                    );

            ViewBag.Shift =
                string.IsNullOrWhiteSpace(
                    selectedShift
                )
                    ? "All"
                    : selectedShift;

            ViewBag.Plant =
                string.IsNullOrWhiteSpace(
                    selectedPlant
                )
                    ? "All"
                    : selectedPlant;

            ViewBag.DocumentNo =
                selectedPlant.Equals(
                    "Rolling Mill 1",
                    StringComparison.OrdinalIgnoreCase
                )
                ||
                selectedPlant.Equals(
                    "RM1",
                    StringComparison.OrdinalIgnoreCase
                )
                    ? "RM1F-FCD-1001"
                    : "RM2F-FCD-1001";

            ViewBag.RevisionNo =
                "02";

            ViewBag.AutoPrint =
                download;

            return View(
                "~/Views/RMReports/FurnaceChargingDischargingSummary.cshtml"
            );
        }
    }
}
