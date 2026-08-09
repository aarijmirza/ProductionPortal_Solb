using BAL.Repositories;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ProductionPortal_Solb.Controllers
{
    public class ConsumptionController : Controller
    {
        private readonly ConsumptionRepository repo;

        public ConsumptionController()
        {
            repo =
                new ConsumptionRepository();
        }


        // =====================================================
        // LIST
        // =====================================================

        [HttpGet]
        public ActionResult list(
            DateTime? fromDate,
            DateTime? toDate)
        {
            try
            {
                DateTime startDate =
                    fromDate ?? DateTime.Today;

                DateTime endDate =
                    toDate ?? DateTime.Today;


                if (
                    startDate.Date >
                    endDate.Date
                )
                {
                    DateTime temp =
                        startDate;

                    startDate =
                        endDate;

                    endDate =
                        temp;
                }


                ViewBag.FromDate =
                    startDate.ToString(
                        "yyyy-MM-dd"
                    );

                ViewBag.ToDate =
                    endDate.ToString(
                        "yyyy-MM-dd"
                    );


                List<PlantConsumptionBLL> records =
                    repo.GetPlantConsumption(
                        startDate.Date,
                        endDate.Date
                    );


                return View(
                    records
                );
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] =
                    "Error loading consumption: "
                    + ex.Message;

                return View(
                    new List<PlantConsumptionBLL>()
                );
            }
        }


        // =====================================================
        // ADD / EDIT GET
        // =====================================================

        [HttpGet]
        public ActionResult add(
            int? ID,
            DateTime? date)
        {
            try
            {
                DateTime selectedDate =
                    date ?? DateTime.Today;

                List<PlantConsumptionBLL> records =
                    new List<PlantConsumptionBLL>();


                /*
                 * EDIT MODE
                 */
                if (
                    ID.HasValue &&
                    ID.Value > 0
                )
                {
                    records =
                        repo.GetPlantConsumptionGroupByID(
                            ID.Value
                        );

                    if (
                        records == null ||
                        records.Count == 0
                    )
                    {
                        TempData["ErrorMessage"] =
                            "Consumption record not found.";

                        return RedirectToAction(
                            "list"
                        );
                    }


                    PlantConsumptionBLL firstRecord =
                        records.FirstOrDefault();

                    if (
                        firstRecord != null &&
                        firstRecord.Date.HasValue
                    )
                    {
                        selectedDate =
                            firstRecord.Date.Value.Date;
                    }
                }
                else
                {
                    /*
                     * ADD MODE
                     *
                     * Date ke against already records
                     * hain to load kar do.
                     */
                    records =
                        repo.GetPlantConsumptionByDateAll(
                            selectedDate
                        );

                    if (records == null)
                    {
                        records =
                            new List<PlantConsumptionBLL>();
                    }
                }


                // =============================================
                // SMP
                // =============================================

                PlantConsumptionBLL smp =
                    records.FirstOrDefault(
                        x =>
                            string.Equals(
                                x.Plant,
                                "SMP",
                                StringComparison.OrdinalIgnoreCase
                            )
                    );

                if (smp == null)
                {
                    smp =
                        new PlantConsumptionBLL
                        {
                            Plant = "SMP",
                            Date = selectedDate,
                            StatusID = 1
                        };
                }


                // =============================================
                // RM1
                // =============================================

                PlantConsumptionBLL rm1 =
                    records.FirstOrDefault(
                        x =>
                            string.Equals(
                                x.Plant,
                                "RM1",
                                StringComparison.OrdinalIgnoreCase
                            )
                    );

                if (rm1 == null)
                {
                    rm1 =
                        new PlantConsumptionBLL
                        {
                            Plant = "RM1",
                            Date = selectedDate,
                            StatusID = 1
                        };
                }


                // =============================================
                // RM2
                // =============================================

                PlantConsumptionBLL rm2 =
                    records.FirstOrDefault(
                        x =>
                            string.Equals(
                                x.Plant,
                                "RM2",
                                StringComparison.OrdinalIgnoreCase
                            )
                    );

                if (rm2 == null)
                {
                    rm2 =
                        new PlantConsumptionBLL
                        {
                            Plant = "RM2",
                            Date = selectedDate,
                            StatusID = 1
                        };
                }


                ViewBag.SMP =
                    smp;

                ViewBag.RM1 =
                    rm1;

                ViewBag.RM2 =
                    rm2;

                ViewBag.ConsumptionDate =
                    selectedDate.ToString(
                        "yyyy-MM-dd"
                    );


                ViewBag.IsEditMode =
                    smp.ID > 0 ||
                    rm1.ID > 0 ||
                    rm2.ID > 0;


                /*
                 * Existing strongly typed View ko
                 * maintain karne ke liye.
                 */
                PlantConsumptionBLL model =
                    records.FirstOrDefault()
                    ?? smp;


                return View(
                    model
                );
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] =
                    "Error loading record: "
                    + ex.Message;

                return RedirectToAction(
                    "list"
                );
            }
        }


        // =====================================================
        // ADD / EDIT POST
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult add(
            FormCollection form)
        {
            try
            {
                DateTime consumptionDate;


                if (
                    !DateTime.TryParse(
                        form["ConsumptionDate"],
                        out consumptionDate
                    )
                )
                {
                    TempData["ErrorMessage"] =
                        "Please select a valid date.";

                    return RedirectToAction(
                        "list"
                    );
                }


                string currentUser =
                    User.Identity.Name;

                DateTime now =
                    DateTime.Now;


                int smpID =
                    ToInt(
                        form["SMP_ID"]
                    );

                int rm1ID =
                    ToInt(
                        form["RM1_ID"]
                    );

                int rm2ID =
                    ToInt(
                        form["RM2_ID"]
                    );


                List<PlantConsumptionBLL> records =
                    new List<PlantConsumptionBLL>();


                // =============================================
                // SMP
                // =============================================

                PlantConsumptionBLL smp =
                    new PlantConsumptionBLL
                    {
                        ID =
                            smpID,

                        Date =
                            consumptionDate.Date,

                        Plant =
                            "SMP",

                        TotalProductBillet =
                            form[
                                "SMP_TotalProductBillet"
                            ],

                        LPG =
                            form["SMP_LPG"],

                        Oxygen =
                            form["SMP_Oxygen"],

                        Nitrogen =
                            form["SMP_Nitrogen"],

                        Argon =
                            form["SMP_Argon"],

                        WaterConsumption =
                            form[
                                "SMP_WaterConsumption"
                            ],

                        PowerConsumption =
                            form[
                                "SMP_PowerConsumption"
                            ],

                        FuelConsumption =
                            null,

                        StatusID = 1
                    };


                CalculateConsumptionValues(
                    smp
                );

                SetAudit(
                    smp,
                    currentUser,
                    now
                );

                records.Add(
                    smp
                );


                // =============================================
                // RM1
                // =============================================

                PlantConsumptionBLL rm1 =
                    new PlantConsumptionBLL
                    {
                        ID =
                            rm1ID,

                        Date =
                            consumptionDate.Date,

                        Plant =
                            "RM1",

                        TotalProductBillet =
                            form[
                                "RM1_TotalProductBillet"
                            ],

                        WaterConsumption =
                            form[
                                "RM1_WaterConsumption"
                            ],

                        PowerConsumption =
                            form[
                                "RM1_PowerConsumption"
                            ],

                        FuelConsumption =
                            ToNullableDecimal(
                                form[
                                    "RM1_FuelConsumption"
                                ]
                            ),

                        StatusID = 1
                    };


                CalculateConsumptionValues(
                    rm1
                );

                SetAudit(
                    rm1,
                    currentUser,
                    now
                );

                records.Add(
                    rm1
                );


                // =============================================
                // RM2
                // =============================================

                PlantConsumptionBLL rm2 =
                    new PlantConsumptionBLL
                    {
                        ID =
                            rm2ID,

                        Date =
                            consumptionDate.Date,

                        Plant =
                            "RM2",

                        TotalProductBillet =
                            form[
                                "RM2_TotalProductBillet"
                            ],

                        WaterConsumption =
                            form[
                                "RM2_WaterConsumption"
                            ],

                        PowerConsumption =
                            form[
                                "RM2_PowerConsumption"
                            ],

                        FuelConsumption =
                            ToNullableDecimal(
                                form[
                                    "RM2_FuelConsumption"
                                ]
                            ),

                        StatusID = 1
                    };


                CalculateConsumptionValues(
                    rm2
                );

                SetAudit(
                    rm2,
                    currentUser,
                    now
                );

                records.Add(
                    rm2
                );


                // =============================================
                // SAVE
                // =============================================

                int savedID =
                    repo.SavePlantWiseConsumption(
                        records
                    );


                if (savedID == 0)
                {
                    TempData["ErrorMessage"] =
                        "Plant wise consumption could not be saved.";

                    return RedirectToAction(
                        "list",
                        new
                        {
                            date =
                                consumptionDate
                                    .ToString(
                                        "yyyy-MM-dd"
                                    )
                        }
                    );
                }


                bool isEdit =
                    smpID > 0 ||
                    rm1ID > 0 ||
                    rm2ID > 0;


                TempData["SuccessMessage"] =
                    isEdit
                        ? "Plant wise consumption updated successfully."
                        : "Plant wise consumption saved successfully.";


                return RedirectToAction(
                    "list",
                    new
                    {
                        ID = savedID
                    }
                );
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] =
                    "Error: "
                    + ex.Message;

                return RedirectToAction(
                    "list"
                );
            }
        }


        // =====================================================
        // DELETE
        // =====================================================

        [HttpGet]
        public ActionResult delete(
            int ID)
        {
            try
            {
                if (ID <= 0)
                {
                    TempData["ErrorMessage"] =
                        "Invalid consumption record.";

                    return RedirectToAction(
                        "list"
                    );
                }


                int result =
                    repo.DeletePlantConsumption(
                        ID,
                        User.Identity.Name
                    );


                if (result > 0)
                {
                    TempData["SuccessMessage"] =
                        "Consumption record deleted successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] =
                        "Consumption record could not be deleted.";
                }


                return RedirectToAction(
                    "list"
                );
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] =
                    "Delete failed: "
                    + ex.Message;

                return RedirectToAction(
                    "list"
                );
            }
        }


        // =====================================================
        // HELPERS
        // =====================================================

        private void SetAudit(
            PlantConsumptionBLL model,
            string userName,
            DateTime now)
        {
            if (model.ID > 0)
            {
                model.UpdatedBy =
                    userName;

                model.UpdatedDate =
                    now;
            }
            else
            {
                model.CreatedBy =
                    userName;

                model.CreatedDate =
                    now;
            }
        }


        private void CalculateConsumptionValues(
            PlantConsumptionBLL model)
        {
            decimal production =
                ToDecimalValue(
                    model.TotalProductBillet
                );


            model.LPGm3ton =
                CalculatePerTon(
                    model.LPG,
                    production
                );

            model.OxygenNm3ton =
                CalculatePerTon(
                    model.Oxygen,
                    production
                );

            model.NitrogenNm3ton =
                CalculatePerTon(
                    model.Nitrogen,
                    production
                );

            model.ArgonNm3ton =
                CalculatePerTon(
                    model.Argon,
                    production
                );

            model.PowerConsumptionKWHton =
                CalculatePerTon(
                    model.PowerConsumption,
                    production
                );

            model.WaterConsumptionM3 =
                CalculatePerTon(
                    model.WaterConsumption,
                    production
                );
        }


        private string CalculatePerTon(
            string consumptionValue,
            decimal production)
        {
            decimal consumption =
                ToDecimalValue(
                    consumptionValue
                );

            if (
                production <= 0 ||
                consumption <= 0
            )
            {
                return null;
            }


            decimal result =
                consumption /
                production;


            return result.ToString(
                "0.000"
            );
        }


        private decimal ToDecimalValue(
            string value)
        {
            decimal result;

            if (
                string.IsNullOrWhiteSpace(
                    value
                )
            )
            {
                return 0;
            }


            return decimal.TryParse(
                value.Trim(),
                out result
            )
                ? result
                : 0;
        }


        private decimal? ToNullableDecimal(
            string value)
        {
            decimal result;

            if (
                string.IsNullOrWhiteSpace(
                    value
                )
            )
            {
                return null;
            }


            return decimal.TryParse(
                value.Trim(),
                out result
            )
                ? result
                : (decimal?)null;
        }


        private int ToInt(
            string value)
        {
            int result;

            return int.TryParse(
                value,
                out result
            )
                ? result
                : 0;
        }
    }
}