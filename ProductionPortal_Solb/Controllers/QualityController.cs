using BAL.Repositories;
using ClosedXML.Excel;
using DAL.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using Org.BouncyCastle.Asn1.X500;
using ProductionPortal_Solb.App_Start;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using static DAL.Models.ViewModel;

namespace ProductionPortal_Solb.Controllers
{
    [SessionState(
    SessionStateBehavior.ReadOnly
    )]
    public class QualityController : Controller
    {
        QualityRepository repo;
        public QualityController()
        {
            repo = new QualityRepository();
        }
        // GET: ss
        public ActionResult BilletBoard()
        {
            var billets = repo.GetAllBoarding();
            return View("~/Views/Quality/BilletBoard/BilletBoard.cshtml", billets);
        }
        public ActionResult HeatChemistry()
        {
            var chemistry = repo.GetAllChemistry();
            return View("~/Views/Quality/HeatChemistry/HeatChemistry.cshtml", chemistry);
        }
        public ActionResult AddChemistry()
        {
            return View("~/Views/Quality/HeatChemistry/AddChemistry.cshtml");
        }
        [HttpPost]
        public ActionResult AddChemistry(ChemistryInputModel model)
        {
            if (model.data != null)
            {
                foreach (var sampleItem in model.data)
                {
                    HeatChemistryBLL bll = new HeatChemistryBLL()
                    {
                        PlantName = model.PlantName,
                        Date = model.Date,
                        HeatNo = model.HeatNo,
                        SteelGrade = model.Grade,
                        Weight = model.Weight,
                        Area = model.Area,
                        Size = model.Size,
                        Time = model.Time,
                        Shift = model.Shift,

                        SampleNo = sampleItem.SampleNo,
                        C = sampleItem.C,
                        Si = sampleItem.Si,
                        Mn = sampleItem.Mn,
                        P = sampleItem.P,
                        S = sampleItem.S,
                        Ni = sampleItem.Ni,
                        Cr = sampleItem.Cr,
                        Mo = sampleItem.Mo,
                        V = sampleItem.V,
                        Cu = sampleItem.Cu,
                        Ti = sampleItem.Ti,
                        Sn = sampleItem.Sn,
                        Al = sampleItem.Al,
                        Pb = sampleItem.Pb,
                        B = sampleItem.B,
                        Zn = sampleItem.Zn,
                        N = sampleItem.N,
                        MnS = sampleItem.MnS,
                        Ceq = sampleItem.Ceq,

                        StatusID = 1,
                        CreatedDate = DateTime.Now,
                        CreatedBy = User.Identity.Name,
                    };
                    repo.AddHeatChemistry(bll);
                }
            }

            TempData["msg"] = "Heat Chemistry Saved Successfully!";
            return RedirectToAction("~/Views/Quality/HeatChemistry/HeatChemistry.cshtml");
        }
        [HttpGet]
        public ActionResult Chemistrydetails(string heatNo)
        {
            if (string.IsNullOrEmpty(heatNo))
            {
                return RedirectToAction("~/Views/Quality/HeatChemistry/HeatChemistry.cshtml");
            }
            var data = repo.GetChemsitryHeatDetails(heatNo);
            if (data == null || !data.Any())
            {
                TempData["Error"] = $"No records found for Heat # {heatNo}.";
                return RedirectToAction("~/Views/Quality/HeatChemistry/HeatChemistry.cshtml");
            }
            return View("~/Views/Quality/HeatChemistry/Chemistrydetails.cshtml", data);
        }
        [HttpGet]
        public ActionResult AddBillet(int? id)
        {
            try
            {
                var heat = repo.GetAllChemistry()
                    .Where(x =>
                        x.Area == "Rolling Mill 1" ||
                        x.Area == "Rolling Mill 2"
                    )
                    .Select(x => new
                    {
                        x.HeatNo,
                        x.Area
                    })
                    .Distinct()
                    .ToList();

                ViewBag.HeatNo =
                    new SelectList(heat);

                var BilletGradeList =
                    repo.GetBilletGrade();

                ViewBag.BilletGrade =
                    new SelectList(
                        BilletGradeList,
                        "ProductID",
                        "SpecGrade"
                    );

                ViewBag.GradeDataJson =
                    JsonConvert.SerializeObject(
                        BilletGradeList
                    );

                BilletBoardBLL model =
                    new BilletBoardBLL();

                /*
                 * EDIT MODE
                 */
                if (
                    id.HasValue &&
                    id.Value > 0
                )
                {
                    model =
                        repo.GetBilletForEdit(
                            id.Value
                        );

                    if (model == null)
                    {
                        TempData["ErrorMessage"] =
                            "Billet Boarding record not found.";

                        return RedirectToAction(
                            "BilletBoard"
                        );
                    }

                    /*
                     * Existing chemistry rows load
                     */
                    model.Chemistry =
                        repo.GetBilletChemistryForEdit(
                            model.ID
                        );

                    ViewBag.IsEdit =
                        true;
                }
                else
                {
                    ViewBag.IsEdit =
                        false;
                }

                return View(
                    "~/Views/Quality/BilletBoard/AddBillet.cshtml",
                    model
                );
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] =
                    ex.Message;

                return RedirectToAction(
                    "BilletBoard"
                );
            }
        }
        [HttpPost]
        public JsonResult CheckDuplicateHeatNos(
            string[] heatNos,
            int currentID = 0)
        {
            try
            {
                var cleanHeatNos =
                    (heatNos ?? new string[0])
                        .Where(
                            x =>
                                !string.IsNullOrWhiteSpace(
                                    x
                                )
                        )
                        .Select(
                            x =>
                                x.Trim()
                                    .ToUpper()
                        )
                        .Distinct(
                            StringComparer.OrdinalIgnoreCase
                        )
                        .ToList();


                if (
                    cleanHeatNos.Count == 0
                )
                {
                    return Json(
                        new
                        {
                            success = true,
                            duplicates =
                                new string[0]
                        }
                    );
                }


                List<string> duplicates;


                if (
                    currentID > 0
                )
                {
                    duplicates =
                        repo.GetDuplicateHeatNosForEdit(
                            cleanHeatNos,
                            currentID
                        )
                        ??
                        new List<string>();
                }
                else
                {
                    duplicates =
                        repo.GetDuplicateHeatNos(
                            cleanHeatNos
                        )
                        ??
                        new List<string>();
                }


                duplicates =
                    duplicates
                        .Where(
                            x =>
                                !string.IsNullOrWhiteSpace(
                                    x
                                )
                        )
                        .Select(
                            x => x.Trim()
                        )
                        .Distinct(
                            StringComparer.OrdinalIgnoreCase
                        )
                        .ToList();


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
                        message =
                            "Duplicate Heat validation failed: "
                            +
                            ex.Message
                    }
                );
            }
        }

        [HttpGet]
        public ActionResult Boardingdetails(int id)
        {
            var data = repo.GetBilletDetails(id);
            return View("~/Views/Quality/BilletBoard/Boardingdetails.cshtml", data);
        }
        public JsonResult GetChemistryByHeat(string heatNo)
        {
            var data = repo.GetAllChemistry()
                           .Where(x => x.HeatNo == heatNo)
                           .OrderBy(x => x.NoOfBillets)
                           .ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult QCInspectionRM()
        {
            return View("~/Views/Quality/RMMechanical/list.cshtml");
        }
        [HttpGet]
        public ActionResult QCInspectionRMadd(
            string rm = "RM1",
            int? inspectionID = null,
            int? boardingID = null,
            string mtcHeatNo = null)
        {
            var model =
                new QCInspectionRMPageVM();

            model.SelectedRM =
                string.IsNullOrWhiteSpace(rm)
                    ? "RM1"
                    : rm.Trim();

            model.BilletBoardingRows =
                repo.GetBilletBoardingRows(
                    model.SelectedRM
                )
                ?? new List<QCBilletBoardingRowBLL>();

            model.MTCRows =
                repo.GetMTCRows(
                    mtcHeatNo
                )
                ?? new List<QCMTCRowBLL>();

            if (
                inspectionID.HasValue &&
                inspectionID.Value > 0
            )
            {
                model.Detail =
                    repo.GetQCInspectionRMByID(
                        inspectionID.Value
                    )
                    ?? new QCInspectionRMDetailBLL();

                model.Detail.ID =
                    inspectionID.Value;
            }
            else if (
                boardingID.HasValue &&
                boardingID.Value > 0
            )
            {
                model.Detail =
                    repo.GetQCInspectionRMFromBoarding(
                        boardingID.Value
                    )
                    ?? new QCInspectionRMDetailBLL();
            }
            else
            {
                model.Detail =
                    new QCInspectionRMDetailBLL
                    {
                        ProductionDate =
                            DateTime.Today.ToString(
                                "dd-MM-yyyy"
                            ),

                        ProductionShift =
                            "Morning",

                        DatabaseServer =
                            @"10.1.10.115\PROD01",

                        GaugeLength =
                            "200",

                        YieldStrength =
                            "0.0",

                        TensileStrength =
                            "0.0",

                        TensileYieldRatio =
                            "0.0",

                        Elongation =
                            "0.0"
                    };
            }

            return View("~/Views/Quality/RMMechanical/add.cshtml",
            model
            );
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult QCInspectionRMadd(
            QCInspectionRMDetailBLL model)
        {
            if (model == null)
            {
                TempData["ErrorMessage"] =
                    "Invalid QC inspection data.";

                return RedirectToAction(
                    "QCInspectionRMadd"
                );
            }

            try
            {
                model.CreatedBy =
                    User != null &&
                    User.Identity != null
                        ? User.Identity.Name
                        : "";

                model.CreatedDate =
                    DateTime.Now;

                int savedID =
                    repo.SaveQCInspectionRM(
                        model
                    );

                if (savedID <= 0)
                {
                    TempData["ErrorMessage"] =
                        "QC inspection record was not saved.";

                    return RedirectToAction(
                        "QCInspectionRMadd",
                        new
                        {
                            rm = model.Site
                        }
                    );
                }

                TempData["SuccessMessage"] =
                    model.ID > 0
                        ? "QC inspection data updated successfully."
                        : "QC inspection data saved successfully.";

                // Redirect ke baad GET dobara chalega.
                // GetMTCRows() ab QCInspectionRM se saved data load karega.
                return RedirectToAction(
                    "QCInspectionRMadd",
                    new
                    {
                        inspectionID = savedID,
                        rm = model.Site
                    }
                );
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] =
                    "Unable to save QC inspection record. " +
                    ex.Message;

                return RedirectToAction(
                    "QCInspectionRMadd",
                    new
                    {
                        inspectionID =
                            model.ID > 0
                                ? (int?)model.ID
                                : null,

                        rm =
                            model.Site
                    }
                );
            }
        }
        [HttpGet]
        public JsonResult GetBoardingDetails(
            int boardingID)
        {
            try
            {
                var data =
                    repo.GetQCInspectionRMFromBoarding(
                        boardingID
                    );

                return Json(
                    new
                    {
                        success =
                            data != null,

                        data =
                            data
                    },
                    JsonRequestBehavior.AllowGet
                );
            }
            catch (Exception ex)
            {
                Response.StatusCode =
                    500;

                return Json(
                    new
                    {
                        success =
                            false,

                        message =
                            ex.Message
                    },
                    JsonRequestBehavior.AllowGet
                );
            }
        }

        [HttpGet]
        public JsonResult GetMTCDetails(
            int mtcID)
        {
            try
            {
                if (mtcID <= 0)
                {
                    Response.StatusCode = 400;

                    return Json(
                        new
                        {
                            success = false,
                            message =
                                "A valid MTC record must be selected."
                        },
                        JsonRequestBehavior.AllowGet
                    );
                }

                /*
                 * Single repository call.
                 * Product + Mechanical = QCInspectionRM
                 * Chemistry = RMChemicalAnalysis
                 */
                var data =
                    repo.GetMTCDetails(
                        mtcID
                    );

                if (data == null)
                {
                    Response.StatusCode = 404;

                    return Json(
                        new
                        {
                            success = false,
                            message =
                                "The selected MTC record was not found."
                        },
                        JsonRequestBehavior.AllowGet
                    );
                }

                return Json(
                    new
                    {
                        success = true,
                        data = data
                    },
                    JsonRequestBehavior.AllowGet
                );
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;

                return Json(
                    new
                    {
                        success = false,
                        message =
                            "The selected MTC data could not be loaded. "
                            + ex.Message
                    },
                    JsonRequestBehavior.AllowGet
                );
            }
        }


        [HttpGet]
        public JsonResult GetMTCRowsJson(
            string heatNo = null)
        {
            try
            {
                var rows =
                    repo.GetMTCRows(
                        heatNo
                    )
                    ?? new List<QCMTCRowBLL>();

                /*
                 * Remove duplicate MTC rows by HeatNo, NOT by ID.
                 * Latest QCInspectionRM.ID is kept for each heat.
                 */
                rows =
                    rows
                        .Where(
                            x =>
                                x != null
                                &&
                                !string.IsNullOrWhiteSpace(
                                    x.HeatNo
                                )
                        )
                        .GroupBy(
                            x => x.HeatNo.Trim(),
                            StringComparer.OrdinalIgnoreCase
                        )
                        .Select(
                            g =>
                                g
                                    .OrderByDescending(
                                        x => x.ID
                                    )
                                    .First()
                        )
                        .OrderByDescending(
                            x => x.ID
                        )
                        .ToList();

                return Json(
                    new
                    {
                        success = true,
                        data = rows
                    },
                    JsonRequestBehavior.AllowGet
                );
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;

                return Json(
                    new
                    {
                        success = false,
                        message =
                            "MTC data could not be loaded. "
                            + ex.Message
                    },
                    JsonRequestBehavior.AllowGet
                );
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteQCInspectionRM(
            int id)
        {
            try
            {
                int affected =
                    repo.DeleteQCInspectionRM(
                        id,
                        User.Identity.Name
                    );

                TempData[
                    affected > 0
                        ? "SuccessMessage"
                        : "ErrorMessage"
                ] =
                    affected > 0
                        ? "QC inspection record deleted successfully."
                        : "QC inspection record was not deleted.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] =
                    ex.Message;
            }

            return RedirectToAction(
                "QCInspectionRMadd"
            );
        }

        public ActionResult InspectionRMlist()
        {
            return View("~/Views/Quality/QCInspectionRM/InspectionRMlist.cshtml");
        }

        public ActionResult AddInspectionRM()
        {
            return View("~/Views/Quality/QCInspectionRM/AddInspectionRM.cshtml");
        }
        [HttpPost]
        public ActionResult AddInspectionRM(RMQCInspectionBLL model)
        {
            // 🔥 Auto Calculation (IMPORTANT)
            model.Accepted = model.TotalBundles - model.OnHold - model.Rejected;

            model.CreatedOn = DateTime.Now;
            model.CreatedBy = User.Identity.Name;
            model.StatusID = 1;

            bool isSaved = repo.SaveQCInspection(model);

            if (isSaved)
            {
                TempData["Success"] = "QC Record Saved Successfully";
            }
            else
            {
                TempData["Error"] = "Error while saving";
            }

            return RedirectToAction("Index");
        }
        public ActionResult Inspectionlist()
        {
            return View("~/Views/Quality/QCInspectionData/Inspectionlist.cshtml");
        }

        public ActionResult AddInspection()
        {
            return View("~/Views/Quality/QCInspectionData/AddInspection.cshtml");
        }

        [Route("SlagByProduct")]
        public ActionResult SlagByProductList()
        {
            var data = repo.GetSlagByProduct();
            return View("~/Views/Quality/SlagByProduct/SlagByProductList.cshtml", data);
        }

        [Route("SlagByProductDetail")]
        public ActionResult SlagByProductDetail(int id)
        {
            var model = repo.GetSlagByID(id);

            if (model == null)
                return HttpNotFound();

            // Load samples
            model.Samples = repo.GetSlagSamplesById(id);

            return View("~/Views/Quality/SlagByProduct/SlagByProductDetail.cshtml", model);
        }


        [Route("AddSlagByProduct")]
        public ActionResult AddSlagByProduct(int? id)
        {
            SlagByProductAnalysisBLL model;

            if (id == null)
            {
                // ✅ ADD MODE
                model = new SlagByProductAnalysisBLL();
                model.Samples = new List<SlagSampleAnalysisBLL>(); // VERY IMPORTANT
            }
            else
            {
                // ✅ EDIT MODE
                model = repo.GetSlagByID(id.Value);

                if (model == null)
                    model = new SlagByProductAnalysisBLL();

                model.Samples = repo.GetSlagSamplesById(id.Value)
                                    ?? new List<SlagSampleAnalysisBLL>();
            }

            return View("~/Views/Quality/SlagByProduct/AddSlagByProduct.cshtml", model); // ❌ NEVER return View() alone
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSlagByProduct(SlagByProductAnalysisBLL data)
        {
            if (data == null)
            {
                TempData["ErrorMessage"] = "Invalid data submitted.";
                return RedirectToAction("SlagByProductList");
            }

            if (data.Samples == null)
                data.Samples = new List<SlagSampleAnalysisBLL>();

            data.HeatNo = (data.HeatNo ?? string.Empty).Trim();
            data.CertificateNo = (data.CertificateNo ?? string.Empty).Trim();
            data.ByProductType = (data.ByProductType ?? string.Empty).Trim();

            if (!data.DateOfProduction.HasValue)
                return SlagSaveError(data, "Production Date is required.");

            if (string.IsNullOrWhiteSpace(data.HeatNo))
                return SlagSaveError(data, "Heat No is required.");

            if (string.IsNullOrWhiteSpace(data.ByProductType))
                return SlagSaveError(data, "By-Product Type is required.");

            var activeSamples = data.Samples
                .Where(x => x != null && !IsEmptySlagSample(x))
                .ToList();

            foreach (var sample in activeSamples)
            {
                sample.SampleCode = (sample.SampleCode ?? string.Empty).Trim();
            }

            var duplicateSampleCodes = activeSamples
                .Where(x => !string.IsNullOrWhiteSpace(x.SampleCode))
                .GroupBy(
                    x => x.SampleCode,
                    StringComparer.OrdinalIgnoreCase
                )
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .OrderBy(x => x)
                .ToList();

            if (duplicateSampleCodes.Any())
            {
                return SlagSaveError(
                    data,
                    "Duplicate Sample Code(s): " +
                    string.Join(", ", duplicateSampleCodes)
                );
            }

            /*
             * One active entry is allowed for the same Production Date,
             * Heat No and By-Product Type. In Edit, the current ID is excluded.
             */
            bool duplicateEntry = repo.IsSlagByProductDuplicate(
                data.DateOfProduction.Value,
                data.HeatNo,
                data.ByProductType,
                data.ID
            );

            if (duplicateEntry)
            {
                return SlagSaveError(
                    data,
                    "This Production Date, Heat No and By-Product Type already exists."
                );
            }

            DateTime now = DateTime.Now;
            string currentUser = User.Identity.Name;

            // ============================
            // ADD MODE
            // ============================
            if (data.ID <= 0)
            {
                data.StatusID = 1;
                data.CreatedDate = now;
                data.CreatedBy = currentUser;

                int newID = repo.InsertSlagByProduct(data);

                // -1 means the transaction-level DB duplicate check blocked it.
                if (newID == -1)
                {
                    return SlagSaveError(
                        data,
                        "Duplicate entry blocked. This record already exists."
                    );
                }

                if (newID <= 0)
                {
                    return SlagSaveError(
                        data,
                        "Data not saved. Please try again."
                    );
                }

                foreach (var item in activeSamples)
                {
                    repo.InsertSlagSample(
                        BuildSlagSample(
                            item,
                            newID,
                            currentUser,
                            now
                        )
                    );
                }

                TempData["SuccessMessage"] = "Data saved successfully";
                return RedirectToAction("SlagByProductList");
            }

            // ============================
            // EDIT MODE
            // ============================
            var existing = repo.GetSlagByID(data.ID);

            if (existing == null || existing.ID <= 0)
            {
                return SlagSaveError(data, "Record not found.");
            }

            existing.DateOfProduction = data.DateOfProduction;
            existing.DateOfAnalysis = data.DateOfAnalysis;
            existing.HeatNo = data.HeatNo;
            existing.CertificateNo = data.CertificateNo;
            existing.ByProductType = data.ByProductType;
            existing.StatusID = 1;
            existing.UpdatedDate = now;
            existing.UpdatedBy = currentUser;

            int updateResult = repo.UpdateSlagByProduct(existing);

            if (updateResult == -1)
            {
                return SlagSaveError(
                    data,
                    "Duplicate entry blocked. This record already exists."
                );
            }

            if (updateResult <= 0)
            {
                return SlagSaveError(
                    data,
                    "Data not updated. Please try again."
                );
            }

            /*
             * Existing active samples are soft-deleted and the posted sample
             * list is inserted again. This makes removed and edited rows match
             * the form exactly without leaving duplicate active samples.
             */
            repo.DeleteSlagSamplesBySlagID(
                existing.ID,
                currentUser
            );

            foreach (var item in activeSamples)
            {
                repo.InsertSlagSample(
                    BuildSlagSample(
                        item,
                        existing.ID,
                        currentUser,
                        now
                    )
                );
            }

            TempData["SuccessMessage"] = "Data updated successfully";
            return RedirectToAction("SlagByProductList");
        }

        private ActionResult SlagSaveError(
            SlagByProductAnalysisBLL data,
            string message)
        {
            ModelState.AddModelError(string.Empty, message);

            if (data.Samples == null)
                data.Samples = new List<SlagSampleAnalysisBLL>();

            return View(
                "~/Views/Quality/SlagByProduct/AddSlagByProduct.cshtml",
                data
            );
        }

        private static bool IsEmptySlagSample(
            SlagSampleAnalysisBLL item)
        {
            return string.IsNullOrWhiteSpace(item.SampleCode)
                && item.SampleTime == null
                && item.CaO == null
                && item.MgO == null
                && item.SiO2 == null
                && item.Al2O3 == null
                && item.Fe2O3 == null
                && item.S == null
                && item.MnO == null
                && item.Cr2O3 == null
                && item.P2O5 == null
                && item.V2O5 == null
                && item.TiO2 == null
                && item.ZnO == null
                && item.TotalFe == null
                && item.Basicity4 == null
                && string.IsNullOrWhiteSpace(item.Comment);
        }

        private static SlagSampleAnalysisBLL BuildSlagSample(
            SlagSampleAnalysisBLL source,
            int slagID,
            string currentUser,
            DateTime now)
        {
            return new SlagSampleAnalysisBLL
            {
                SampleCode = source.SampleCode,
                SampleTime = source.SampleTime,
                CaO = source.CaO,
                MgO = source.MgO,
                SiO2 = source.SiO2,
                Al2O3 = source.Al2O3,
                Fe2O3 = source.Fe2O3,
                S = source.S,
                MnO = source.MnO,
                Cr2O3 = source.Cr2O3,
                P2O5 = source.P2O5,
                V2O5 = source.V2O5,
                TiO2 = source.TiO2,
                ZnO = source.ZnO,
                TotalFe = source.TotalFe,
                Basicity4 = source.Basicity4,
                Comment = source.Comment,
                SlagID = slagID,
                StatusID = 1,
                CreatedDate = now,
                CreatedBy = currentUser
            };
        }

        [Route("SlagByProductDelete")]
        public ActionResult SlagByProductDelete(int id)
        {
            var UpdatedBy = User.Identity.Name;
            int rtn = repo.SlagByProductDelete(id, UpdatedBy);
            int rtn1 = repo.DeleteSlagSamplesBySlagID(id, UpdatedBy);
            TempData["SuccessMessage"] = "Data Delete Successfully";

            return RedirectToAction("SlagByProductList");
        }


        [Route("HBI/DRIAnalysis")]
        public ActionResult HBIDRIlist()
        {
            var data = repo.GetDRIHBIAnalysis();
            return View("~/Views/Quality/HBIDRIAnalysis/HBIDRIlist.cshtml", data);
        }

        [Route("AddHBI/DRIAnalysis")]
        public ActionResult AddHBIDRIAnalysis(int? id)
        {
            QCHBIDRIAnalysisBLL model;

            if (id == null)
            {
                // ✅ ADD MODE
                model = new QCHBIDRIAnalysisBLL();

                // 🔑 VERY IMPORTANT
                model.Samples = new List<SampleHBIDRIBLL>
                {
                    new SampleHBIDRIBLL() // at least 1 row
                };
            }
            else
            {
                // ✅ EDIT MODE
                model = repo.GetDRIHBIDetailByID(id);

                if (model == null)
                {
                    model = new QCHBIDRIAnalysisBLL();
                }

                if (model.Samples == null)
                {
                    model.Samples = new List<SampleHBIDRIBLL>();
                }
            }

            return View("~/Views/Quality/HBIDRIAnalysis/AddHBIDRIAnalysis.cshtml", model);
        }

        [HttpPost]
        public ActionResult AddHBIDRIAnalysis(QCHBIDRIAnalysisBLL data)
        {
            if (data == null)
            {
                TempData["ErrorMessage"] = "Invalid data submitted.";
                return RedirectToAction("HBIDRIlist");
            }

            // ✅ Ensure samples list not null
            if (data.Samples == null)
                data.Samples = new List<SampleHBIDRIBLL>();

            // ============================
            // ✅ ADD MODE (Insert)
            // ============================
            if (data.ID == 0)
            {
                data.StatusID = 1;
                data.CreatedDate = DateTime.Now;
                data.CreatedBy = User.Identity.Name;

                int newID = repo.InsertDRIAnalysisData(data);   // returns newly inserted SlagID

                if (newID <= 0)
                {
                    TempData["ErrorMessage"] = "Data not saved. Please try again.";
                    return RedirectToAction("HBIDRIlist");
                }

                foreach (var item in data.Samples)
                {
                    // optional: skip empty rows
                    if (string.IsNullOrWhiteSpace(item.SampleCode) &&
                        item.CaO == null && item.MgO == null && item.SiO2 == null)
                        continue;

                    var bll = new SampleHBIDRIBLL
                    {
                        SampleCode = item.SampleCode,
                        FeTotal = item.FeTotal,
                        FeMetallic = item.FeMetallic,
                        Metallization = item.Metallization,
                        C = item.C,
                        S = item.S,
                        P = item.P,
                        SiO2 = item.SiO2,
                        Al2O3 = item.Al2O3,
                        MgO = item.MgO,
                        CaO = item.CaO,
                        TotalGangue = item.TotalGangue,
                        GrainSize = item.GrainSize,
                        Comment = item.Comment,

                        AnalysisID = newID,
                        StatusID = 1,
                        CreatedDate = data.CreatedDate,
                        CreatedBy = data.CreatedBy
                    };

                    repo.AddDRISample(bll);
                }

                TempData["SuccessMessage"] = "Data saved successfully";
                return RedirectToAction("HBIDRIlist");
            }

            // ============================
            // ✅ EDIT MODE (Update)
            // ============================
            else
            {
                // 1) Fetch existing master
                var existing = repo.GetDRIHBIDetailByID(data.ID);   // ✅ create this repo method
                if (existing == null)
                {
                    TempData["ErrorMessage"] = "Record not found.";
                    return RedirectToAction("HBIDRIlist");
                }

                // 2) Update master fields (map only those you allow)
                existing.ReceivingDate = data.ReceivingDate;
                existing.Material = data.Material;
                existing.ShipmentCodeNo = data.ShipmentCodeNo;
                existing.Supplier = data.Supplier;
                existing.Quantity = data.Quantity;
                existing.AnalysisDate = data.AnalysisDate;
                existing.ReceivedQuantity = data.ReceivedQuantity;
                existing.ReferenceNo = data.ReferenceNo;
                existing.PhysicalAnalysis = data.PhysicalAnalysis;

                existing.StatusID = 1;
                existing.UpdatedDate = DateTime.Now;     // ✅ add in model if not present
                existing.UpdatedBy = User.Identity.Name; // ✅ add in model if not present

                int upd = repo.UpdateDRIAnalysisData(existing); // ✅ create this repo method

                if (upd != 0)
                {
                    TempData["ErrorMessage"] = "Data not updated. Please try again.";
                    return RedirectToAction("HBIDRIlist");
                }
                existing.UpdatedBy = User.Identity.Name;
                // 3) Replace samples
                // ✅ OPTION A: Hard delete previous samples then insert new
                repo.DeleteDRISamplesByID(existing.ID, existing.UpdatedBy); // ✅ create this method (recommended)

                foreach (var item in data.Samples)
                {
                    // optional: skip empty rows
                    if (string.IsNullOrWhiteSpace(item.SampleCode) &&
                        item.CaO == null && item.MgO == null && item.SiO2 == null)
                        continue;

                    var bll = new SampleHBIDRIBLL
                    {
                        SampleCode = item.SampleCode,
                        FeTotal = item.FeTotal,
                        FeMetallic = item.FeMetallic,
                        Metallization = item.Metallization,
                        C = item.C,
                        S = item.S,
                        P = item.P,
                        SiO2 = item.SiO2,
                        Al2O3 = item.Al2O3,
                        MgO = item.MgO,
                        CaO = item.CaO,
                        TotalGangue = item.TotalGangue,
                        GrainSize = item.GrainSize,
                        Comment = item.Comment,

                        AnalysisID = data.ID,
                        StatusID = 1,
                        CreatedDate = data.CreatedDate,
                        CreatedBy = data.CreatedBy
                    };

                    repo.AddDRISample(bll);
                }

                TempData["SuccessMessage"] = "Data updated successfully";
                return RedirectToAction("HBIDRIlist");
            }
        }

        [Route("HBI/DRIAnalysisDetail")]
        public ActionResult HBIDRIAnalysisDetail(int id)
        {
            var model = repo.GetDRIHBIDetailByID(id);

            if (model == null)
                return HttpNotFound();

            //// Load samples
            //model.Samples = repo.GetDRIHBIDetailByID(id);

            return View("~/Views/Quality/HBIDRIAnalysis/HBIDRIAnalysisDetail.cshtml", model);
        }

        [Route("HBI/DRIAnalysisDelete")]
        public ActionResult HBIDRIAnalysisDelete(int id)
        {
            var UpdatedBy = User.Identity.Name;
            int rtn = repo.DeleteHBIDRIAnalysis(id, UpdatedBy);
            int rtn1 = repo.DeleteDRISamplesByID(id, UpdatedBy);
            TempData["SuccessMessage"] = "Data Delete Successfully";

            return RedirectToAction("HBIDRIlist");
        }

        public ActionResult SlagByProductPDF(DateTime? from, DateTime? to)
        {
            DateTime fromDate = from ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime toDate = to ?? DateTime.Now;

            // ✅ include whole last day
            DateTime toInclusive = toDate.Date.AddDays(1);

            var vm = new SlagByProductPDFVM
            {
                SlagData = repo.GetSlagByProductByDate(fromDate.Date, toInclusive),
                Samples = repo.GetSlagSamplesByDate(fromDate.Date, toInclusive),
                FromDate = fromDate,
                ToDate = toDate
            };

            return View(
                "~/Views/Quality/SlagByProduct/SlagByProductPDF.cshtml",
                vm
            );
        }

        public ActionResult HBIDRIAnalysisPDF(DateTime? from, DateTime? to)
        {
            DateTime fromDate = from ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime toDate = to ?? DateTime.Now;

            // ✅ include whole last day
            DateTime toInclusive = toDate.Date.AddDays(1);

            var vm = new HBIDRIAnalysisPDFVM
            {
                HBIDRIData = repo.GetHBDRIAnalysisByDate(fromDate.Date, toInclusive),
                Samples = repo.GetHBDRISamplesByDate(fromDate.Date, toInclusive),
                FromDate = fromDate,
                ToDate = toDate
            };

            return View(
                "~/Views/Quality/SlagByProduct/SlagByProductPDF.cshtml",
                vm
            );
        }
        public ActionResult BilletBoardPDF(DateTime? from, DateTime? to)
        {
            DateTime fromDate = from ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime toDate = to ?? DateTime.Now;

            // ✅ include whole last day
            DateTime toInclusive = toDate.Date.AddDays(1);


            var vm = new BilletBoardingPDFVM
            {
                BilletBoards = repo.GetBilletBoardingByDate(fromDate.Date, toInclusive),
                Samples = repo.GetHeatChemistryByDate(fromDate.Date, toInclusive),
                FromDate = fromDate,
                ToDate = toDate
            };

            return View(
                "~/Views/Quality/BilletBoard/BilletBoardingPDF.cshtml",
                vm
            );
        }

        public ActionResult castmillCertificate()
        {
            var billets = repo.GetAllBoarding();
            return View("~/Views/Quality/CastMillCertificate/castmillCertificate.cshtml", billets);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GenerateMTC(
    QCInspectionRMDetailBLL model)
        {
            try
            {
                if (model == null)
                {
                    throw new Exception(
                        "MTC data was not received."
                    );
                }

                if (model.MTCID <= 0)
                {
                    throw new Exception(
                        "Please select an MTC record."
                    );
                }

                if (
                    string.IsNullOrWhiteSpace(
                        model.HeatNo
                    )
                )
                {
                    throw new Exception(
                        "Heat number is required."
                    );
                }

                string templatePath =
                    Server.MapPath(
                        "~/Templates/MTCTemplate.xlsx"
                    );

                if (!System.IO.File.Exists(templatePath))
                {
                    throw new FileNotFoundException(
                        "MTC Excel template was not found.",
                        templatePath
                    );
                }

                using (
                    XLWorkbook workbook =
                        new XLWorkbook(
                            templatePath
                        )
                )
                {
                    IXLWorksheet sheet =
                        workbook.Worksheet(1);

                    /*
                     * Header information
                     */
                    sheet.Cell("B7").Value =
                        model.Specification ?? "";

                    sheet.Cell("E7").Value =
                        model.SteelGrade ?? "";

                    sheet.Cell("N10").Value =
                        DateTime.Today;

                    sheet.Cell("N10")
                        .Style
                        .DateFormat
                        .Format =
                        "dd-MMM-yyyy";

                    /*
                     * First MTC result row
                     */
                    int rowNo = 16;

                    sheet.Cell(
                        rowNo,
                        2
                    ).Value =
                        model.BarSize;

                    sheet.Cell(
                        rowNo,
                        3
                    ).Value =
                        model.NominalWeight;

                    sheet.Cell(
                        rowNo,
                        4
                    ).Value =
                        model.IsWireRodOrCoil
                            ? "Wire Rod / Coil"
                            : "Deformed Steel Bar";

                    sheet.Cell(
                        rowNo,
                        5
                    ).Value =
                        model.HeatNo ?? "";

                    sheet.Cell(
                        rowNo,
                        6
                    ).Value =
                        model.YieldStrength;

                    sheet.Cell(
                        rowNo,
                        7
                    ).Value =
                        model.TensileStrength;

                    sheet.Cell(
                        rowNo,
                        8
                    ).Value =
                        model.TensileYieldRatio;

                    sheet.Cell(
                        rowNo,
                        9
                    ).Value =
                        model.Elongation;

                    sheet.Cell(
                        rowNo,
                        10
                    ).Value =
                        model.BendTestObserved
                            ? "Satisfactory"
                            : "";

                    /*
                     * Chemical analysis
                     */
                    sheet.Cell(
                        rowNo,
                        11
                    ).Value =
                        model.C;

                    sheet.Cell(
                        rowNo,
                        12
                    ).Value =
                        model.Si;

                    sheet.Cell(
                        rowNo,
                        13
                    ).Value =
                        model.Mn;

                    sheet.Cell(
                        rowNo,
                        14
                    ).Value =
                        model.P;

                    sheet.Cell(
                        rowNo,
                        15
                    ).Value =
                        model.S;

                    /*
                     * P16 = Cu
                     * Q16 = V
                     * R16 = B
                     *
                     * Current QC BLL mein ye properties available
                     * nahi hain, isliye filhal blank rakhe hain.
                     */
                    sheet.Cell(
                        rowNo,
                        16
                    ).Value = "";

                    sheet.Cell(
                        rowNo,
                        17
                    ).Value = "";

                    sheet.Cell(
                        rowNo,
                        18
                    ).Value = "";

                    sheet.Cell(
                        rowNo,
                        19
                    ).Value =
                        model.N;

                    /*
                     * CE% template mein T:W merged area hai.
                     * Merged range ka first cell T16 hota hai.
                     */
                    sheet.Cell(
                        rowNo,
                        20
                    ).Value =
                        model.Ceq;

                    /*
                     * Generated by footer
                     */
                    sheet.Cell("B49").Value =
                        "MTC generated using : "
                        + GetCurrentUser()
                        + "  "
                        + DateTime.Now.ToString(
                            "dd/MM/yyyy hh:mm:ss tt",
                            CultureInfo.InvariantCulture
                        );

                    string safeHeatNo =
                        MakeSafeFileName(
                            model.HeatNo
                        );

                    string fileName =
                        "MTC_"
                        + safeHeatNo
                        + "_"
                        + DateTime.Now.ToString(
                            "yyyyMMddHHmmss"
                        )
                        + ".xlsx";

                    using (
                        MemoryStream stream =
                            new MemoryStream()
                    )
                    {
                        workbook.SaveAs(
                            stream
                        );

                        byte[] fileBytes =
                            stream.ToArray();

                        return File(
                            fileBytes,
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            fileName
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] =
                    ex.Message;

                return RedirectToAction(
                    "QCInspectionRMadd",
                    new
                    {
                        rm =
                            model != null
                                ? model.RollingMill
                                : "RM1"
                    }
                );
            }
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
        private string MakeSafeFileName(
    string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "UnknownHeat";
            }

            foreach (
                char invalidCharacter
                in Path.GetInvalidFileNameChars()
            )
            {
                value =
                    value.Replace(
                        invalidCharacter,
                        '_'
                    );
            }

            return value.Trim();
        }

        [HttpGet]
        public ActionResult EditBillet(int id)
        {
            try
            {
                if (id <= 0)
                {
                    TempData["ErrorMessage"] =
                        "Invalid billet boarding record.";

                    return RedirectToAction(
                        "BilletBoard"
                    );
                }

                BilletBoardBLL model =
                    repo.GetBilletDetails(id);

                if (model == null)
                {
                    TempData["ErrorMessage"] =
                        "Billet boarding record not found.";

                    return RedirectToAction(
                        "BilletBoard"
                    );
                }

                return View(
                    "EditBillet",
                    model
                );
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] =
                    ex.Message;

                return RedirectToAction(
                    "BilletBoard"
                );
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditBillet(
    BilletBoardBLL model)
        {
            try
            {
                if (
                    model == null ||
                    model.ID <= 0
                )
                {
                    throw new Exception(
                        "Invalid billet boarding record."
                    );
                }

                model.UpdatedBy =
                    GetCurrentUser();

                model.UpdatedDate =
                    DateTime.Now;

                repo.UpdateBillet(
                    model
                );

                TempData["SuccessMessage"] =
                    "Billet boarding record updated successfully.";

                return RedirectToAction(
                    "BilletBoard"
                );
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(
                    "",
                    ex.Message
                );

                return View(
                    "EditBillet",
                    model
                );
            }
        }

        private void LoadDropdowns(
    string selectedPlant = null,
    string selectedShift = null)
        {
            List<SelectListItem> plantList =
                new List<SelectListItem>
                {
            new SelectListItem
            {
                Text = "RM1",
                Value = "RM1"
            },

            new SelectListItem
            {
                Text = "RM2",
                Value = "RM2"
            }
                };

            List<SelectListItem> shiftList =
                new List<SelectListItem>
                {
            new SelectListItem
            {
                Text = "Morning",
                Value = "Morning"
            },

            new SelectListItem
            {
                Text = "Evening",
                Value = "Evening"
            },

            new SelectListItem
            {
                Text = "Night",
                Value = "Night"
            }
                };

            ViewBag.PlantList =
                new SelectList(
                    plantList,
                    "Value",
                    "Text",
                    selectedPlant
                );

            ViewBag.ShiftList =
                new SelectList(
                    shiftList,
                    "Value",
                    "Text",
                    selectedShift
                );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddBillet(BilletBoardBLL data)
        {
            bool isEdit = data != null && data.ID > 0;

            try
            {
                if (data == null)
                    return BilletSaveError("Invalid data.", false, 0);

                data.BilletBoarding =
                    (data.BilletBoarding ?? string.Empty).Trim();

                if (string.IsNullOrWhiteSpace(data.BilletBoarding))
                    return BilletSaveError(
                        "Billet Boarding number is required.",
                        isEdit,
                        data.ID
                    );

                var chemistry =
                    (data.Chemistry ?? new List<RMChemicalAnalysisBLL>())
                        .Where(x =>
                            x != null &&
                            !string.IsNullOrWhiteSpace(x.HeatNo))
                        .ToList();

                if (!chemistry.Any())
                    return BilletSaveError(
                        "At least one Chemical Analysis Heat No is required.",
                        isEdit,
                        data.ID
                    );

                foreach (var item in chemistry)
                    item.HeatNo = item.HeatNo.Trim();

                // Duplicate inside the current form.
                var formDuplicateHeats = chemistry
                    .GroupBy(x => x.HeatNo, StringComparer.OrdinalIgnoreCase)
                    .Where(x => x.Count() > 1)
                    .Select(x => x.Key)
                    .ToList();

                if (formDuplicateHeats.Any())
                    return BilletSaveError(
                        "Duplicate Heat No(s) entered: " +
                        string.Join(", ", formDuplicateHeats),
                        isEdit,
                        data.ID
                    );

                var heatNos = chemistry
                    .Select(x => x.HeatNo)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                var oldChemistry = new List<RMChemicalAnalysisBLL>();

                if (isEdit)
                {
                    oldChemistry =
                        repo.GetBilletChemistryForEdit(data.ID)
                        ?? new List<RMChemicalAnalysisBLL>();

                    oldChemistry = oldChemistry
                        .Where(x => x != null && x.StatusID != 3)
                        .ToList();

                    var allowedIDs = new HashSet<int>(
                        oldChemistry.Select(x => x.ID)
                    );

                    // Do not allow a chemistry ID from another boarding.
                    bool invalidIDPosted = chemistry.Any(x =>
                        x.ID > 0 && !allowedIDs.Contains(x.ID));

                    if (invalidIDPosted)
                        return BilletSaveError(
                            "Invalid Chemical Analysis record submitted.",
                            true,
                            data.ID
                        );
                }

                // Boarding number must be unique; Edit excludes the current group.
                bool boardingDuplicate = !isEdit
                    ? repo.IsBilletBoardingExists(data.BilletBoarding)
                    : repo.IsBilletBoardingExistsForEdit(
                        data.BilletBoarding,
                        data.ID
                      );

                if (boardingDuplicate)
                    return BilletSaveError(
                        "This Billet Boarding number already exists.",
                        isEdit,
                        data.ID
                    );

                /*
                 * DB Heat duplicate check ONLY in RMChemicalAnalysis.
                 * StatusID = 3 is ignored.
                 * Edit excludes all chemistry IDs belonging to the current boarding,
                 * so an unchanged existing Heat does not duplicate itself.
                 */
                var excludedIDs = isEdit
                    ? oldChemistry.Select(x => x.ID).ToList()
                    : new List<int>();

                // In Edit, unchanged existing Heat Nos do not need another DB check.
                // Validate only newly added rows or rows whose Heat No was changed.
                var heatNosForDatabaseCheck = !isEdit
                    ? heatNos
                    : chemistry
                        .Where(x =>
                            x.ID <= 0 ||
                            oldChemistry.Any(old =>
                                old.ID == x.ID &&
                                !string.Equals(
                                    (old.HeatNo ?? string.Empty).Trim(),
                                    x.HeatNo,
                                    StringComparison.OrdinalIgnoreCase
                                )
                            )
                        )
                        .Select(x => x.HeatNo)
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToList();

                var databaseDuplicateHeats = heatNosForDatabaseCheck.Any()
                    ? repo.GetDuplicateHeatNosExcludingIDs(
                        heatNosForDatabaseCheck,
                        excludedIDs
                      ) ?? new List<string>()
                    : new List<string>();

                if (databaseDuplicateHeats.Any())
                    return BilletSaveError(
                        "These Heat No(s) already exist in active Chemical Analysis: " +
                        string.Join(", ", databaseDuplicateHeats),
                        isEdit,
                        data.ID
                    );

                CalculateBilletWeight(data);

                string currentUser = User.Identity.Name;
                DateTime now = DateTime.Now;

                if (!isEdit)
                {
                    data.StatusID = 1;
                    data.CreatedBy = currentUser;
                    data.CreatedDate = now;

                    for (int index = 0; index < chemistry.Count; index++)
                    {
                        var item = chemistry[index];
                        int srNo = index + 1;

                        item.StatusID = 1;
                        item.CreatedBy = currentUser;
                        item.CreatedDate = now;

                        repo.InsertChemicalAnalysisRM(item, srNo);

                        repo.InsertBilletBoarding(
                            CreateBilletHeatRow(
                                data,
                                item.HeatNo,
                                currentUser,
                                now
                            )
                        );
                    }

                    TempData["SuccessMessage"] =
                        "Billet Boarding and Chemical Analysis added successfully.";

                    return RedirectToAction("BilletBoard");
                }

                /* ============================ EDIT ============================ */

                data.StatusID = 1;
                data.UpdatedBy = currentUser;
                data.UpdatedDate = now;

                // Updates common boarding details for all heats of this boarding.
                repo.UpdateBilletBoarding(data);

                var postedExistingIDs = new HashSet<int>(
                    chemistry
                        .Where(x => x.ID > 0)
                        .Select(x => x.ID)
                );

                // Existing Heat removed from the form -> StatusID = 3.
                var removedChemistry = oldChemistry
                    .Where(x => !postedExistingIDs.Contains(x.ID))
                    .ToList();

                foreach (var removed in removedChemistry)
                {
                    repo.DeactivateChemicalAnalysisRM(
                        removed.ID,
                        currentUser,
                        now
                    );
                }

                for (int index = 0; index < chemistry.Count; index++)
                {
                    var item = chemistry[index];
                    int srNo = index + 1;

                    if (item.ID > 0)
                    {
                        var oldItem = oldChemistry.First(x => x.ID == item.ID);
                        string oldHeatNo =
                            (oldItem.HeatNo ?? string.Empty).Trim();

                        if (!oldHeatNo.Equals(
                            item.HeatNo,
                            StringComparison.OrdinalIgnoreCase))
                        {
                            repo.UpdateBilletBoardHeatNo(
                                data.ID,
                                oldHeatNo,
                                item.HeatNo,
                                currentUser
                            );
                        }

                        item.StatusID = 1;
                        item.UpdatedBy = currentUser;
                        item.UpdatedDate = now;

                        // Update HeatNo, SrNo and all Chemical Analysis values.
                        repo.UpdateChemicalAnalysisRM(item, srNo);
                    }
                    else
                    {
                        item.StatusID = 1;
                        item.CreatedBy = currentUser;
                        item.CreatedDate = now;

                        repo.InsertChemicalAnalysisRM(item, srNo);

                        // Newly added Heat gets a corresponding BilletBoard row.
                        repo.InsertBilletBoarding(
                            CreateBilletHeatRow(
                                data,
                                item.HeatNo,
                                currentUser,
                                now
                            )
                        );
                    }
                }

                TempData["SuccessMessage"] =
                    "Billet Boarding and Chemical Analysis updated successfully.";

                return RedirectToAction(
                    "BilletBoard",
                    new { id = data.ID }
                );
            }
            catch (Exception ex)
            {
                return BilletSaveError(
                    "Error while saving Billet Boarding: " + ex.Message,
                    isEdit,
                    data != null ? data.ID : 0
                );
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult CheckDuplicateHeatNosForBilletSave(
            List<string> heatNos,
            List<int> excludedIDs,
            int currentID = 0)
        {
            try
            {
                var normalizedHeatNos =
                    (heatNos ?? new List<string>())
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Select(x => x.Trim())
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToList();

                if (!normalizedHeatNos.Any())
                {
                    return Json(new
                    {
                        success = false,
                        message = "Please enter at least one Heat No.",
                        duplicates = new List<string>()
                    });
                }

                // Edit View posts the exact existing chemistry IDs currently on screen.
                // Excluding these prevents every unchanged Heat from duplicating itself.
                excludedIDs = (excludedIDs ?? new List<int>())
                    .Where(x => x > 0)
                    .Distinct()
                    .ToList();

                var duplicates =
                    repo.GetDuplicateHeatNosExcludingIDs(
                        normalizedHeatNos,
                        excludedIDs
                    ) ?? new List<string>();

                return Json(new
                {
                    success = true,
                    duplicates = duplicates
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Unable to validate Heat No: " + ex.Message,
                    duplicates = new List<string>()
                });
            }
        }


        private ActionResult BilletSaveError(
            string message,
            bool isEdit,
            int id)
        {
            TempData["ErrorMessage"] = message;

            return RedirectToAction(
                "AddBillet",
                isEdit ? new { id = id } : null
            );
        }


        private void CalculateBilletWeight(BilletBoardBLL data)
        {
            decimal billetLength = 0m;
            decimal.TryParse(data.BilletLength, out billetLength);

            decimal billetWeight = 0m;

            if (!string.IsNullOrWhiteSpace(data.CrossSection))
            {
                string[] parts = data.CrossSection
                    .Replace(" ", string.Empty)
                    .ToLowerInvariant()
                    .Split('x');

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
                    billetWeight = 175m * billetLength / 1000m;
                }
            }

            data.BilletWeight = billetWeight;
        }


        private BilletBoardBLL CreateBilletHeatRow(
            BilletBoardBLL source,
            string heatNo,
            string currentUser,
            DateTime now)
        {
            return new BilletBoardBLL
            {
                HeatNo = heatNo,
                Date = source.Date,
                BilletBoarding = source.BilletBoarding,
                PlantName = source.PlantName,
                Shift = source.Shift,
                ProductSpecs = source.ProductSpecs,
                BilletLength = source.BilletLength,
                CrossSection = source.CrossSection,
                BilletWeight = source.BilletWeight,
                SteelGrade = source.SteelGrade,
                Size = source.Size,
                Profile = source.Profile,
                Remarks = source.Remarks,
                StatusID = 1,
                CreatedBy = currentUser,
                CreatedDate = now
            };
        }

    }
}