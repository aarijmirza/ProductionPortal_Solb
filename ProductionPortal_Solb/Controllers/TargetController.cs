using BAL.Repositories;
using DAL.Models;
using DAL.Repository;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProductionPortal_Solb.Controllers
{
    public class TargetController : Controller
    {
        private readonly RollingMillTargetsRepository repo;
        private readonly RollingMillDailyTargetRepository dailyTargetRepo;

        public TargetController()
        {
            repo = new RollingMillTargetsRepository();
            dailyTargetRepo = new RollingMillDailyTargetRepository();
        }

        public ActionResult list()
        {
            return View();
        }

        [HttpGet]
        public ActionResult add(
            int? id,
            int? dailyId,
            string mode)
        {
            RollingMillTargetsBLL model;

            if (id.HasValue && id.Value > 0)
            {
                model = repo.GetByID(id.Value);

                if (model == null)
                {
                    model = new RollingMillTargetsBLL
                    {
                        Month = DateTime.Today.ToString("MMMM"),
                        Year = DateTime.Today.Year.ToString(),
                        StatusID = 1
                    };
                }
            }
            else
            {
                model = new RollingMillTargetsBLL
                {
                    Month = DateTime.Today.ToString("MMMM"),
                    Year = DateTime.Today.Year.ToString(),
                    StatusID = 1
                };
            }

            // Monthly target list
            ViewBag.TargetList =
                repo.GetAll() ??
                new List<RollingMillTargetsBLL>();

            // Daily target list
            ViewBag.DailyTargetList =
                dailyTargetRepo.GetAll() ??
                new List<RollingMillDailyTargetBLL>();

            // Daily target edit record
            RollingMillDailyTargetBLL dailyTargetModel;

            if (dailyId.HasValue && dailyId.Value > 0)
            {
                dailyTargetModel =
                    dailyTargetRepo.GetByID(dailyId.Value);
            }
            else
            {
                dailyTargetModel =
                    new RollingMillDailyTargetBLL
                    {
                        TargetDate = DateTime.Today,
                        StatusID = 1
                    };
            }

            ViewBag.DailyTarget = dailyTargetModel;

            // Screen mode
            ViewBag.TargetMode =
                string.Equals(
                    mode,
                    "daily",
                    StringComparison.OrdinalIgnoreCase
                )
                    ? "daily"
                    : "monthly";

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(RollingMillTargetsBLL model)
        {
            ViewBag.TargetList = repo.GetAll();

            if (string.IsNullOrWhiteSpace(model.Month))
            {
                ModelState.AddModelError(
                    "Month",
                    "Month is required."
                );
            }

            if (string.IsNullOrWhiteSpace(model.Year))
            {
                ModelState.AddModelError(
                    "Year",
                    "Year is required."
                );
            }

            if (!ModelState.IsValid)
            {
                return View("add", model);
            }

            try
            {
                if (model.ID > 0)
                {
                    repo.Update(model);

                    TempData["Success"] =
                        "Rolling Mill target updated successfully.";
                }
                else
                {
                    model.StatusID = 1;

                    model.CreatedBy =
                        Convert.ToString(Session["UserName"]);

                    if (string.IsNullOrWhiteSpace(model.CreatedBy))
                    {
                        model.CreatedBy = User.Identity.Name;
                    }

                    repo.Insert(model);

                    TempData["Success"] =
                        "Rolling Mill target saved successfully.";
                }

                return RedirectToAction("add");
            }
            catch (SqlException ex)
            {
                ModelState.AddModelError("", ex.Message);

                ViewBag.TargetList = repo.GetAll();

                return View("add", model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(
                    "",
                    "Unable to save record. " + ex.Message
                );

                ViewBag.TargetList = repo.GetAll();

                return View("add", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            repo.Delete(id);

            TempData["Success"] =
                "Rolling Mill target deleted successfully.";

            return RedirectToAction("add");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveDaily(
            DateTime? date1,
            decimal? dailyProductionTarget1,
            decimal? fuelConsumption1,

            DateTime? date2,
            decimal? dailyProductionTarget2,
            decimal? fuelConsumption2)
        {
            try
            {
                /* ======================================
                   VALIDATION - RM1
                   ====================================== */

                if (!date1.HasValue)
                {
                    TempData["ErrorMessage"] =
                        "RM1 Date is required.";

                    return RedirectToAction(
                        "list"
                    );
                }


                if (!dailyProductionTarget1.HasValue)
                {
                    TempData["ErrorMessage"] =
                        "RM1 Daily Production Target is required.";

                    return RedirectToAction(
                        "list"
                    );
                }


                if (!fuelConsumption1.HasValue)
                {
                    TempData["ErrorMessage"] =
                        "RM1 Fuel Consumption is required.";

                    return RedirectToAction(
                        "list"
                    );
                }


                /* ======================================
                   VALIDATION - RM2
                   ====================================== */

                if (!date2.HasValue)
                {
                    TempData["ErrorMessage"] =
                        "RM2 Date is required.";

                    return RedirectToAction(
                        "list"
                    );
                }


                if (!dailyProductionTarget2.HasValue)
                {
                    TempData["ErrorMessage"] =
                        "RM2 Daily Production Target is required.";

                    return RedirectToAction(
                        "list"
                    );
                }


                if (!fuelConsumption2.HasValue)
                {
                    TempData["ErrorMessage"] =
                        "RM2 Fuel Consumption is required.";

                    return RedirectToAction(
                        "list"
                    );
                }


                /* ======================================
                   RM1
                   ====================================== */

                RollingMillDailyTargetBLL rm1 =
                    new RollingMillDailyTargetBLL
                    {
                        TargetDate =
                            date1.Value.Date,

                        Plant =
                            "RM1",

                        DailyProductionTarget =
                            dailyProductionTarget1,

                        FuelConsumption =
                            fuelConsumption1,

                        StatusID =
                            1,

                        CreatedBy =
                            User.Identity.Name,

                        CreatedDate =
                            DateTime.Now
                    };


                /* ======================================
                   RM2
                   ====================================== */

                RollingMillDailyTargetBLL rm2 =
                    new RollingMillDailyTargetBLL
                    {
                        TargetDate =
                            date2.Value.Date,

                        Plant =
                            "RM2",

                        DailyProductionTarget =
                            dailyProductionTarget2,

                        FuelConsumption =
                            fuelConsumption2,

                        StatusID =
                            1,

                        CreatedBy =
                            User.Identity.Name,

                        CreatedDate =
                            DateTime.Now
                    };


                /* ======================================
                   SAVE
                   ====================================== */

                int rtnRM1 =
                    dailyTargetRepo.SaveDailyTarget(
                        rm1
                    );


                int rtnRM2 =
                    dailyTargetRepo.SaveDailyTarget(
                        rm2
                    );


                if (
                    rtnRM1 > 0 &&
                    rtnRM2 > 0
                )
                {
                    TempData["SuccessMessage"] =
                        "RM1 and RM2 Daily Targets saved successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] =
                        "Daily Targets could not be saved completely.";
                }


                return RedirectToAction(
                    "list"
                );
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] =
                    "Error while saving Daily Target: "
                    +
                    ex.Message;


                return RedirectToAction(
                    "list"
                );
            }
        }

        [HttpGet]
        public ActionResult EditDaily(int id)
        {
            var model = dailyTargetRepo.GetByID(id);

            if (model == null)
            {
                TempData["Error"] =
                    "Daily target record not found.";

                return RedirectToAction("add");
            }

            ViewBag.DailyTarget = model;

            return RedirectToAction(
                "add",
                new
                {
                    dailyId = id,
                    mode = "daily"
                }
            );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteDaily(int id)
        {
            string currentUser =
                Convert.ToString(
                    Session["UserName"] ??
                    User.Identity.Name
                );

            bool deleted =
                dailyTargetRepo.Delete(
                    id,
                    currentUser
                );

            TempData[
                deleted
                    ? "Success"
                    : "Error"
            ] =
                deleted
                    ? "Daily target deleted successfully."
                    : "Daily target could not be deleted.";

            return RedirectToAction(
                "add",
                new { mode = "daily" }
            );
        }
    }
}