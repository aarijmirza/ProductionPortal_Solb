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
using System.Web.SessionState;
using WebAPICode.Helpers;
using static DAL.Models.ViewModel;
using static iTextSharp.text.pdf.AcroFields;

namespace ProductionPortal_Solb.Controllers
{
    [SessionState(
    SessionStateBehavior.Required
    )]
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
            if (string.IsNullOrWhiteSpace(heatNo))
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            heatNo = heatNo.Trim();

            var x = repo.GetBilletDetails(heatNo);

            if (x == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            decimal requiredBillet = Convert.ToDecimal(x.NoOfBillets);

            decimal alreadyChargedBillet = repo.GetAllCharging()
                .Where(c => !string.IsNullOrWhiteSpace(c.HeatNo)
                         && c.HeatNo.Trim().Equals(heatNo, StringComparison.OrdinalIgnoreCase)
                         && c.StatusID == 1)
                .Sum(c => Convert.ToDecimal(c.TotalBillet));

            decimal remainingBillet = requiredBillet - alreadyChargedBillet;

            if (remainingBillet < 0)
                remainingBillet = 0;

            var data = new
            {
                BoardingNo = x.BilletBoarding,
                SteelGrade = x.Grade,
                BilletSize = x.CrossSection,
                NoOfBillet = requiredBillet,
                AlreadyChargedBillet = alreadyChargedBillet,
                RemainingBillet = remainingBillet,
                BilletWeight = x.BilletWeight,
                BilletLength = x.BilletLength
            };

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        private void SetRollingMillSession(
            RMShiftDetailsBLL shiftDetail)
        {
            if (shiftDetail == null)
            {
                return;
            }

            Session["RM_ShiftDetailID"] =
                shiftDetail.ID;

            Session["RM_Date"] =
                shiftDetail.Date;

            Session["RM_Shift"] =
                shiftDetail.Shift;

            Session["RM_Plant"] =
                shiftDetail.Plant;

            Session["RM_Team"] =
                shiftDetail.Team;

            Session["RM_ShiftIncharge"] =
                shiftDetail.ShiftIncharge;
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
            var heats = repo.GetAllBoarding_RMCharging();
            var submittedAll = repo.GetAllCharging();



            if (Session["RM_Date"] == null || Session["RM_Shift"] == null || Session["RM_Plant"] == null || Session["RM_ShiftDetailID"] == null)
            {
                TempData["ErrorMessage"] = "Please select Rolling Mill Shift first.";
                return RedirectToAction("AddDetails", "RollingMill");
            }

            DateTime selectedDate = Convert.ToDateTime(Session["RM_Date"]);
            string selectedShift = Convert.ToString(Session["RM_Shift"]).Trim();
            string selectedPlant = Convert.ToString(Session["RM_Plant"]).Trim();

            DateTime start = selectedDate.Date;
            DateTime end = start.AddDays(1);

            int shiftDetailId = Convert.ToInt32(Session["RM_ShiftDetailID"]);

            var shiftDetails = rm.RollingMillDetails()
                .Where(x => x.ID == shiftDetailId && x.StatusID == 1)
                .FirstOrDefault();

            //var shiftDetails = rm.RollingMillDetails()
            //    .Where(x =>
            //        x.Date >= start &&
            //        x.Date < end &&
            //        !string.IsNullOrEmpty(x.Shift) &&
            //        !string.IsNullOrEmpty(x.Plant) &&
            //        x.Shift.Trim().Equals(selectedShift, StringComparison.OrdinalIgnoreCase) &&
            //        x.Plant.Trim().Equals(selectedPlant, StringComparison.OrdinalIgnoreCase)
            //    )
            //    .OrderByDescending(x => x.ID)
            //    .FirstOrDefault();

            if (shiftDetails == null)
            {
                TempData["ErrorMessage"] = "Selected shift details not found. Please select shift again.";
                return RedirectToAction("AddDetails", "RollingMill");
            }

            // ✅ Table ke liye selected date ki all shifts ka data lao
            // Taake table filter se Morning / Afternoon / Night show ho sake
            //var submittedToday = submittedAll
            //    .Where(x =>
            //        x.Date >= start &&
            //        x.Date < end &&
            //        x.StatusID == 1
            //    )
            //    .OrderBy(x => x.Shift)
            //    .ThenBy(x => x.HeatSequence)
            //    .ThenBy(x => x.CreatedDate)
            //    .ToList();

            var submittedToday = submittedAll
              .Where(x =>
                    x.Date >= selectedDate &&
                    x.Date < selectedDate.AddDays(1) &&
                    x.StatusID == 1 &&
                    !string.IsNullOrEmpty(x.Plant) &&
                    x.Plant.Trim().Equals(selectedPlant, StringComparison.OrdinalIgnoreCase) &&
                    !string.IsNullOrEmpty(x.Shift) &&
                    x.Shift.Trim().Equals(selectedShift, StringComparison.OrdinalIgnoreCase)
                )
                .OrderBy(x => x.HeatSequence)
                .ThenBy(x => x.CreatedDate)
                .ToList();

            // ✅ Total charged billet against each Heat No
            // Ye global rahega taake heat fully charged hone par disable ho
            var chargedBilletByHeat = submittedAll
                .Where(x =>
                    !string.IsNullOrWhiteSpace(x.HeatNo) &&
                    x.StatusID == 1
                )
                .GroupBy(x => x.HeatNo.Trim(), StringComparer.OrdinalIgnoreCase)
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(x => Convert.ToDecimal(x.TotalBillet)),
                    StringComparer.OrdinalIgnoreCase
                );

            ViewBag.HeatNo = heats.Select(h =>
            {
                string heatNo = h.HeatNo == null ? "" : h.HeatNo.Trim();

                decimal requiredBillet = 0;

                if (h.NoOfBillets != null)
                {
                    requiredBillet = Convert.ToDecimal(h.NoOfBillets);
                }

                decimal alreadyCharged = chargedBilletByHeat.ContainsKey(heatNo)
                    ? chargedBilletByHeat[heatNo]
                    : 0;

                decimal remainingBillet = requiredBillet - alreadyCharged;

                if (remainingBillet < 0)
                    remainingBillet = 0;

                bool isFullyUsed = requiredBillet > 0 && alreadyCharged >= requiredBillet;

                return new SelectListItem
                {
                    Value = heatNo,
                    Text = isFullyUsed
                        ? heatNo + " - Used"
                        : heatNo + " - Remaining: " + remainingBillet,
                    Disabled = isFullyUsed
                };
            }).ToList();

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
            try
            {
                if (Session["RM_Date"] == null ||
                    Session["RM_Shift"] == null ||
                    Session["RM_ShiftDetailID"] == null)
                {
                    TempData["ErrorMessage"] = "Please select Rolling Mill Shift first.";
                    return RedirectToAction("AddDetails", "RollingMill");
                }

                int shiftDetailId = Convert.ToInt32(Session["RM_ShiftDetailID"]);

                var shiftDetails = rm.RollingMillDetails()
                    .Where(x => x.ID == shiftDetailId && x.StatusID == 1)
                    .FirstOrDefault();

                if (shiftDetails == null)
                {
                    TempData["ErrorMessage"] = "Selected shift details not found. Please select shift again.";
                    return RedirectToAction("AddDetails", "RollingMill");
                }

                if (model == null)
                {
                    TempData["ErrorMessage"] = "Invalid data.";
                    return RedirectToAction("AddCharge");
                }

                if (string.IsNullOrWhiteSpace(model.HeatNo))
                {
                    TempData["ErrorMessage"] = "Please select Heat No.";
                    return RedirectToAction("AddCharge");
                }

                if (model.TotalBillet <= 0)
                {
                    TempData["ErrorMessage"] = "Please enter valid Total Billet.";
                    return RedirectToAction("AddCharge");
                }

                if (model.Weight <= 0)
                {
                    TempData["ErrorMessage"] = "Please enter valid Weight.";
                    return RedirectToAction("AddCharge");
                }

                model.HeatNo = model.HeatNo.Trim();

                var heatInfo = repo.GetBilletDetails(model.HeatNo);

                if (heatInfo == null)
                {
                    TempData["ErrorMessage"] = "Heat details not found.";
                    return RedirectToAction("AddCharge");
                }

                decimal requiredBillet = Convert.ToDecimal(heatInfo.NoOfBillets);
                decimal currentBillet = Convert.ToDecimal(model.TotalBillet);

                bool isUpdate = model.ID > 0;

                decimal alreadyChargedBillet = repo.GetAllCharging()
                    .Where(x =>
                        x.StatusID == 1 &&
                        x.ID != model.ID &&
                        !string.IsNullOrWhiteSpace(x.HeatNo) &&
                        x.HeatNo.Trim().Equals(model.HeatNo, StringComparison.OrdinalIgnoreCase)
                    )
                    .Sum(x => Convert.ToDecimal(x.TotalBillet));

                decimal remainingBillet = requiredBillet - alreadyChargedBillet;

                if (remainingBillet < 0)
                    remainingBillet = 0;

                if (remainingBillet <= 0)
                {
                    TempData["ErrorMessage"] = "This Heat No is already fully charged.";
                    return RedirectToAction("AddCharge");
                }

                if (currentBillet > remainingBillet)
                {
                    TempData["ErrorMessage"] =
                        "Only " + remainingBillet + " billet remaining for this Heat No. You cannot charge " + currentBillet + ".";

                    return RedirectToAction("AddCharge");
                }

                // Selected Rolling Mill Details se Date / Shift / Plant assign hogi
                model.Date = shiftDetails.Date;
                model.Shift = shiftDetails.Shift;
                model.Plant = shiftDetails.Plant;
                model.StatusID = 1;

                // Server side total weight calculation
                model.TotalWeight = model.TotalBillet * model.Weight;

                // ==========================
                // UPDATE MODE
                // ==========================
                if (isUpdate)
                {
                    /*
                     * Update mode:
                     * Agar form se HeatSequence aa rahi hai to wohi save hogi.
                     * Agar form se 0 ya null/default value aaye to existing sequence preserve karni chahiye.
                     */

                    if (model.HeatSequence <= 0)
                    {
                        var existingRecord = repo.GetAllCharging()
                            .Where(x => x.ID == model.ID && x.StatusID == 1)
                            .FirstOrDefault();

                        if (existingRecord != null)
                        {
                            model.HeatSequence = existingRecord.HeatSequence;
                        }
                    }

                    model.UpdatedBy = User.Identity.Name;
                    // model.UpdatedDate = DateTime.Now;

                    int update = rm.UpdateBilletCharging(model);

                    if (update < 0)
                    {
                        TempData["SuccessMessage"] = "Charged heat updated successfully.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Charged heat not updated.";
                    }

                    return RedirectToAction("AddCharge");
                }

                // ==========================
                // INSERT MODE
                // ==========================

                /*
                 * Insert mode:
                 * Agar form se HeatSequence > 0 aa rahi hai, to wohi submit hogi.
                 * Agar form se nahi aa rahi ya 0 hai, to system next sequence generate karega.
                 */

                if (model.HeatSequence <= 0)
                {
                    var existingHeats = repo.GetAllCharging()
                        .Where(x =>
                            x.Date >= shiftDetails.Date.Date &&
                            x.Date < shiftDetails.Date.Date.AddDays(1) &&
                            x.StatusID == 1 &&
                            !string.IsNullOrWhiteSpace(x.Shift) &&
                            x.Shift.Trim().Equals(shiftDetails.Shift.Trim(), StringComparison.OrdinalIgnoreCase) &&
                            !string.IsNullOrWhiteSpace(x.Plant) &&
                            x.Plant.Trim().Equals(shiftDetails.Plant.Trim(), StringComparison.OrdinalIgnoreCase)
                        )
                        .ToList();

                    int nextSequence = existingHeats
                        .Select(x => x.HeatSequence)
                        .Where(x => x > 0)
                        .DefaultIfEmpty(0)
                        .Max() + 1;

                    model.HeatSequence = nextSequence;
                }

                model.CreatedDate = DateTime.Now;
                model.CreatedBy = User.Identity.Name;

                int rtn = rm.InsertBilletCharging(model);

                if (rtn < 0)
                {
                    TempData["SuccessMessage"] = "Heat charged successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Data not saved.";
                }

                return RedirectToAction("AddCharge");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error: " + ex.Message;
                return RedirectToAction("AddCharge");
            }
        }

