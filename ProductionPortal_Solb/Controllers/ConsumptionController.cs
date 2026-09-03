using BAL.Repositories;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using static DAL.Models.ViewModel;

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


                /*
                 * Production is always loaded from production source tables:
                 *
                 * SMP  = SMPDayWiseProduction.TotalCastedTon by selected date
                 * RM1  = BundlesSection.TotalWeight by selected date + RM1
                 * RM2  = BundlesSection.TotalWeight by selected date + RM2
                 *
                 * Do not depend on previously-saved PlantConsumption
                 * production values.
                 */
                ApplyProductionByDate(
                    selectedDate.Date,
                    smp,
                    rm1,
                    rm2
                );


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

                        /*
                         * Production will be loaded from
                         * SMPDayWiseProduction.TotalCastedTon below.
                         */
                        TotalProductBillet =
                            null,

                        LPG =
                            ToNullableDecimal(
                                form["SMP_LPG"]
                            ),

                        Oxygen =
                            ToNullableDecimal(
                                form["SMP_Oxygen"]
                            ),

                        Nitrogen =
                            ToNullableDecimal(
                                form["SMP_Nitrogen"]
                            ),

                        Argon =
                            ToNullableDecimal(
                                form["SMP_Argon"]
                            ),

                        WaterConsumption =
                            ToNullableDecimal(
                                form[
                                    "SMP_WaterConsumption"
                                ]
                            ),

                        PowerConsumption =
                            ToNullableDecimal(
                                form[
                                    "SMP_PowerConsumption"
                                ]
                            ),

                        FuelConsumption =
                            null,

                        StatusID = 1
                    };


                SetAudit(
                    smp,
                    currentUser,
                    now
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

                        /*
                         * Production will be loaded from BundlesSection below.
                         */
                        TotalProductBillet =
                            null,

                        WaterConsumption =
                            ToNullableDecimal(
                                form[
                                    "RM1_WaterConsumption"
                                ]
                            ),

                        PowerConsumption =
                            ToNullableDecimal(
                                form[
                                    "RM1_PowerConsumption"
                                ]
                            ),

                        FuelConsumption =
                            ToNullableDecimal(
                                form[
                                    "RM1_FuelConsumption"
                                ]
                            ),

                        StatusID = 1
                    };


                SetAudit(
                    rm1,
                    currentUser,
                    now
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

                        /*
                         * Production will be loaded from BundlesSection below.
                         */
                        TotalProductBillet =
                            null,

                        WaterConsumption =
                            ToNullableDecimal(
                                form[
                                    "RM2_WaterConsumption"
                                ]
                            ),

                        PowerConsumption =
                            ToNullableDecimal(
                                form[
                                    "RM2_PowerConsumption"
                                ]
                            ),

                        FuelConsumption =
                            ToNullableDecimal(
                                form[
                                    "RM2_FuelConsumption"
                                ]
                            ),

                        StatusID = 1
                    };


                SetAudit(
                    rm2,
                    currentUser,
                    now
                );


                /*
                 * IMPORTANT:
                 * Always take production from live production tables
                 * using the selected Consumption Date.
                 *
                 * SMP  -> SMPDayWiseProduction.TotalCastedTon
                 * RM1  -> BundlesSection.TotalWeight (RM1)
                 * RM2  -> BundlesSection.TotalWeight (RM2)
                 */
                ApplyProductionByDate(
                    consumptionDate.Date,
                    smp,
                    rm1,
                    rm2
                );


                /*
                 * Recalculate all per-ton values after assigning
                 * the correct production.
                 */
                CalculateConsumptionValues(
                    smp
                );

                CalculateConsumptionValues(
                    rm1
                );

                CalculateConsumptionValues(
                    rm2
                );


                records.Add(
                    smp
                );

                records.Add(
                    rm1
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


                if (savedID != 0)
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


                if (result != 0)
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
        // GET PRODUCTION BY DATE - AJAX
        // =====================================================

        [HttpGet]
        public JsonResult GetProductionByDate(
            DateTime? date)
        {
            try
            {
                if (!date.HasValue)
                {
                    return Json(
                        new
                        {
                            success = false,
                            message = "Date is required."
                        },
                        JsonRequestBehavior.AllowGet
                    );
                }


                DateTime selectedDate =
                    date.Value.Date;


                /*
                 * Production:
                 * SMP -> SMPDayWiseProduction.TotalCastedTon
                 * RM1/RM2 -> BundlesSection
                 */
                PlantConsumptionBLL smpProductionModel =
                    new PlantConsumptionBLL
                    {
                        Plant = "SMP"
                    };

                PlantConsumptionBLL rm1ProductionModel =
                    new PlantConsumptionBLL
                    {
                        Plant = "RM1"
                    };

                PlantConsumptionBLL rm2ProductionModel =
                    new PlantConsumptionBLL
                    {
                        Plant = "RM2"
                    };


                ApplyProductionByDate(
                    selectedDate,
                    smpProductionModel,
                    rm1ProductionModel,
                    rm2ProductionModel
                );


                /*
                 * Existing Utility Consumption:
                 * If selected date already exists in PlantConsumption,
                 * return saved SMP / RM1 / RM2 values as well.
                 */
                List<PlantConsumptionBLL> existingRecords =
                    repo.GetPlantConsumptionByDateAll(
                        selectedDate
                    )
                    ?? new List<PlantConsumptionBLL>();


                PlantConsumptionBLL smp =
                    existingRecords.FirstOrDefault(
                        x =>
                            string.Equals(
                                x.Plant,
                                "SMP",
                                StringComparison.OrdinalIgnoreCase
                            )
                    )
                    ?? new PlantConsumptionBLL
                    {
                        Plant = "SMP"
                    };


                PlantConsumptionBLL rm1 =
                    existingRecords.FirstOrDefault(
                        x =>
                            string.Equals(
                                x.Plant,
                                "RM1",
                                StringComparison.OrdinalIgnoreCase
                            )
                    )
                    ?? new PlantConsumptionBLL
                    {
                        Plant = "RM1"
                    };


                PlantConsumptionBLL rm2 =
                    existingRecords.FirstOrDefault(
                        x =>
                            string.Equals(
                                x.Plant,
                                "RM2",
                                StringComparison.OrdinalIgnoreCase
                            )
                    )
                    ?? new PlantConsumptionBLL
                    {
                        Plant = "RM2"
                    };


                return Json(
                    new
                    {
                        success = true,

                        smpProduction =
                            smpProductionModel.TotalProductBillet ?? 0m,

                        rm1Production =
                            rm1ProductionModel.TotalProductBillet ?? 0m,

                        rm2Production =
                            rm2ProductionModel.TotalProductBillet ?? 0m,


                        smp = new
                        {
                            id = smp.ID,
                            lpg = smp.LPG,
                            oxygen = smp.Oxygen,
                            nitrogen = smp.Nitrogen,
                            argon = smp.Argon,
                            waterConsumption = smp.WaterConsumption,
                            powerConsumption = smp.PowerConsumption
                        },


                        rm1 = new
                        {
                            id = rm1.ID,
                            waterConsumption = rm1.WaterConsumption,
                            fuelConsumption = rm1.FuelConsumption,
                            powerConsumption = rm1.PowerConsumption
                        },


                        rm2 = new
                        {
                            id = rm2.ID,
                            waterConsumption = rm2.WaterConsumption,
                            fuelConsumption = rm2.FuelConsumption,
                            powerConsumption = rm2.PowerConsumption
                        }
                    },
                    JsonRequestBehavior.AllowGet
                );
            }
            catch (Exception ex)
            {
                return Json(
                    new
                    {
                        success = false,
                        message = ex.Message
                    },
                    JsonRequestBehavior.AllowGet
                );
            }
        }


        // =====================================================
        // APPLY PRODUCTION FROM SOURCE TABLES
        // =====================================================

        private void ApplyProductionByDate(
            DateTime date,
            PlantConsumptionBLL smp,
            PlantConsumptionBLL rm1,
            PlantConsumptionBLL rm2)
        {
            decimal smpProduction =
                repo.GetSMPProductionByDate(
                    date.Date
                );


            /*
             * Existing procedure remains the source for
             * RM1 and RM2 production only.
             */
            DataTable dt =
                repo.GetProductionByDate(
                    date.Date
                );

            decimal rm1Production =
                0m;

            decimal rm2Production =
                0m;


            if (
                dt != null &&
                dt.Rows.Count > 0
            )
            {
                DataRow row =
                    dt.Rows[0];


                if (
                    row.Table.Columns.Contains(
                        "RM1Production"
                    ) &&
                    row["RM1Production"] != DBNull.Value
                )
                {
                    rm1Production =
                        Convert.ToDecimal(
                            row["RM1Production"]
                        );
                }


                if (
                    row.Table.Columns.Contains(
                        "RM2Production"
                    ) &&
                    row["RM2Production"] != DBNull.Value
                )
                {
                    rm2Production =
                        Convert.ToDecimal(
                            row["RM2Production"]
                        );
                }
            }


            if (smp != null)
            {
                smp.TotalProductBillet =
                    Math.Round(
                        smpProduction,
                        3
                    );
            }


            if (rm1 != null)
            {
                rm1.TotalProductBillet =
                    Math.Round(
                        rm1Production,
                        3
                    );
            }


            if (rm2 != null)
            {
                rm2.TotalProductBillet =
                    Math.Round(
                        rm2Production,
                        3
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
                model.TotalProductBillet.HasValue
                    ? model.TotalProductBillet.Value
                    : 0m;


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


        private decimal? CalculatePerTon(
            decimal? consumptionValue,
            decimal production)
        {
            if (
                production <= 0 ||
                !consumptionValue.HasValue ||
                consumptionValue.Value <= 0
            )
            {
                return null;
            }


            decimal result =
                consumptionValue.Value /
                production;

            return Math.Round(
                result,
                3
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

        [HttpGet]
        public ActionResult UtilityDailyReport(DateTime? date)
        {
            DateTime reportDate = date ?? DateTime.Today;

            UtilityDailyReportVM model =
                repo.GetUtilityDailyReport(reportDate.Date);

            return View(
                "~/Views/Reporting/UtilityDailyReport.cshtml",
                model
            );
        }
    }
}