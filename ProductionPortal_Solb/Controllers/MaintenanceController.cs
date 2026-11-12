using BAL.Repositories;
using ClosedXML.Excel;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using System.Xml.Linq;
using SaveOptions = System.Xml.Linq.SaveOptions;

namespace ProductionPortal_Solb.Controllers
{
    [SessionState(SessionStateBehavior.ReadOnly)]
    public class MaintenanceController : Controller
    {
        private readonly DelayRespository repo;

        public MaintenanceController()
        {
            repo = new DelayRespository();
        }

        // =========================================================
        // LIST
        // =========================================================

        public ActionResult list(
            DateTime? steelFromDate,
            DateTime? steelToDate,
            string[] steelPlant,
            string[] steelAgency,
            string[] steelDelayType,
            DateTime? rollingFromDate,
            DateTime? rollingToDate,
            string[] rollingPlant,
            string[] rollingAgency,
            string[] rollingDelayType,
            string tab = null)
        {
            DateTime today =
                DateTime.Today;

            DateTime steelStartDate =
                steelFromDate ?? today;

            DateTime steelEndDate =
                steelToDate ?? steelStartDate;

            DateTime rollingStartDate =
                rollingFromDate ?? today;

            DateTime rollingEndDate =
                rollingToDate ?? rollingStartDate;


            if (
                steelStartDate.Date >
                steelEndDate.Date
            )
            {
                DateTime temp =
                    steelStartDate;

                steelStartDate =
                    steelEndDate;

                steelEndDate =
                    temp;
            }


            if (
                rollingStartDate.Date >
                rollingEndDate.Date
            )
            {
                DateTime temp =
                    rollingStartDate;

                rollingStartDate =
                    rollingEndDate;

                rollingEndDate =
                    temp;
            }


            string activeTab =
                string.Equals(
                    tab,
                    "rolling",
                    StringComparison.OrdinalIgnoreCase
                )
                    ? "rolling"
                    : "steel";


            string steelPlantCsv =
                JoinFilterValues(
                    steelPlant
                );

            string steelAgencyCsv =
                JoinFilterValues(
                    steelAgency
                );

            string steelDelayTypeCsv =
                JoinFilterValues(
                    steelDelayType
                );


            string rollingPlantCsv =
                JoinFilterValues(
                    rollingPlant
                );

            string rollingAgencyCsv =
                JoinFilterValues(
                    rollingAgency
                );

            string rollingDelayTypeCsv =
                JoinFilterValues(
                    rollingDelayType
                );


            // =====================================================
            // AGENCY MASTER - SOURCE SPECIFIC
            // DelayAgency.Area values:
            // Steel Making Plant
            // Rolling Mill
            // =====================================================
            List<PlantDelayBLL> allAgencies =
                repo.GetAllAgency()
                ?? new List<PlantDelayBLL>();


            List<PlantDelayBLL> steelAgencies =
                allAgencies
                    .Where(
                        x =>
                        {
                            string areaKey =
                                (x.Area ?? "")
                                    .Trim()
                                    .Replace(" ", "")
                                    .Replace("-", "")
                                    .Replace("_", "")
                                    .ToUpperInvariant();

                            return
                                areaKey == "STEELMAKINGPLANT" ||
                                areaKey == "STEELMAKING" ||
                                areaKey == "SMP";
                        }
                    )
                    .OrderBy(
                        x => x.AgencyName
                    )
                    .ToList();


            List<PlantDelayBLL> rollingAgencies =
                allAgencies
                    .Where(
                        x =>
                        {
                            string areaKey =
                                (x.Area ?? "")
                                    .Trim()
                                    .Replace(" ", "")
                                    .Replace("-", "")
                                    .Replace("_", "")
                                    .ToUpperInvariant();

                            return
                                areaKey == "ROLLINGMILL" ||
                                areaKey == "RM";
                        }
                    )
                    .OrderBy(
                        x => x.AgencyName
                    )
                    .ToList();


            ViewBag.SteelAgencies =
                steelAgencies;

            ViewBag.RollingAgencies =
                rollingAgencies;


            // =====================================================
            // STEEL MAKING FILTER STATE
            // =====================================================
            ViewBag.SteelFromDate =
                steelStartDate.ToString(
                    "yyyy-MM-dd"
                );

            ViewBag.SteelToDate =
                steelEndDate.ToString(
                    "yyyy-MM-dd"
                );

            ViewBag.SteelPlant =
                steelPlantCsv;

            ViewBag.SteelAgency =
                steelAgencyCsv;

            ViewBag.SteelDelayType =
                steelDelayTypeCsv;


            // =====================================================
            // ROLLING MILL FILTER STATE
            // =====================================================
            ViewBag.RollingFromDate =
                rollingStartDate.ToString(
                    "yyyy-MM-dd"
                );

            ViewBag.RollingToDate =
                rollingEndDate.ToString(
                    "yyyy-MM-dd"
                );

            ViewBag.RollingPlant =
                rollingPlantCsv;

            ViewBag.RollingAgency =
                rollingAgencyCsv;

            ViewBag.RollingDelayType =
                rollingDelayTypeCsv;


            ViewBag.ActiveTab =
                activeTab;


            // =====================================================
            // STEEL MAKING
            // ONLY Steel Making filters are applied here.
            // =====================================================
            List<PlantDelayBLL> steelMakingRecords =
                repo.GetMaintenanceRecordsBySource(
                    steelStartDate.Date,
                    steelEndDate.Date,
                    "SteelMaking",
                    steelPlantCsv,
                    steelDelayTypeCsv,
                    steelAgencyCsv,
                    false
                )
                ?? new List<PlantDelayBLL>();


            // =====================================================
            // ROLLING MILL
            // ONLY Rolling Mill filters are applied here.
            // =====================================================
            List<PlantDelayBLL> rollingMillRecords =
                repo.GetMaintenanceRecordsBySource(
                    rollingStartDate.Date,
                    rollingEndDate.Date,
                    "RollingMill",
                    rollingPlantCsv,
                    rollingDelayTypeCsv,
                    rollingAgencyCsv,
                    false
                )
                ?? new List<PlantDelayBLL>();


            ViewBag.SteelMakingRecords =
                steelMakingRecords;

            ViewBag.RollingMillRecords =
                rollingMillRecords;


            // =====================================================
            // FAILURE ANALYSIS INDICATOR
            // =====================================================
            Dictionary<int, bool> failureAnalysisStatus =
                new Dictionary<int, bool>();


            foreach (
                PlantDelayBLL record
                in steelMakingRecords
            )
            {
                if (
                    record == null ||
                    record.ID <= 0
                )
                {
                    continue;
                }


                bool isFilled =
                    record.LastPMDate.HasValue
                    ||
                    !string.IsNullOrWhiteSpace(
                        record.FailureReportStatus
                    )
                    ||
                    !string.IsNullOrWhiteSpace(
                        record.IncreaseMTBF
                    )
                    ||
                    !string.IsNullOrWhiteSpace(
                        record.DecreaseMTTR
                    )
                    ||
                    !string.IsNullOrWhiteSpace(
                        record.SAPBreakdownOrder
                    )
                    ||
                    !string.IsNullOrWhiteSpace(
                        record.FailureCategory1Component
                    )
                    ||
                    !string.IsNullOrWhiteSpace(
                        record.FailureCategory2RootCause
                    );


                failureAnalysisStatus[
                    record.ID
                ] =
                    isFilled;
            }


            foreach (
                PlantDelayBLL record
                in rollingMillRecords
            )
            {
                if (
                    record == null ||
                    record.ID <= 0
                )
                {
                    continue;
                }


                /*
                 * PERFORMANCE FIX:
                 * sp_GetMaintenanceRecordsBySource already returns the latest
                 * FailureAnalysis fields for Rolling Mill. Do NOT call
                 * GetMaintenanceAnalysisByDelayID once per row (N+1 queries).
                 */
                bool isFilled =
                    record.LastPMDate.HasValue
                    ||
                    !string.IsNullOrWhiteSpace(
                        record.FailureReportStatus
                    )
                    ||
                    !string.IsNullOrWhiteSpace(
                        record.IncreaseMTBF
                    )
                    ||
                    !string.IsNullOrWhiteSpace(
                        record.IncreaseMTBF1
                    )
                    ||
                    !string.IsNullOrWhiteSpace(
                        record.DecreaseMTTR
                    )
                    ||
                    !string.IsNullOrWhiteSpace(
                        record.DecreaseMTTR1
                    )
                    ||
                    !string.IsNullOrWhiteSpace(
                        record.SAPBreakdownOrder
                    )
                    ||
                    !string.IsNullOrWhiteSpace(
                        record.FailureCategory1Component
                    )
                    ||
                    !string.IsNullOrWhiteSpace(
                        record.FailureCategory2RootCause
                    );


                failureAnalysisStatus[
                    record.ID
                ] =
                    isFilled;
            }


            ViewBag.FailureAnalysisStatus =
                failureAnalysisStatus;


            return View();
        }


