using BAL.Repositories;
using DAL.Models;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProductionPortal_Solb.Controllers
{
    public class ConsumptionController : Controller
    {
        ConsumptionRepository repo;

        public ConsumptionController()
        {
            repo = new ConsumptionRepository();
        }
        // GET: Consumption
        public ActionResult list()
        {
            return View();
        }

        public ActionResult add(DateTime? date)
        {
            DateTime selectedDate = date ?? DateTime.Today;

            var model = repo.GetPlantConsumptionByDate(selectedDate);

            // Agar DB se null aaye, to new entry model create karo
            if (model == null)
            {
                model = new PlantConsumptionBLL
                {
                    Date = selectedDate,
                    TotalProductBillet = "0",
                    LPG = "0",
                    Oxygen = "0",
                    Nitrogen = "0",
                    Argon = "0",
                    WaterConsumption = "0",
                    PowerConsumption = "0",

                    LPGm3ton = "0",
                    OxygenNm3ton = "0",
                    NitrogenNm3ton = "0",
                    ArgonNm3ton = "0",
                    PowerConsumptionKWHton = "0",
                    WaterConsumptionM3 = "0",

                    LPGNm3tonTarget = "0",
                    OxygenNm3tonTarget = "0",
                    NitrogenNm3tonTarget = "0",
                    ArgonNm3tonTarget = "0",
                    PowerConsumptionKWHtarget = "0",
                    WaterConsumptionTarget = "0",

                    StatusID = 1
                };
            }

            return View(model);
        }

        //[HttpPost]
        //public ActionResult add(PlantConsumptionBLL data)
        //{
        //    try
        //    {
        //        if (data == null)
        //        {
        //            TempData["ErrorMessage"] = "Invalid data.";
        //            return RedirectToAction("add");
        //        }

        //        if (data.Date == null)
        //        {
        //            data.Date = DateTime.Today;
        //        }

        //        int result = 0;

        //        // Existing record check by date
        //        var existing = repo.GetPlantConsumptionByDate(data.Date.Value);

        //        if (existing == null || existing.ID <= 0)
        //        {
        //            // New Entry
        //            data.StatusID = 1;
        //            data.CreatedBy = User.Identity.Name;
        //            data.CreatedDate = DateTime.Now;

        //            data.LPGNm3tonTarget = "10";
        //            data.OxygenNm3tonTarget = "10";
        //            data.NitrogenNm3tonTarget = "10";
        //            data.ArgonNm3tonTarget = "10";
        //            data.PowerConsumptionKWHtarget = "10";
        //            data.WaterConsumptionTarget = "10";


        //            result = repo.InsertPlantConsumption(data);

        //            TempData["SuccessMessage"] = result > 0
        //                ? "Plant Consumption record saved successfully."
        //                : "Record could not be saved.";
        //        }
        //        else
        //        {
        //            // Update only targets
        //            data.ID = existing.ID;
        //            data.UpdatedBy = User.Identity.Name;
        //            data.UpdatedDate = DateTime.Now;

        //            //result = repo.UpdatePlantConsumptionTargets(data);

        //            TempData["SuccessMessage"] = result > 0
        //                ? "Plant Consumption targets updated successfully."
        //                : "Record could not be updated.";
        //        }

        //        return RedirectToAction("add", new { date = data.Date.Value.ToString("yyyy-MM-dd") });
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["ErrorMessage"] = "Error: " + ex.Message;
        //        return RedirectToAction("add");
        //    }
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult add(FormCollection form)
        {
            try
            {
                DateTime consumptionDate;

                if (!DateTime.TryParse(form["ConsumptionDate"], out consumptionDate))
                {
                    TempData["ErrorMessage"] = "Please select a valid date.";
                    return RedirectToAction("add");
                }

                string createdBy = User.Identity.Name;
                DateTime createdDate = DateTime.Now;

                var records = new List<PlantConsumptionBLL>();

                // SMP Row
                var smp = new PlantConsumptionBLL
                {
                    Date = consumptionDate,
                    Plant = "SMP",

                    TotalProductBillet = form["SMP_TotalProductBillet"],
                    LPG = form["SMP_LPG"],
                    Oxygen = form["SMP_Oxygen"],
                    Nitrogen = form["SMP_Nitrogen"],
                    Argon = form["SMP_Argon"],
                    WaterConsumption = form["SMP_WaterConsumption"],
                    PowerConsumption = form["SMP_PowerConsumption"],

                    FuelConsumption = null,

                    StatusID = 1,
                    CreatedBy = createdBy,
                    CreatedDate = createdDate
                };

                CalculateConsumptionValues(smp);
                records.Add(smp);

                // RM1 Row
                var rm1 = new PlantConsumptionBLL
                {
                    Date = consumptionDate,
                    Plant = "RM1",

                    TotalProductBillet = form["RM1_TotalProductBillet"],
                    WaterConsumption = form["RM1_WaterConsumption"],
                    PowerConsumption = form["RM1_PowerConsumption"],
                    FuelConsumption = ToNullableDecimal(form["RM1_FuelConsumption"]),

                    StatusID = 1,
                    CreatedBy = createdBy,
                    CreatedDate = createdDate
                };

                CalculateConsumptionValues(rm1);
                records.Add(rm1);

                // RM2 Row
                var rm2 = new PlantConsumptionBLL
                {
                    Date = consumptionDate,
                    Plant = "RM2",

                    TotalProductBillet = form["RM2_TotalProductBillet"],
                    WaterConsumption = form["RM2_WaterConsumption"],
                    PowerConsumption = form["RM2_PowerConsumption"],
                    FuelConsumption = ToNullableDecimal(form["RM2_FuelConsumption"]),

                    StatusID = 1,
                    CreatedBy = createdBy,
                    CreatedDate = createdDate
                };

                CalculateConsumptionValues(rm2);
                records.Add(rm2);

                int result = repo.InsertPlantWiseConsumption(records);

                if (result < 0)
                {
                    TempData["SuccessMessage"] = "Plant wise consumption saved successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Plant wise consumption not saved.";
                }

                return RedirectToAction("add");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error: " + ex.Message;
                return RedirectToAction("add");
            }
        }

        private decimal? ToDecimal(string value)
        {
            decimal result;

            if (string.IsNullOrWhiteSpace(value))
                return null;

            if (decimal.TryParse(value, out result))
                return result;

            return null;
        }

        private void CalculateConsumptionValues(PlantConsumptionBLL model)
        {
            decimal production = ToDecimalValue(model.TotalProductBillet);

            model.LPGm3ton = CalculatePerTon(model.LPG, production);
            model.OxygenNm3ton = CalculatePerTon(model.Oxygen, production);
            model.NitrogenNm3ton = CalculatePerTon(model.Nitrogen, production);
            model.ArgonNm3ton = CalculatePerTon(model.Argon, production);
            model.PowerConsumptionKWHton = CalculatePerTon(model.PowerConsumption, production);
            model.WaterConsumptionM3 = CalculatePerTon(model.WaterConsumption, production);
        }

        private string CalculatePerTon(string consumptionValue, decimal production)
        {
            decimal consumption = ToDecimalValue(consumptionValue);

            if (production <= 0 || consumption <= 0)
                return null;

            decimal result = consumption / production;

            return result.ToString("0.000");
        }

        private decimal ToDecimalValue(string value)
        {
            decimal result;

            if (string.IsNullOrWhiteSpace(value))
                return 0;

            value = value.Trim();

            if (decimal.TryParse(value, out result))
                return result;

            return 0;
        }

        private decimal? ToNullableDecimal(string value)
        {
            decimal result;

            if (string.IsNullOrWhiteSpace(value))
                return null;

            value = value.Trim();

            if (decimal.TryParse(value, out result))
                return result;

            return null;
        }
    }
}