        public ActionResult Discharging(DateTime? from, DateTime? to, string shift)
        {
            // 🔑 Default = TODAY
            DateTime startDate = from ?? DateTime.Today;
            DateTime endDate = to ?? DateTime.Today;

            var data = rm.GetDichargedHeat(startDate, endDate, shift);

            return View("~/Views/RollingMill/Discharging/list.cshtml", data);
        }

        [HttpGet]
        public JsonResult GetChargingByHeat(string heatNo)
        {
            if (string.IsNullOrWhiteSpace(heatNo))
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            heatNo = heatNo.Trim();

            var chargingRows = repo.GetAllCharging()
                .Where(x =>
                    !string.IsNullOrWhiteSpace(x.HeatNo) &&
                    x.HeatNo.Trim().Equals(heatNo, StringComparison.OrdinalIgnoreCase) &&
                    x.StatusID == 1
                )
                .ToList();

            if (!chargingRows.Any())
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            var first = chargingRows.OrderByDescending(x => x.ID).FirstOrDefault();

            decimal chargedBillet = chargingRows.Sum(x => Convert.ToDecimal(x.TotalBillet));
            decimal chargedWeight = chargingRows.Sum(x => Convert.ToDecimal(x.TotalWeight));

            decimal alreadyDischargedBillet = rm.GetDichargedHeat2()
                .Where(x =>
                    !string.IsNullOrWhiteSpace(x.HeatNo) &&
                    x.HeatNo.Trim().Equals(heatNo, StringComparison.OrdinalIgnoreCase) &&
                    x.StatusID == 1
                )
                .Sum(x => Convert.ToDecimal(x.TotalBillet));

            decimal remainingBillet = chargedBillet - alreadyDischargedBillet;

            if (remainingBillet < 0)
                remainingBillet = 0;

            decimal weightPerBillet = chargedBillet > 0
                ? chargedWeight / chargedBillet
                : 0;

            decimal remainingWeight = remainingBillet * weightPerBillet;

            var data = new
            {
                HeatNo = first.HeatNo,
                BoardingNo = first.BoardingNo,
                SteelGrade = first.SteelGrade,
                Profile = first.Profile,
                ProductSpecs = first.ProductSpecs,

                ChargedBillet = chargedBillet,
                AlreadyDischargedBillet = alreadyDischargedBillet,
                RemainingBillet = remainingBillet,

                TotalBillet = remainingBillet,
                TotalWeight = remainingWeight
            };

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddDischarge()
        {
            var chargedHeatsAll = repo.GetAllCharging();
            var dischargedAll = rm.GetDichargedHeat2();

            if (Session["RM_Date"] == null || Session["RM_Plant"] == null || Session["RM_Shift"] == null || Session["RM_ShiftDetailID"] == null)
            {
                TempData["ErrorMessage"] = "Please select Rolling Mill Details first.";
                return RedirectToAction("AddDetails", "RollingMill");
            }

            DateTime selectedDate = Convert.ToDateTime(Session["RM_Date"]);
            DateTime start = selectedDate.Date;
            DateTime end = start.AddDays(1);

            string selectedPlant = Convert.ToString(Session["RM_Plant"]);
            string selectedShift = Convert.ToString(Session["RM_Shift"]);

            int shiftDetailId = Convert.ToInt32(Session["RM_ShiftDetailID"]);

            var selectedShiftDetails = rm.RollingMillDetails()
             .Where(x => x.ID == shiftDetailId && x.StatusID == 1)
             .FirstOrDefault();

            if (selectedShiftDetails == null)
            {
                TempData["ErrorMessage"] = "Selected Rolling Mill Details not found. Please select again.";
                return RedirectToAction("AddDetails", "RollingMill");
            }

            // ✅ Table ke liye selected date ki all shifts ka data lao
            // Taake table filter se Morning / Afternoon / Night show ho sake
            //var submittedSelected = dischargedAll
            //    .Where(x =>
            //        x.Date >= start &&
            //        x.Date < end &&
            //        x.StatusID == 1
            //    )
            //    .OrderBy(x => x.Shift)
            //    .ThenBy(x => x.DischargingSequence)
            //    .ThenBy(x => x.CreatedOn)
            //    .ToList();

            var submittedSelected = dischargedAll
                .Where(x =>
                    x.Date >= start &&
                    x.Date < end &&
                    x.StatusID == 1 &&
                    !string.IsNullOrEmpty(x.Plant) &&
                    x.Plant.Trim().Equals(selectedPlant, StringComparison.OrdinalIgnoreCase)
                )
                .OrderBy(x => x.Shift)
                .ThenBy(x => x.DischargingSequence)
                .ThenBy(x => x.CreatedOn)
                .ToList();

            // ✅ Dropdown ke liye selected shift ke charged heats
            // Form selected shift par hi discharge karega
            //var chargedHeats = chargedHeatsAll
            //    .Where(x =>
            //        x.StatusID == 1 &&
            //        x.Date >= start &&
            //        x.Date < end &&
            //        !string.IsNullOrWhiteSpace(x.Shift) &&
            //        x.Shift.Trim().Equals(selectedShift.Trim(), StringComparison.OrdinalIgnoreCase)
            //    )
            //    .ToList();
            var chargedHeats = chargedHeatsAll
    .Where(x =>
        x.StatusID == 1 &&
        !string.IsNullOrWhiteSpace(x.HeatNo)
    )
    .ToList();

            // ✅ Total charged billet against each Heat No
            var chargedBilletByHeat = chargedHeatsAll
                .Where(x =>
                    !string.IsNullOrWhiteSpace(x.HeatNo) &&
                    x.StatusID == 1
                )
                .GroupBy(x => x.HeatNo.Trim(), StringComparer.OrdinalIgnoreCase)
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(x => Convert.ToDecimal(x.TotalBillet)),
                    StringComparer.OrdinalIgnoreCase
                );

            // ✅ Total discharged billet against each Heat No
            var dischargedBilletByHeat = dischargedAll
                .Where(x =>
                    !string.IsNullOrWhiteSpace(x.HeatNo) &&
                    x.StatusID == 1
                )
                .GroupBy(x => x.HeatNo.Trim(), StringComparer.OrdinalIgnoreCase)
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(x => Convert.ToDecimal(x.TotalBillet)),
                    StringComparer.OrdinalIgnoreCase
                );