        // =========================================================
        // DETAIL
        // =========================================================

        public ActionResult detail(
            int id,
            string source = "RollingMill")
        {
            try
            {
                if (id <= 0)
                {
                    TempData["ErrorMessage"] =
                        "Invalid record ID.";

                    return RedirectToAction(
                        "list"
                    );
                }


                string sourceKey =
                    string.Equals(
                        source,
                        "SteelMaking",
                        StringComparison.OrdinalIgnoreCase
                    )
                        ? "SteelMaking"
                        : "RollingMill";


                PlantDelayBLL model =
                    repo.GetMaintenanceDetailBySource(
                        id,
                        sourceKey
                    );


                if (model == null)
                {
                    TempData["ErrorMessage"] =
                        "Record not found.";

                    return RedirectToAction(
                        "list",
                        new
                        {
                            tab =
                                sourceKey == "SteelMaking"
                                    ? "steel"
                                    : "rolling"
                        }
                    );
                }


                ViewBag.RecordSource =
                    sourceKey;


                if (sourceKey == "SteelMaking")
                {
                    bool hasSmpAnalysis =
                        model.LastPMDate.HasValue
                        ||
                        !string.IsNullOrWhiteSpace(
                            model.FailureReportStatus
                        )
                        ||
                        !string.IsNullOrWhiteSpace(
                            model.IncreaseMTBF
                        )
                        ||
                        !string.IsNullOrWhiteSpace(
                            model.IncreaseMTBF1
                        )
                        ||
                        !string.IsNullOrWhiteSpace(
                            model.DecreaseMTTR
                        )
                        ||
                        !string.IsNullOrWhiteSpace(
                            model.DecreaseMTTR1
                        )
                        ||
                        !string.IsNullOrWhiteSpace(
                            model.SAPBreakdownOrder
                        )
                        ||
                        !string.IsNullOrWhiteSpace(
                            model.FailureCategory1Component
                        )
                        ||
                        !string.IsNullOrWhiteSpace(
                            model.FailureCategory2RootCause
                        );


                    ViewBag.Analysis =
                        new FailureAnalysisBLL
                        {
                            ID =
                                hasSmpAnalysis
                                    ? model.ID
                                    : 0,

                            DelayID =
                                model.ID,

                            LastPMDate =
                                model.LastPMDate,

                            FailureReportStatus =
                                model.FailureReportStatus,

                            IncreaseMTBF =
                                model.IncreaseMTBF,

                            IncreaseMTBF1 =
                                model.IncreaseMTBF1,

                            DecreaseMTTR =
                                model.DecreaseMTTR,

                            DecreaseMTTR1 =
                                model.DecreaseMTTR1,

                            SAPBreakdownOrder =
                                model.SAPBreakdownOrder,

                            FailureCategory1Component =
                                model.FailureCategory1Component,

                            FailureCategory2RootCause =
                                model.FailureCategory2RootCause
                        };


                    ViewBag.Actions =
                        new List<FailureAnalysisActionBLL>();


                    ViewBag.FailureAnalysisFile =
                        null;


                    if (
                        User != null &&
                        User.Identity != null &&
                        User.Identity.IsAuthenticated &&
                        (
                            User.IsInRole("User") ||
                            User.IsInRole("Administrator")
                        )
                    )
                    {
                        ViewBag.EquipmentItems =
                            repo.GetSMPMaintenanceEquipmentList(
                                model.Area,
                                model.Equipments
                            );
                    }
                    else
                    {
                        ViewBag.EquipmentItems =
                            new List<string>();
                    }
                }
                else
                {
                    ViewBag.Analysis =
                        repo.GetMaintenanceAnalysisByDelayID(
                            id
                        );

                    ViewBag.Actions =
                        repo.GetFailureAnalysisActionsByDelayID(
                            id
                        );

                    ViewBag.FailureAnalysisFile =
                        repo.GetFailureAnalysisFileByDelayID(
                            id
                        );


                    if (
                        User != null &&
                        User.Identity != null &&
                        User.Identity.IsAuthenticated &&
                        (
                            User.IsInRole("User") ||
                            User.IsInRole("Administrator")
                        )
                    )
                    {
                        ViewBag.EquipmentItems =
                            repo.GetMaintenanceEquipmentListByPlant(
                                model.Plant,
                                model.Equipments
                            );
                    }
                    else
                    {
                        ViewBag.EquipmentItems =
                            new List<string>();
                    }
                }


                return View(
                    model
                );
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] =
                    "Unable to load maintenance detail. Error: " +
                    ex.Message;

                return RedirectToAction(
                    "list",
                    new
                    {
                        tab =
                            string.Equals(
                                source,
                                "SteelMaking",
                                StringComparison.OrdinalIgnoreCase
                            )
                                ? "steel"
                                : "rolling"
                    }
                );
            }
        }

