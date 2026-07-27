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
            DateTime date,
            decimal dailyProductionTarget,
            decimal fuelConsumption)
        {
            try
            {
                if (date == DateTime.MinValue)
                {
                    TempData["Error"] =
                        "Please select target date.";

                    return RedirectToAction("add");
                }

                if (dailyProductionTarget < 0)
                {
                    TempData["Error"] =
                        "Daily production target cannot be negative.";

                    return RedirectToAction("add");
                }

                if (fuelConsumption < 0)
                {
                    TempData["Error"] =
                        "Fuel consumption cannot be negative.";

                    return RedirectToAction("add");
                }

                string currentUser =
                    Convert.ToString(
                        Session["UserName"] ??
                        User.Identity.Name
                    );

                var model =
                    new RollingMillDailyTargetBLL
                    {
                        TargetDate = date.Date,
                        DailyProductionTarget =
                            dailyProductionTarget,
                        FuelConsumption =
                            fuelConsumption,
                        StatusID = 1,
                        CreatedBy = currentUser,
                        CreatedDate = DateTime.Now
                    };

                int id = dailyTargetRepo.Save(model);

                if (id > 0)
                {
                    TempData["Success"] =
                        "Daily target saved successfully.";
                }
                else
                {
                    TempData["Error"] =
                        "Daily target could not be saved.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("add");
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