            // ✅ Heat disable only when charged billet fully discharged
            ViewBag.HeatNo = chargedHeats
                .Where(h => !string.IsNullOrWhiteSpace(h.HeatNo))
                .GroupBy(h => h.HeatNo.Trim(), StringComparer.OrdinalIgnoreCase)
                .Select(g => g.First())
                .Select(h =>
                {
                    string heatNo = h.HeatNo.Trim();

                    decimal chargedBillet = chargedBilletByHeat.ContainsKey(heatNo)
                        ? chargedBilletByHeat[heatNo]
                        : 0;

                    decimal dischargedBillet = dischargedBilletByHeat.ContainsKey(heatNo)
                        ? dischargedBilletByHeat[heatNo]
                        : 0;

                    decimal remainingBillet = chargedBillet - dischargedBillet;

                    if (remainingBillet < 0)
                        remainingBillet = 0;

                    bool isFullyDischarged = chargedBillet > 0 && dischargedBillet >= chargedBillet;

                    return new SelectListItem
                    {
                        Text = isFullyDischarged
                            ? heatNo + " - Discharged"
                            : heatNo + " - Remaining: " + remainingBillet,
                        Value = heatNo,
                        Disabled = isFullyDischarged
                    };
                })
                .ToList();

            var vm = new RMDischargingVM
            {
                Date = selectedShiftDetails.Date,
                Shift = selectedShiftDetails.Shift,
                Plant = selectedShiftDetails.Plant,
                Team = selectedShiftDetails.Team,
                ShiftIncharge = selectedShiftDetails.ShiftIncharge,
                SubmittedHeat = submittedSelected
            };