        // =========================================================
        // UPDATE EQUIPMENT AGAINST THIS DELAY
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateDelayEquipment(
            int DelayID,
            string Equipments,
            string source = "RollingMill")
        {
            string sourceKey =
                string.Equals(
                    source,
                    "SteelMaking",
                    StringComparison.OrdinalIgnoreCase
                )
                    ? "SteelMaking"
                    : "RollingMill";

            try
            {
                if (
                    User == null ||
                    User.Identity == null ||
                    !User.Identity.IsAuthenticated ||
                    !User.IsInRole("User")
                )
                {
                    TempData["ErrorMessage"] =
                        "You are not authorized to update equipment.";

                    return RedirectToAction(
                        "detail",
                        new
                        {
                            id = DelayID,
                            source = sourceKey
                        }
                    );
                }


                if (DelayID <= 0)
                {
                    TempData["ErrorMessage"] =
                        "Invalid delay record.";

                    return RedirectToAction(
                        "list",
                        new
                        {
                            tab =
                                sourceKey == "SteelMaking"
                                    ? "steel"
                                    : "rolling"
                        }
                    );
                }


                if (string.IsNullOrWhiteSpace(
                    Equipments
                ))
                {
                    TempData["ErrorMessage"] =
                        "Please select equipment.";

                    return RedirectToAction(
                        "detail",
                        new
                        {
                            id = DelayID,
                            source = sourceKey
                        }
                    );
                }


                if (sourceKey == "SteelMaking")
                {
                    PlantDelayBLL delay =
                        repo.GetMaintenanceDetailBySource(
                            DelayID,
                            "SteelMaking"
                        );


                    if (delay == null)
                    {
                        TempData["ErrorMessage"] =
                            "Steel Making delay record was not found.";

                        return RedirectToAction(
                            "list",
                            new { tab = "steel" }
                        );
                    }


                    List<string> validEquipments =
                        repo.GetSMPMaintenanceEquipmentList(
                            delay.Area,
                            delay.Equipments
                        )
                        ?? new List<string>();


                    bool isValidEquipment =
                        validEquipments.Any(
                            x =>
                                string.Equals(
                                    x,
                                    Equipments.Trim(),
                                    StringComparison.OrdinalIgnoreCase
                                )
                        );


                    if (!isValidEquipment)
                    {
                        TempData["ErrorMessage"] =
                            "Selected equipment is not valid for Steel Making area " +
                            (delay.Area ?? "-") +
                            ".";

                        return RedirectToAction(
                            "detail",
                            new
                            {
                                id = DelayID,
                                source = "SteelMaking"
                            }
                        );
                    }


                    int result =
                        repo.UpdateSMPMaintenanceEquipment(
                            DelayID,
                            Equipments.Trim(),
                            GetCurrentUser()
                        );


                    TempData[
                        result > 0
                            ? "SuccessMessage"
                            : "ErrorMessage"
                    ] =
                        result > 0
                            ? "Steel Making equipment updated successfully."
                            : "Steel Making equipment was not updated.";
                }
                else
                {
                    PlantDelayBLL delay =
                        repo.GetDelayByID(
                            DelayID
                        );


                    if (delay == null)
                    {
                        TempData["ErrorMessage"] =
                            "Delay record was not found.";

                        return RedirectToAction(
                            "list",
                            new { tab = "rolling" }
                        );
                    }


                    List<string> validEquipments =
                        repo.GetMaintenanceEquipmentListByPlant(
                            delay.Plant,
                            delay.Equipments
                        )
                        ?? new List<string>();


                    bool isValidEquipment =
                        validEquipments.Any(
                            x =>
                                string.Equals(
                                    x,
                                    Equipments.Trim(),
                                    StringComparison.OrdinalIgnoreCase
                                )
                        );


                    if (!isValidEquipment)
                    {
                        TempData["ErrorMessage"] =
                            "Selected equipment is not valid for plant " +
                            (delay.Plant ?? "-") +
                            ".";

                        return RedirectToAction(
                            "detail",
                            new
                            {
                                id = DelayID,
                                source = "RollingMill"
                            }
                        );
                    }


                    int result =
                        repo.UpdateDelayEquipment(
                            DelayID,
                            Equipments.Trim(),
                            GetCurrentUser()
                        );


                    TempData[
                        result > 0
                            ? "SuccessMessage"
                            : "ErrorMessage"
                    ] =
                        result > 0
                            ? "Equipment updated successfully."
                            : "Equipment was not updated.";
                }


                return RedirectToAction(
                    "detail",
                    new
                    {
                        id = DelayID,
                        source = sourceKey
                    }
                );
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] =
                    "Unable to update equipment. Error: " +
                    ex.Message;

                return RedirectToAction(
                    "detail",
                    new
                    {
                        id = DelayID,
                        source = sourceKey
                    }
                );
            }
        }

        // =========================================================
        // DELAY CORRECTION
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateDelayCorrection(
            int DelayID,
            string DelayDescription1,
            string ReasonforOccurence1,
            string ActionTaken1,
            string source = "RollingMill")
        {
            string sourceKey =
                string.Equals(
                    source,
                    "SteelMaking",
                    StringComparison.OrdinalIgnoreCase
                )
                    ? "SteelMaking"
                    : "RollingMill";

            try
            {
                if (DelayID <= 0)
                {
                    TempData["ErrorMessage"] =
                        "Invalid delay record.";

                    return RedirectToAction(
                        "list",
                        new
                        {
                            tab =
                                sourceKey == "SteelMaking"
                                    ? "steel"
                                    : "rolling"
                        }
                    );
                }


                int result;

                if (sourceKey == "SteelMaking")
                {
                    result =
                        repo.UpdateSMPMaintenanceDelayCorrection(
                            DelayID,
                            DelayDescription1,
                            ReasonforOccurence1,
                            ActionTaken1,
                            GetCurrentUser()
                        );
                }
                else
                {
                    var model =
                        new PlantDelayBLL
                        {
                            ID =
                                DelayID,

                            DelayDescription1 =
                                string.IsNullOrWhiteSpace(
                                    DelayDescription1
                                )
                                    ? null
                                    : DelayDescription1.Trim(),

                            ReasonForOccurence1 =
                                string.IsNullOrWhiteSpace(
                                    ReasonforOccurence1
                                )
                                    ? null
                                    : ReasonforOccurence1.Trim(),

                            ActionTaken1 =
                                string.IsNullOrWhiteSpace(
                                    ActionTaken1
                                )
                                    ? null
                                    : ActionTaken1.Trim(),

                            UpdatedBy =
                                GetCurrentUser(),

                            UpdatedDate =
                                DateTime.Now
                        };


                    result =
                        repo.UpdateDelayCorrection(
                            model
                        );
                }


                bool saved =
                    sourceKey == "SteelMaking"
                        ? result > 0
                        : result < 0;


                TempData[
                    saved
                        ? "SuccessMessage"
                        : "ErrorMessage"
                ] =
                    saved
                        ? "Delay remarks updated successfully."
                        : "Delay remarks were not updated.";


                return RedirectToAction(
                    "detail",
                    new
                    {
                        id = DelayID,
                        source = sourceKey
                    }
                );
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] =
                    "Unable to update delay remarks. Error: " +
                    ex.Message;

                return RedirectToAction(
                    "detail",
                    new
                    {
                        id = DelayID,
                        source = sourceKey
                    }
                );
            }
        }

        // =========================================================
        // MAINTENANCE ANALYSIS
        // =========================================================

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult InsertMaintenanceAnalysis(
        //    FailureAnalysisBLL model,
        //    HttpPostedFileBase FailureAnalysisFile,
        //    string FailureAnalysisFileRemarks,
        //    string[] increaseMTBFActions,
        //    string[] decreaseMTTRActions,
        //    string source = "RollingMill")
        //{
        //    int delayID =
        //        model != null
        //            ? model.DelayID ?? 0
        //            : 0;


        //    string sourceKey =
        //        string.Equals(
        //            source,
        //            "SteelMaking",
        //            StringComparison.OrdinalIgnoreCase
        //        )
        //            ? "SteelMaking"
        //            : "RollingMill";


        //    try
        //    {
        //        if (
        //            model == null ||
        //            model.DelayID <= 0
        //        )
        //        {
        //            TempData["Error"] =
        //                "Invalid maintenance analysis request.";

        //            return RedirectToAction(
        //                "list",
        //                new
        //                {
        //                    tab =
        //                        sourceKey == "SteelMaking"
        //                            ? "steel"
        //                            : "rolling"
        //                }
        //            );
        //        }


        //        string currentUser =
        //            GetCurrentUser();


        //        if (sourceKey == "SteelMaking")
        //        {
        //            string increaseMTBF1 =
        //                MergeSMPAnalysisActions(
        //                    model.IncreaseMTBF1,
        //                    increaseMTBFActions
        //                );

        //            string decreaseMTTR1 =
        //                MergeSMPAnalysisActions(
        //                    model.DecreaseMTTR1,
        //                    decreaseMTTRActions
        //                );


        //            string savedRelativePath =
        //                null;

        //            string originalFileName =
        //                null;


        //            if (
        //                FailureAnalysisFile != null &&
        //                FailureAnalysisFile.ContentLength > 0
        //            )
        //            {
        //                originalFileName =
        //                    Path.GetFileName(
        //                        FailureAnalysisFile.FileName
        //                    );

        //                savedRelativePath =
        //                    SaveFailureAnalysisFile(
        //                        FailureAnalysisFile,
        //                        delayID
        //                    );
        //            }


        //            int result =
        //                repo.UpdateSMPMaintenanceFailureAnalysis(
        //                    delayID,
        //                    model.LastPMDate,
        //                    model.FailureReportStatus,
        //                    model.IncreaseMTBF,
        //                    increaseMTBF1,
        //                    model.DecreaseMTTR,
        //                    decreaseMTTR1,
        //                    model.SAPBreakdownOrder,
        //                    model.FailureCategory1Component,
        //                    model.FailureCategory2RootCause,
        //                    savedRelativePath,
        //                    originalFileName,
        //                    FailureAnalysisFileRemarks,
        //                    currentUser
        //                );


        //            if (result <= 0)
        //            {
        //                if (!string.IsNullOrWhiteSpace(
        //                    savedRelativePath
        //                ))
        //                {
        //                    DeleteUploadedFile(
        //                        savedRelativePath
        //                    );
        //                }

        //                throw new Exception(
        //                    "Steel Making failure analysis could not be saved."
        //                );
        //            }


        //            TempData["Success"] =
        //                "Steel Making failure analysis saved successfully.";


        //            return RedirectToAction(
        //                "detail",
        //                new
        //                {
        //                    id = delayID,
        //                    source = "SteelMaking"
        //                }
        //            );
        //        }


        //        // =====================================================
        //        // ROLLING MILL - EXISTING FLOW UNCHANGED
        //        // =====================================================
        //        model.StatusID =
        //            1;


        //        if (model.ID > 0)
        //        {
        //            model.UpdatedBy =
        //                currentUser;

        //            model.UpdatedDate =
        //                DateTime.Now;

        //            repo.UpdateMissingMaintenanceAnalysis(
        //                model
        //            );
        //        }
        //        else
        //        {
        //            model.CreatedBy =
        //                currentUser;

        //            model.CreatedDate =
        //                DateTime.Now;

        //            repo.InsertMaintenanceAnalysis(
        //                model
        //            );
        //        }


        //        FailureAnalysisBLL savedAnalysis =
        //            repo.GetMaintenanceAnalysisByDelayID(
        //                delayID
        //            );


        //        if (
        //            savedAnalysis == null ||
        //            savedAnalysis.ID <= 0
        //        )
        //        {
        //            throw new Exception(
        //                "Failure Analysis record was not found after save."
        //            );
        //        }


        //        int savedDynamicActions =
        //            SaveMtbfMttrActions(
        //                delayID,
        //                savedAnalysis.ID,
        //                increaseMTBFActions,
        //                decreaseMTTRActions
        //            );


        //        if (
        //            FailureAnalysisFile != null &&
        //            FailureAnalysisFile.ContentLength > 0
        //        )
        //        {
        //            string originalFileName =
        //                Path.GetFileName(
        //                    FailureAnalysisFile.FileName
        //                );

        //            string savedRelativePath =
        //                SaveFailureAnalysisFile(
        //                    FailureAnalysisFile,
        //                    delayID
        //                );

        //            int attachmentResult =
        //                repo.SaveFailureAnalysisFile(
        //                    delayID,
        //                    savedRelativePath,
        //                    originalFileName,
        //                    FailureAnalysisFileRemarks,
        //                    currentUser
        //                );

        //            if (attachmentResult <= 0)
        //            {
        //                DeleteUploadedFile(
        //                    savedRelativePath
        //                );

        //                throw new Exception(
        //                    "Failure analysis attachment could not be saved."
        //                );
        //            }
        //        }


        //        TempData["Success"] =
        //            savedDynamicActions > 0
        //                ? "Maintenance analysis and " +
        //                  savedDynamicActions +
        //                  " dynamic MTBF/MTTR action(s) saved successfully."
        //                : "Maintenance analysis saved successfully.";


        //        return RedirectToAction(
        //            "detail",
        //            new
        //            {
        //                id = delayID,
        //                source = "RollingMill"
        //            }
        //        );
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["Error"] =
        //            "Unable to save maintenance analysis. Error: " +
        //            ex.Message;


        //        if (delayID > 0)
        //        {
        //            return RedirectToAction(
        //                "detail",
        //                new
        //                {
        //                    id = delayID,
        //                    source = sourceKey
        //                }
        //            );
        //        }


        //        return RedirectToAction(
        //            "list",
        //            new
        //            {
        //                tab =
        //                    sourceKey == "SteelMaking"
        //                        ? "steel"
        //                        : "rolling"
        //            }
        //        );
        //    }
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InsertMaintenanceAnalysis(
    FailureAnalysisBLL model,
    HttpPostedFileBase FailureAnalysisFile,
    string FailureAnalysisFileRemarks,
    string[] increaseMTBFActions,
    string[] decreaseMTTRActions,
    string source = "RollingMill")
        {
            int delayID =
                model != null
                    ? model.DelayID ?? 0
                    : 0;

            string sourceKey =
                string.Equals(
                    source,
                    "SteelMaking",
                    StringComparison.OrdinalIgnoreCase
                )
                    ? "SteelMaking"
                    : "RollingMill";

            try
            {
                if (
                    model == null ||
                    !model.DelayID.HasValue ||
                    model.DelayID.Value <= 0
                )
                {
                    TempData["Error"] =
                        "Invalid maintenance analysis request.";

                    return RedirectToAction(
                        "list",
                        new
                        {
                            tab =
                                sourceKey == "SteelMaking"
                                    ? "steel"
                                    : "rolling"
                        }
                    );
                }

                delayID = model.DelayID.Value;

                string currentUser =
                    GetCurrentUser();

                // =========================================================
                // STEEL MAKING
                // SAVE DIRECTLY INTO dbo.SMPDelayDaywise
                // =========================================================
                if (
                    string.Equals(
                        sourceKey,
                        "SteelMaking",
                        StringComparison.OrdinalIgnoreCase
                    )
                )
                {
                    string increaseMTBF1 =
                        MergeSMPAnalysisActions(
                            model.IncreaseMTBF1,
                            increaseMTBFActions
                        );

                    string decreaseMTTR1 =
                        MergeSMPAnalysisActions(
                            model.DecreaseMTTR1,
                            decreaseMTTRActions
                        );

                    string savedRelativePath = null;
                    string originalFileName = null;

                    try
                    {
                        if (
                            FailureAnalysisFile != null &&
                            FailureAnalysisFile.ContentLength > 0
                        )
                        {
                            originalFileName =
                                Path.GetFileName(
                                    FailureAnalysisFile.FileName
                                );

                            savedRelativePath =
                                SaveFailureAnalysisFile(
                                    FailureAnalysisFile,
                                    delayID
                                );
                        }

                        int result =
                            repo.UpdateSMPMaintenanceFailureAnalysis(
                                delayID,
                                model.LastPMDate,
                                model.FailureReportStatus,
                                model.IncreaseMTBF,
                                increaseMTBF1,
                                model.DecreaseMTTR,
                                decreaseMTTR1,
                                model.SAPBreakdownOrder,
                                model.FailureCategory1Component,
                                model.FailureCategory2RootCause,
                                savedRelativePath,
                                originalFileName,
                                FailureAnalysisFileRemarks,
                                currentUser
                            );

                        if (result <= 0)
                        {
                            if (
                                !string.IsNullOrWhiteSpace(
                                    savedRelativePath
                                )
                            )
                            {
                                DeleteUploadedFile(
                                    savedRelativePath
                                );
                            }

                            throw new Exception(
                                "Steel Making failure analysis could not be saved."
                            );
                        }

                        TempData["Success"] =
                            "Steel Making maintenance analysis saved successfully.";

                        return RedirectToAction(
                            "detail",
                            new
                            {
                                id = delayID,
                                source = "SteelMaking"
                            }
                        );
                    }
                    catch
                    {
                        if (
                            !string.IsNullOrWhiteSpace(
                                savedRelativePath
                            )
                        )
                        {
                            DeleteUploadedFile(
                                savedRelativePath
                            );
                        }

                        throw;
                    }
                }

                // =========================================================
                // ROLLING MILL
                // SAVE INTO dbo.FailureAnalysis
                // =========================================================
                model.StatusID = 1;

                FailureAnalysisBLL existingRollingAnalysis =
                    repo.GetMaintenanceAnalysisByDelayID(
                        delayID
                    );

                if (
                    existingRollingAnalysis != null &&
                    existingRollingAnalysis.ID > 0
                )
                {
                    model.ID =
                        existingRollingAnalysis.ID;

                    model.AnalysisCode =
                        existingRollingAnalysis.AnalysisCode;

                    model.UpdatedBy =
                        currentUser;

                    model.UpdatedDate =
                        DateTime.Now;

                    repo.UpdateMissingMaintenanceAnalysis(
                        model
                    );
                }
                else
                {
                    model.ID = 0;

                    model.CreatedBy =
                        currentUser;

                    model.CreatedDate =
                        DateTime.Now;

                    repo.InsertMaintenanceAnalysis(
                        model
                    );
                }

                FailureAnalysisBLL savedAnalysis =
                    repo.GetMaintenanceAnalysisByDelayID(
                        delayID
                    );

                if (
                    savedAnalysis == null ||
                    savedAnalysis.ID <= 0
                )
                {
                    throw new Exception(
                        "Rolling Mill Failure Analysis record was not found after save."
                    );
                }

                int savedDynamicActions =
                    SaveMtbfMttrActions(
                        delayID,
                        savedAnalysis.ID,
                        increaseMTBFActions,
                        decreaseMTTRActions
                    );

                if (
                    FailureAnalysisFile != null &&
                    FailureAnalysisFile.ContentLength > 0
                )
                {
                    string originalFileName =
                        Path.GetFileName(
                            FailureAnalysisFile.FileName
                        );

                    string savedRelativePath =
                        SaveFailureAnalysisFile(
                            FailureAnalysisFile,
                            delayID
                        );

                    int attachmentResult =
                        repo.SaveFailureAnalysisFile(
                            delayID,
                            savedRelativePath,
                            originalFileName,
                            FailureAnalysisFileRemarks,
                            currentUser
                        );

                    if (attachmentResult <= 0)
                    {
                        DeleteUploadedFile(
                            savedRelativePath
                        );

                        throw new Exception(
                            "Rolling Mill failure analysis attachment could not be saved."
                        );
                    }
                }

                TempData["Success"] =
                    savedDynamicActions > 0
                        ? "Rolling Mill maintenance analysis and " +
                          savedDynamicActions +
                          " dynamic MTBF/MTTR action(s) saved successfully."
                        : "Rolling Mill maintenance analysis saved successfully.";

                return RedirectToAction(
                    "detail",
                    new
                    {
                        id = delayID,
                        source = "RollingMill"
                    }
                );
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    "Unable to save maintenance analysis. Error: " +
                    ex.Message;

                if (delayID > 0)
                {
                    return RedirectToAction(
                        "detail",
                        new
                        {
                            id = delayID,
                            source = sourceKey
                        }
                    );
                }

                return RedirectToAction(
                    "list",
                    new
                    {
                        tab =
                            sourceKey == "SteelMaking"
                                ? "steel"
                                : "rolling"
                    }
                );
            }
        }


        // =========================================================
        // MTBF / MTTR ACTIONS
        // =========================================================

        private int SaveMtbfMttrActions(
            int delayID,
            int analysisID,
            string[] increaseMTBFActions,
            string[] decreaseMTTRActions)
        {
            if (
                delayID <= 0 ||
                analysisID <= 0
            )
            {
                return 0;
            }

            int savedCount =
                0;

            string currentUser =
                GetCurrentUser();


            // =====================================================
            // MTBF - every box becomes a separate DB row/code
            // =====================================================
            if (increaseMTBFActions != null)
            {
                foreach (
                    string remarks
                    in increaseMTBFActions
                )
                {
                    if (string.IsNullOrWhiteSpace(
                        remarks
                    ))
                    {
                        continue;
                    }

                    FailureAnalysisActionBLL action =
                        new FailureAnalysisActionBLL
                        {
                            DelayID =
                                delayID,

                            AnalysisID =
                                analysisID,

                            ActionType =
                                "IncreaseMTBF",

                            ActionRemarks =
                                remarks.Trim(),

                            StatusID =
                                1,

                            CreatedBy =
                                currentUser,

                            CreatedDate =
                                DateTime.Now
                        };

                    /*
                     * ActionCode is intentionally NOT generated here.
                     * SQL generates it after the identity ActionID exists,
                     * which makes every MTBF/MTTR code concurrency-safe
                     * and unique.
                     */
                    int actionID =
                        repo.InsertFailureAnalysisAction(
                            action
                        );

                    if (actionID <= 0)
                    {
                        throw new Exception(
                            "Dynamic MTBF action could not be saved."
                        );
                    }

                    savedCount++;
                }
            }


            // =====================================================
            // MTTR - every box becomes a separate DB row/code
            // =====================================================
            if (decreaseMTTRActions != null)
            {
                foreach (
                    string remarks
                    in decreaseMTTRActions
                )
                {
                    if (string.IsNullOrWhiteSpace(
                        remarks
                    ))
                    {
                        continue;
                    }

                    FailureAnalysisActionBLL action =
                        new FailureAnalysisActionBLL
                        {
                            DelayID =
                                delayID,

                            AnalysisID =
                                analysisID,

                            ActionType =
                                "DecreaseMTTR",

                            ActionRemarks =
                                remarks.Trim(),

                            StatusID =
                                1,

                            CreatedBy =
                                currentUser,

                            CreatedDate =
                                DateTime.Now
                        };

                    int actionID =
                        repo.InsertFailureAnalysisAction(
                            action
                        );

                    if (actionID <= 0)
                    {
                        throw new Exception(
                            "Dynamic MTTR action could not be saved."
                        );
                    }

                    savedCount++;
                }
            }


            return savedCount;
        }


        // =========================================================
        // EXCEL EXPORT
        // =========================================================


        // Add these usings if not already present:
        // using System;
        // using System.Collections.Generic;
        // using System.Globalization;
        // using System.IO;
        // using System.IO.Compression;
        // using System.Linq;
        // using System.Text;
        // using System.Xml.Linq;

        public ActionResult ExportFailureAnalysisExcel(
            DateTime? steelFromDate,
            DateTime? steelToDate,
            string[] steelPlant,
            string[] steelAgency,
            string[] steelDelayType,
            DateTime? rollingFromDate,
            DateTime? rollingToDate,
            string[] rollingPlant,
            string[] rollingAgency,
            string[] rollingDelayType,
            string tab = null)
        {
            string activeTab =
                string.Equals(
                    tab,
                    "rolling",
                    StringComparison.OrdinalIgnoreCase
                )
                    ? "rolling"
                    : "steel";

            DateTime startDate =
                activeTab == "rolling"
                    ? (rollingFromDate ?? DateTime.Today)
                    : (steelFromDate ?? DateTime.Today);

            DateTime endDate =
                activeTab == "rolling"
                    ? (rollingToDate ?? startDate)
                    : (steelToDate ?? startDate);

            if (startDate.Date > endDate.Date)
            {
                DateTime temp = startDate;
                startDate = endDate;
                endDate = temp;
            }

            string plantCsv =
                activeTab == "rolling"
                    ? JoinFilterValues(rollingPlant)
                    : JoinFilterValues(steelPlant);

            string agencyCsv =
                activeTab == "rolling"
                    ? JoinFilterValues(rollingAgency)
                    : JoinFilterValues(steelAgency);

            string delayTypeCsv =
                activeTab == "rolling"
                    ? JoinFilterValues(rollingDelayType)
                    : JoinFilterValues(steelDelayType);

            string source =
                activeTab == "rolling"
                    ? "RollingMill"
                    : "SteelMaking";

            var records =
                repo.GetMaintenanceRecordsBySource(
                    startDate.Date,
                    endDate.Date,
                    source,
                    plantCsv,
                    delayTypeCsv,
                    agencyCsv,
                    true
                );

            string[] headers =
            {
        "Delay ID",
        "Delay Code",
        "Plant",
        "Product Size",
        "Production Date",
        "Delay Start",
        "Delay End",
        "Total Minutes",
        "Agency",
        "Area",
        "Equipment",
        "Delay Description",
        "Reason for Occurrence",
        "Action Taken",
        "Last PM Date",
        "Failure Report Status",
        "Long Term Action to Increase MTBF",
        "Additional Action to Increase MTBF",
        "Long Term Action to Decrease MTTR",
        "Additional Action to Decrease MTTR",
        "SAP Breakdown No.",
        "Failure Category 1 (Component)",
        "Failure Category 2 (Root Cause)"
    };

            List<string[]> rows =
                new List<string[]>();

            if (records != null)
            {
                foreach (var item in records)
                {
                    rows.Add(
                        new string[]
                        {
                    Convert.ToString(
                        item.ID,
                        CultureInfo.InvariantCulture
                    ),

                    item.Delaycode ?? "",

                    item.Plant ?? "",

                    item.ProductSize ?? "",

                    item.Date.HasValue
                        ? item.Date.Value.ToString(
                            "dd-MMM-yyyy",
                            CultureInfo.InvariantCulture
                        )
                        : "",

                    item.StartTime.HasValue
                        ? item.StartTime.Value.ToString(
                            @"hh\:mm",
                            CultureInfo.InvariantCulture
                        )
                        : "",

                    item.EndTime.HasValue
                        ? item.EndTime.Value.ToString(
                            @"hh\:mm",
                            CultureInfo.InvariantCulture
                        )
                        : "",

                    Convert.ToString(
                        item.TotalDuration,
                        CultureInfo.InvariantCulture
                    ),

                    item.AgencyName ?? "",

                    item.Area ?? "",

                    item.Equipments ?? "",

                    // FIXED
                    item.DelayDescription ?? "",

                    // FIXED
                    item.ReasonForOccurence ?? "",

                    // FIXED
                    item.ActionTaken ?? "",

                    item.LastPMDate.HasValue
                        ? item.LastPMDate.Value.ToString(
                            "dd-MMM-yyyy",
                            CultureInfo.InvariantCulture
                        )
                        : "",

                    item.FailureReportStatus ?? "",

                    item.IncreaseMTBF ?? "",

                    item.IncreaseMTBF1 ?? "",

                    item.DecreaseMTTR ?? "",

                    item.DecreaseMTTR1 ?? "",

                    item.SAPBreakdownOrder ?? "",

                    item.FailureCategory1Component ?? "",

                    item.FailureCategory2RootCause ?? ""
                        }
                    );
                }
            }

            byte[] excelBytes =
                BuildFailureAnalysisXlsx(
                    headers,
                    rows
                );

            string fileName =
                "Failure_Analysis_" +
                startDate.ToString(
                    "yyyyMMdd",
                    CultureInfo.InvariantCulture
                ) +
                "_to_" +
                endDate.ToString(
                    "yyyyMMdd",
                    CultureInfo.InvariantCulture
                ) +
                ".xlsx";

            return File(
                excelBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName
            );
        }


        private byte[] BuildFailureAnalysisXlsx(
            string[] headers,
            List<string[]> rows)
        {
            using (MemoryStream output = new MemoryStream())
            {
                using (
                    ZipArchive archive =
                        new ZipArchive(
                            output,
                            ZipArchiveMode.Create,
                            true
                        )
                )
                {
                    WriteZipTextEntry(
                        archive,
                        "[Content_Types].xml",
                        BuildFailureAnalysisContentTypesXml()
                    );

                    WriteZipTextEntry(
                        archive,
                        "_rels/.rels",
                        BuildFailureAnalysisRootRelationshipsXml()
                    );

                    WriteZipTextEntry(
                        archive,
                        "xl/workbook.xml",
                        BuildFailureAnalysisWorkbookXml()
                    );

                    WriteZipTextEntry(
                        archive,
                        "xl/_rels/workbook.xml.rels",
                        BuildFailureAnalysisWorkbookRelationshipsXml()
                    );

                    WriteZipTextEntry(
                        archive,
                        "xl/styles.xml",
                        BuildFailureAnalysisStylesXml()
                    );

                    WriteZipTextEntry(
                        archive,
                        "xl/worksheets/sheet1.xml",
                        BuildFailureAnalysisWorksheetXml(
                            headers,
                            rows
                        )
                    );
                }

                return output.ToArray();
            }
        }


        private string BuildFailureAnalysisContentTypesXml()
        {
            XNamespace ns =
                "http://schemas.openxmlformats.org/package/2006/content-types";

            XDocument doc =
                new XDocument(
                    new XDeclaration("1.0", "UTF-8", "yes"),
                    new XElement(
                        ns + "Types",
                        new XElement(
                            ns + "Default",
                            new XAttribute("Extension", "rels"),
                            new XAttribute(
                                "ContentType",
                                "application/vnd.openxmlformats-package.relationships+xml"
                            )
                        ),
                        new XElement(
                            ns + "Default",
                            new XAttribute("Extension", "xml"),
                            new XAttribute("ContentType", "application/xml")
                        ),
                        new XElement(
                            ns + "Override",
                            new XAttribute("PartName", "/xl/workbook.xml"),
                            new XAttribute(
                                "ContentType",
                                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml"
                            )
                        ),
                        new XElement(
                            ns + "Override",
                            new XAttribute("PartName", "/xl/worksheets/sheet1.xml"),
                            new XAttribute(
                                "ContentType",
                                "application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml"
                            )
                        ),
                        new XElement(
                            ns + "Override",
                            new XAttribute("PartName", "/xl/styles.xml"),
                            new XAttribute(
                                "ContentType",
                                "application/vnd.openxmlformats-officedocument.spreadsheetml.styles+xml"
                            )
                        )
                    )
                );

            return ToXmlString(doc);
        }


        private string BuildFailureAnalysisRootRelationshipsXml()
        {
            XNamespace ns =
                "http://schemas.openxmlformats.org/package/2006/relationships";

            XDocument doc =
                new XDocument(
                    new XDeclaration("1.0", "UTF-8", "yes"),
                    new XElement(
                        ns + "Relationships",
                        new XElement(
                            ns + "Relationship",
                            new XAttribute("Id", "rId1"),
                            new XAttribute(
                                "Type",
                                "http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument"
                            ),
                            new XAttribute("Target", "xl/workbook.xml")
                        )
                    )
                );

            return ToXmlString(doc);
        }


        private string BuildFailureAnalysisWorkbookXml()
        {
            XNamespace ns =
                "http://schemas.openxmlformats.org/spreadsheetml/2006/main";

            XNamespace r =
                "http://schemas.openxmlformats.org/officeDocument/2006/relationships";

            XDocument doc =
                new XDocument(
                    new XDeclaration("1.0", "UTF-8", "yes"),
                    new XElement(
                        ns + "workbook",
                        new XAttribute(XNamespace.Xmlns + "r", r),
                        new XElement(
                            ns + "sheets",
                            new XElement(
                                ns + "sheet",
                                new XAttribute("name", "Failure Analysis"),
                                new XAttribute("sheetId", "1"),
                                new XAttribute(r + "id", "rId1")
                            )
                        )
                    )
                );

            return ToXmlString(doc);
        }


        private string BuildFailureAnalysisWorkbookRelationshipsXml()
        {
            XNamespace ns =
                "http://schemas.openxmlformats.org/package/2006/relationships";

            XDocument doc =
                new XDocument(
                    new XDeclaration("1.0", "UTF-8", "yes"),
                    new XElement(
                        ns + "Relationships",
                        new XElement(
                            ns + "Relationship",
                            new XAttribute("Id", "rId1"),
                            new XAttribute(
                                "Type",
                                "http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet"
                            ),
                            new XAttribute("Target", "worksheets/sheet1.xml")
                        ),
                        new XElement(
                            ns + "Relationship",
                            new XAttribute("Id", "rId2"),
                            new XAttribute(
                                "Type",
                                "http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles"
                            ),
                            new XAttribute("Target", "styles.xml")
                        )
                    )
                );

            return ToXmlString(doc);
        }


        private string BuildFailureAnalysisStylesXml()
        {
            XNamespace ns =
                "http://schemas.openxmlformats.org/spreadsheetml/2006/main";

            XDocument doc =
                new XDocument(
                    new XDeclaration("1.0", "UTF-8", "yes"),
                    new XElement(
                        ns + "styleSheet",

                        new XElement(
                            ns + "fonts",
                            new XAttribute("count", "2"),
                            new XElement(
                                ns + "font",
                                new XElement(ns + "sz", new XAttribute("val", "11")),
                                new XElement(ns + "name", new XAttribute("val", "Calibri"))
                            ),
                            new XElement(
                                ns + "font",
                                new XElement(ns + "b"),
                                new XElement(
                                    ns + "color",
                                    new XAttribute("rgb", "FFFFFFFF")
                                ),
                                new XElement(ns + "sz", new XAttribute("val", "11")),
                                new XElement(ns + "name", new XAttribute("val", "Calibri"))
                            )
                        ),

                        new XElement(
                            ns + "fills",
                            new XAttribute("count", "3"),
                            new XElement(
                                ns + "fill",
                                new XElement(
                                    ns + "patternFill",
                                    new XAttribute("patternType", "none")
                                )
                            ),
                            new XElement(
                                ns + "fill",
                                new XElement(
                                    ns + "patternFill",
                                    new XAttribute("patternType", "gray125")
                                )
                            ),
                            new XElement(
                                ns + "fill",
                                new XElement(
                                    ns + "patternFill",
                                    new XAttribute("patternType", "solid"),
                                    new XElement(
                                        ns + "fgColor",
                                        new XAttribute("rgb", "FF0B7285")
                                    ),
                                    new XElement(
                                        ns + "bgColor",
                                        new XAttribute("indexed", "64")
                                    )
                                )
                            )
                        ),

                        new XElement(
                            ns + "borders",
                            new XAttribute("count", "2"),
                            new XElement(ns + "border"),
                            new XElement(
                                ns + "border",
                                BuildThinBorderSide(ns, "left"),
                                BuildThinBorderSide(ns, "right"),
                                BuildThinBorderSide(ns, "top"),
                                BuildThinBorderSide(ns, "bottom"),
                                new XElement(ns + "diagonal")
                            )
                        ),

                        new XElement(
                            ns + "cellStyleXfs",
                            new XAttribute("count", "1"),
                            new XElement(
                                ns + "xf",
                                new XAttribute("numFmtId", "0"),
                                new XAttribute("fontId", "0"),
                                new XAttribute("fillId", "0"),
                                new XAttribute("borderId", "0")
                            )
                        ),

                        new XElement(
                            ns + "cellXfs",
                            new XAttribute("count", "3"),

                            // style 0 - normal
                            new XElement(
                                ns + "xf",
                                new XAttribute("numFmtId", "0"),
                                new XAttribute("fontId", "0"),
                                new XAttribute("fillId", "0"),
                                new XAttribute("borderId", "0"),
                                new XAttribute("xfId", "0")
                            ),

                            // style 1 - header
                            new XElement(
                                ns + "xf",
                                new XAttribute("numFmtId", "0"),
                                new XAttribute("fontId", "1"),
                                new XAttribute("fillId", "2"),
                                new XAttribute("borderId", "1"),
                                new XAttribute("xfId", "0"),
                                new XAttribute("applyAlignment", "1"),
                                new XElement(
                                    ns + "alignment",
                                    new XAttribute("horizontal", "center"),
                                    new XAttribute("vertical", "center"),
                                    new XAttribute("wrapText", "1")
                                )
                            ),

                            // style 2 - data
                            new XElement(
                                ns + "xf",
                                new XAttribute("numFmtId", "0"),
                                new XAttribute("fontId", "0"),
                                new XAttribute("fillId", "0"),
                                new XAttribute("borderId", "1"),
                                new XAttribute("xfId", "0"),
                                new XAttribute("applyAlignment", "1"),
                                new XElement(
                                    ns + "alignment",
                                    new XAttribute("vertical", "center"),
                                    new XAttribute("wrapText", "1")
                                )
                            )
                        ),

                        new XElement(
                            ns + "cellStyles",
                            new XAttribute("count", "1"),
                            new XElement(
                                ns + "cellStyle",
                                new XAttribute("name", "Normal"),
                                new XAttribute("xfId", "0"),
                                new XAttribute("builtinId", "0")
                            )
                        )
                    )
                );

            return ToXmlString(doc);
        }


        private XElement BuildThinBorderSide(
            XNamespace ns,
            string sideName)
        {
            return
                new XElement(
                    ns + sideName,
                    new XAttribute("style", "thin"),
                    new XElement(
                        ns + "color",
                        new XAttribute("auto", "1")
                    )
                );
        }


        private string BuildFailureAnalysisWorksheetXml(
            string[] headers,
            List<string[]> rows)
        {
            XNamespace ns =
                "http://schemas.openxmlformats.org/spreadsheetml/2006/main";

            XElement sheetData =
                new XElement(ns + "sheetData");

            // Header row
            XElement headerRow =
                new XElement(
                    ns + "row",
                    new XAttribute("r", "1"),
                    new XAttribute("ht", "42"),
                    new XAttribute("customHeight", "1")
                );

            for (int i = 0; i < headers.Length; i++)
            {
                headerRow.Add(
                    BuildInlineStringCell(
                        ns,
                        GetExcelColumnNameForFailureExport(i + 1) + "1",
                        headers[i],
                        1
                    )
                );
            }

            sheetData.Add(headerRow);

            // Data rows
            for (int r = 0; r < rows.Count; r++)
            {
                int excelRow = r + 2;

                XElement row =
                    new XElement(
                        ns + "row",
                        new XAttribute(
                            "r",
                            excelRow.ToString(CultureInfo.InvariantCulture)
                        )
                    );

                string[] values =
                    rows[r] ?? new string[0];

                for (int c = 0; c < headers.Length; c++)
                {
                    string value =
                        c < values.Length
                            ? values[c] ?? ""
                            : "";

                    row.Add(
                        BuildInlineStringCell(
                            ns,
                            GetExcelColumnNameForFailureExport(c + 1) +
                            excelRow.ToString(CultureInfo.InvariantCulture),
                            value,
                            2
                        )
                    );
                }

                sheetData.Add(row);
            }

            int lastRow =
                Math.Max(
                    1,
                    rows.Count + 1
                );

            XElement columns =
                new XElement(ns + "cols");

            // Columns 1-11 moderately sized
            for (int c = 1; c <= 11; c++)
            {
                columns.Add(
                    new XElement(
                        ns + "col",
                        new XAttribute("min", c),
                        new XAttribute("max", c),
                        new XAttribute(
                            "width",
                            c == 1 ? "12" :
                            c == 2 ? "16" :
                            c == 5 ? "16" :
                            c == 6 || c == 7 ? "12" :
                            "18"
                        ),
                        new XAttribute("customWidth", "1")
                    )
                );
            }

            // Columns 12-23 wider, matching old report intent
            columns.Add(
                new XElement(
                    ns + "col",
                    new XAttribute("min", "12"),
                    new XAttribute("max", headers.Length),
                    new XAttribute("width", "30"),
                    new XAttribute("customWidth", "1")
                )
            );

            XElement worksheet =
                new XElement(
                    ns + "worksheet",

                    new XElement(
                        ns + "sheetViews",
                        new XElement(
                            ns + "sheetView",
                            new XAttribute("workbookViewId", "0"),
                            new XElement(
                                ns + "pane",
                                new XAttribute("ySplit", "1"),
                                new XAttribute("topLeftCell", "A2"),
                                new XAttribute("activePane", "bottomLeft"),
                                new XAttribute("state", "frozen")
                            )
                        )
                    ),

                    columns,
                    sheetData,

                    new XElement(
                        ns + "autoFilter",
                        new XAttribute(
                            "ref",
                            "A1:" +
                            GetExcelColumnNameForFailureExport(headers.Length) +
                            lastRow.ToString(CultureInfo.InvariantCulture)
                        )
                    )
                );

            XDocument doc =
                new XDocument(
                    new XDeclaration("1.0", "UTF-8", "yes"),
                    worksheet
                );

            return ToXmlString(doc);
        }


        private XElement BuildInlineStringCell(
            XNamespace ns,
            string cellReference,
            string value,
            int styleIndex)
        {
            XElement text =
                new XElement(
                    ns + "t",
                    value ?? ""
                );

            if (
                !string.IsNullOrEmpty(value)
                &&
                (
                    value.StartsWith(" ", StringComparison.Ordinal)
                    ||
                    value.EndsWith(" ", StringComparison.Ordinal)
                )
            )
            {
                text.SetAttributeValue(
                    XNamespace.Xml + "space",
                    "preserve"
                );
            }

            return
                new XElement(
                    ns + "c",
                    new XAttribute("r", cellReference),
                    new XAttribute("t", "inlineStr"),
                    new XAttribute("s", styleIndex),
                    new XElement(
                        ns + "is",
                        text
                    )
                );
        }


        private void WriteZipTextEntry(
            ZipArchive archive,
            string entryName,
            string content)
        {
            ZipArchiveEntry entry =
                archive.CreateEntry(
                    entryName,
                    CompressionLevel.Optimal
                );

            using (
                Stream stream =
                    entry.Open()
            )
            using (
                StreamWriter writer =
                    new StreamWriter(
                        stream,
                        new UTF8Encoding(false)
                    )
            )
            {
                writer.Write(content);
            }
        }


        private string ToXmlString(
            XDocument document)
        {
            using (
                StringWriter writer =
                    new Utf8StringWriter()
            )
            {
                document.Save(
                    writer,
                    SaveOptions.DisableFormatting
                );

                return writer.ToString();
            }
        }


        private string GetExcelColumnNameForFailureExport(
            int columnNumber)
        {
            string columnName = "";

            while (columnNumber > 0)
            {
                int modulo =
                    (columnNumber - 1) % 26;

                columnName =
                    Convert.ToChar(
                        'A' + modulo
                    )
                    +
                    columnName;

                columnNumber =
                    (columnNumber - modulo) / 26;
            }

            return columnName;
        }


        private sealed class Utf8StringWriter
            : StringWriter
        {
            public override Encoding Encoding
            {
                get
                {
                    return Encoding.UTF8;
                }
            }
        }



        // =========================================================
        // HELPERS
        // =========================================================

        private string MergeSMPAnalysisActions(
            string existingValue,
            string[] additionalValues)
        {
            List<string> values =
                new List<string>();


            if (!string.IsNullOrWhiteSpace(
                existingValue
            ))
            {
                values.Add(
                    existingValue.Trim()
                );
            }


            if (additionalValues != null)
            {
                foreach (
                    string value
                    in additionalValues
                )
                {
                    if (
                        !string.IsNullOrWhiteSpace(
                            value
                        )
                        &&
                        !values.Any(
                            x =>
                                string.Equals(
                                    x,
                                    value.Trim(),
                                    StringComparison.OrdinalIgnoreCase
                                )
                        )
                    )
                    {
                        values.Add(
                            value.Trim()
                        );
                    }
                }
            }


            return
                values.Count == 0
                    ? null
                    : string.Join(
                        Environment.NewLine,
                        values
                    );
        }


        private string SaveFailureAnalysisFile(
            HttpPostedFileBase uploadedFile,
            int delayID)
        {
            const int maximumFileSize =
                10 * 1024 * 1024;

            if (
                uploadedFile == null ||
                uploadedFile.ContentLength <= 0
            )
            {
                return null;
            }

            if (
                uploadedFile.ContentLength >
                maximumFileSize
            )
            {
                throw new Exception(
                    "Attachment size cannot exceed 10 MB."
                );
            }

            string extension =
                Path.GetExtension(
                    uploadedFile.FileName
                );

            extension =
                string.IsNullOrWhiteSpace(extension)
                    ? ""
                    : extension.ToLowerInvariant();

            HashSet<string> allowedExtensions =
                new HashSet<string>(
                    StringComparer.OrdinalIgnoreCase
                )
                {
                    ".pdf",
                    ".doc",
                    ".docx",
                    ".xls",
                    ".xlsx",
                    ".jpg",
                    ".jpeg",
                    ".png"
                };

            if (!allowedExtensions.Contains(extension))
            {
                throw new Exception(
                    "Only PDF, Word, Excel, JPG, JPEG and PNG files are allowed."
                );
            }

            string uploadDirectory =
                Server.MapPath(
                    "~/Uploads/FailureAnalysis"
                );

            if (!Directory.Exists(uploadDirectory))
            {
                Directory.CreateDirectory(
                    uploadDirectory
                );
            }

            string storedFileName =
                "FA-" +
                delayID +
                "-" +
                DateTime.Now.ToString(
                    "yyyyMMddHHmmssfff"
                ) +
                "-" +
                Guid.NewGuid()
                    .ToString("N")
                    .Substring(0, 8) +
                extension;

            string physicalPath =
                Path.Combine(
                    uploadDirectory,
                    storedFileName
                );

            uploadedFile.SaveAs(
                physicalPath
            );

            return
                "~/Uploads/FailureAnalysis/" +
                storedFileName;
        }


        private void DeleteUploadedFile(
            string relativePath)
        {
            if (string.IsNullOrWhiteSpace(
                relativePath
            ))
            {
                return;
            }

            string physicalPath =
                Server.MapPath(
                    relativePath
                );

            if (System.IO.File.Exists(
                physicalPath
            ))
            {
                System.IO.File.Delete(
                    physicalPath
                );
            }
        }

        private string JoinFilterValues(
            string[] values)
        {
            if (
                values == null ||
                values.Length == 0
            )
            {
                return null;
            }


            string result =
                string.Join(
                    ",",
                    values
                        .Where(
                            x =>
                                !string.IsNullOrWhiteSpace(x)
                        )
                        .Select(
                            x => x.Trim()
                        )
                        .Distinct(
                            StringComparer.OrdinalIgnoreCase
                        )
                );


            return
                string.IsNullOrWhiteSpace(result)
                    ? null
                    : result;
        }


        private string GetCurrentUser()
        {
            string currentUser =
                Convert.ToString(
                    Session["UserName"]
                );

            if (
                string.IsNullOrWhiteSpace(
                    currentUser
                ) &&
                User != null &&
                User.Identity != null
            )
            {
                currentUser =
                    User.Identity.Name;
            }

            return
                string.IsNullOrWhiteSpace(
                    currentUser
                )
                    ? "System"
                    : currentUser.Trim();
        }


        [HttpGet]
        public ActionResult countermeasurelist(
        DateTime? fromDate,
        DateTime? toDate,
        string plant,
        string status)
        {
            try
            {
                /*
                 * No date supplied = show all countermeasures.
                 * If user supplies only one side of the range,
                 * the SP handles the open-ended filter.
                 */
                List<CounterMeasureFollowupVM> model =
                    repo.GetFollowupTracker(
                        fromDate,
                        toDate,
                        plant,
                        status
                    );

                ViewBag.FromDate =
                    fromDate.HasValue
                        ? fromDate.Value.ToString(
                            "yyyy-MM-dd"
                        )
                        : "";

                ViewBag.ToDate =
                    toDate.HasValue
                        ? toDate.Value.ToString(
                            "yyyy-MM-dd"
                        )
                        : "";

                ViewBag.Plant =
                    plant ?? "";

                ViewBag.Status =
                    status ?? "";

                return View(
                    model
                );
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    "Unable to load Counter Measure Follow-up Tracker. Error: " +
                    ex.Message;

                return View(
                    new List<CounterMeasureFollowupVM>()
                );
            }
        }

    }
}
