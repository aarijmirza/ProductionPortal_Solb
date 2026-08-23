using BAL.Repositories;
using DAL.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ProductionPortal_Solb.Controllers
{
    /// <summary>
    /// Separate controller for the Billet Board input screen.
    /// View path: Views/BilletBoard/Add.cshtml
    /// </summary>
    public class BilletBoardController : Controller
    {
        private readonly BilletBoardRepository repo =
            new BilletBoardRepository();

        [HttpGet]
        public ActionResult Add(int? id)
        {
            BilletBoardBLL model;

            if (id.HasValue && id.Value > 0)
            {
                model = repo.GetForEdit(id.Value);

                if (model == null)
                {
                    TempData["ErrorMessage"] =
                        "Billet Boarding record not found.";

                    return RedirectToAction(
                        "BilletBoard",
                        "Quality"
                    );
                }

                model.Chemistry =
                    repo.GetChemistryForEdit(id.Value)
                    ?? new List<RMChemicalAnalysisBLL>();
            }
            else
            {
                model = new BilletBoardBLL
                {
                    Date = DateTime.Today,
                    Chemistry =
                        new List<RMChemicalAnalysisBLL>
                        {
                            new RMChemicalAnalysisBLL()
                        }
                };
            }

            PrepareViewData();
            return View("AddBillet", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(BilletBoardBLL model)
        {
            if (model == null)
            {
                TempData["ErrorMessage"] = "Invalid Billet Board data.";
                return RedirectToAction("AddBillet");
            }

            NormalizeChemistry(model);
            model.BilletWeight = CalculateBilletWeight(model);

            string validationError = ValidateModel(model);
            if (!string.IsNullOrWhiteSpace(validationError))
            {
                TempData["ErrorMessage"] = validationError;
                PrepareViewData();
                return View("AddBillet", model);
            }

            try
            {
                string userName = GetCurrentUser();

                repo.Save(model, userName);

                TempData["SuccessMessage"] =
                    model.ID > 0
                        ? "Billet Boarding updated successfully."
                        : "Billet Boarding saved successfully.";

                return RedirectToAction(
                    "BilletBoard",
                    "Quality"
                );
            }
            catch (Exception ex)
            {
                // The SQL procedure also validates duplicates, so a direct
                // POST cannot bypass the AJAX SweetAlert validation.
                TempData["ErrorMessage"] =
                    GetUsefulErrorMessage(ex);

                PrepareViewData();
                return View("AddBillet", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult CheckDuplicateHeatNos(
            List<string> heatNos,
            List<int> excludedIDs,
            int currentID = 0)
        {
            try
            {
                List<string> normalized =
                    (heatNos ?? new List<string>())
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Select(x => x.Trim())
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToList();

                List<string> duplicates =
                    repo.GetDuplicateHeatNos(
                        normalized,
                        excludedIDs ?? new List<int>(),
                        currentID
                    );

                return Json(
                    new
                    {
                        success = true,
                        duplicates = duplicates
                    }
                );
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;

                return Json(
                    new
                    {
                        success = false,
                        message = GetUsefulErrorMessage(ex)
                    }
                );
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(int id)
        {
            if (id <= 0)
            {
                return Json(
                    new
                    {
                        success = false,
                        message = "Invalid Billet Boarding ID."
                    }
                );
            }

            try
            {
                int affectedRows =
                    repo.Delete(id, GetCurrentUser());

                return Json(
                    new
                    {
                        success = true,
                        affectedRows = affectedRows,
                        message =
                            "Billet Boarding and Chemical Analysis deleted successfully.",
                        redirectUrl =
                            Url.Action("BilletBoard", "Quality")
                    }
                );
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;

                return Json(
                    new
                    {
                        success = false,
                        message = GetUsefulErrorMessage(ex)
                    }
                );
            }
        }

        private void PrepareViewData()
        {
            ViewBag.GradeDataJson =
                JsonConvert.SerializeObject(
                    repo.GetBilletGrades()
                    ?? new List<BilletGrades>()
                );
        }

        private static void NormalizeChemistry(BilletBoardBLL model)
        {
            model.BilletBoarding =
                (model.BilletBoarding ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(model.SteelGrade))
            {
                model.SteelGrade = model.Grade;
            }

            model.Chemistry =
                (model.Chemistry ??
                    new List<RMChemicalAnalysisBLL>())
                    .Where(
                        x =>
                            x != null &&
                            !string.IsNullOrWhiteSpace(x.HeatNo)
                    )
                    .ToList();

            foreach (RMChemicalAnalysisBLL row in model.Chemistry)
            {
                row.HeatNo = row.HeatNo.Trim();
            }
        }

        private static string ValidateModel(BilletBoardBLL model)
        {
            if (string.IsNullOrWhiteSpace(model.BilletBoarding))
            {
                return "Billet Boarding number is required.";
            }

            if (model.Chemistry == null || !model.Chemistry.Any())
            {
                return "Please enter at least one Heat No.";
            }

            List<string> duplicateHeatNos =
                model.Chemistry
                    .GroupBy(
                        x => x.HeatNo,
                        StringComparer.OrdinalIgnoreCase
                    )
                    .Where(x => x.Count() > 1)
                    .Select(x => x.Key)
                    .ToList();

            if (duplicateHeatNos.Any())
            {
                return
                    "Duplicate Heat No(s) in the form: " +
                    string.Join(", ", duplicateHeatNos);
            }

            return null;
        }

        private static decimal CalculateBilletWeight(
            BilletBoardBLL model)
        {
            decimal billetLength;

            if (
                !decimal.TryParse(
                    model.BilletLength,
                    out billetLength
                )
            )
            {
                return 0m;
            }

            string crossSection =
                (model.CrossSection ?? string.Empty)
                    .Replace(" ", string.Empty)
                    .ToLowerInvariant();

            string[] parts = crossSection.Split('x');

            int width;
            int height;

            if (
                parts.Length == 2 &&
                int.TryParse(parts[0], out width) &&
                int.TryParse(parts[1], out height) &&
                width == 150 &&
                height == 150
            )
            {
                return 175m * billetLength / 1000m;
            }

            return 0m;
        }

        private string GetCurrentUser()
        {
            string userName = Convert.ToString(Session["UserName"]);

            if (string.IsNullOrWhiteSpace(userName))
            {
                userName = Convert.ToString(Session["UserID"]);
            }

            if (
                string.IsNullOrWhiteSpace(userName) &&
                User != null &&
                User.Identity != null
            )
            {
                userName = User.Identity.Name;
            }

            return string.IsNullOrWhiteSpace(userName)
                ? "System"
                : userName.Trim();
        }

        private static string GetUsefulErrorMessage(Exception ex)
        {
            Exception current = ex;

            while (current.InnerException != null)
            {
                current = current.InnerException;
            }

            return string.IsNullOrWhiteSpace(current.Message)
                ? "The operation could not be completed."
                : current.Message;
        }
    }
}