            var BilletGradeList = repo.GetBilletGrade();
            ViewBag.BilletGrade = new SelectList(BilletGradeList, "ProductID", "SpecGrade");
            ViewBag.GradeDataJson = JsonConvert.SerializeObject(BilletGradeList);

            return View("~/Views/RollingMill/Discharging/Add.cshtml", vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddDischarge(BilletDischargingBLL data)
        {
            try
            {
                if (Session["RM_Date"] == null || Session["RM_Plant"] == null || Session["RM_Shift"] == null || Session["RM_ShiftDetailID"] == null)
                {
                    TempData["ErrorMessage"] = "Please select Rolling Mill Details first.";
                    return RedirectToAction("AddDetails", "RollingMill");
                }

                DateTime selectedDate = Convert.ToDateTime(Session["RM_Date"]).Date;
                DateTime start = selectedDate.Date;
                DateTime end = start.AddDays(1);

                string selectedPlant = Convert.ToString(Session["RM_Plant"]);
                string selectedShift = Convert.ToString(Session["RM_Shift"]);

                if (data == null)
                {
                    TempData["ErrorMessage"] = "Invalid data.";
                    return RedirectToAction("AddDischarge");
                }

                if (string.IsNullOrWhiteSpace(data.HeatNo))
                {
                    TempData["ErrorMessage"] = "Please select Heat No.";
                    return RedirectToAction("AddDischarge");
                }

                if (data.TotalBillet <= 0)
                {
                    TempData["ErrorMessage"] = "Please enter valid Total Billet.";
                    return RedirectToAction("AddDischarge");
                }

                int shiftDetailId = Convert.ToInt32(Session["RM_ShiftDetailID"]);

                var shiftDetails = rm.RollingMillDetails()
                 .Where(x => x.ID == shiftDetailId && x.StatusID == 1)
                 .FirstOrDefault();

                if (shiftDetails == null)
                {
                    TempData["ErrorMessage"] = "Selected shift details not found.";
                    return RedirectToAction("AddDetails", "RollingMill");
                }

                data.HeatNo = data.HeatNo.Trim();

                bool isUpdate = data.ID > 0;

                var chargingRows = repo.GetAllCharging()
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(x.HeatNo) &&
                        x.HeatNo.Trim().Equals(data.HeatNo, StringComparison.OrdinalIgnoreCase) &&
                        x.StatusID == 1
                    )
                    .ToList();

                if (!chargingRows.Any())
                {
                    TempData["ErrorMessage"] = "Charging record not found against this Heat No.";
                    return RedirectToAction("AddDischarge");
                }

                decimal chargedBillet = chargingRows.Sum(x => Convert.ToDecimal(x.TotalBillet));

                decimal alreadyDischargedBillet = rm.GetDichargedHeat2()
                    .Where(x =>
                        x.StatusID == 1 &&
                        x.ID != data.ID &&
                        !string.IsNullOrWhiteSpace(x.HeatNo) &&
                        x.HeatNo.Trim().Equals(data.HeatNo, StringComparison.OrdinalIgnoreCase)
                    )
                    .Sum(x => Convert.ToDecimal(x.TotalBillet));

                decimal currentBillet = Convert.ToDecimal(data.TotalBillet);
                decimal remainingBillet = chargedBillet - alreadyDischargedBillet;

                if (remainingBillet < 0)
                    remainingBillet = 0;

                if (remainingBillet <= 0)
                {
                    TempData["ErrorMessage"] = "This Heat No is already fully discharged.";
                    return RedirectToAction("AddDischarge");
                }

                if (currentBillet > remainingBillet)
                {
                    TempData["ErrorMessage"] = "Only " + remainingBillet + " billet remaining for discharge. You cannot discharge " + currentBillet + ".";
                    return RedirectToAction("AddDischarge");
                }

                data.Date = shiftDetails.Date;
                data.Shift = shiftDetails.Shift;
                data.Plant = shiftDetails.Plant;
                data.StatusID = 1;

                if (isUpdate)
                {
                    data.UpdatedBy = User.Identity.Name;
                    data.UpdatedDate = DateTime.Now;

                    int update = rm.UpdateDischarging(data);

                    if (update < 0)
                        TempData["SuccessMessage"] = "Discharge record updated successfully.";
                    else
                        TempData["ErrorMessage"] = "Discharge record not updated.";

                    return RedirectToAction("AddDischarge");
                }

                //var existingDischarge = rm.GetDichargedHeat2()
                //    .Where(x =>
                //        x.Date >= start &&
                //        x.Date < end &&
                //        !string.IsNullOrWhiteSpace(x.Shift) &&
                //        x.Shift.Trim().Equals(selectedShift.Trim(), StringComparison.OrdinalIgnoreCase) &&
                //        !string.IsNullOrWhiteSpace(x.PlantName) &&
                //        x.PlantName.Trim().Equals(selectedPlant.Trim(), StringComparison.OrdinalIgnoreCase) &&
                //        x.StatusID == 1
                //    )
                //    .ToList();

                //int nextSequence = existingDischarge
                //    .Select(x => x.DischargingSequence)
                //    .Where(x => x > 0)
                //    .DefaultIfEmpty(0)
                //    .Max() + 1;

                // Insert mode: sequence next generate hogi
                var existingHeats = rm.GetDichargedHeat2()
                    .Where(x =>
                        x.Date >= shiftDetails.Date.Date &&
                        x.Date < shiftDetails.Date.Date.AddDays(1) &&
                        !string.IsNullOrEmpty(x.Shift) &&
                        x.Shift.Trim().Equals(shiftDetails.Shift.Trim(), StringComparison.OrdinalIgnoreCase) &&
                        x.StatusID == 1
                    )
                    .ToList();

                int nextSequence = existingHeats
                    .Select(x => x.DischargingSequence)
                    .Where(x => x > 0)
                    .DefaultIfEmpty(0)
                    .Max() + 1;


                data.DischargingSequence = nextSequence;
                data.CreatedOn = DateTime.Now;
                data.CreatedBy = User.Identity.Name;

                int rtn = rm.InsertDischarging(data);

                if (rtn < 0)
                    TempData["SuccessMessage"] = "Heat discharged successfully.";
                else
                    TempData["ErrorMessage"] = "Data not saved.";

                return RedirectToAction("AddDischarge");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error: " + ex.Message;
                return RedirectToAction("AddDischarge");
            }
        }


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

