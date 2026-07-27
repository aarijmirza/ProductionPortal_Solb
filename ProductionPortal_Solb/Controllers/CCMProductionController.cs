using BAL.Repositories;
using DAL.Models;
using DAL.Repository;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ProductionPortal_Solb.Controllers
{
    public class CCMProductionController : Controller
    {
        private readonly CCMDailyProductionRepository repo;

        public CCMProductionController()
        {
            repo = new CCMDailyProductionRepository();
        }

        #region Add / Edit GET

        [HttpGet]
        public ActionResult Add(int? id)
        {
            CCMDailyProductionReportBLL model;

            if (id.HasValue && id.Value > 0)
            {
                model = repo.GetByID(id.Value);

                if (model == null)
                {
                    return HttpNotFound();
                }
            }
            else
            {
                model = new CCMDailyProductionReportBLL
                {
                    ReportDate = DateTime.Today,

                    ReportNo =
                        "CCM-" +
                        DateTime.Today.ToString("yyyyMMdd"),

                    StatusID = 1,

                    Details =
                        new List<
                            CCMDailyProductionReportDetailBLL
                        >()
                };
            }

            PrepareDetailRows(model);
            LoadDropdowns(model.Shift);

            return View(model);
        }

        #endregion

        #region Add / Edit POST

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(
            CCMDailyProductionReportBLL model)
        {
            PrepareDetailRows(model);

            ValidateReport(model);

            if (!ModelState.IsValid)
            {
                LoadDropdowns(model.Shift);
                return View(model);
            }

            try
            {
                string userName =
                    Convert.ToString(
                        Session["UserName"] ??
                        Session["EmployeeNo"] ??
                        User.Identity.Name
                    );

                model.CreatedBy =
                    string.IsNullOrWhiteSpace(userName)
                        ? "System"
                        : userName;

                int savedID = repo.Save(model);

                if (savedID > 0)
                {
                    TempData["Success"] =
                        model.ID > 0
                            ? "CCM daily production report updated successfully."
                            : "CCM daily production report saved successfully.";

                    return RedirectToAction(
                        "Details",
                        new
                        {
                            id = savedID
                        }
                    );
                }

                ModelState.AddModelError(
                    "",
                    "Report could not be saved."
                );
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(
                    "",
                    ex.Message
                );
            }

            LoadDropdowns(model.Shift);

            return View(model);
        }

        #endregion

        #region Details

        [HttpGet]
        public ActionResult Details(int id)
        {
            CCMDailyProductionReportBLL model =
                repo.GetByID(id);

            if (model == null)
            {
                return HttpNotFound();
            }

            return View(model);
        }

        #endregion

        #region List

        [HttpGet]
        public ActionResult List(
            DateTime? fromDate,
            DateTime? toDate,
            string shift)
        {
            DateTime startDate =
                fromDate ?? DateTime.Today;

            DateTime endDate =
                toDate ?? DateTime.Today;

            if (endDate < startDate)
            {
                endDate = startDate;
            }

            ViewBag.FromDate =
                startDate.ToString("yyyy-MM-dd");

            ViewBag.ToDate =
                endDate.ToString("yyyy-MM-dd");

            ViewBag.Shift = shift;

            List<CCMDailyProductionReportBLL> model =
                repo.GetAll(
                    startDate,
                    endDate,
                    shift
                );

            return View(model);
        }

        #endregion

        #region Delete

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                string userName =
                    Convert.ToString(
                        Session["UserName"] ??
                        Session["EmployeeNo"] ??
                        User.Identity.Name
                    );

                bool deleted = repo.Delete(
                    id,
                    string.IsNullOrWhiteSpace(userName)
                        ? "System"
                        : userName
                );

                if (deleted)
                {
                    TempData["Success"] =
                        "CCM production report deleted successfully.";
                }
                else
                {
                    TempData["Error"] =
                        "Report could not be deleted.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("List");
        }

        #endregion

        #region Private Methods

        private void PrepareDetailRows(
            CCMDailyProductionReportBLL model)
        {
            if (model.Details == null)
            {
                model.Details =
                    new List<
                        CCMDailyProductionReportDetailBLL
                    >();
            }

            int requiredRows = 9;

            while (model.Details.Count < requiredRows)
            {
                model.Details.Add(
                    new CCMDailyProductionReportDetailBLL()
                );
            }

            for (
                int index = 0;
                index < model.Details.Count;
                index++)
            {
                model.Details[index].SequenceNo =
                    index + 1;
            }
        }

        private void ValidateReport(
            CCMDailyProductionReportBLL model)
        {
            if (model.ReportDate == DateTime.MinValue)
            {
                ModelState.AddModelError(
                    "ReportDate",
                    "Report date is required."
                );
            }

            if (string.IsNullOrWhiteSpace(model.ReportNo))
            {
                ModelState.AddModelError(
                    "ReportNo",
                    "Report number is required."
                );
            }

            if (string.IsNullOrWhiteSpace(model.Shift))
            {
                ModelState.AddModelError(
                    "Shift",
                    "Shift is required."
                );
            }

            bool hasAnyProductionRow =
                model.Details != null &&
                model.Details.Any(x =>
                    !string.IsNullOrWhiteSpace(
                        x.Grade
                    ) ||
                    x.TotalBillets > 0 ||
                    x.GoodBillets > 0 ||
                    !string.IsNullOrWhiteSpace(
                        x.Remarks
                    )
                );

            if (!hasAnyProductionRow)
            {
                ModelState.AddModelError(
                    "",
                    "Please enter at least one production row."
                );
            }

            if (model.Details == null)
            {
                return;
            }

            for (
                int index = 0;
                index < model.Details.Count;
                index++)
            {
                CCMDailyProductionReportDetailBLL row =
                    model.Details[index];

                if (row.GoodBillets < 0)
                {
                    ModelState.AddModelError(
                        "Details[" +
                        index +
                        "].GoodBillets",
                        "Good billets cannot be negative."
                    );
                }

                if (row.GoodBillets >
                    row.TotalBillets)
                {
                    ModelState.AddModelError(
                        "Details[" +
                        index +
                        "].GoodBillets",
                        "Good billets cannot exceed total billets."
                    );
                }
            }
        }

        private void LoadDropdowns(
            string selectedShift = null)
        {
            List<string> shifts =
                new List<string>
                {
                    "Morning",
                    "Evening",
                    "Night",
                    "Long Morning",
                    "Long Night"
                };

            ViewBag.Shifts =
                new SelectList(
                    shifts,
                    selectedShift
                );
        }

        #endregion

        [HttpGet]
        public ActionResult DownloadPdf(int id)
        {
            CCMDailyProductionReportBLL model =
                repo.GetByID(id);

            if (model == null)
            {
                return HttpNotFound();
            }

            string fileName =
                "CCM_Daily_Production_" +
                model.ReportDate.ToString("yyyyMMdd") +
                "_" +
                model.Shift.Replace(" ", "_") +
                ".pdf";

            return new ViewAsPdf(
                "PdfReport",
                model
            )
            {
                FileName = fileName,

                PageSize =
                    Rotativa.Options.Size.A4,

                PageOrientation =
                    Rotativa.Options.Orientation.Landscape,

                PageMargins =
                    new Rotativa.Options.Margins
                    {
                        Top = 8,
                        Bottom = 8,
                        Left = 8,
                        Right = 8
                    },

                CustomSwitches =
                    "--print-media-type " +
                    "--disable-smart-shrinking " +
                    "--encoding utf-8"
            };
        }
    }
}