        [Route("BundleSection")]
        public ActionResult BundleSection()
        {
            var submitted = rm.GetBundlesHeats();
            return View("~/Views/RollingMill/BundleSection/BundleSection.cshtml", submitted);
        }

        [Route("AddBundleSection")]
        public ActionResult AddBundleSection()
        {
            if (Session["RM_Date"] == null || Session["RM_Shift"] == null || Session["RM_ShiftDetailID"] == null)
            {
                TempData["ErrorMessage"] = "Please select Rolling Mill Shift first.";
                return RedirectToAction("AddDetails", "RollingMill");
            }

            DateTime selectedDate = Convert.ToDateTime(Session["RM_Date"]).Date;
            DateTime start = selectedDate.Date;
            DateTime end = start.AddDays(1);

            string selectedShift = Convert.ToString(Session["RM_Shift"]);
            string selectedPlant = Convert.ToString(Session["RM_Plant"]);

            int shiftDetailId = Convert.ToInt32(Session["RM_ShiftDetailID"]);

            var shiftDetails = rm.RollingMillDetails()
             .Where(x => x.ID == shiftDetailId && x.StatusID == 1)
             .FirstOrDefault();

            if (shiftDetails == null)
            {
                TempData["ErrorMessage"] = "Selected shift details not found. Please select shift again.";
                return RedirectToAction("AddDetails", "RollingMill");
            }

            // ✅ Only selected Date + Shift charged heats
            var heats = rm.GetDichargedHeat2()
                .Where(h =>
                    h.StatusID == 1 &&
                    h.Date >= start &&
                    h.Date < end &&
                    !string.IsNullOrWhiteSpace(h.HeatNo) &&
                    !string.IsNullOrWhiteSpace(h.Shift) &&
                    h.Shift.Trim().Equals(selectedShift.Trim(), StringComparison.OrdinalIgnoreCase)
                )
                .ToList();

            ViewBag.HeatNo = heats
                .GroupBy(h => h.HeatNo.Trim(), StringComparer.OrdinalIgnoreCase)
                .Select(g => g.First())
                .Select(h => new SelectListItem
                {
                    Text = h.HeatNo,
                    Value = h.HeatNo,
                    Disabled = false
                })
                .ToList();

            // ✅ Only selected Date + Shift bundle records

            var submittedHeat = rm.GetBundlesHeats()
                .Where(x =>
                    x.Date >= start &&
                    x.Date < end &&
                    x.StatusID == 1 &&
                    !string.IsNullOrEmpty(x.Plant) &&
                    x.Plant.Trim().Equals(selectedPlant, StringComparison.OrdinalIgnoreCase)
                )
                .OrderBy(x => x.Shift)
                .ThenBy(x => x.CreatedDate)
                .ToList();

            //var submittedHeat = rm.GetBundlesHeats()
            //    .Where(x =>
            //        x.Date.HasValue &&
            //        x.Date.Value >= start &&
            //        x.Date.Value < end
            //    )
            //    .OrderBy(x => x.Shift)
            //    .ThenByDescending(x => x.ID)
            //    .ToList();

            var vm = new RollingMillChargeVM
            {
                Date = shiftDetails.Date,
                Plant = shiftDetails.Plant,
                Team = shiftDetails.Team,
                Shift = shiftDetails.Shift,
                ShiftIncharge = shiftDetails.ShiftIncharge,
                SubmittedHeat = submittedHeat
            };

            return View("~/Views/RollingMill/BundleSection/AddBundleSection.cshtml", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InsertBundleSection(BundlingSectionBLL data)
        {
            try
            {
                if (Session["RM_Date"] == null || Session["RM_Shift"] == null || Session["RM_ShiftDetailID"] == null)
                {
                    TempData["ErrorMessage"] = "Please select Rolling Mill Shift first.";
                    return RedirectToAction("AddDetails", "RollingMill");
                }

                DateTime selectedDate = Convert.ToDateTime(Session["RM_Date"]).Date;
                string selectedShift = Convert.ToString(Session["RM_Shift"]);
                string selectedPlant = Convert.ToString(Session["RM_Plant"]);

                DateTime start = selectedDate.Date;
                DateTime end = start.AddDays(1);

                int shiftDetailId = Convert.ToInt32(Session["RM_ShiftDetailID"]);

                var shiftDetails = rm.RollingMillDetails()
                 .Where(x => x.ID == shiftDetailId && x.StatusID == 1)
                 .FirstOrDefault();

                if (shiftDetails == null)
                {
                    TempData["ErrorMessage"] = "Selected shift details not found. Please select shift again.";
                    return RedirectToAction("AddDetails", "RollingMill");
                }

                // ✅ Insert/Update dono main selected details ki Date & Shift use hogi
                data.Date = shiftDetails.Date;
                data.Shift = shiftDetails.Shift;
                data.Plant = shiftDetails.Plant;
                data.StatusID = 1;

                if (data.ID > 0)
                {
                    data.UpdatedBy = User.Identity.Name;
                    data.UpdatedDate = DateTime.Now;

                    int update = rm.UpdateBundlingSection(data);

                    if (update < 0)
                        TempData["SuccessMessage"] = "Bundle record updated successfully.";
                    else
                        TempData["ErrorMessage"] = "Bundle record not updated.";
                }
                else
                {
                    data.CreatedDate = DateTime.Now;
                    data.CreatedBy = User.Identity.Name;

                    int insert = rm.AddBundlingSection(data);

                    if (insert < 0)
                        TempData["SuccessMessage"] = "Bundle record saved successfully.";
                    else
                        TempData["ErrorMessage"] = "Bundle record not saved.";
                }

                return RedirectToAction("AddBundleSection");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error: " + ex.Message;
                return RedirectToAction("AddBundleSection");
            }
        }
        public ActionResult delete(int? id)
        {

            //vardStatusID = 3;
            //data.CreatedDate = DateTime.Now;
            var CreatedBy = User.Identity.Name;

            int rtn;

            // 🔥 INSERT
            rtn = rm.DeleteShiftDetails(id, CreatedBy);
            TempData["SuccessMessage"] = "Data saved successfully";

            return RedirectToAction("AddDetails");
        }
        public ActionResult deleteCharging(int? id)
        {

            //vardStatusID = 3;
            //data.CreatedDate = DateTime.Now;
            var CreatedBy = User.Identity.Name;

            int rtn;

            // 🔥 INSERT
            rtn = rm.DeleteCharging(id, CreatedBy);
            TempData["SuccessMessage"] = "Data saved successfully";

            return RedirectToAction("AddCharge");
        }

        public ActionResult deleteDischarging(int? id)
        {

            //vardStatusID = 3;
            //data.CreatedDate = DateTime.Now;
            var CreatedBy = User.Identity.Name;

            int rtn;

            // 🔥 INSERT
            rtn = rm.DeleteDischarging(id, CreatedBy);
            TempData["SuccessMessage"] = "Data saved successfully";

            return RedirectToAction("AddDischarge");
        }

        public ActionResult deleteBundle(int? id)
        {

            //vardStatusID = 3;
            //data.CreatedDate = DateTime.Now;
            var CreatedBy = User.Identity.Name;

            int rtn;

            // 🔥 INSERT
            rtn = rm.DeleteBundle(id, CreatedBy);
            TempData["SuccessMessage"] = "Data saved successfully";

            return RedirectToAction("AddBundleSection");
        }

        //[HttpPost]
        //public JsonResult SetSelectedShift(string date, string shift, string plant)
        //{
        //    Session["RM_Date"] = Convert.ToDateTime(date);
        //    Session["RM_Shift"] = shift;
        //    Session["RM_Plant"] = plant;

        //    return Json(new { success = true });
        //}

        //[HttpPost]
        //public JsonResult SetSelectedShift(int id, DateTime date, string shift, string plant, string team, string shiftincharge)
        //{
        //    if (id <= 0)
        //    {
        //        return Json(new { success = false, message = "Invalid shift detail ID." });
        //    }

        //    Session["RM_ShiftDetailID"] = id;
        //    Session["RM_Date"] = date;
        //    Session["RM_Shift"] = shift;
        //    Session["RM_Plant"] = plant;
        //    Session["RM_Team"] = team;
        //    Session["RM_ShiftIncharge"] = shiftincharge;

        //    return Json(new { success = true });
        //}

        [HttpPost]
        public JsonResult SetSelectedShift(
            int shiftDetailID,
            string date,
            string shift,
            string plant,
            string team,
            string shiftincharge)
        {
            try
            {
                if (shiftDetailID <= 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Invalid shift detail ID."
                    });
                }

                RMShiftDetailsBLL selectedShift =
                    rm.RollingMillDetails()
                        .FirstOrDefault(x =>
                            x.ID == shiftDetailID &&
                            x.StatusID == 1
                        );

                if (selectedShift == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Selected shift details were not found."
                    });
                }

                SetRollingMillSession(
                    selectedShift
                );

                return Json(new
                {
                    success = true,
                    message = "Shift loaded successfully."
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        public ActionResult AddDetails(DateTime? date)
        {
            DateTime selectedDate =
                date ?? DateTime.Today;

            DateTime start =
                selectedDate.Date;

            DateTime end =
                start.AddDays(1);

            var allDetails =
                rm.RollingMillDetails()
                    .Where(x =>
                        x.Date >= start &&
                        x.Date < end &&
                        x.StatusID == 1
                    )
                    .OrderByDescending(x => x.ID)
                    .ToList();

            /*
                Agar session empty hai aur selected date par
                shift detail available hai, latest shift load karo.
            */
            bool sessionIsEmpty =
                Session["RM_ShiftDetailID"] == null ||
                Session["RM_Date"] == null ||
                Session["RM_Shift"] == null;

            if (
                sessionIsEmpty &&
                allDetails.Any()
            )
            {
                var latest =
                    allDetails.FirstOrDefault();

                SetRollingMillSession(
                    latest
                );
            }

            var vm =
                new RMShiftDetailsVM
                {
                    List = allDetails
                };

            ViewBag.SelectedDate =
                selectedDate.ToString(
                    "yyyy-MM-dd"
                );

            ViewBag.LoadedShiftDetailID =
                Session["RM_ShiftDetailID"] ??
                0;

            ViewBag.LoadedDate =
                Session["RM_Date"] != null
                    ? Convert.ToDateTime(
                        Session["RM_Date"]
                    ).ToString(
                        "dd-MM-yyyy"
                    )
                    : "-";

            ViewBag.LoadedPlant =
                Session["RM_Plant"] ??
                "-";

            ViewBag.LoadedShift =
                Session["RM_Shift"] ??
                "-";

            ViewBag.LoadedTeam =
                Session["RM_Team"] ??
                "-";

            ViewBag.LoadedIncharge =
                Session["RM_ShiftIncharge"] ??
                "-";

            return View(vm);
        }

        [HttpPost]
        public ActionResult AddDetails(RMShiftDetailsBLL data)
        {
            try
            {
                if (data == null)
                {
                    TempData["ErrorMessage"] = "Invalid data.";
                    return RedirectToAction("AddDetails");
                }

                if (data.Date == null)
                {
                    TempData["ErrorMessage"] = "Date is required.";
                    return RedirectToAction("AddDetails");
                }

                if (string.IsNullOrWhiteSpace(data.Plant))
                {
                    TempData["ErrorMessage"] = "Plant is required.";
                    return RedirectToAction("AddDetails");
                }

                if (string.IsNullOrWhiteSpace(data.Team))
                {
                    TempData["ErrorMessage"] = "Team is required.";
                    return RedirectToAction("AddDetails");
                }

                if (string.IsNullOrWhiteSpace(data.Shift))
                {
                    TempData["ErrorMessage"] = "Shift is required.";
                    return RedirectToAction("AddDetails");
                }

                if (string.IsNullOrWhiteSpace(data.ShiftIncharge))
                {
                    TempData["ErrorMessage"] = "Shift Incharge is required.";
                    return RedirectToAction("AddDetails");
                }

                data.Plant = data.Plant.Trim();
                data.Team = data.Team.Trim();
                data.Shift = data.Shift.Trim();
                data.ShiftIncharge = data.ShiftIncharge.Trim();

                data.StatusID = 1;
                data.CreatedBy = User.Identity.Name;
                data.CreatedDate = DateTime.Now;

                int result = rm.AddRMShiftDetails(data);

                if (result > 0 || result < 0)
                {
                    DateTime savedDate =
                        Convert.ToDateTime(
                            data.Date
                        ).Date;

                    RMShiftDetailsBLL savedShift =
                        rm.RollingMillDetails()
                            .Where(x =>
                                x.StatusID == 1 &&
                                x.Date >= savedDate &&
                                x.Date < savedDate.AddDays(1) &&
                                !string.IsNullOrWhiteSpace(x.Plant) &&
                                x.Plant.Trim().Equals(
                                    data.Plant,
                                    StringComparison.OrdinalIgnoreCase
                                ) &&
                                !string.IsNullOrWhiteSpace(x.Shift) &&
                                x.Shift.Trim().Equals(
                                    data.Shift,
                                    StringComparison.OrdinalIgnoreCase
                                ) &&
                                !string.IsNullOrWhiteSpace(x.Team) &&
                                x.Team.Trim().Equals(
                                    data.Team,
                                    StringComparison.OrdinalIgnoreCase
                                )
                            )
                            .OrderByDescending(x => x.ID)
                            .FirstOrDefault();

                    SetRollingMillSession(
                        savedShift
                    );

                    TempData["SuccessMessage"] =
                        "Shift details saved and loaded successfully.";

                    return RedirectToAction("AddDetails", new
                    {
                        date = Convert.ToDateTime(data.Date).ToString("yyyy-MM-dd")
                    });
                }
                else
                {
                    TempData["ErrorMessage"] = "Shift details could not be saved.";
                    return RedirectToAction("AddDetails");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error: " + ex.Message;
                return RedirectToAction("AddDetails");
            }
        }

        [HttpPost]
        public JsonResult SetSelectedDateAjax(DateTime date)
        {
            Session["RollingMillSelectedDate"] = date.Date;

            return Json(new
            {
                success = true
            });
        }

        public ActionResult RMHourlyDischarge()
        {
            if (Session["RM_Date"] == null || Session["RM_Shift"] == null || Session["RM_ShiftDetailID"] == null)
            {
                TempData["ErrorMessage"] = "Please select Rolling Mill Shift first.";
                return RedirectToAction("AddDetails", "RollingMill");
            }

            DateTime selectedDate = Convert.ToDateTime(Session["RM_Date"]);
            string selectedShift = Convert.ToString(Session["RM_Shift"]);
            string selectedPlant = Convert.ToString(Session["RM_Plant"]);

            DateTime start = selectedDate.Date;
            DateTime end = start.AddDays(1);

            var shiftDetails = rm.RollingMillDetails()
                .Where(x =>
                    x.Date >= start &&
                    x.Date < end &&
                    !string.IsNullOrEmpty(x.Shift) &&
                    x.Shift.Trim().Equals(selectedShift.Trim(), StringComparison.OrdinalIgnoreCase) &&
                    (
                        string.IsNullOrEmpty(selectedPlant) ||
                        string.IsNullOrEmpty(x.Plant) ||
                        x.Plant.Trim().Equals(selectedPlant.Trim(), StringComparison.OrdinalIgnoreCase)
                    )
                )
                .OrderByDescending(x => x.ID)
                .FirstOrDefault();

            if (shiftDetails == null)
            {
                TempData["ErrorMessage"] = "Selected shift details not found. Please select shift again.";
                return RedirectToAction("AddDetails", "RollingMill");
            }

            var vm = new RMHourlyDischargeVM
            {
                Date = shiftDetails.Date,
                Plant = shiftDetails.Plant,
                Shift = shiftDetails.Shift,
                Team = shiftDetails.Team,
                ShiftIncharge = shiftDetails.ShiftIncharge,
                //HourlyDischarge = new List<RMHourlyDischargeBLL>()
            };

            return View("~/Views/RollingMill/RMHourlyDischarge/add.cshtml", vm);
        }


        [HttpPost]
        public ActionResult RMHourlyDischarge(RMHourlyDischargeVM model)
        {
            try
            {
                if (Session["RM_Date"] == null || Session["RM_Shift"] == null || Session["RM_ShiftDetailID"] == null)
                {
                    TempData["ErrorMessage"] = "Please select Rolling Mill Shift first.";
                    return RedirectToAction("AddDetails", "RollingMill");
                }

                DateTime selectedDate = Convert.ToDateTime(Session["RM_Date"]).Date;
                string selectedShift = Convert.ToString(Session["RM_Shift"]);

                if (string.IsNullOrWhiteSpace(selectedShift))
                {
                    TempData["ErrorMessage"] = "Selected shift is missing. Please select shift again.";
                    return RedirectToAction("AddDetails", "RollingMill");
                }

                if (model == null || model.RMHourlyDischarge == null || !model.RMHourlyDischarge.Any())
                {
                    TempData["ErrorMessage"] = "No hourly discharge data found.";
                    return RedirectToAction("RMHourlyDischarge");
                }

                bool isAlreadyExist = rm.IsRMHourlyDischargeExist(selectedDate, selectedShift);

                if (isAlreadyExist)
                {
                    TempData["ErrorMessage"] = "Hourly discharge data already exists for this date and shift.";
                    return RedirectToAction("RMHourlyDischarge");
                }

                foreach (var item in model.RMHourlyDischarge)
                {
                    bool hasData =
                        !string.IsNullOrWhiteSpace(item.NoofBillets) ||
                        item.NoofCobble.HasValue ||
                        item.Reject.HasValue ||
                        !string.IsNullOrWhiteSpace(item.BilletHeatNo);

                    if (!hasData)
                    {
                        continue;
                    }

                    item.Date = selectedDate;
                    item.Shift = selectedShift;

                    item.SafetyIssueShift = model.Form != null ? model.SafetyIssueShift : "";
                    item.MessageShift = model.Form != null ? model.MessageShift : "";

                    item.FuelConsumptionStart = model.Form != null ? model.FuelConsumptionStart : "";
                    item.FuelConsumptionEnd = model.Form != null ? model.FuelConsumptionEnd : "";
                    item.TotalConsumption = model.Form != null ? model.TotalConsumption : "";
                    item.ElectricityConsumption = model.Form != null ? model.ElectricityConsumption : "";

                    item.StatusID = 1;
                    item.CreatedDate = DateTime.Now;
                    item.CreatedBy = User.Identity.Name;

                    rm.InsertRMHourlyDischarge(item);
                }

                TempData["SuccessMessage"] = "Hourly discharge saved successfully.";
                return RedirectToAction("RMHourlyDischarge");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error: " + ex.Message;
                return RedirectToAction("RMHourlyDischarge");
            }
        }
    }
}