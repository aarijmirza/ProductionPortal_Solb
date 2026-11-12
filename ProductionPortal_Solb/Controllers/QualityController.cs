using BAL.Repositories;
using DAL.Models;
using Newtonsoft.Json;
using ProductionPortal_Solb.App_Start;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using System.Xml.Linq;
using static DAL.Models.ViewModel;
using static DAL.Models.ViewModel.RollingMillChargeVM;

namespace ProductionPortal_Solb.Controllers
{
    [SessionState(SessionStateBehavior.Required)]
    public class QualityController : Controller
    {
        private readonly QualityRepository repo;
        private readonly RMMechanicalRepository rrepo;

        public QualityController()
        {
            repo = new QualityRepository();
            rrepo = new RMMechanicalRepository();
        }

        // ============================================================
        // BILLET BOARD
        // ============================================================
        //public ActionResult BilletBoard()
        //{
        //    var billets = repo.GetAllBoarding();
        //    return View("~/Views/Quality/BilletBoard/BilletBoard.cshtml", billets);
        //}

        public ActionResult BilletBoard(DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                DateTime today = DateTime.Today;
                DateTime resolvedFromDate = fromDate.HasValue
                    ? fromDate.Value.Date
                    : new DateTime(today.Year, today.Month, 1);
                DateTime resolvedToDate = toDate.HasValue
                    ? toDate.Value.Date
                    : today;

                if (resolvedFromDate > resolvedToDate)
                {
                    DateTime tempDate = resolvedFromDate;
                    resolvedFromDate = resolvedToDate;
                    resolvedToDate = tempDate;
                }

                var billets = repo.GetAllBoarding(resolvedFromDate, resolvedToDate);

                ViewBag.FromDate = resolvedFromDate.ToString("yyyy-MM-dd");
                ViewBag.ToDate = resolvedToDate.ToString("yyyy-MM-dd");

                return View("~/Views/Quality/BilletBoard/BilletBoard.cshtml", billets);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unable to load Billet Board records. Error: " + ex.Message;
                return View("~/Views/Quality/BilletBoard/BilletBoard.cshtml", new List<BilletBoardBLL>());
            }
        }

        // ============================================================
        // HEAT CHEMISTRY
        // ============================================================
        public ActionResult HeatChemistry()
        {
            var chemistry = repo.GetAllChemistry();
            return View("~/Views/Quality/HeatChemistry/HeatChemistry.cshtml", chemistry);
        }

        [HttpGet]
        public ActionResult AddChemistry()
        {
            return View("~/Views/Quality/HeatChemistry/AddChemistry.cshtml");
        }

        [HttpPost]
        public ActionResult AddChemistry(ChemistryInputModel model)
        {
            try
            {
                if (model != null && model.data != null)
                {
                    foreach (var sampleItem in model.data)
                    {
                        HeatChemistryBLL bll = new HeatChemistryBLL
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
                            CreatedBy = GetCurrentUser()
                        };

                        repo.AddHeatChemistry(bll);
                    }
                }

                TempData["SuccessMessage"] = "Heat Chemistry saved successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Heat Chemistry could not be saved. " + ex.Message;
            }

            return RedirectToAction("HeatChemistry");
        }

        [HttpGet]
        public ActionResult Chemistrydetails(string heatNo)
        {
            if (string.IsNullOrWhiteSpace(heatNo))
                return RedirectToAction("HeatChemistry");

            var data = repo.GetChemsitryHeatDetails(heatNo);

            if (data == null || !data.Any())
            {
                TempData["ErrorMessage"] = "No records found for Heat # " + heatNo;
                return RedirectToAction("HeatChemistry");
            }

            return View("~/Views/Quality/HeatChemistry/Chemistrydetails.cshtml", data);
        }

        // ============================================================
        // ADD / EDIT BILLET
        // ============================================================
        [HttpGet]
        public ActionResult AddBillet(int? id)
        {
            try
            {
                var heat = repo.GetAllChemistry()
                    .Where(x => x.Area == "Rolling Mill 1" || x.Area == "Rolling Mill 2")
                    .Select(x => new { x.HeatNo, x.Area })
                    .Distinct()
                    .ToList();

                ViewBag.HeatNo = new SelectList(heat);

                var billetGradeList = repo.GetBilletGrade();
                ViewBag.BilletGrade = new SelectList(billetGradeList, "ProductID", "SpecGrade");
                ViewBag.GradeDataJson = JsonConvert.SerializeObject(billetGradeList);

                BilletBoardBLL model = new BilletBoardBLL();

                if (id.HasValue && id.Value > 0)
                {
                    model = repo.GetBilletForEdit(id.Value);

                    if (model == null)
                    {
                        TempData["ErrorMessage"] = "Billet Boarding record not found.";
                        return RedirectToAction("BilletBoard");
                    }

                    model.Chemistry = repo.GetBilletChemistryForEdit(model.ID)
                        ?? new List<RMChemicalAnalysisBLL>();
                    ViewBag.IsEdit = true;
                }
                else
                {
                    model.Chemistry = new List<RMChemicalAnalysisBLL>();
                    ViewBag.IsEdit = false;
                }

                return View("~/Views/Quality/BilletBoard/AddBillet.cshtml", model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("BilletBoard");
            }
        }

        [HttpPost]
        public JsonResult CheckDuplicateHeatNos(string[] heatNos, int currentID = 0)
        {
            try
            {
                var cleanHeatNos = (heatNos ?? new string[0])
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => x.Trim().ToUpper())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                if (cleanHeatNos.Count == 0)
                    return Json(new { success = true, duplicates = new string[0] });

                List<string> duplicates = currentID > 0
                    ? repo.GetDuplicateHeatNosForEdit(cleanHeatNos, currentID) ?? new List<string>()
                    : repo.GetDuplicateHeatNos(cleanHeatNos) ?? new List<string>();

                duplicates = duplicates
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => x.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                return Json(new { success = true, duplicates = duplicates });
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new
                {
                    success = false,
                    message = "Duplicate Heat validation failed: " + ex.Message
                });
            }
        }

        [HttpGet]
        public ActionResult Boardingdetails(int id)
        {
            var data = repo.GetBilletDetails(id);
            return View("~/Views/Quality/BilletBoard/Boardingdetails.cshtml", data);
        }

        [HttpGet]
        public JsonResult GetChemistryByHeat(string heatNo)
        {
            var data = repo.GetAllChemistry()
                .Where(x => x.HeatNo == heatNo)
                .OrderBy(x => x.NoOfBillets)
                .ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // ============================================================
        // RM MECHANICAL INSPECTION
        // ============================================================
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
            var model = new QCInspectionRMPageVM();

            model.SelectedRM = string.IsNullOrWhiteSpace(rm) ? "RM1" : rm.Trim();
            model.BilletBoardingRows = repo.GetBilletBoardingRows(model.SelectedRM)
                ?? new List<QCBilletBoardingRowBLL>();
            model.MTCRows = repo.GetMTCRows(mtcHeatNo)
                ?? new List<QCMTCRowBLL>();

            if (inspectionID.HasValue && inspectionID.Value > 0)
            {
                model.Detail = repo.GetQCInspectionRMByID(inspectionID.Value)
                    ?? new QCInspectionRMDetailBLL();
                model.Detail.ID = inspectionID.Value;
            }
            else if (boardingID.HasValue && boardingID.Value > 0)
            {
                model.Detail = repo.GetQCInspectionRMFromBoarding(boardingID.Value)
                    ?? new QCInspectionRMDetailBLL();
            }
            else
            {
                model.Detail = new QCInspectionRMDetailBLL
                {
                    ProductionDate = DateTime.Today.ToString("dd-MM-yyyy"),
                    ProductionDateValue = DateTime.Today,
                    ProductionShift = "Morning",
                    DatabaseServer = @"10.1.10.115\PROD01",
                    GaugeLength = "200",
                    YieldStrength = "0.0",
                    TensileStrength = "0.0",
                    TensileYieldRatio = "0.0",
                    Elongation = "0.0"
                };
            }

            return View("~/Views/Quality/RMMechanical/add.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult QCInspectionRMadd(QCInspectionRMDetailBLL model)
        {
            if (model == null)
            {
                TempData["ErrorMessage"] = "Invalid QC inspection data.";
                return RedirectToAction("QCInspectionRMadd");
            }

            try
            {
                model.CreatedBy = GetCurrentUser();
                model.CreatedDate = DateTime.Now;

                int savedID = repo.SaveQCInspectionRM(model);

                if (savedID <= 0)
                {
                    TempData["ErrorMessage"] = "QC inspection record was not saved.";
                    return RedirectToAction("QCInspectionRMadd", new { rm = model.Site });
                }

                TempData["SuccessMessage"] = model.ID > 0
                    ? "QC inspection data updated successfully."
                    : "QC inspection data saved successfully.";

                return RedirectToAction("QCInspectionRMadd", new
                {
                    inspectionID = savedID,
                    rm = model.Site
                });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unable to save QC inspection record. " + ex.Message;
                return RedirectToAction("QCInspectionRMadd", new
                {
                    inspectionID = model.ID > 0 ? (int?)model.ID : null,
                    rm = model.Site
                });
            }
        }

        [HttpGet]
        public JsonResult GetBoardingDetails(int boardingID)
        {
            try
            {
                var data = repo.GetQCInspectionRMFromBoarding(boardingID);
                return Json(new { success = data != null, data = data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
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
                            message = "A valid MTC record must be selected."
                        },
                        JsonRequestBehavior.AllowGet
                    );
                }

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
                            message = "The selected MTC record was not found."
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
        public JsonResult GetMTCRowsJson(string heatNo = null)
        {
            try
            {
                var rows = repo.GetMTCRows(heatNo) ?? new List<QCMTCRowBLL>();

                rows = rows
                    .Where(x => x != null && !string.IsNullOrWhiteSpace(x.HeatNo))
                    .GroupBy(x => x.HeatNo.Trim(), StringComparer.OrdinalIgnoreCase)
                    .Select(g => g.OrderByDescending(x => x.ID).First())
                    .OrderByDescending(x => x.ID)
                    .ToList();

                return Json(new { success = true, data = rows }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new
                {
                    success = false,
                    message = "MTC data could not be loaded. " + ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteQCInspectionRM(int id)
        {
            try
            {
                int affected = repo.DeleteQCInspectionRM(id, GetCurrentUser());
                TempData[affected > 0 ? "SuccessMessage" : "ErrorMessage"] = affected > 0
                    ? "QC inspection record deleted successfully."
                    : "QC inspection record was not deleted.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("QCInspectionRMadd");
        }

        // ============================================================
        // NEW RM QC INSPECTION
        // BundlingSection -> RM_QCInspection
        // ============================================================
        public ActionResult InspectionRMlist()
        {
            return View("~/Views/Quality/QCInspectionRM/InspectionRMlist.cshtml");
        }

        [HttpGet]
        public ActionResult AddInspectionRM()
        {
            try
            {
                RMBundlesVM vm = new RMBundlesVM
                {
                    BundlingList = repo.GetBundlingRowsForQC() ?? new List<RMBundlingQCRowBLL>(),
                    Inspection = new RMQCInspectionBLL()
                };

                return View("~/Views/Quality/QCInspectionRM/AddInspectionRM.cshtml", vm);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unable to load Bundling Section data. " + ex.Message;

                RMBundlesVM vm = new RMBundlesVM
                {
                    BundlingList = new List<RMBundlingQCRowBLL>(),
                    Inspection = new RMQCInspectionBLL()
                };

                return View("~/Views/Quality/QCInspectionRM/AddInspectionRM.cshtml", vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddInspectionRM(RMBundlesVM vm)
        {
            try
            {
                if (vm == null)
                    vm = new RMBundlesVM();

                if (vm.Inspection == null)
                    return RMInspectionSaveError(vm, "Please select a Bundling record.");

                RMQCInspectionBLL model = vm.Inspection;

                model.Shift = CleanRMText(model.Shift);
                model.HeatNo = CleanRMText(model.HeatNo);
                model.SteelGrade = CleanRMText(model.SteelGrade);
                model.BarSize = CleanRMText(model.BarSize);
                model.BundleSeriesOnHold = CleanRMText(model.BundleSeriesOnHold);
                model.DefectCodes = CleanRMText(model.DefectCodes);
                model.MRBNo = CleanRMText(model.MRBNo);
                model.QCStatus = CleanRMText(model.QCStatus);
                model.Remarks = CleanRMText(model.Remarks);

                // RMQCInspectionBLL.ProductionDate is DateTime, not DateTime?
                if (model.ProductionDate == DateTime.MinValue)
                    return RMInspectionSaveError(vm, "Production Date is required.");

                if (string.IsNullOrWhiteSpace(model.Shift))
                    return RMInspectionSaveError(vm, "Shift is required.");

                if (string.IsNullOrWhiteSpace(model.HeatNo))
                    return RMInspectionSaveError(vm, "Heat No is required.");

                if (model.TotalBundles < 0)
                    return RMInspectionSaveError(vm, "Total Bundles cannot be negative.");

                if (model.OnHold < 0)
                    return RMInspectionSaveError(vm, "On-Hold Bundles cannot be negative.");

                if (model.Rejected < 0)
                    return RMInspectionSaveError(vm, "Rejected Bundles cannot be negative.");

                int accepted = model.TotalBundles - model.OnHold - model.Rejected;

                if (accepted < 0)
                    return RMInspectionSaveError(vm, "On-Hold + Rejected Bundles cannot exceed Total Bundles.");

                model.Accepted = accepted;

                bool duplicate = repo.IsRMQCInspectionDuplicate(
                    model.ProductionDate,
                    model.Shift,
                    model.HeatNo
                );

                if (duplicate)
                    return RMInspectionSaveError(vm,
                        "QC Inspection already exists for this Production Date, Shift and Heat No.");

                model.CreatedOn = DateTime.Now;
                model.CreatedBy = GetCurrentUser();
                model.StatusID = 1;

                // New Bundling QC flow uses SaveQCInspection(RMQCInspectionBLL)
                int savedID = repo.SaveQCInspection(model);

                if (savedID == -1)
                    return RMInspectionSaveError(vm, "This Bundling record has already been inspected.");

                if (savedID <= 0)
                    return RMInspectionSaveError(vm, "QC Inspection could not be saved.");

                TempData["SuccessMessage"] = "QC Inspection saved successfully.";
                return RedirectToAction("AddInspectionRM");
            }
            catch (Exception ex)
            {
                return RMInspectionSaveError(vm, "Unable to save QC Inspection. " + ex.Message);
            }
        }

        private ActionResult RMInspectionSaveError(RMBundlesVM vm, string message)
        {
            ModelState.AddModelError(string.Empty, message);

            if (vm == null)
                vm = new RMBundlesVM();

            if (vm.Inspection == null)
                vm.Inspection = new RMQCInspectionBLL();

            try
            {
                vm.BundlingList = repo.GetBundlingRowsForQC()
                    ?? new List<RMBundlingQCRowBLL>();
            }
            catch
            {
                vm.BundlingList = new List<RMBundlingQCRowBLL>();
            }

            return View("~/Views/Quality/QCInspectionRM/AddInspectionRM.cshtml", vm);
        }

        private static string CleanRMText(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
        }

        // ============================================================
        // OTHER QC INSPECTION
        // ============================================================
        public ActionResult Inspectionlist()
        {
            return View("~/Views/Quality/QCInspectionData/Inspectionlist.cshtml");
        }

        public ActionResult AddInspection()
        {
            return View("~/Views/Quality/QCInspectionData/AddInspection.cshtml");
        }

        // ============================================================
        // SLAG BY PRODUCT
        // ============================================================
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
            if (model == null) return HttpNotFound();
            model.Samples = repo.GetSlagSamplesById(id);
            return View("~/Views/Quality/SlagByProduct/SlagByProductDetail.cshtml", model);
        }

        [HttpGet]
        [Route("AddSlagByProduct")]
        public ActionResult AddSlagByProduct(int? id)
        {
            SlagByProductAnalysisBLL model;

            if (!id.HasValue)
            {
                model = new SlagByProductAnalysisBLL
                {
                    Samples = new List<SlagSampleAnalysisBLL>()
                };
            }
            else
            {
                model = repo.GetSlagByID(id.Value) ?? new SlagByProductAnalysisBLL();
                model.Samples = repo.GetSlagSamplesById(id.Value) ?? new List<SlagSampleAnalysisBLL>();
            }

            return View("~/Views/Quality/SlagByProduct/AddSlagByProduct.cshtml", model);
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

            data.HeatNo = CleanRMText(data.HeatNo);
            data.CertificateNo = CleanRMText(data.CertificateNo);
            data.ByProductType = CleanRMText(data.ByProductType);

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
                sample.SampleCode = CleanRMText(sample.SampleCode);

            var duplicateSampleCodes = activeSamples
                .Where(x => !string.IsNullOrWhiteSpace(x.SampleCode))
                .GroupBy(x => x.SampleCode, StringComparer.OrdinalIgnoreCase)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .OrderBy(x => x)
                .ToList();

            if (duplicateSampleCodes.Any())
                return SlagSaveError(data,
                    "Duplicate Sample Code(s): " + string.Join(", ", duplicateSampleCodes));

            bool duplicateEntry = repo.IsSlagByProductDuplicate(
                data.DateOfProduction.Value,
                data.HeatNo,
                data.ByProductType,
                data.ID
            );

            if (duplicateEntry)
                return SlagSaveError(data,
                    "This Production Date, Heat No and By-Product Type already exists.");

            DateTime now = DateTime.Now;
            string currentUser = GetCurrentUser();

            if (data.ID <= 0)
            {
                data.StatusID = 1;
                data.CreatedDate = now;
                data.CreatedBy = currentUser;

                int newID = repo.InsertSlagByProduct(data);

                if (newID == -1)
                    return SlagSaveError(data, "Duplicate entry blocked. This record already exists.");

                if (newID <= 0)
                    return SlagSaveError(data, "Data not saved. Please try again.");

                foreach (var item in activeSamples)
                    repo.InsertSlagSample(BuildSlagSample(item, newID, currentUser, now));

                TempData["SuccessMessage"] = "Data saved successfully";
                return RedirectToAction("SlagByProductList");
            }

            var existing = repo.GetSlagByID(data.ID);

            if (existing == null || existing.ID <= 0)
                return SlagSaveError(data, "Record not found.");

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
                return SlagSaveError(data, "Duplicate entry blocked. This record already exists.");

            if (updateResult <= 0)
                return SlagSaveError(data, "Data not updated. Please try again.");

            repo.DeleteSlagSamplesBySlagID(existing.ID, currentUser);

            foreach (var item in activeSamples)
                repo.InsertSlagSample(BuildSlagSample(item, existing.ID, currentUser, now));

            TempData["SuccessMessage"] = "Data updated successfully";
            return RedirectToAction("SlagByProductList");
        }

        private ActionResult SlagSaveError(SlagByProductAnalysisBLL data, string message)
        {
            ModelState.AddModelError(string.Empty, message);
            if (data.Samples == null) data.Samples = new List<SlagSampleAnalysisBLL>();
            return View("~/Views/Quality/SlagByProduct/AddSlagByProduct.cshtml", data);
        }

        private static bool IsEmptySlagSample(SlagSampleAnalysisBLL item)
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
            string updatedBy = GetCurrentUser();
            repo.SlagByProductDelete(id, updatedBy);
            repo.DeleteSlagSamplesBySlagID(id, updatedBy);
            TempData["SuccessMessage"] = "Data deleted successfully";
            return RedirectToAction("SlagByProductList");
        }

        // ============================================================
        // HBI / DRI
        // ============================================================
        [HttpGet]
        [Route("HBI/DRIAnalysis")]
        public ActionResult HBIDRIlist()
        {
            var data = repo.GetDRIHBIAnalysis() ?? new List<QCHBIDRIAnalysisBLL>();
            return View("~/Views/Quality/HBIDRIAnalysis/HBIDRIlist.cshtml", data);
        }

        [HttpGet]
        public ActionResult AddHBIDRIAnalysis(int? id)
        {
            QCHBIDRIAnalysisBLL model;

            if (!id.HasValue || id.Value <= 0)
            {
                model = new QCHBIDRIAnalysisBLL
                {
                    ReceivingDate = DateTime.Today,
                    AnalysisDate = DateTime.Today,
                    Samples = new List<SampleHBIDRIBLL> { new SampleHBIDRIBLL() }
                };

                return View("~/Views/Quality/HBIDRIAnalysis/AddHBIDRIAnalysis.cshtml", model);
            }

            model = repo.GetDRIHBIDetailByID(id.Value);

            if (model == null)
            {
                TempData["ErrorMessage"] = "HBI / DRI Analysis record not found.";
                return RedirectToAction("HBIDRIlist");
            }

            if (model.Samples == null || model.Samples.Count == 0)
                model.Samples = new List<SampleHBIDRIBLL> { new SampleHBIDRIBLL() };

            return View("~/Views/Quality/HBIDRIAnalysis/AddHBIDRIAnalysis.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddHBIDRIAnalysis(QCHBIDRIAnalysisBLL data)
        {
            if (data == null)
            {
                TempData["ErrorMessage"] = "Invalid HBI / DRI Analysis data.";
                return RedirectToAction("HBIDRIlist");
            }

            data.Material = CleanRMText(data.Material);
            data.ShipmentCodeNo = CleanRMText(data.ShipmentCodeNo);
            data.Supplier = CleanRMText(data.Supplier);
            data.ReferenceNo = CleanRMText(data.ReferenceNo);
            data.ReceivedQuantity = CleanRMText(data.ReceivedQuantity);
            data.PhysicalAnalysis = CleanRMText(data.PhysicalAnalysis);

            if (!data.ReceivingDate.HasValue)
                return HBIDRISaveError(data, "Receiving Date is required.");

            if (string.IsNullOrWhiteSpace(data.Material))
                return HBIDRISaveError(data, "Material is required.");

            if (string.IsNullOrWhiteSpace(data.Supplier))
                return HBIDRISaveError(data, "Supplier is required.");

            if (!data.AnalysisDate.HasValue)
                return HBIDRISaveError(data, "Analysis Date is required.");

            if (data.Samples == null)
                data.Samples = new List<SampleHBIDRIBLL>();

            var activeSamples = data.Samples
                .Where(x => x != null && !IsEmptyHBIDRISample(x))
                .ToList();

            foreach (var sample in activeSamples)
            {
                sample.SampleCode = CleanRMText(sample.SampleCode);
                sample.GrainSize = CleanRMText(sample.GrainSize);
                sample.Comment = CleanRMText(sample.Comment);
            }

            var duplicateSampleCodes = activeSamples
                .Where(x => !string.IsNullOrWhiteSpace(x.SampleCode))
                .GroupBy(x => x.SampleCode, StringComparer.OrdinalIgnoreCase)
                .Where(x => x.Count() > 1)
                .Select(x => x.Key)
                .OrderBy(x => x)
                .ToList();

            if (duplicateSampleCodes.Any())
                return HBIDRISaveError(data,
                    "Duplicate Sample Code(s): " + string.Join(", ", duplicateSampleCodes));

            DateTime now = DateTime.Now;
            string currentUser = GetCurrentUser();

            try
            {
                if (data.ID <= 0)
                {
                    data.StatusID = 1;
                    data.CreatedDate = now;
                    data.CreatedBy = currentUser;

                    int newID = repo.InsertDRIAnalysisData(data);

                    if (newID <= 0)
                        return HBIDRISaveError(data, "HBI / DRI Analysis could not be saved.");

                    foreach (var item in activeSamples)
                    {
                        var child = BuildHBIDRISample(item, newID, currentUser, now);
                        int result = repo.AddDRISample(child);
                        if (result <= 0)
                            throw new Exception("One or more sample rows could not be saved.");
                    }

                    TempData["SuccessMessage"] = "HBI / DRI Analysis saved successfully.";
                    return RedirectToAction("HBIDRIlist");
                }

                var existing = repo.GetDRIHBIDetailByID(data.ID);

                if (existing == null)
                    return HBIDRISaveError(data, "HBI / DRI Analysis record not found.");

                existing.ReceivingDate = data.ReceivingDate;
                existing.Material = data.Material;
                existing.ShipmentCodeNo = data.ShipmentCodeNo;
                existing.Supplier = data.Supplier;
                existing.Quantity = data.Quantity;
                existing.AnalysisDate = data.AnalysisDate;
                existing.ReferenceNo = data.ReferenceNo;
                existing.ReceivedQuantity = data.ReceivedQuantity;
                existing.PhysicalAnalysis = data.PhysicalAnalysis;
                existing.StatusID = 1;
                existing.UpdatedDate = now;
                existing.UpdatedBy = currentUser;

                int updateResult = repo.UpdateDRIAnalysisData(existing);

                if (updateResult <= 0)
                    return HBIDRISaveError(data, "HBI / DRI Analysis could not be updated.");

                repo.DeleteDRISamplesByID(existing.ID, currentUser);

                foreach (var item in activeSamples)
                {
                    var child = BuildHBIDRISample(item, existing.ID, currentUser, now);
                    int result = repo.AddDRISample(child);
                    if (result <= 0)
                        throw new Exception("One or more updated sample rows could not be saved.");
                }

                TempData["SuccessMessage"] = "HBI / DRI Analysis updated successfully.";
                return RedirectToAction("HBIDRIlist");
            }
            catch (Exception ex)
            {
                return HBIDRISaveError(data, "Unable to save HBI / DRI Analysis. " + ex.Message);
            }
        }

        [HttpGet]
        [Route("HBI/DRIAnalysisDetail")]
        public ActionResult HBIDRIAnalysisDetail(int id)
        {
            if (id <= 0) return HttpNotFound();
            var model = repo.GetDRIHBIDetailByID(id);
            if (model == null) return HttpNotFound();
            return View("~/Views/Quality/HBIDRIAnalysis/HBIDRIAnalysisDetail.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("HBI/DRIAnalysisDelete")]
        public ActionResult HBIDRIAnalysisDelete(int id)
        {
            try
            {
                if (id <= 0)
                {
                    TempData["ErrorMessage"] = "Invalid HBI / DRI Analysis record.";
                    return RedirectToAction("HBIDRIlist");
                }

                string currentUser = GetCurrentUser();
                int masterResult = repo.DeleteHBIDRIAnalysis(id, currentUser);
                repo.DeleteDRISamplesByID(id, currentUser);

                TempData[masterResult > 0 ? "SuccessMessage" : "ErrorMessage"] = masterResult > 0
                    ? "HBI / DRI Analysis deleted successfully."
                    : "Record could not be deleted.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unable to delete HBI / DRI Analysis. " + ex.Message;
            }

            return RedirectToAction("HBIDRIlist");
        }

        private ActionResult HBIDRISaveError(QCHBIDRIAnalysisBLL data, string message)
        {
            ModelState.AddModelError(string.Empty, message);

            if (data == null)
                data = new QCHBIDRIAnalysisBLL();

            if (data.Samples == null || data.Samples.Count == 0)
                data.Samples = new List<SampleHBIDRIBLL> { new SampleHBIDRIBLL() };

            return View("~/Views/Quality/HBIDRIAnalysis/AddHBIDRIAnalysis.cshtml", data);
        }

        private static bool IsEmptyHBIDRISample(SampleHBIDRIBLL item)
        {
            if (item == null) return true;

            return string.IsNullOrWhiteSpace(item.SampleCode)
                && item.FeTotal == null
                && item.FeMetallic == null
                && item.Metallization == null
                && item.C == null
                && item.S == null
                && item.P == null
                && item.SiO2 == null
                && item.Al2O3 == null
                && item.MgO == null
                && item.CaO == null
                && item.TotalGangue == null
                && string.IsNullOrWhiteSpace(item.GrainSize)
                && string.IsNullOrWhiteSpace(item.Comment);
        }

        private static SampleHBIDRIBLL BuildHBIDRISample(
            SampleHBIDRIBLL source,
            int analysisID,
            string currentUser,
            DateTime now)
        {
            return new SampleHBIDRIBLL
            {
                AnalysisID = analysisID,
                SampleCode = source.SampleCode,
                FeTotal = source.FeTotal,
                FeMetallic = source.FeMetallic,
                Metallization = source.Metallization,
                C = source.C,
                S = source.S,
                P = source.P,
                SiO2 = source.SiO2,
                Al2O3 = source.Al2O3,
                MgO = source.MgO,
                CaO = source.CaO,
                TotalGangue = source.TotalGangue,
                GrainSize = source.GrainSize,
                Comment = source.Comment,
                StatusID = 1,
                CreatedDate = now,
                CreatedBy = currentUser
            };
        }

        // ============================================================
        // PDFs
        // ============================================================
        [HttpGet]
        public ActionResult HBIDRIAnalysisPDF(DateTime? from, DateTime? to)
        {
            DateTime fromDate = from ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime toDate = to ?? DateTime.Now;
            DateTime toExclusive = toDate.Date.AddDays(1);

            var vm = new HBIDRIAnalysisPDFVM
            {
                HBIDRIData = repo.GetHBDRIAnalysisByDate(fromDate.Date, toExclusive),
                Samples = repo.GetHBDRISamplesByDate(fromDate.Date, toExclusive),
                FromDate = fromDate,
                ToDate = toDate
            };

            return View("~/Views/Quality/HBIDRIAnalysis/HBIDRIAnalysisPDF.cshtml", vm);
        }

        public ActionResult SlagByProductPDF(DateTime? from, DateTime? to)
        {
            DateTime fromDate = from ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime toDate = to ?? DateTime.Now;
            DateTime toInclusive = toDate.Date.AddDays(1);

            var vm = new SlagByProductPDFVM
            {
                SlagData = repo.GetSlagByProductByDate(fromDate.Date, toInclusive),
                Samples = repo.GetSlagSamplesByDate(fromDate.Date, toInclusive),
                FromDate = fromDate,
                ToDate = toDate
            };

            return View("~/Views/Quality/SlagByProduct/SlagByProductPDF.cshtml", vm);
        }

        public ActionResult BilletBoardPDF(DateTime? from, DateTime? to)
        {
            DateTime fromDate = from ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime toDate = to ?? DateTime.Now;
            DateTime toInclusive = toDate.Date.AddDays(1);

            var vm = new BilletBoardingPDFVM
            {
                BilletBoards = repo.GetBilletBoardingByDate(fromDate.Date, toInclusive),
                Samples = repo.GetHeatChemistryByDate(fromDate.Date, toInclusive),
                FromDate = fromDate,
                ToDate = toDate
            };

            return View("~/Views/Quality/BilletBoard/BilletBoardingPDF.cshtml", vm);
        }

        // ============================================================
        // MTC
        // ============================================================
        //public ActionResult castmillCertificate()
        //{
        //    var billets = repo.GetAllBoarding();
        //    return View("~/Views/Quality/CastMillCertificate/castmillCertificate.cshtml", billets);
        //}


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GenerateMTC(QCInspectionRMDetailBLL model)
        {
            string selectedRM =
                model != null &&
                !string.IsNullOrWhiteSpace(model.Site) &&
                (
                    model.Site.Trim().Equals("RM2", StringComparison.OrdinalIgnoreCase) ||
                    model.Site.Trim().Equals("Rolling Mill 2", StringComparison.OrdinalIgnoreCase)
                )
                    ? "RM2"
                    : "RM1";

            try
            {
                if (model == null)
                    throw new Exception("MTC data was not received.");

                /*
                 * NON-BLOCKING MTC GENERATION:
                 * Missing record IDs / Heat No. / Grade / Specification /
                 * Product Type / test values must NOT stop certificate generation.
                 * Whatever data is available will be printed.
                 */

                /*
                 * MTC Product authority:
                 * HeatNo -> latest active BundlingSection row.
                 *
                 * SteelGrade comes from BundlingSection.SteelGrade.
                 * Product Type comes from BundlingSection.Profile:
                 *
                 * PB  -> Plain Bar
                 * WR  -> Wire Rod (Coil)
                 * RIC -> Rebar In Coil
                 * RB  -> Rebar
                 */
                /*
                 * BundlingSection enrichment is OPTIONAL.
                 * If HeatNo/Profile/Grade is not found, keep posted/current
                 * values and continue generating the MTC.
                 */
                /*
                 * PRODUCT TYPE PRIORITY FOR MTC:
                 *
                 * 1. Keep the ProductType posted/saved from QC form.
                 * 2. If BundlingSection has Profile, use it as authoritative
                 *    fallback / normalization:
                 *
                 *    RIC -> Rebar Coil
                 *    PB  -> Plain Rod
                 *    WR  -> Wire Rod
                 *    RB  -> Rebar
                 *
                 * 3. Never block MTC generation if Bundling data is missing.
                 */
                string postedProductType =
                    NormalizeMTCProductType(
                        model.ProductType
                    );

                BundlingMTCInfoBLL bundlingInfo =
                    null;

                if (
                    !string.IsNullOrWhiteSpace(
                        model.HeatNo
                    )
                )
                {
                    try
                    {
                        bundlingInfo =
                            rrepo.GetBundlingMTCInfoByHeatNo(
                                model.HeatNo
                            );
                    }
                    catch
                    {
                        bundlingInfo = null;
                    }
                }

                if (bundlingInfo != null)
                {
                    if (
                        !string.IsNullOrWhiteSpace(
                            bundlingInfo.SteelGrade
                        )
                    )
                    {
                        model.SteelGrade =
                            bundlingInfo.SteelGrade.Trim();
                    }

                    string profileProductType =
                        MapBundlingProfileToMTCProductType(
                            bundlingInfo.Profile
                        );

                    string repositoryProductType =
                        NormalizeMTCProductType(
                            bundlingInfo.ProductType
                        );

                    /*
                     * User-saved dropdown value gets first priority.
                     * Otherwise use Profile mapping, then repository text.
                     */
                    if (
                        !string.IsNullOrWhiteSpace(
                            postedProductType
                        )
                    )
                    {
                        model.ProductType =
                            postedProductType;
                    }
                    else if (
                        !string.IsNullOrWhiteSpace(
                            profileProductType
                        )
                    )
                    {
                        model.ProductType =
                            profileProductType;
                    }
                    else if (
                        !string.IsNullOrWhiteSpace(
                            repositoryProductType
                        )
                    )
                    {
                        model.ProductType =
                            repositoryProductType;
                    }

                    /*
                     * Checkbox state is only UI/business context.
                     * Do NOT clear ProductType because Profile is RB/PB/RIC.
                     */
                    if (
                        !string.IsNullOrWhiteSpace(
                            bundlingInfo.Profile
                        )
                    )
                    {
                        string normalizedProfile =
                            bundlingInfo.Profile
                                .Trim()
                                .ToUpperInvariant();

                        model.IsWireRodOrCoil =
                            normalizedProfile == "WR"
                            ||
                            normalizedProfile == "RIC"
                            ||
                            normalizedProfile == "PB";
                    }
                }
                else
                {
                    model.ProductType =
                        postedProductType;
                }

                /*
                 * Final non-blocking fallback.
                 * Product Type cell should not stay blank when the basic
                 * product category can still be inferred.
                 */
                if (
                    string.IsNullOrWhiteSpace(
                        model.ProductType
                    )
                )
                {
                    model.ProductType =
                        model.IsWireRodOrCoil
                            ? "Wire Rod"
                            : "Rebar";
                }

                decimal? barSizeValue = TryParseMTCDecimal(model.BarSize);
                decimal? yieldStrengthValue = TryParseMTCDecimal(model.YieldStrength);
                decimal? tensileStrengthValue = TryParseMTCDecimal(model.TensileStrength);
                decimal? elongationValue = TryParseMTCDecimal(model.Elongation);
                decimal? cValue = TryParseMTCDecimal(model.C);
                decimal? siValue = TryParseMTCDecimal(model.Si);
                decimal? mnValue = TryParseMTCDecimal(model.Mn);
                decimal? pValue = TryParseMTCDecimal(model.P);
                decimal? sValue = TryParseMTCDecimal(model.S);
                decimal? cuValue = TryParseMTCDecimal(model.Cu);
                decimal? niValue = TryParseMTCDecimal(model.Ni);
                decimal? crValue = TryParseMTCDecimal(model.Cr);
                decimal? moValue = TryParseMTCDecimal(model.Mo);
                decimal? vValue = TryParseMTCDecimal(model.V);
                decimal? bValue = TryParseMTCDecimal(model.B);
                decimal? nValue = TryParseMTCDecimal(model.N);
                decimal? ceqValue = TryParseMTCDecimal(model.Ceq);

                decimal? tensileYieldRatioValue =
                    null;

                if (
                    yieldStrengthValue.HasValue &&
                    tensileStrengthValue.HasValue &&
                    yieldStrengthValue.Value > 0
                )
                {
                    tensileYieldRatioValue =
                        decimal.Round(
                            tensileStrengthValue.Value /
                            yieldStrengthValue.Value,
                            3,
                            MidpointRounding.AwayFromZero
                        );

                    model.TensileYieldRatio =
                        tensileYieldRatioValue.Value.ToString(
                            "0.###",
                            CultureInfo.InvariantCulture
                        );
                }
                else
                {
                    model.TensileYieldRatio = "";
                }

                string gradeLimitsPath =
                    Server.MapPath("~/Templates/Grades Limits.xlsx");

                bool hasValidGradeLimits =
                    false;

                if (
                    System.IO.File.Exists(
                        gradeLimitsPath
                    )
                )
                {
                    try
                    {
                        EnsureValidXlsxPackage(
                            gradeLimitsPath,
                            "Grades Limits.xlsx"
                        );

                        hasValidGradeLimits =
                            true;
                    }
                    catch
                    {
                        /*
                         * Limits file problems must not block MTC generation.
                         */
                        hasValidGradeLimits =
                            false;
                    }
                }

                /*
                 * Specification/Grade resolver is optional enrichment only.
                 */
                if (hasValidGradeLimits)
                {
                    try
                    {
                        ResolveMTCSpecificationAndGradeFromLimits(
                            gradeLimitsPath,
                            model,
                            barSizeValue
                        );
                    }
                    catch
                    {
                        /*
                         * Keep current values and continue.
                         */
                    }
                }

                /*
                 * IMPORTANT:
                 * Raw OpenXML works only with a REAL .xlsx ZIP package.
                 * A legacy .xls file cannot be renamed to .xlsx.
                 *
                 * Preferred template:
                 *   ~/Templates/MTC Form.xlsx
                 *
                 * Fallback keeps compatibility with the previous template.
                 */
                string templatePath =
                    ResolveMtcTemplatePath();

                /*
                 * IMPORTANT:
                 * No ClosedXML, NPOI or Spire is used below.
                 * Grades Limits.xlsx and the MTC template are handled with
                 * .NET Framework built-in ZIP/XML APIs only.
                 */
                /*
                 * Validation is informational/best-effort only.
                 * It must NEVER stop MTC generation.
                 */
                List<string> validationErrors =
                    new List<string>();

                if (hasValidGradeLimits)
                {
                    try
                    {
                        validationErrors =
                            ValidateMTCGradeLimitsRaw(
                                gradeLimitsPath,
                                model.Specification,
                                model.SteelGrade,
                                model.ProductType,
                                barSizeValue,
                                yieldStrengthValue,
                                tensileStrengthValue,
                                tensileYieldRatioValue,
                                elongationValue,
                                cValue,
                                siValue,
                                mnValue,
                                pValue,
                                sValue,
                                cuValue,
                                niValue,
                                crValue,
                                moValue,
                                vValue,
                                bValue,
                                nValue,
                                ceqValue
                            )
                            ??
                            new List<string>();
                    }
                    catch
                    {
                        validationErrors =
                            new List<string>();
                    }
                }

                Dictionary<string, string> mtcDisplayLimits =
                    CreateEmptyMTCDisplayLimits();

                if (hasValidGradeLimits)
                {
                    try
                    {
                        mtcDisplayLimits =
                            GetMTCGradeLimitsForDisplayRaw(
                                gradeLimitsPath,
                                model.Specification,
                                model.SteelGrade,
                                model.ProductType,
                                barSizeValue
                            )
                            ??
                            CreateEmptyMTCDisplayLimits();
                    }
                    catch
                    {
                        mtcDisplayLimits =
                            CreateEmptyMTCDisplayLimits();
                    }
                }

                /*
                 * Final certificate display normalization.
                 * Validation already completed using the resolved standard.
                 */
                model.Specification =
                    FormatMTCSpecificationForDisplay(
                        model.Specification
                    );

                byte[] outputBytes =
                    BuildMTCFromTemplateRaw(
                        templatePath,
                        model,
                        tensileYieldRatioValue,
                        mtcDisplayLimits
                    );

                string safeHeatNo =
                    string.IsNullOrWhiteSpace(
                        model.HeatNo
                    )
                        ? "NO_HEAT"
                        : MakeSafeFileName(
                            model.HeatNo
                        );

                string fileName =
                    "MTC_" +
                    safeHeatNo +
                    "_" +
                    DateTime.Now.ToString("yyyyMMddHHmmss") +
                    ".xlsx";

                return File(
                    outputBytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName
                );
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;

                return RedirectToAction(
                    "QCInspectionRMadd",
                    new { rm = selectedRM }
                );
            }
        }

        private static string MapBundlingProfileToMTCProductType(
            string profile)
        {
            string value =
                (profile ?? string.Empty)
                    .Trim()
                    .ToUpperInvariant();

            switch (value)
            {
                case "RIC":
                    return "Rebar Coil";

                case "PB":
                    return "Plain Rod";

                case "WR":
                    return "Wire Rod";

                case "RB":
                    return "Rebar";

                default:
                    return "";
            }
        }


        private static string NormalizeMTCProductType(
            string productType)
        {
            string value =
                (productType ?? string.Empty)
                    .Trim();

            if (
                value.Equals(
                    "Rebar Coil",
                    StringComparison.OrdinalIgnoreCase
                )
                ||
                value.Equals(
                    "Rebar In Coil",
                    StringComparison.OrdinalIgnoreCase
                )
                ||
                value.Equals(
                    "RIC",
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                return "Rebar Coil";
            }

            if (
                value.Equals(
                    "Plain Rod",
                    StringComparison.OrdinalIgnoreCase
                )
                ||
                value.Equals(
                    "Plain Bar",
                    StringComparison.OrdinalIgnoreCase
                )
                ||
                value.Equals(
                    "PB",
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                return "Plain Rod";
            }

            if (
                value.Equals(
                    "Wire Rod",
                    StringComparison.OrdinalIgnoreCase
                )
                ||
                value.Equals(
                    "Wire Rod (Coil)",
                    StringComparison.OrdinalIgnoreCase
                )
                ||
                value.Equals(
                    "WR",
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                return "Wire Rod";
            }

            if (
                value.Equals(
                    "Rebar",
                    StringComparison.OrdinalIgnoreCase
                )
                ||
                value.Equals(
                    "RB",
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                return "Rebar";
            }

            return value;
        }




        private decimal? TryParseMTCDecimal(object value)
        {
            string textValue =
                Convert.ToString(
                    value,
                    CultureInfo.InvariantCulture
                );

            if (string.IsNullOrWhiteSpace(textValue))
                return null;

            decimal parsed;

            if (
                decimal.TryParse(
                    textValue,
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out parsed
                )
            )
            {
                return parsed;
            }

            return null;
        }


        private string ResolveMtcTemplatePath()
        {
            string preferredXlsx =
                Server.MapPath(
                    "~/Templates/MTC Form.xlsx"
                );

            if (
                System.IO.File.Exists(
                    preferredXlsx
                )
            )
            {
                EnsureValidXlsxPackage(
                    preferredXlsx,
                    "MTC Form.xlsx"
                );

                return preferredXlsx;
            }

            string previousXlsx =
                Server.MapPath(
                    "~/Templates/1- MTC -TEMPELETE (1).xlsx"
                );

            if (
                System.IO.File.Exists(
                    previousXlsx
                )
            )
            {
                EnsureValidXlsxPackage(
                    previousXlsx,
                    "1- MTC -TEMPELETE (1).xlsx"
                );

                return previousXlsx;
            }

            string legacyXls =
                Server.MapPath(
                    "~/Templates/MTC Form.xls"
                );

            if (
                System.IO.File.Exists(
                    legacyXls
                )
            )
            {
                throw new Exception(
                    "MTC Form.xls is a legacy Excel 97-2003 file. " +
                    "Do not rename it to .xlsx. Open it in Microsoft Excel " +
                    "and use Save As > Excel Workbook (*.xlsx), then save it as " +
                    "'MTC Form.xlsx' inside the Templates folder."
                );
            }

            throw new FileNotFoundException(
                "No valid MTC .xlsx template was found. Expected " +
                "'MTC Form.xlsx' in the Templates folder."
            );
        }


        private void EnsureValidXlsxPackage(
            string filePath,
            string displayName)
        {
            if (
                string.IsNullOrWhiteSpace(
                    filePath
                )
                ||
                !System.IO.File.Exists(
                    filePath
                )
            )
            {
                throw new FileNotFoundException(
                    (displayName ?? "Excel file") +
                    " was not found.",
                    filePath
                );
            }

            byte[] signature =
                new byte[4];

            using (
                FileStream stream =
                    System.IO.File.OpenRead(
                        filePath
                    )
            )
            {
                if (
                    stream.Length < 4
                    ||
                    stream.Read(
                        signature,
                        0,
                        signature.Length
                    ) != signature.Length
                )
                {
                    throw new Exception(
                        (displayName ?? "Excel file") +
                        " is empty or invalid."
                    );
                }
            }

            bool hasZipSignature =
                signature[0] == 0x50
                &&
                signature[1] == 0x4B
                &&
                (
                    (
                        signature[2] == 0x03
                        &&
                        signature[3] == 0x04
                    )
                    ||
                    (
                        signature[2] == 0x05
                        &&
                        signature[3] == 0x06
                    )
                    ||
                    (
                        signature[2] == 0x07
                        &&
                        signature[3] == 0x08
                    )
                );

            if (!hasZipSignature)
            {
                throw new Exception(
                    (displayName ?? "Excel file") +
                    " is not a real .xlsx OpenXML workbook. " +
                    "If this file was originally .xls, renaming the extension " +
                    "to .xlsx does not convert it. Use Excel > Save As > " +
                    "Excel Workbook (*.xlsx)."
                );
            }

            try
            {
                using (
                    FileStream stream =
                        System.IO.File.OpenRead(
                            filePath
                        )
                )
                using (
                    ZipArchive archive =
                        new ZipArchive(
                            stream,
                            ZipArchiveMode.Read,
                            false
                        )
                )
                {
                    if (
                        archive.GetEntry(
                            "[Content_Types].xml"
                        ) == null
                        ||
                        archive.GetEntry(
                            "xl/workbook.xml"
                        ) == null
                    )
                    {
                        throw new Exception(
                            (displayName ?? "Excel file") +
                            " is not a complete XLSX workbook."
                        );
                    }

                    /*
                     * Force Central Directory enumeration here.
                     * A damaged ZIP will fail before MTC generation starts.
                     */
                    int entryCount =
                        archive.Entries.Count;

                    if (entryCount <= 0)
                    {
                        throw new Exception(
                            (displayName ?? "Excel file") +
                            " contains no XLSX entries."
                        );
                    }
                }
            }
            catch (InvalidDataException ex)
            {
                throw new Exception(
                    (displayName ?? "Excel file") +
                    " is corrupt or is not a valid XLSX ZIP package. " +
                    "Open the original file in Excel and Save As a new " +
                    "Excel Workbook (*.xlsx). Details: " +
                    ex.Message,
                    ex
                );
            }
        }


        private void ValidateGeneratedXlsxBytes(
            byte[] bytes)
        {
            if (
                bytes == null
                ||
                bytes.Length == 0
            )
            {
                throw new Exception(
                    "Generated MTC workbook is empty."
                );
            }

            try
            {
                using (
                    MemoryStream stream =
                        new MemoryStream(
                            bytes,
                            false
                        )
                )
                using (
                    ZipArchive archive =
                        new ZipArchive(
                            stream,
                            ZipArchiveMode.Read,
                            false
                        )
                )
                {
                    if (
                        archive.GetEntry(
                            "[Content_Types].xml"
                        ) == null
                        ||
                        archive.GetEntry(
                            "xl/workbook.xml"
                        ) == null
                    )
                    {
                        throw new Exception(
                            "Generated MTC workbook is incomplete."
                        );
                    }

                    int entryCount =
                        archive.Entries.Count;

                    if (entryCount <= 0)
                    {
                        throw new Exception(
                            "Generated MTC workbook contains no entries."
                        );
                    }
                }
            }
            catch (InvalidDataException ex)
            {
                throw new Exception(
                    "Generated MTC workbook failed ZIP validation: " +
                    ex.Message,
                    ex
                );
            }
        }


        private byte[] BuildMTCFromTemplateRaw(
            string templatePath,
            QCInspectionRMDetailBLL model,
            decimal? tensileYieldRatio,
            Dictionary<string, string> mtcDisplayLimits)
        {
            EnsureValidXlsxPackage(
                templatePath,
                System.IO.Path.GetFileName(templatePath)
            );

            byte[] templateBytes =
                System.IO.File.ReadAllBytes(
                    templatePath
                );

            using (
                MemoryStream sourceStream =
                    new MemoryStream(
                        templateBytes,
                        false
                    )
            )
            using (
                ZipArchive sourceArchive =
                    new ZipArchive(
                        sourceStream,
                        ZipArchiveMode.Read,
                        false
                    )
            )
            {
                string sheetPath =
                    GetWorksheetPath(
                        sourceArchive,
                        null
                    );

                XDocument sheetXml =
                    LoadZipXml(
                        sourceArchive,
                        sheetPath
                    );

                List<string> templateSharedStrings =
                    ReadSharedStrings(
                        sourceArchive
                    );

                /*
                 * Header/value mapping.
                 * Values go BELOW Specification / Grade headings.
                 */
                SetXlsxTextCell(
                    sheetXml,
                    "B7",
                    "Specification"
                );

                SetXlsxTextCell(
                    sheetXml,
                    "E7",
                    "Grade"
                );

                SetXlsxTextCell(
                    sheetXml,
                    "B8",
                    model.Specification
                );

                SetXlsxTextCell(
                    sheetXml,
                    "E8",
                    model.SteelGrade
                );

                SetXlsxTextCell(
                    sheetXml,
                    "N10",
                    DateTime.Today.ToString(
                        "dd-MMM-yyyy",
                        CultureInfo.InvariantCulture
                    )
                );

                /*
                 * Row 15 = limits
                 * Row 16 = actual values
                 *
                 * The original template vertically merges several
                 * header/limit cells. Split those merges first so row 15
                 * can visibly hold the limit without overwriting the header.
                 */
                PrepareMtcLimitRowLayout(
                    sheetXml
                );

                EnsureMtcLimitRowCells(
                    sheetXml
                );

                WriteMTCGradeLimitsRow(
                    sheetXml,
                    templateSharedStrings,
                    mtcDisplayLimits
                );

                SetXlsxNumberCell(
                    sheetXml,
                    "B16",
                    model.BarSize
                );

                SetXlsxNumberCell(
                    sheetXml,
                    "C16",
                    ResolveMTCNominalWeight(
                        model
                    )
                );

                SetXlsxTextCell(
                    sheetXml,
                    "D16",
                    model.ProductType ?? ""
                );

                SetXlsxTextCell(
                    sheetXml,
                    "E16",
                    model.HeatNo
                );

                SetXlsxNumberCell(
                    sheetXml,
                    "F16",
                    model.YieldStrength
                );

                SetXlsxNumberCell(
                    sheetXml,
                    "G16",
                    model.TensileStrength
                );

                if (tensileYieldRatio.HasValue)
                {
                    SetXlsxNumberCell(
                        sheetXml,
                        "H16",
                        tensileYieldRatio.Value
                    );
                }
                else
                {
                    SetXlsxTextCell(
                        sheetXml,
                        "H16",
                        ""
                    );
                }

                SetXlsxNumberCell(
                    sheetXml,
                    "I16",
                    model.Elongation
                );

                SetXlsxTextCell(
                    sheetXml,
                    "J16",
                    model.BendTestObserved
                        ? "Satisfactory"
                        : ""
                );

                /*
                 * Chemistry actual values are written according to the
                 * EXISTING MTC template headers. Template structure is
                 * not changed by code.
                 */
                WriteMTCActualChemistryByTemplateHeaders(
                    sheetXml,
                    templateSharedStrings,
                    model
                );

                SetXlsxTextCell(
                    sheetXml,
                    "B49",
                    "MTC generated using : " +
                    GetCurrentUser() +
                    "  " +
                    DateTime.Now.ToString(
                        "dd/MM/yyyy hh:mm:ss tt",
                        CultureInfo.InvariantCulture
                    )
                );

                /*
                 * LOCK GENERATED MTC
                 *
                 * Normal Excel users cannot edit cells, formatting,
                 * rows/columns or drawing objects (logos/signatures).
                 */
                const string mtcProtectionPassword =
                    "SOLB-MTC-LOCK";

                ApplyMTCWorksheetProtection(
                    sheetXml,
                    mtcProtectionPassword
                );

                /*
                 * Do NOT update the source ZIP in-place.
                 * Rebuild a brand-new XLSX package. This prevents
                 * Central Directory corruption on some .NET/IIS setups.
                 */
                using (
                    MemoryStream outputStream =
                        new MemoryStream()
                )
                {
                    using (
                        ZipArchive outputArchive =
                            new ZipArchive(
                                outputStream,
                                ZipArchiveMode.Create,
                                true
                            )
                    )
                    {
                        foreach (
                            ZipArchiveEntry sourceEntry
                            in sourceArchive.Entries
                        )
                        {
                            if (
                                string.Equals(
                                    sourceEntry.FullName,
                                    sheetPath,
                                    StringComparison.OrdinalIgnoreCase
                                )
                            )
                            {
                                continue;
                            }

                            ZipArchiveEntry destinationEntry =
                                outputArchive.CreateEntry(
                                    sourceEntry.FullName,
                                    CompressionLevel.Optimal
                                );

                            using (
                                Stream input =
                                    sourceEntry.Open()
                            )
                            using (
                                Stream output =
                                    destinationEntry.Open()
                            )
                            {
                                input.CopyTo(
                                    output
                                );
                            }
                        }

                        ZipArchiveEntry worksheetEntry =
                            outputArchive.CreateEntry(
                                sheetPath,
                                CompressionLevel.Optimal
                            );

                        using (
                            Stream worksheetStream =
                                worksheetEntry.Open()
                        )
                        {
                            sheetXml.Save(
                                worksheetStream,
                                SaveOptions.DisableFormatting
                            );
                        }

                    }

                    byte[] result =
                        outputStream.ToArray();

                    ValidateGeneratedXlsxBytes(
                        result
                    );

                    return result;
                }
            }
        }


        private List<string> ValidateMTCGradeLimitsRaw(
            string gradeLimitsPath,
            string specification,
            string steelGrade,
            string productType,
            decimal? barSize,
            decimal? yieldStrength,
            decimal? tensileStrength,
            decimal? tensileYieldRatio,
            decimal? elongation,
            decimal? c,
            decimal? si,
            decimal? mn,
            decimal? p,
            decimal? s,
            decimal? cu,
            decimal? ni,
            decimal? cr,
            decimal? mo,
            decimal? v,
            decimal? b,
            decimal? n,
            decimal? ceq)
        {
            List<string> errors =
                new List<string>();

            using (
                FileStream stream =
                    new FileStream(
                        gradeLimitsPath,
                        FileMode.Open,
                        FileAccess.Read,
                        FileShare.ReadWrite
                    )
            )
            using (
                ZipArchive archive =
                    new ZipArchive(
                        stream,
                        ZipArchiveMode.Read,
                        false
                    )
            )
            {
                string sheetPath =
                    GetWorksheetPath(
                        archive,
                        "Grades Limits"
                    );

                XDocument sheetXml =
                    LoadZipXml(
                        archive,
                        sheetPath
                    );

                List<string> sharedStrings =
                    ReadSharedStrings(
                        archive
                    );

                XNamespace ns =
                    "http://schemas.openxmlformats.org/spreadsheetml/2006/main";

                List<XElement> rows =
                    sheetXml
                        .Descendants(ns + "row")
                        .Where(
                            x =>
                                GetIntAttribute(x, "r") >= 4
                        )
                        .ToList();

                /*
                 * First collect all rules matching:
                 * Specification + Grade + Size.
                 *
                 * Then prefer Product Type match.
                 * If no product-type match exists but there is only one
                 * valid rule for the grade/size, use that rule.
                 *
                 * This avoids rejecting valid grades such as
                 * ASTM A510M / AISI 1008 only because the UI checkbox
                 * was not ticked, while still keeping deterministic
                 * behavior when multiple product-type rules exist.
                 */
                /*
                 * STRICT LIMIT MATCH:
                 * Specification + Grade + Product Type + Size
                 * must point to the same Grades Limits row.
                 *
                 * No exact row keeps MTC generation non-blocking
                 * and leaves limit cells as "--".
                 */
                string requiredProductType =
                    NormalizeMTCProductTypeForLimits(
                        productType
                    );

                string combined =
                    (
                        (specification ?? "")
                        + " "
                        + (steelGrade ?? "")
                    ).Trim();

                XElement matchedRow =
                    rows.FirstOrDefault(
                        row =>
                        {
                            string rowSpec =
                                GetXlsxCellText(
                                    row,
                                    "A",
                                    sharedStrings,
                                    ns
                                );

                            string rowGrade =
                                GetXlsxCellText(
                                    row,
                                    "B",
                                    sharedStrings,
                                    ns
                                );

                            bool specMatch =
                                MTCSpecificationMatches(
                                    rowSpec,
                                    specification
                                )
                                ||
                                MTCSpecificationMatches(
                                    rowSpec,
                                    steelGrade
                                )
                                ||
                                MTCSpecificationMatches(
                                    rowSpec,
                                    combined
                                );

                            bool gradeMatch =
                                MTCGradeCandidateMatchesLoose(
                                    rowGrade,
                                    steelGrade
                                )
                                ||
                                MTCGradeCandidateMatchesLoose(
                                    rowGrade,
                                    specification
                                )
                                ||
                                MTCGradeCandidateMatchesLoose(
                                    rowGrade,
                                    combined
                                );

                            return
                                specMatch
                                &&
                                gradeMatch
                                &&
                                MTCProductTypeMatchesExact(
                                    GetXlsxCellText(
                                        row,
                                        "C",
                                        sharedStrings,
                                        ns
                                    ),
                                    requiredProductType
                                )
                                &&
                                MTCSizeRuleMatches(
                                    GetXlsxCellText(
                                        row,
                                        "D",
                                        sharedStrings,
                                        ns
                                    ),
                                    barSize
                                );
                        }
                    );

                if (matchedRow == null)
                {
                    /*
                     * IMPORTANT BUSINESS RULE:
                     * No matching Grades Limits row must NOT block MTC.
                     *
                     * If no applicable rule exists for the selected
                     * Specification / Grade / Product Type / Size,
                     * simply skip grade-limit validation and allow
                     * the MTC template to generate.
                     */
                    return errors;
                }

                ValidateMTCLimit(
                                    errors,
                                    "Yield Strength",
                                    yieldStrength,
                                    GetXlsxCellText(matchedRow, "E", sharedStrings, ns)
                                );

                ValidateMTCLimit(
                    errors,
                    "Tensile Strength",
                    tensileStrength,
                    GetXlsxCellText(matchedRow, "F", sharedStrings, ns)
                );

                ValidateMTCLimit(
                    errors,
                    "Tensile / Yield Ratio",
                    tensileYieldRatio,
                    GetXlsxCellText(matchedRow, "G", sharedStrings, ns)
                );

                ValidateMTCLimit(
                    errors,
                    "Elongation",
                    elongation,
                    GetXlsxCellText(matchedRow, "H", sharedStrings, ns)
                );

                ValidateMTCLimit(
                    errors,
                    "C",
                    c,
                    GetXlsxCellText(matchedRow, "I", sharedStrings, ns)
                );

                ValidateMTCLimit(
                    errors,
                    "Si",
                    si,
                    GetXlsxCellText(matchedRow, "J", sharedStrings, ns)
                );

                ValidateMTCLimit(
                    errors,
                    "Mn",
                    mn,
                    GetXlsxCellText(matchedRow, "K", sharedStrings, ns)
                );

                ValidateMTCLimit(
                    errors,
                    "P",
                    p,
                    GetXlsxCellText(matchedRow, "L", sharedStrings, ns)
                );

                ValidateMTCLimit(
                    errors,
                    "S",
                    s,
                    GetXlsxCellText(matchedRow, "M", sharedStrings, ns)
                );

                ValidateMTCLimit(
                    errors,
                    "Cu",
                    cu,
                    GetXlsxCellText(matchedRow, "N", sharedStrings, ns)
                );

                ValidateMTCLimit(
                    errors,
                    "Ni",
                    ni,
                    GetXlsxCellText(matchedRow, "O", sharedStrings, ns)
                );

                ValidateMTCLimit(
                    errors,
                    "Cr",
                    cr,
                    GetXlsxCellText(matchedRow, "P", sharedStrings, ns)
                );

                ValidateMTCLimit(
                    errors,
                    "Mo",
                    mo,
                    GetXlsxCellText(matchedRow, "Q", sharedStrings, ns)
                );

                ValidateMTCLimit(
                    errors,
                    "V",
                    v,
                    GetXlsxCellText(matchedRow, "R", sharedStrings, ns)
                );

                ValidateMTCLimit(
                    errors,
                    "B",
                    b,
                    GetXlsxCellText(matchedRow, "S", sharedStrings, ns)
                );

                ValidateMTCLimit(
                    errors,
                    "N",
                    n,
                    GetXlsxCellText(matchedRow, "T", sharedStrings, ns)
                );

                ValidateMTCLimit(
                    errors,
                    "Ceq",
                    ceq,
                    GetXlsxCellText(matchedRow, "U", sharedStrings, ns)
                );
            }

            return errors;
        }


        private string GetWorksheetPath(
            ZipArchive archive,
            string requestedSheetName)
        {
            XNamespace mainNs =
                "http://schemas.openxmlformats.org/spreadsheetml/2006/main";

            XNamespace relationshipNs =
                "http://schemas.openxmlformats.org/officeDocument/2006/relationships";

            XNamespace packageRelationshipNs =
                "http://schemas.openxmlformats.org/package/2006/relationships";

            XDocument workbookXml =
                LoadZipXml(
                    archive,
                    "xl/workbook.xml"
                );

            XDocument relsXml =
                LoadZipXml(
                    archive,
                    "xl/_rels/workbook.xml.rels"
                );

            XElement sheet =
                workbookXml
                    .Descendants(mainNs + "sheet")
                    .FirstOrDefault(
                        x =>
                            string.IsNullOrWhiteSpace(
                                requestedSheetName
                            )
                            ||
                            string.Equals(
                                (
                                    x.Attribute("name") != null
                                        ? x.Attribute("name").Value
                                        : ""
                                ),
                                requestedSheetName,
                                StringComparison.OrdinalIgnoreCase
                            )
                    );

            if (sheet == null)
            {
                throw new Exception(
                    string.IsNullOrWhiteSpace(requestedSheetName)
                        ? "Excel worksheet was not found."
                        : "Worksheet '" + requestedSheetName + "' was not found."
                );
            }

            XAttribute relationshipAttribute =
                sheet.Attribute(
                    relationshipNs + "id"
                );

            if (relationshipAttribute == null)
                throw new Exception("Excel worksheet relationship was not found.");

            string relationshipId =
                relationshipAttribute.Value;

            XElement relationship =
                relsXml
                    .Descendants(
                        packageRelationshipNs + "Relationship"
                    )
                    .FirstOrDefault(
                        x =>
                            string.Equals(
                                (
                                    x.Attribute("Id") != null
                                        ? x.Attribute("Id").Value
                                        : ""
                                ),
                                relationshipId,
                                StringComparison.OrdinalIgnoreCase
                            )
                    );

            if (relationship == null)
                throw new Exception("Excel worksheet relationship target was not found.");

            string target =
                relationship.Attribute("Target") != null
                    ? relationship.Attribute("Target").Value
                    : "";

            if (string.IsNullOrWhiteSpace(target))
                throw new Exception("Excel worksheet target is empty.");

            target =
                target.Replace("\\", "/");

            if (target.StartsWith("/", StringComparison.Ordinal))
                target = target.Substring(1);

            if (!target.StartsWith("xl/", StringComparison.OrdinalIgnoreCase))
                target = "xl/" + target;

            return target;
        }


        private XDocument LoadZipXml(
            ZipArchive archive,
            string entryPath)
        {
            ZipArchiveEntry entry =
                archive.GetEntry(entryPath);

            if (entry == null)
                throw new Exception("Excel file entry not found: " + entryPath);

            using (Stream stream = entry.Open())
            {
                return XDocument.Load(
                    stream,
                    LoadOptions.PreserveWhitespace
                );
            }
        }


        private void ReplaceZipXml(
            ZipArchive archive,
            string entryPath,
            XDocument document)
        {
            ZipArchiveEntry oldEntry =
                archive.GetEntry(entryPath);

            if (oldEntry == null)
                throw new Exception("Excel file entry not found: " + entryPath);

            oldEntry.Delete();

            ZipArchiveEntry newEntry =
                archive.CreateEntry(
                    entryPath,
                    CompressionLevel.Optimal
                );

            using (Stream stream = newEntry.Open())
            {
                document.Save(
                    stream,
                    SaveOptions.DisableFormatting
                );
            }
        }


        private List<string> ReadSharedStrings(
            ZipArchive archive)
        {
            List<string> result =
                new List<string>();

            ZipArchiveEntry entry =
                archive.GetEntry(
                    "xl/sharedStrings.xml"
                );

            if (entry == null)
                return result;

            XNamespace ns =
                "http://schemas.openxmlformats.org/spreadsheetml/2006/main";

            using (Stream stream = entry.Open())
            {
                XDocument xml =
                    XDocument.Load(stream);

                foreach (
                    XElement item
                    in xml.Descendants(ns + "si")
                )
                {
                    string value =
                        string.Concat(
                            item
                                .Descendants(ns + "t")
                                .Select(
                                    x => x.Value
                                )
                        );

                    result.Add(value);
                }
            }

            return result;
        }


        private string GetXlsxCellText(
            XElement row,
            string columnName,
            List<string> sharedStrings,
            XNamespace ns)
        {
            string rowNumber =
                row.Attribute("r") != null
                    ? row.Attribute("r").Value
                    : "";

            string cellReference =
                columnName + rowNumber;

            XElement cell =
                row
                    .Elements(ns + "c")
                    .FirstOrDefault(
                        x =>
                            string.Equals(
                                (
                                    x.Attribute("r") != null
                                        ? x.Attribute("r").Value
                                        : ""
                                ),
                                cellReference,
                                StringComparison.OrdinalIgnoreCase
                            )
                    );

            if (cell == null)
                return "";

            string type =
                cell.Attribute("t") != null
                    ? cell.Attribute("t").Value
                    : "";

            if (
                string.Equals(
                    type,
                    "inlineStr",
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                return string.Concat(
                    cell
                        .Descendants(ns + "t")
                        .Select(x => x.Value)
                ).Trim();
            }

            XElement valueElement =
                cell.Element(ns + "v");

            if (valueElement == null)
                return "";

            string rawValue =
                valueElement.Value;

            if (
                string.Equals(
                    type,
                    "s",
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                int index;

                if (
                    int.TryParse(
                        rawValue,
                        NumberStyles.Integer,
                        CultureInfo.InvariantCulture,
                        out index
                    )
                    &&
                    index >= 0
                    &&
                    index < sharedStrings.Count
                )
                {
                    return sharedStrings[index].Trim();
                }
            }

            return rawValue.Trim();
        }


        private void ConfigureMTCHeaderLayout(
            XDocument sheetXml)
        {
            XNamespace ns =
                "http://schemas.openxmlformats.org/spreadsheetml/2006/main";

            XElement worksheet =
                sheetXml.Root;

            if (worksheet == null)
                throw new Exception("MTC worksheet XML is invalid.");

            XElement mergeCells =
                worksheet.Element(ns + "mergeCells");

            if (mergeCells == null)
            {
                XElement sheetData =
                    worksheet.Element(ns + "sheetData");

                mergeCells =
                    new XElement(
                        ns + "mergeCells"
                    );

                if (sheetData != null)
                    sheetData.AddAfterSelf(mergeCells);
                else
                    worksheet.Add(mergeCells);
            }

            /*
             * Remove every existing merge touching B7:G7.
             * This clears old template merges such as B7:D7 / F7:G7
             * that previously caused labels and values to occupy the
             * same header block.
             */
            List<XElement> mergesToRemove =
                mergeCells
                    .Elements(ns + "mergeCell")
                    .Where(
                        x =>
                            MergeRangeTouches(
                                x.Attribute("ref") != null
                                    ? x.Attribute("ref").Value
                                    : "",
                                7,
                                2,
                                7
                            )
                    )
                    .ToList();

            foreach (XElement merge in mergesToRemove)
                merge.Remove();

            /*
             * Final header layout:
             * B7      heading
             * C7:D7   specification value
             * E7      heading
             * F7:G7   grade value
             */
            AddMergeRange(
                mergeCells,
                ns,
                "C7:D7"
            );

            AddMergeRange(
                mergeCells,
                ns,
                "F7:G7"
            );

            mergeCells.SetAttributeValue(
                "count",
                mergeCells.Elements(ns + "mergeCell").Count()
            );

            /*
             * Clear covered cells so stale shared-string values cannot
             * reappear inside the new merged value blocks.
             */
            ClearXlsxCellContents(
                sheetXml,
                "D7",
                ns
            );

            ClearXlsxCellContents(
                sheetXml,
                "G7",
                ns
            );
        }


        private bool MergeRangeTouches(
            string range,
            int rowNumber,
            int fromColumn,
            int toColumn)
        {
            if (string.IsNullOrWhiteSpace(range))
                return false;

            string[] parts =
                range.Split(':');

            string startRef =
                parts[0];

            string endRef =
                parts.Length > 1
                    ? parts[1]
                    : parts[0];

            int startRow =
                GetCellRowNumber(startRef);

            int endRow =
                GetCellRowNumber(endRef);

            int startColumn =
                GetCellColumnNumber(startRef);

            int endColumn =
                GetCellColumnNumber(endRef);

            return
                rowNumber >= startRow
                &&
                rowNumber <= endRow
                &&
                endColumn >= fromColumn
                &&
                startColumn <= toColumn;
        }


        private void AddMergeRange(
            XElement mergeCells,
            XNamespace ns,
            string range)
        {
            bool exists =
                mergeCells
                    .Elements(ns + "mergeCell")
                    .Any(
                        x =>
                            string.Equals(
                                x.Attribute("ref") != null
                                    ? x.Attribute("ref").Value
                                    : "",
                                range,
                                StringComparison.OrdinalIgnoreCase
                            )
                    );

            if (!exists)
            {
                mergeCells.Add(
                    new XElement(
                        ns + "mergeCell",
                        new XAttribute(
                            "ref",
                            range
                        )
                    )
                );
            }
        }


        private void ClearXlsxCellContents(
            XDocument sheetXml,
            string cellReference,
            XNamespace ns)
        {
            XElement cell =
                sheetXml
                    .Descendants(ns + "c")
                    .FirstOrDefault(
                        x =>
                            string.Equals(
                                x.Attribute("r") != null
                                    ? x.Attribute("r").Value
                                    : "",
                                cellReference,
                                StringComparison.OrdinalIgnoreCase
                            )
                    );

            if (cell == null)
                return;

            cell.Elements(ns + "v").Remove();
            cell.Elements(ns + "f").Remove();
            cell.Elements(ns + "is").Remove();
            cell.Attribute("t")?.Remove();
        }


        private bool SetXlsxValueAfterLabel(
            XDocument sheetXml,
            List<string> sharedStrings,
            string[] labelCandidates,
            object value)
        {
            XNamespace ns =
                "http://schemas.openxmlformats.org/spreadsheetml/2006/main";

            List<XElement> cells =
                sheetXml
                    .Descendants(ns + "c")
                    .ToList();

            foreach (XElement cell in cells)
            {
                string cellText =
                    GetSingleXlsxCellText(
                        cell,
                        sharedStrings,
                        ns
                    );

                if (string.IsNullOrWhiteSpace(cellText))
                    continue;

                bool isMatch =
                    labelCandidates.Any(
                        x =>
                            string.Equals(
                                NormalizeTemplateLabel(cellText),
                                NormalizeTemplateLabel(x),
                                StringComparison.OrdinalIgnoreCase
                            )
                    );

                if (!isMatch)
                    continue;

                string labelCellReference =
                    cell.Attribute("r") != null
                        ? cell.Attribute("r").Value
                        : "";

                if (string.IsNullOrWhiteSpace(labelCellReference))
                    continue;

                string targetCellReference =
                    GetCellAfterHeading(
                        sheetXml,
                        labelCellReference,
                        ns
                    );

                if (string.IsNullOrWhiteSpace(targetCellReference))
                    continue;

                SetXlsxTextCell(
                    sheetXml,
                    targetCellReference,
                    value
                );

                return true;
            }

            return false;
        }


        private string GetSingleXlsxCellText(
            XElement cell,
            List<string> sharedStrings,
            XNamespace ns)
        {
            if (cell == null)
                return "";

            string type =
                cell.Attribute("t") != null
                    ? cell.Attribute("t").Value
                    : "";

            if (
                string.Equals(
                    type,
                    "inlineStr",
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                return string.Concat(
                    cell
                        .Descendants(ns + "t")
                        .Select(x => x.Value)
                ).Trim();
            }

            XElement valueElement =
                cell.Element(ns + "v");

            if (valueElement == null)
                return "";

            string rawValue =
                valueElement.Value;

            if (
                string.Equals(
                    type,
                    "s",
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                int index;

                if (
                    int.TryParse(
                        rawValue,
                        NumberStyles.Integer,
                        CultureInfo.InvariantCulture,
                        out index
                    )
                    &&
                    index >= 0
                    &&
                    index < sharedStrings.Count
                )
                {
                    return sharedStrings[index].Trim();
                }
            }

            return rawValue.Trim();
        }


        private string GetCellAfterHeading(
            XDocument sheetXml,
            string headingCellReference,
            XNamespace ns)
        {
            int headingRow =
                GetCellRowNumber(
                    headingCellReference
                );

            int headingColumn =
                GetCellColumnNumber(
                    headingCellReference
                );

            int targetColumn =
                headingColumn + 1;

            /*
             * If the heading belongs to a merged range (for example B7:C7),
             * write after the END of that merged heading, not inside it.
             */
            XElement mergeCells =
                sheetXml
                    .Descendants(ns + "mergeCells")
                    .FirstOrDefault();

            if (mergeCells != null)
            {
                foreach (
                    XElement mergeCell
                    in mergeCells.Elements(ns + "mergeCell")
                )
                {
                    string range =
                        mergeCell.Attribute("ref") != null
                            ? mergeCell.Attribute("ref").Value
                            : "";

                    if (string.IsNullOrWhiteSpace(range))
                        continue;

                    string[] parts =
                        range.Split(':');

                    string startRef =
                        parts[0];

                    string endRef =
                        parts.Length > 1
                            ? parts[1]
                            : parts[0];

                    int startRow =
                        GetCellRowNumber(startRef);

                    int endRow =
                        GetCellRowNumber(endRef);

                    int startColumn =
                        GetCellColumnNumber(startRef);

                    int endColumn =
                        GetCellColumnNumber(endRef);

                    if (
                        headingRow >= startRow
                        &&
                        headingRow <= endRow
                        &&
                        headingColumn >= startColumn
                        &&
                        headingColumn <= endColumn
                    )
                    {
                        targetColumn =
                            endColumn + 1;

                        break;
                    }
                }
            }

            return
                GetExcelColumnName(
                    targetColumn
                )
                +
                headingRow.ToString(
                    CultureInfo.InvariantCulture
                );
        }


        private string GetExcelColumnName(int columnNumber)
        {
            if (columnNumber <= 0)
                return "";

            string result =
                "";

            int number =
                columnNumber;

            while (number > 0)
            {
                int remainder =
                    (number - 1) % 26;

                result =
                    Convert.ToChar(
                        'A' + remainder
                    )
                    +
                    result;

                number =
                    (number - 1) / 26;
            }

            return result;
        }


        private string NormalizeTemplateLabel(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "";

            return
                value
                    .Replace(":", "")
                    .Trim()
                    .ToUpperInvariant();
        }


        private void ApplyMTCWorksheetProtection(
            XDocument sheetXml,
            string password)
        {
            if (
                sheetXml == null
                ||
                sheetXml.Root == null
            )
            {
                return;
            }

            XNamespace ns =
                "http://schemas.openxmlformats.org/spreadsheetml/2006/main";

            XElement existing =
                sheetXml.Root.Element(
                    ns + "sheetProtection"
                );

            if (existing != null)
            {
                existing.Remove();
            }

            XElement protection =
                new XElement(
                    ns + "sheetProtection",

                    new XAttribute(
                        "password",
                        GetLegacyExcelPasswordHash(
                            password
                        )
                    ),

                    new XAttribute(
                        "sheet",
                        "1"
                    ),

                    new XAttribute(
                        "objects",
                        "1"
                    ),

                    new XAttribute(
                        "scenarios",
                        "1"
                    ),

                    new XAttribute(
                        "formatCells",
                        "1"
                    ),

                    new XAttribute(
                        "formatColumns",
                        "1"
                    ),

                    new XAttribute(
                        "formatRows",
                        "1"
                    ),

                    new XAttribute(
                        "insertColumns",
                        "1"
                    ),

                    new XAttribute(
                        "insertRows",
                        "1"
                    ),

                    new XAttribute(
                        "deleteColumns",
                        "1"
                    ),

                    new XAttribute(
                        "deleteRows",
                        "1"
                    )
                );

            /*
             * Worksheet schema requires sheetProtection after sheetData.
             */
            XElement sheetData =
                sheetXml.Root.Element(
                    ns + "sheetData"
                );

            if (sheetData != null)
            {
                sheetData.AddAfterSelf(
                    protection
                );
            }
            else
            {
                sheetXml.Root.Add(
                    protection
                );
            }
        }


        private string GetLegacyExcelPasswordHash(
            string password)
        {
            string value =
                password
                ??
                "";

            ushort hash =
                0;

            for (
                int i =
                    value.Length - 1;

                i >= 0;

                i--
            )
            {
                hash =
                    (ushort)(
                        (
                            (
                                hash >> 14
                            )
                            &
                            0x0001
                        )
                        |
                        (
                            (
                                hash << 1
                            )
                            &
                            0x7FFF
                        )
                    );

                hash =
                    (ushort)(
                        hash
                        ^
                        value[i]
                    );
            }

            hash =
                (ushort)(
                    hash
                    ^
                    value.Length
                    ^
                    0xCE4B
                );

            return
                hash.ToString(
                    "X4",
                    CultureInfo.InvariantCulture
                );
        }


        private void SetXlsxTextCell(
            XDocument sheetXml,
            string cellReference,
            object value)
        {
            XNamespace ns =
                "http://schemas.openxmlformats.org/spreadsheetml/2006/main";

            XElement cell =
                GetOrCreateXlsxCell(
                    sheetXml,
                    cellReference,
                    ns
                );

            cell.SetAttributeValue(
                "t",
                "inlineStr"
            );

            cell.Elements(ns + "v").Remove();
            cell.Elements(ns + "f").Remove();
            cell.Elements(ns + "is").Remove();

            XElement textElement =
                new XElement(
                    ns + "t",
                    value == null
                        ? ""
                        : Convert.ToString(
                            value,
                            CultureInfo.InvariantCulture
                        )
                );

            textElement.SetAttributeValue(
                XNamespace.Xml + "space",
                "preserve"
            );

            cell.Add(
                new XElement(
                    ns + "is",
                    textElement
                )
            );
        }


        private object ResolveMTCNominalWeight(
            QCInspectionRMDetailBLL model)
        {
            if (model == null)
            {
                return "";
            }

            string productType =
                (
                    model.ProductType
                    ??
                    ""
                )
                .Trim();

            decimal? barSize =
                model.BarSizeValue;

            if (!barSize.HasValue)
            {
                decimal parsedSize;

                if (
                    decimal.TryParse(
                        model.BarSize,
                        NumberStyles.Any,
                        CultureInfo.InvariantCulture,
                        out parsedSize
                    )
                )
                {
                    barSize =
                        parsedSize;
                }
            }

            /*
             * IMPORTANT:
             * This helper is used ONLY while generating the MTC Excel.
             * It does not change:
             * - Billet Boarding selection
             * - QC form binding
             * - Save Record
             * - Stored procedures
             * - Existing screen values
             */
            if (!barSize.HasValue)
            {
                return GetExistingMTCNominalWeight(
                    model
                );
            }

            decimal size =
                decimal.Round(
                    barSize.Value,
                    3,
                    MidpointRounding.AwayFromZero
                );

            string sizeKey =
                size.ToString(
                    "0.###",
                    CultureInfo.InvariantCulture
                );


            /* =====================================================
               WIRE ROD NOMINAL WEIGHT (Kg/m)
               ===================================================== */

            bool isWireRod =
                productType.Equals(
                    "Wire Rod",
                    StringComparison.OrdinalIgnoreCase
                )
                ||
                productType.Equals(
                    "WR",
                    StringComparison.OrdinalIgnoreCase
                )
                ||
                productType.Equals(
                    "Wire Rod - Coil",
                    StringComparison.OrdinalIgnoreCase
                )
                ||
                productType.Equals(
                    "Wire Rod (Coil)",
                    StringComparison.OrdinalIgnoreCase
                );

            if (isWireRod)
            {
                switch (sizeKey)
                {
                    case "5.5":
                        return 0.187m;

                    case "6":
                        return 0.222m;

                    case "6.5":
                        return 0.261m;

                    case "7":
                        return 0.302m;

                    case "7.5":
                        return 0.347m;

                    case "8":
                        return 0.395m;

                    case "9":
                        return 0.500m;

                    case "10":
                        return 0.617m;

                    case "11":
                        return 0.747m;

                    case "12":
                        return 0.888m;

                    case "13":
                        return 1.043m;

                    case "14":
                        return 1.209m;

                    case "15":
                        return 1.388m;

                    case "16":
                        return 1.580m;
                }

                return GetExistingMTCNominalWeight(
                    model
                );
            }


            /* =====================================================
               REBAR NOMINAL WEIGHT (Kg/m)
               Source: Rebar Nominal Weight & Dimensions master
               ===================================================== */

            bool isRebar =
                productType.Equals(
                    "Rebar",
                    StringComparison.OrdinalIgnoreCase
                )
                ||
                productType.Equals(
                    "RB",
                    StringComparison.OrdinalIgnoreCase
                )
                ||
                productType.Equals(
                    "Rebar Coil",
                    StringComparison.OrdinalIgnoreCase
                )
                ||
                productType.Equals(
                    "Rebar In Coil",
                    StringComparison.OrdinalIgnoreCase
                )
                ||
                productType.Equals(
                    "RIC",
                    StringComparison.OrdinalIgnoreCase
                );

            if (isRebar)
            {
                switch (sizeKey)
                {
                    case "8":
                        return 0.395m;

                    case "10":
                        return 0.617m;

                    case "12":
                        return 0.888m;

                    case "14":
                        return 1.210m;

                    case "16":
                        return 1.580m;

                    case "18":
                        return 1.990m;

                    case "20":
                        return 2.470m;

                    case "22":
                        return 2.980m;

                    case "25":
                        return 3.850m;

                    case "28":
                        return 4.840m;

                    case "32":
                        return 6.310m;

                    case "36":
                        return 7.990m;

                    case "40":
                        return 9.870m;

                    case "50":
                        return 15.410m;
                }

                return GetExistingMTCNominalWeight(
                    model
                );
            }


            /*
             * Plain Rod / any other Product Type:
             * leave existing saved nominal weight unchanged.
             */
            return GetExistingMTCNominalWeight(
                model
            );
        }


        private object GetExistingMTCNominalWeight(
            QCInspectionRMDetailBLL model)
        {
            if (
                model != null
                &&
                model.NominalWeightValue.HasValue
            )
            {
                return
                    model.NominalWeightValue.Value;
            }

            return
                model == null
                    ? ""
                    : (
                        model.NominalWeight
                        ??
                        ""
                    );
        }


        private void SetXlsxNumberCell(
            XDocument sheetXml,
            string cellReference,
            object value)
        {
            XNamespace ns =
                "http://schemas.openxmlformats.org/spreadsheetml/2006/main";

            XElement cell =
                GetOrCreateXlsxCell(
                    sheetXml,
                    cellReference,
                    ns
                );

            cell.Attribute("t")?.Remove();
            cell.Elements(ns + "is").Remove();
            cell.Elements(ns + "f").Remove();
            cell.Elements(ns + "v").Remove();

            decimal parsed;

            string raw =
                Convert.ToString(
                    value,
                    CultureInfo.InvariantCulture
                );

            if (
                !decimal.TryParse(
                    raw,
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out parsed
                )
            )
            {
                SetXlsxTextCell(
                    sheetXml,
                    cellReference,
                    raw
                );

                return;
            }

            cell.Add(
                new XElement(
                    ns + "v",
                    parsed.ToString(
                        CultureInfo.InvariantCulture
                    )
                )
            );
        }


        private XElement GetOrCreateXlsxCell(
            XDocument sheetXml,
            string cellReference,
            XNamespace ns)
        {
            int rowNumber =
                GetCellRowNumber(
                    cellReference
                );

            XElement sheetData =
                sheetXml
                    .Descendants(ns + "sheetData")
                    .FirstOrDefault();

            if (sheetData == null)
                throw new Exception("Excel worksheet sheetData is missing.");

            XElement row =
                sheetData
                    .Elements(ns + "row")
                    .FirstOrDefault(
                        x =>
                            GetIntAttribute(
                                x,
                                "r"
                            )
                            ==
                            rowNumber
                    );

            if (row == null)
            {
                row =
                    new XElement(
                        ns + "row",
                        new XAttribute(
                            "r",
                            rowNumber
                        )
                    );

                XElement nextRow =
                    sheetData
                        .Elements(ns + "row")
                        .FirstOrDefault(
                            x =>
                                GetIntAttribute(
                                    x,
                                    "r"
                                )
                                >
                                rowNumber
                        );

                if (nextRow != null)
                    nextRow.AddBeforeSelf(row);
                else
                    sheetData.Add(row);
            }

            XElement cell =
                row
                    .Elements(ns + "c")
                    .FirstOrDefault(
                        x =>
                            string.Equals(
                                (
                                    x.Attribute("r") != null
                                        ? x.Attribute("r").Value
                                        : ""
                                ),
                                cellReference,
                                StringComparison.OrdinalIgnoreCase
                            )
                    );

            if (cell != null)
                return cell;

            cell =
                new XElement(
                    ns + "c",
                    new XAttribute(
                        "r",
                        cellReference
                    )
                );

            /*
             * IMPORTANT:
             * Blank Excel cells are often omitted from worksheet XML.
             * When we create a missing Row-15 limit cell, copy the style
             * from the same column in Row 16 so the value is visible and
             * keeps the existing MTC borders/alignment/font.
             */
            string columnLetters =
                new string(
                    cellReference
                        .TakeWhile(
                            x => !char.IsDigit(x)
                        )
                        .ToArray()
                );

            string styleSourceReference =
                columnLetters + "16";

            XElement styleSourceCell =
                sheetData
                    .Descendants(ns + "c")
                    .FirstOrDefault(
                        x =>
                            string.Equals(
                                x.Attribute("r") != null
                                    ? x.Attribute("r").Value
                                    : "",
                                styleSourceReference,
                                StringComparison.OrdinalIgnoreCase
                            )
                    );

            if (
                styleSourceCell != null
                &&
                styleSourceCell.Attribute("s") != null
            )
            {
                cell.SetAttributeValue(
                    "s",
                    styleSourceCell.Attribute("s").Value
                );
            }

            int columnNumber =
                GetCellColumnNumber(
                    cellReference
                );

            XElement nextCell =
                row
                    .Elements(ns + "c")
                    .FirstOrDefault(
                        x =>
                            GetCellColumnNumber(
                                (
                                    x.Attribute("r") != null
                                        ? x.Attribute("r").Value
                                        : ""
                                )
                            )
                            >
                            columnNumber
                    );

            if (nextCell != null)
                nextCell.AddBeforeSelf(cell);
            else
                row.Add(cell);

            return cell;
        }


        private int GetCellRowNumber(
            string cellReference)
        {
            string digits =
                new string(
                    (cellReference ?? "")
                        .Where(char.IsDigit)
                        .ToArray()
                );

            int rowNumber;

            if (
                !int.TryParse(
                    digits,
                    NumberStyles.Integer,
                    CultureInfo.InvariantCulture,
                    out rowNumber
                )
            )
            {
                throw new Exception(
                    "Invalid Excel cell reference: " +
                    cellReference
                );
            }

            return rowNumber;
        }


        private int GetCellColumnNumber(
            string cellReference)
        {
            string letters =
                new string(
                    (cellReference ?? "")
                        .Where(char.IsLetter)
                        .ToArray()
                )
                .ToUpperInvariant();

            int result = 0;

            foreach (char character in letters)
            {
                result =
                    result * 26 +
                    (character - 'A' + 1);
            }

            return result;
        }


        private int GetIntAttribute(
            XElement element,
            string attributeName)
        {
            int value;

            XAttribute attribute =
                element != null
                    ? element.Attribute(attributeName)
                    : null;

            if (
                attribute == null
                ||
                !int.TryParse(
                    attribute.Value,
                    NumberStyles.Integer,
                    CultureInfo.InvariantCulture,
                    out value
                )
            )
            {
                return 0;
            }

            return value;
        }


        /*
         * FINAL LIMIT MATCH ORDER
         *
         * 1. Specification
         * 2. Mapped Grade
         * 3. Product Type
         * 4. Bar Size rule
         *
         * Examples:
         * ASTM A615 G60 + 20mm
         *   -> Grade 60 + Rebar + Size <= 20mm
         *
         * ASTM A615 G60 + 22mm
         *   -> Grade 60 + Rebar + Size = 22mm,25mm
         *
         * ASTM A615 G60 + 32mm
         *   -> Grade 60 + Rebar + Size > 25mm
         *
         * ASTM A706 G60 + 20mm
         *   -> Grade 60 + Rebar + Size <= 20mm
         *
         * ASTM A706 G60 + 32mm
         *   -> Grade 60 + Rebar + Size > 20mm
         */
        private Dictionary<string, string> GetMTCGradeLimitsForDisplayRaw(
            string gradeLimitsPath,
            string specification,
            string steelGrade,
            string productType,
            decimal? barSize)
        {
            Dictionary<string, string> limits =
                CreateEmptyMTCDisplayLimits();

            using (
                FileStream stream =
                    new FileStream(
                        gradeLimitsPath,
                        FileMode.Open,
                        FileAccess.Read,
                        FileShare.ReadWrite
                    )
            )
            using (
                ZipArchive archive =
                    new ZipArchive(
                        stream,
                        ZipArchiveMode.Read,
                        false
                    )
            )
            {
                string sheetPath =
                    GetWorksheetPath(
                        archive,
                        "Grades Limits"
                    );

                XDocument sheetXml =
                    LoadZipXml(
                        archive,
                        sheetPath
                    );

                List<string> sharedStrings =
                    ReadSharedStrings(
                        archive
                    );

                XNamespace ns =
                    "http://schemas.openxmlformats.org/spreadsheetml/2006/main";

                List<XElement> rows =
                    sheetXml
                        .Descendants(ns + "row")
                        .Where(
                            x =>
                                GetIntAttribute(
                                    x,
                                    "r"
                                ) >= 4
                        )
                        .ToList();

                string combined =
                    (
                        (specification ?? "")
                        + " "
                        + (steelGrade ?? "")
                    ).Trim();

                List<XElement> identityMatches =
                    rows
                        .Where(
                            row =>
                            {
                                string rowSpec =
                                    GetXlsxCellText(
                                        row,
                                        "A",
                                        sharedStrings,
                                        ns
                                    );

                                string rowGrade =
                                    GetXlsxCellText(
                                        row,
                                        "B",
                                        sharedStrings,
                                        ns
                                    );

                                string rowSize =
                                    GetXlsxCellText(
                                        row,
                                        "D",
                                        sharedStrings,
                                        ns
                                    );

                                bool specMatch =
                                    MTCSpecificationMatches(
                                        rowSpec,
                                        specification
                                    )
                                    ||
                                    MTCSpecificationMatches(
                                        rowSpec,
                                        steelGrade
                                    )
                                    ||
                                    MTCSpecificationMatches(
                                        rowSpec,
                                        combined
                                    );

                                bool gradeMatch =
                                    MTCGradeCandidateMatchesLoose(
                                        rowGrade,
                                        steelGrade
                                    )
                                    ||
                                    MTCGradeCandidateMatchesLoose(
                                        rowGrade,
                                        specification
                                    )
                                    ||
                                    MTCGradeCandidateMatchesLoose(
                                        rowGrade,
                                        combined
                                    );

                                return
                                    specMatch
                                    &&
                                    gradeMatch
                                    &&
                                    MTCSizeRuleMatches(
                                        rowSize,
                                        barSize
                                    );
                            }
                        )
                        .ToList();

                if (identityMatches.Count == 0)
                {
                    return limits;
                }

                string requiredProductType =
                    NormalizeMTCProductTypeForLimits(
                        productType
                    );

                XElement matchedRow =
                    identityMatches
                        .FirstOrDefault(
                            row =>
                                MTCProductTypeMatchesExact(
                                    GetXlsxCellText(
                                        row,
                                        "C",
                                        sharedStrings,
                                        ns
                                    ),
                                    requiredProductType
                                )
                        );

                /*
                 * WORKBOOK-AWARE FALLBACK
                 *
                 * Grades Limits.xlsx defines:
                 *
                 * ASTM A510M  -> Wire Rod - Coil
                 * ASTM A615M  -> Rebar / Plain Bar
                 * ASTM A706M  -> Rebar
                 * BS 4449     -> Rebar
                 *
                 * If the selected form ProductType is inconsistent with the
                 * standard, use the standard's defined product family rather
                 * than leaving the complete limits row blank.
                 */
                if (matchedRow == null)
                {
                    string normalizedSpec =
                        NormalizeMTCSpecification(
                            combined
                        );

                    string fallbackProductType =
                        "";

                    if (
                        normalizedSpec.Contains(
                            "A510"
                        )
                    )
                    {
                        fallbackProductType =
                            "WIRERODCOIL";
                    }
                    else if (
                        normalizedSpec.Contains(
                            "A615"
                        )
                        ||
                        normalizedSpec.Contains(
                            "A706"
                        )
                        ||
                        normalizedSpec.Contains(
                            "BS4449"
                        )
                    )
                    {
                        fallbackProductType =
                            "REBAR";
                    }

                    if (
                        !string.IsNullOrWhiteSpace(
                            fallbackProductType
                        )
                    )
                    {
                        matchedRow =
                            identityMatches
                                .FirstOrDefault(
                                    row =>
                                        MTCProductTypeMatchesExact(
                                            GetXlsxCellText(
                                                row,
                                                "C",
                                                sharedStrings,
                                                ns
                                            ),
                                            fallbackProductType
                                        )
                                );
                    }
                }

                /*
                 * Final safe fallback:
                 * identityMatches already guarantees same
                 * Specification + Grade + Size.
                 */
                if (matchedRow == null)
                {
                    matchedRow =
                        identityMatches[0];
                }

                FillMTCDisplayLimitsFromRow(
                    limits,
                    matchedRow,
                    sharedStrings,
                    ns
                );
            }

            return limits;
        }


        private void FillMTCDisplayLimitsFromRow(
            Dictionary<string, string> limits,
            XElement matchedRow,
            List<string> sharedStrings,
            XNamespace ns)
        {
            if (
                limits == null
                ||
                matchedRow == null
            )
            {
                return;
            }

            limits["Yield"] =
                CleanMTCDisplayLimit(
                    GetXlsxCellText(
                        matchedRow,
                        "E",
                        sharedStrings,
                        ns
                    )
                );

            limits["Tensile"] =
                CleanMTCDisplayLimit(
                    GetXlsxCellText(
                        matchedRow,
                        "F",
                        sharedStrings,
                        ns
                    )
                );

            limits["Ratio"] =
                CleanMTCDisplayLimit(
                    GetXlsxCellText(
                        matchedRow,
                        "G",
                        sharedStrings,
                        ns
                    )
                );

            limits["Elongation"] =
                CleanMTCDisplayLimit(
                    GetXlsxCellText(
                        matchedRow,
                        "H",
                        sharedStrings,
                        ns
                    )
                );

            limits["C"] =
                CleanMTCDisplayLimit(
                    GetXlsxCellText(
                        matchedRow,
                        "I",
                        sharedStrings,
                        ns
                    )
                );

            limits["Si"] =
                CleanMTCDisplayLimit(
                    GetXlsxCellText(
                        matchedRow,
                        "J",
                        sharedStrings,
                        ns
                    )
                );

            limits["Mn"] =
                CleanMTCDisplayLimit(
                    GetXlsxCellText(
                        matchedRow,
                        "K",
                        sharedStrings,
                        ns
                    )
                );

            limits["P"] =
                CleanMTCDisplayLimit(
                    GetXlsxCellText(
                        matchedRow,
                        "L",
                        sharedStrings,
                        ns
                    )
                );

            limits["S"] =
                CleanMTCDisplayLimit(
                    GetXlsxCellText(
                        matchedRow,
                        "M",
                        sharedStrings,
                        ns
                    )
                );

            limits["Cu"] =
                CleanMTCDisplayLimit(
                    GetXlsxCellText(
                        matchedRow,
                        "N",
                        sharedStrings,
                        ns
                    )
                );

            limits["Ni"] =
                CleanMTCDisplayLimit(
                    GetXlsxCellText(
                        matchedRow,
                        "O",
                        sharedStrings,
                        ns
                    )
                );

            limits["Cr"] =
                CleanMTCDisplayLimit(
                    GetXlsxCellText(
                        matchedRow,
                        "P",
                        sharedStrings,
                        ns
                    )
                );

            limits["Mo"] =
                CleanMTCDisplayLimit(
                    GetXlsxCellText(
                        matchedRow,
                        "Q",
                        sharedStrings,
                        ns
                    )
                );

            limits["V"] =
                CleanMTCDisplayLimit(
                    GetXlsxCellText(
                        matchedRow,
                        "R",
                        sharedStrings,
                        ns
                    )
                );

            limits["B"] =
                CleanMTCDisplayLimit(
                    GetXlsxCellText(
                        matchedRow,
                        "S",
                        sharedStrings,
                        ns
                    )
                );

            limits["N"] =
                CleanMTCDisplayLimit(
                    GetXlsxCellText(
                        matchedRow,
                        "T",
                        sharedStrings,
                        ns
                    )
                );

            limits["Ceq"] =
                CleanMTCDisplayLimit(
                    GetXlsxCellText(
                        matchedRow,
                        "U",
                        sharedStrings,
                        ns
                    )
                );
        }


        private Dictionary<string, string> CreateEmptyMTCDisplayLimits()
        {
            return
                new Dictionary<string, string>(
                    StringComparer.OrdinalIgnoreCase
                )
                {
                    { "Yield", "--" },
                    { "Tensile", "--" },
                    { "Ratio", "--" },
                    { "Elongation", "--" },
                    { "C", "--" },
                    { "Si", "--" },
                    { "Mn", "--" },
                    { "P", "--" },
                    { "S", "--" },
                    { "Cu", "--" },
                    { "Ni", "--" },
                    { "Cr", "--" },
                    { "Mo", "--" },
                    { "V", "--" },
                    { "B", "--" },
                    { "N", "--" },
                    { "Ceq", "--" }
                };
        }


        private string CleanMTCDisplayLimit(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "--";

            string clean =
                value.Trim();

            return clean == ""
                ? "--"
                : clean;
        }


        private void PrepareMtcLimitRowLayout(
            XDocument sheetXml)
        {
            if (
                sheetXml == null
                ||
                sheetXml.Root == null
            )
            {
                return;
            }

            XNamespace ns =
                "http://schemas.openxmlformats.org/spreadsheetml/2006/main";

            XElement mergeCells =
                sheetXml.Root.Element(
                    ns + "mergeCells"
                );

            if (mergeCells == null)
            {
                return;
            }

            string[] removeRefs =
            {
                "H14:H15",
                "I14:I15",
                "K14:K15",
                "L14:L15",
                "M14:M15",
                "N14:N15",
                "O14:O15",
                "P14:P15",
                "Q14:Q15",
                "R14:R15",
                "S14:S15",
                "T14:W15"
            };

            List<XElement> removeList =
                mergeCells
                    .Elements(
                        ns + "mergeCell"
                    )
                    .Where(
                        x =>
                            removeRefs.Contains(
                                (
                                    string
                                )
                                x.Attribute(
                                    "ref"
                                ),
                                StringComparer.OrdinalIgnoreCase
                            )
                    )
                    .ToList();

            foreach (
                XElement item
                in removeList
            )
            {
                item.Remove();
            }

            /*
             * CE% spans four template columns.
             * Keep the header on row 14 and give the limit its own row 15.
             */
            mergeCells.Add(
                new XElement(
                    ns + "mergeCell",
                    new XAttribute(
                        "ref",
                        "T14:W14"
                    )
                )
            );

            mergeCells.Add(
                new XElement(
                    ns + "mergeCell",
                    new XAttribute(
                        "ref",
                        "T15:W15"
                    )
                )
            );

            XAttribute countAttribute =
                mergeCells.Attribute(
                    "count"
                );

            if (countAttribute != null)
            {
                countAttribute.Value =
                    mergeCells
                        .Elements(
                            ns + "mergeCell"
                        )
                        .Count()
                        .ToString(
                            CultureInfo.InvariantCulture
                        );
            }
        }


        private void EnsureMtcLimitRowCells(
            XDocument sheetXml)
        {
            XNamespace ns =
                "http://schemas.openxmlformats.org/spreadsheetml/2006/main";

            string[] cells =
            {
                "F15", "G15",
                "H15", "I15",
                "K15", "L15", "M15", "N15", "O15",
                "P15", "Q15", "R15", "S15", "T15"
            };

            foreach (
                string cellReference
                in cells
            )
            {
                XElement cell =
                    GetOrCreateXlsxCell(
                        sheetXml,
                        cellReference,
                        ns
                    );

                /*
                 * Existing F15/G15 already have styles in the template.
                 * Missing H15..T15 inherit style from the actual-value row.
                 */
                if (
                    cell.Attribute("s") == null
                )
                {
                    string columnLetters =
                        new string(
                            cellReference
                                .TakeWhile(
                                    x => !char.IsDigit(x)
                                )
                                .ToArray()
                        );

                    string sourceReference =
                        columnLetters + "16";

                    XElement sourceCell =
                        sheetXml
                            .Descendants(ns + "c")
                            .FirstOrDefault(
                                x =>
                                    string.Equals(
                                        x.Attribute("r") != null
                                            ? x.Attribute("r").Value
                                            : "",
                                        sourceReference,
                                        StringComparison.OrdinalIgnoreCase
                                    )
                            );

                    if (
                        sourceCell != null
                        &&
                        sourceCell.Attribute("s") != null
                    )
                    {
                        cell.SetAttributeValue(
                            "s",
                            sourceCell.Attribute("s").Value
                        );
                    }
                }
            }
        }


        private void WriteMTCGradeLimitsRow(
            XDocument sheetXml,
            List<string> sharedStrings,
            Dictionary<string, string> limits)
        {
            if (limits == null)
            {
                limits =
                    CreateEmptyMTCDisplayLimits();
            }

            /*
             * DO NOT CHANGE THE MTC TEMPLATE.
             *
             * Existing MTC Form mapping:
             *
             * Row 15 = visible limits.
             *
             * F15  Yield
             * G15  Tensile
             * H15  UTS/Y.S Ratio
             * I15  Elongation
             *
             * K15  C
             * L15  Si
             * M15  Mn
             * N15  P
             * O15  S
             * P15  Cu
             * Q15  V
             * R15  B
             * S15  N
             * T15  CE%
             *
             * Row 16 = Actual values.
             * J13:J15 stays Bend/Rebend header, so no Bend limit cell.
             */

            SetXlsxTextCell(
                sheetXml,
                "F15",
                GetMTCDisplayLimit(
                    limits,
                    "Yield"
                )
            );

            SetXlsxTextCell(
                sheetXml,
                "G15",
                GetMTCDisplayLimit(
                    limits,
                    "Tensile"
                )
            );

            SetXlsxTextCell(
                sheetXml,
                "H15",
                GetMTCDisplayLimit(
                    limits,
                    "Ratio"
                )
            );

            SetXlsxTextCell(
                sheetXml,
                "I15",
                GetMTCDisplayLimit(
                    limits,
                    "Elongation"
                )
            );

            SetXlsxTextCell(
                sheetXml,
                "K15",
                GetMTCDisplayLimit(
                    limits,
                    "C"
                )
            );

            SetXlsxTextCell(
                sheetXml,
                "L15",
                GetMTCDisplayLimit(
                    limits,
                    "Si"
                )
            );

            SetXlsxTextCell(
                sheetXml,
                "M15",
                GetMTCDisplayLimit(
                    limits,
                    "Mn"
                )
            );

            SetXlsxTextCell(
                sheetXml,
                "N15",
                GetMTCDisplayLimit(
                    limits,
                    "P"
                )
            );

            SetXlsxTextCell(
                sheetXml,
                "O15",
                GetMTCDisplayLimit(
                    limits,
                    "S"
                )
            );

            SetXlsxTextCell(
                sheetXml,
                "P15",
                GetMTCDisplayLimit(
                    limits,
                    "Cu"
                )
            );

            SetXlsxTextCell(
                sheetXml,
                "Q15",
                GetMTCDisplayLimit(
                    limits,
                    "V"
                )
            );

            SetXlsxTextCell(
                sheetXml,
                "R15",
                GetMTCDisplayLimit(
                    limits,
                    "B"
                )
            );

            SetXlsxTextCell(
                sheetXml,
                "S15",
                GetMTCDisplayLimit(
                    limits,
                    "N"
                )
            );

            SetXlsxTextCell(
                sheetXml,
                "T15",
                GetMTCDisplayLimit(
                    limits,
                    "Ceq"
                )
            );
        }


        private void WriteMTCActualChemistryByTemplateHeaders(
            XDocument sheetXml,
            List<string> sharedStrings,
            QCInspectionRMDetailBLL model)
        {
            if (model == null)
            {
                return;
            }

            /*
             * Same ORIGINAL MTC Form mapping.
             * No columns are inserted or moved.
             */
            SetXlsxNumberCell(
                sheetXml,
                "K16",
                model.C
            );

            SetXlsxNumberCell(
                sheetXml,
                "L16",
                model.Si
            );

            SetXlsxNumberCell(
                sheetXml,
                "M16",
                model.Mn
            );

            SetXlsxNumberCell(
                sheetXml,
                "N16",
                model.P
            );

            SetXlsxNumberCell(
                sheetXml,
                "O16",
                model.S
            );

            SetXlsxNumberCell(
                sheetXml,
                "P16",
                model.Cu
            );

            SetXlsxNumberCell(
                sheetXml,
                "Q16",
                model.V
            );

            SetXlsxNumberCell(
                sheetXml,
                "R16",
                model.B
            );

            SetXlsxNumberCell(
                sheetXml,
                "S16",
                model.N
            );

            SetXlsxNumberCell(
                sheetXml,
                "T16",
                model.Ceq
            );
        }


        private string GetMTCDisplayLimit(
            Dictionary<string, string> limits,
            string key)
        {
            string value;

            if (
                limits != null &&
                limits.TryGetValue(
                    key,
                    out value
                ) &&
                !string.IsNullOrWhiteSpace(value)
            )
            {
                return value.Trim();
            }

            return "--";
        }


        private void ResolveMTCSpecificationAndGradeFromLimits(
            string gradeLimitsPath,
            QCInspectionRMDetailBLL model,
            decimal? barSize)
        {
            if (
                model == null
                ||
                string.IsNullOrWhiteSpace(
                    gradeLimitsPath
                )
            )
            {
                return;
            }

            using (
                FileStream stream =
                    System.IO.File.OpenRead(
                        gradeLimitsPath
                    )
            )
            using (
                ZipArchive archive =
                    new ZipArchive(
                        stream,
                        ZipArchiveMode.Read,
                        false
                    )
            )
            {
                string sheetPath =
                    GetWorksheetPath(
                        archive,
                        "Grades Limits"
                    );

                XDocument sheetXml =
                    LoadZipXml(
                        archive,
                        sheetPath
                    );

                List<string> sharedStrings =
                    ReadSharedStrings(
                        archive
                    );

                XNamespace ns =
                    "http://schemas.openxmlformats.org/spreadsheetml/2006/main";

                List<XElement> rows =
                    sheetXml
                        .Descendants(
                            ns + "row"
                        )
                        .ToList();

                string originalSpecification =
                    model.Specification ?? "";

                string originalGrade =
                    model.SteelGrade ?? "";

                string combined =
                    (
                        originalSpecification +
                        " " +
                        originalGrade
                    ).Trim();


                /*
                 * ============================================================
                 * 1) PRIMARY:
                 * Specification + Grade match.
                 * ============================================================
                 */
                List<XElement> candidates =
                    new List<XElement>();

                foreach (
                    XElement row
                    in rows
                )
                {
                    string rowSpec =
                        GetXlsxCellText(
                            row,
                            "A",
                            sharedStrings,
                            ns
                        );

                    string rowGrade =
                        GetXlsxCellText(
                            row,
                            "B",
                            sharedStrings,
                            ns
                        );

                    if (
                        string.IsNullOrWhiteSpace(
                            rowSpec
                        )
                        ||
                        string.IsNullOrWhiteSpace(
                            rowGrade
                        )
                    )
                    {
                        continue;
                    }

                    bool specMatch =
                        MTCSpecificationMatches(
                            rowSpec,
                            originalSpecification
                        )
                        ||
                        MTCSpecificationMatches(
                            rowSpec,
                            originalGrade
                        )
                        ||
                        MTCSpecificationMatches(
                            rowSpec,
                            combined
                        );

                    if (!specMatch)
                    {
                        continue;
                    }

                    bool gradeMatch =
                        MTCGradeCandidateMatchesLoose(
                            rowGrade,
                            originalGrade
                        )
                        ||
                        MTCGradeCandidateMatchesLoose(
                            rowGrade,
                            originalSpecification
                        )
                        ||
                        MTCGradeCandidateMatchesLoose(
                            rowGrade,
                            combined
                        );

                    if (!gradeMatch)
                    {
                        continue;
                    }

                    candidates.Add(
                        row
                    );
                }


                /*
                 * ============================================================
                 * 2) FALLBACK FOR MALFORMED SPECIFICATION:
                 *
                 * Example:
                 *   Specification = S1006150   (bad source value)
                 *   Grade         = SAE 1006
                 *
                 * Grades Limits has AISI 1006 under ASTM A510M.
                 *
                 * Grade-only fallback is allowed ONLY when every row matching
                 * that grade belongs to ONE standard family. This prevents a
                 * blind guess for grades such as Grade 60, which can exist
                 * under more than one ASTM standard.
                 * ============================================================
                 */
                if (candidates.Count == 0)
                {
                    List<XElement> gradeOnlyCandidates =
                        new List<XElement>();

                    foreach (
                        XElement row
                        in rows
                    )
                    {
                        string rowSpec =
                            GetXlsxCellText(
                                row,
                                "A",
                                sharedStrings,
                                ns
                            );

                        string rowGrade =
                            GetXlsxCellText(
                                row,
                                "B",
                                sharedStrings,
                                ns
                            );

                        if (
                            string.IsNullOrWhiteSpace(
                                rowSpec
                            )
                            ||
                            string.IsNullOrWhiteSpace(
                                rowGrade
                            )
                        )
                        {
                            continue;
                        }

                        if (
                            MTCGradeCandidateMatchesLoose(
                                rowGrade,
                                originalGrade
                            )
                        )
                        {
                            gradeOnlyCandidates.Add(
                                row
                            );
                        }
                    }

                    List<string> standardFamilies =
                        gradeOnlyCandidates
                            .Select(
                                row =>
                                    ExtractMTCStandardCode(
                                        GetXlsxCellText(
                                            row,
                                            "A",
                                            sharedStrings,
                                            ns
                                        )
                                    )
                            )
                            .Where(
                                x =>
                                    !string.IsNullOrWhiteSpace(
                                        x
                                    )
                            )
                            .Distinct(
                                StringComparer.OrdinalIgnoreCase
                            )
                            .ToList();

                    if (
                        standardFamilies.Count == 1
                    )
                    {
                        candidates =
                            gradeOnlyCandidates;
                    }
                }


                if (
                    candidates.Count == 0
                )
                {
                    /*
                     * Keep original values if no safe authoritative row
                     * can be resolved.
                     */
                    return;
                }


                XElement matchedRow = null;


                /*
                 * Prefer Product Type + Size.
                 */
                string requiredProductType =
                    NormalizeMTCProductTypeForLimits(
                        model.ProductType
                    );

                matchedRow =
                    candidates
                        .FirstOrDefault(
                            row =>
                                MTCProductTypeMatchesExact(
                                    GetXlsxCellText(
                                        row,
                                        "C",
                                        sharedStrings,
                                        ns
                                    ),
                                    requiredProductType
                                )
                                &&
                                MTCSizeRuleMatches(
                                    GetXlsxCellText(
                                        row,
                                        "D",
                                        sharedStrings,
                                        ns
                                    ),
                                    barSize
                                )
                        );

                if (matchedRow == null)
                {
                    /*
                     * Product Type may not have a dedicated row in the
                     * workbook. We can still safely repair the certificate
                     * Specification/Grade header when all size-matched
                     * candidates belong to the same Specification + Grade.
                     */
                    List<XElement> sizeMatchedCandidates =
                        candidates
                            .Where(
                                row =>
                                    MTCSizeRuleMatches(
                                        GetXlsxCellText(
                                            row,
                                            "D",
                                            sharedStrings,
                                            ns
                                        ),
                                        barSize
                                    )
                            )
                            .ToList();

                    List<string> identities =
                        sizeMatchedCandidates
                            .Select(
                                row =>
                                    NormalizeMTCSpecification(
                                        GetXlsxCellText(
                                            row,
                                            "A",
                                            sharedStrings,
                                            ns
                                        )
                                    )
                                    +
                                    "|"
                                    +
                                    NormalizeMTCKey(
                                        GetXlsxCellText(
                                            row,
                                            "B",
                                            sharedStrings,
                                            ns
                                        )
                                    )
                            )
                            .Distinct(
                                StringComparer.OrdinalIgnoreCase
                            )
                            .ToList();

                    if (
                        identities.Count == 1
                        &&
                        sizeMatchedCandidates.Count > 0
                    )
                    {
                        matchedRow =
                            sizeMatchedCandidates[0];
                    }
                }

                if (matchedRow == null)
                {
                    return;
                }


                string resolvedSpecification =
                    GetXlsxCellText(
                        matchedRow,
                        "A",
                        sharedStrings,
                        ns
                    );

                string resolvedGrade =
                    GetXlsxCellText(
                        matchedRow,
                        "B",
                        sharedStrings,
                        ns
                    );


                /*
                 * Certificate display:
                 * ASTM A510M -> ASTM A510
                 * ASTM A615M -> ASTM A615
                 * ASTM A706M -> ASTM A706
                 *
                 * Matching still works because MTCSpecificationMatches()
                 * already treats the metric M suffix as optional.
                 */
                if (
                    !string.IsNullOrWhiteSpace(
                        resolvedSpecification
                    )
                )
                {
                    model.Specification =
                        FormatMTCSpecificationForDisplay(
                            resolvedSpecification
                        );
                }


                /*
                 * MTC GRADE DISPLAY RULE
                 *
                 * Rebar standards use the authoritative workbook grade:
                 *
                 * ASTM A615 G40 -> Grade 40
                 * ASTM A615 G60 -> Grade 60
                 * ASTM A615 G80 -> Grade 80
                 * ASTM A706 G60 -> Grade 60
                 * ASTM A706 G80 -> Grade 80
                 *
                 * SAE/AISI wire-rod grades may keep the original source
                 * wording when it identifies the same grade.
                 */
                string resolvedStandard =
                    ExtractMTCStandardCode(
                        resolvedSpecification
                    );

                bool isRebarStandard =
                    string.Equals(
                        resolvedStandard,
                        "A615",
                        StringComparison.OrdinalIgnoreCase
                    )
                    ||
                    string.Equals(
                        resolvedStandard,
                        "A706",
                        StringComparison.OrdinalIgnoreCase
                    );

                if (
                    isRebarStandard
                    &&
                    !string.IsNullOrWhiteSpace(
                        resolvedGrade
                    )
                )
                {
                    model.SteelGrade =
                        resolvedGrade.Trim();
                }
                else if (
                    string.IsNullOrWhiteSpace(
                        originalGrade
                    )
                    ||
                    !MTCGradeCandidateMatchesLoose(
                        resolvedGrade,
                        originalGrade
                    )
                )
                {
                    model.SteelGrade =
                        resolvedGrade.Trim();
                }
                else
                {
                    model.SteelGrade =
                        originalGrade.Trim();
                }
            }
        }


        private string FormatMTCSpecificationForDisplay(
            string specification)
        {
            if (
                string.IsNullOrWhiteSpace(
                    specification
                )
            )
            {
                return "";
            }

            string value =
                specification.Trim();

            /*
             * Remove metric suffix M only when it follows the numeric
             * ASTM designation:
             * A510M -> A510
             * A615M -> A615
             * A706M -> A706
             *
             * Do not alter other letters/text.
             */
            value =
                System.Text.RegularExpressions.Regex.Replace(
                    value,
                    @"(?<=\d)\s*M\b",
                    "",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase
                );

            /*
             * Normalize spacing for certificate display.
             */
            value =
                System.Text.RegularExpressions.Regex.Replace(
                    value,
                    @"\s+",
                    " "
                );

            return value.Trim();
        }


        private bool MTCGradeCandidateMatchesLoose(
            string excelGrade,
            string inputValue)
        {
            if (
                MTCGradeMatches(
                    excelGrade,
                    inputValue
                )
            )
            {
                return true;
            }

            if (
                string.IsNullOrWhiteSpace(
                    excelGrade
                )
                ||
                string.IsNullOrWhiteSpace(
                    inputValue
                )
            )
            {
                return false;
            }

            string expected =
                ExtractMTCGradeNumbers(
                    excelGrade
                );

            if (
                string.IsNullOrWhiteSpace(
                    expected
                )
            )
            {
                return false;
            }

            string input =
                inputValue
                    .ToUpperInvariant()
                    .Replace(" ", "");

            /*
             * Grade 60 / G60 / ...G60...
             */
            if (
                System.Text.RegularExpressions.Regex.IsMatch(
                    input,
                    @"(?:GRADE|GR|G)0*" +
                    System.Text.RegularExpressions.Regex.Escape(
                        expected
                    )
                )
            )
            {
                return true;
            }

            /*
             * AISI 1008 / SAE 1008 / plain 1008.
             */
            if (
                System.Text.RegularExpressions.Regex.IsMatch(
                    input,
                    @"(?:AISI|SAE)?0*" +
                    System.Text.RegularExpressions.Regex.Escape(
                        expected
                    ) +
                    @"(?:$|[^0-9])"
                )
            )
            {
                return true;
            }

            /*
             * Handles malformed concatenated values such as G6015012:
             * the business grade prefix G60 is still present.
             */
            if (
                expected.Length <= 3
                &&
                input.StartsWith(
                    "G" + expected,
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                return true;
            }

            return false;
        }


        private bool MTCSpecificationMatches(
            string excelSpecification,
            string inputSpecification)
        {
            string excelKey =
                NormalizeMTCSpecification(
                    excelSpecification
                );

            string inputKey =
                NormalizeMTCSpecification(
                    inputSpecification
                );

            if (
                string.IsNullOrWhiteSpace(excelKey) ||
                string.IsNullOrWhiteSpace(inputKey)
            )
            {
                return false;
            }

            if (
                string.Equals(
                    excelKey,
                    inputKey,
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                return true;
            }

            /*
             * Also compare the standard letter+number part.
             * Examples:
             * ASTM A510M / ASTM A510 / A510M -> A510
             * ASTM A615M / A615 -> A615
             */
            string excelStandard =
                ExtractMTCStandardCode(
                    excelSpecification
                );

            string inputStandard =
                ExtractMTCStandardCode(
                    inputSpecification
                );

            return
                !string.IsNullOrWhiteSpace(excelStandard)
                &&
                !string.IsNullOrWhiteSpace(inputStandard)
                &&
                string.Equals(
                    excelStandard,
                    inputStandard,
                    StringComparison.OrdinalIgnoreCase
                );
        }


        private string NormalizeMTCSpecification(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "";

            string normalized =
                value
                    .ToUpperInvariant()
                    .Trim();

            /*
             * Metric standard suffix M is optional for matching.
             * A510M == A510, A615M == A615, A706M == A706.
             */
            normalized =
                System.Text.RegularExpressions.Regex.Replace(
                    normalized,
                    @"(?<=\d)\s*M\b",
                    ""
                );

            normalized =
                System.Text.RegularExpressions.Regex.Replace(
                    normalized,
                    @"\bM\b",
                    ""
                );

            return
                System.Text.RegularExpressions.Regex.Replace(
                    normalized,
                    @"[^A-Z0-9]",
                    ""
                );
        }


        private string ExtractMTCStandardCode(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "";

            string upper =
                value.ToUpperInvariant();

            System.Text.RegularExpressions.Match match =
                System.Text.RegularExpressions.Regex.Match(
                    upper,
                    @"([A-Z])\s*([0-9]{2,4})\s*M?\b"
                );

            if (!match.Success)
                return NormalizeMTCSpecification(value);

            return
                match.Groups[1].Value
                +
                match.Groups[2].Value;
        }


        /*
         * Grade mapping examples:
         * ASTM A615 G40 => Grade 40
         * ASTM A615 G60 => Grade 60
         * ASTM A615 G80 => Grade 80
         * ASTM A706 G60 => Grade 60
         * ASTM A706 G80 => Grade 80
         * SAE 1008      => AISI 1008
         */
        private bool MTCGradeMatches(
            string excelGrade,
            string inputGrade)
        {
            string excelNumbers =
                ExtractMTCGradeNumbers(
                    excelGrade
                );

            string inputNumbers =
                ExtractMTCGradeNumbers(
                    inputGrade
                );

            /*
             * Primary business rule:
             * if grade contains numbers, those numbers drive the match.
             *
             * AISI 1008 / SAE 1008 / 1008 => same grade
             * Grade 60 / G60 / 60          => same grade
             */
            if (
                !string.IsNullOrWhiteSpace(excelNumbers) &&
                !string.IsNullOrWhiteSpace(inputNumbers)
            )
            {
                return
                    string.Equals(
                        excelNumbers,
                        inputNumbers,
                        StringComparison.OrdinalIgnoreCase
                    );
            }

            return
                string.Equals(
                    NormalizeMTCKey(excelGrade),
                    NormalizeMTCKey(inputGrade),
                    StringComparison.OrdinalIgnoreCase
                );
        }


        private string ExtractMTCGradeNumbers(
            string value)
        {
            if (
                string.IsNullOrWhiteSpace(
                    value
                )
            )
            {
                return "";
            }

            string upper =
                value
                    .ToUpperInvariant()
                    .Trim();

            /*
             * PRIMARY RULE FOR REBAR GRADES
             *
             * ASTM A615 G40 -> 40
             * ASTM A615 G60 -> 60
             * ASTM A615 G80 -> 80
             * ASTM A706 G60 -> 60
             * Grade 60      -> 60
             * G60           -> 60
             *
             * IMPORTANT:
             * A615/A706 standard number is NOT the grade.
             */
            System.Text.RegularExpressions.Match gradeMatch =
                System.Text.RegularExpressions.Regex.Match(
                    upper,
                    @"(?:GRADE|GR|G)\s*0*([0-9]{2,3})\b"
                );

            if (gradeMatch.Success)
            {
                return
                    gradeMatch
                        .Groups[1]
                        .Value
                        .TrimStart('0');
            }


            /*
             * WIRE ROD / SAE / AISI GRADES
             *
             * AISI 1008 -> 1008
             * SAE 1008  -> 1008
             * AISI 1006 -> 1006
             * SAE 1006  -> 1006
             */
            System.Text.RegularExpressions.Match saeAisiMatch =
                System.Text.RegularExpressions.Regex.Match(
                    upper,
                    @"(?:AISI|SAE)\s*0*([0-9]{3,4})\b"
                );

            if (saeAisiMatch.Success)
            {
                return
                    saeAisiMatch
                        .Groups[1]
                        .Value
                        .TrimStart('0');
            }


            /*
             * BS grade example:
             * B500B -> 500
             */
            System.Text.RegularExpressions.Match bsGradeMatch =
                System.Text.RegularExpressions.Regex.Match(
                    upper,
                    @"\bB\s*0*([0-9]{3})\s*B\b"
                );

            if (bsGradeMatch.Success)
            {
                return
                    bsGradeMatch
                        .Groups[1]
                        .Value
                        .TrimStart('0');
            }


            /*
             * Plain Grade text / numeric fallback.
             *
             * If multiple number groups exist and no explicit G/Grade token
             * was found, use the LAST number because composite values commonly
             * look like:
             *
             * ASTM A615 60
             * ASTM A706 80
             *
             * -> 60 / 80
             */
            List<string> numbers =
                System.Text.RegularExpressions.Regex
                    .Matches(
                        upper,
                        @"\d+"
                    )
                    .Cast<
                        System.Text.RegularExpressions.Match
                    >()
                    .Select(
                        x =>
                            x.Value.TrimStart('0')
                    )
                    .Where(
                        x =>
                            !string.IsNullOrWhiteSpace(
                                x
                            )
                    )
                    .ToList();

            if (
                numbers.Count == 0
            )
            {
                return "";
            }

            return
                numbers[
                    numbers.Count - 1
                ];
        }
        private string NormalizeMTCProductTypeForLimits(
            string productType)
        {
            string key =
                NormalizeMTCKey(
                    productType
                );

            if (
                key == "WR"
                ||
                key.Contains("WIREROD")
            )
            {
                return "WIRERODCOIL";
            }

            if (
                key == "PB"
                ||
                key.Contains("PLAINROD")
                ||
                key.Contains("PLAINBAR")
            )
            {
                return "PLAINBAR";
            }

            if (
                key == "RIC"
                ||
                key.Contains("REBARCOIL")
                ||
                key.Contains("REBARINCOIL")
            )
            {
                return "REBAR";
            }

            if (
                key == "RB"
                ||
                key.Contains("REBAR")
            )
            {
                return "REBAR";
            }

            return key;
        }


        private bool MTCProductTypeMatchesExact(
            string excelProductType,
            string requiredProductType)
        {
            string excelKey =
                NormalizeMTCProductTypeForLimits(
                    excelProductType
                );

            string requiredKey =
                NormalizeMTCProductTypeForLimits(
                    requiredProductType
                );

            if (
                string.IsNullOrWhiteSpace(
                    requiredKey
                )
            )
            {
                return false;
            }

            return string.Equals(
                excelKey,
                requiredKey,
                StringComparison.OrdinalIgnoreCase
            );
        }


        private bool MTCSizeRuleMatches(
            string sizeRule,
            decimal? barSize)
        {
            string rule =
                (
                    sizeRule
                    ??
                    ""
                )
                .Trim()
                .ToUpperInvariant();

            /*
             * Blank / -- means the grade is not size-restricted.
             */
            if (
                string.IsNullOrWhiteSpace(
                    rule
                )
                ||
                rule == "--"
                ||
                rule == "-"
            )
            {
                return true;
            }

            if (!barSize.HasValue)
            {
                return false;
            }

            decimal size =
                barSize.Value;

            /*
             * Normalize workbook text:
             * "Size ≤ 20mm"
             * "Size = 22mm , 25mm"
             * "Size > 25mm"
             * "Size ≥ 28mm"
             */
            string compact =
                rule
                    .Replace("SIZE", "")
                    .Replace("MM", "")
                    .Replace(" ", "")
                    .Replace("≤", "<=")
                    .Replace("≥", ">=")
                    .Replace("=<", "<=")
                    .Replace("=>", ">=");

            /*
             * Exact listed sizes:
             * =22,25
             * =22MM,25MM
             */
            if (
                compact.StartsWith("=")
            )
            {
                string body =
                    compact.Substring(1);

                List<decimal> listedSizes =
                    System.Text.RegularExpressions.Regex
                        .Matches(
                            body,
                            @"\d+(?:\.\d+)?"
                        )
                        .Cast<
                            System.Text.RegularExpressions.Match
                        >()
                        .Select(
                            x =>
                            {
                                decimal parsed;

                                return
                                    decimal.TryParse(
                                        x.Value,
                                        NumberStyles.Any,
                                        CultureInfo.InvariantCulture,
                                        out parsed
                                    )
                                        ? (decimal?)parsed
                                        : null;
                            }
                        )
                        .Where(
                            x =>
                                x.HasValue
                        )
                        .Select(
                            x =>
                                x.Value
                        )
                        .ToList();

                return
                    listedSizes.Any(
                        x =>
                            x == size
                    );
            }

            /*
             * <= value
             */
            System.Text.RegularExpressions.Match lessEqual =
                System.Text.RegularExpressions.Regex.Match(
                    compact,
                    @"<=([0-9]+(?:\.[0-9]+)?)"
                );

            if (lessEqual.Success)
            {
                decimal limit;

                if (
                    decimal.TryParse(
                        lessEqual.Groups[1].Value,
                        NumberStyles.Any,
                        CultureInfo.InvariantCulture,
                        out limit
                    )
                )
                {
                    return
                        size <= limit;
                }
            }

            /*
             * >= value
             */
            System.Text.RegularExpressions.Match greaterEqual =
                System.Text.RegularExpressions.Regex.Match(
                    compact,
                    @">=([0-9]+(?:\.[0-9]+)?)"
                );

            if (greaterEqual.Success)
            {
                decimal limit;

                if (
                    decimal.TryParse(
                        greaterEqual.Groups[1].Value,
                        NumberStyles.Any,
                        CultureInfo.InvariantCulture,
                        out limit
                    )
                )
                {
                    return
                        size >= limit;
                }
            }

            /*
             * > value
             */
            System.Text.RegularExpressions.Match greater =
                System.Text.RegularExpressions.Regex.Match(
                    compact,
                    @">([0-9]+(?:\.[0-9]+)?)"
                );

            if (greater.Success)
            {
                decimal limit;

                if (
                    decimal.TryParse(
                        greater.Groups[1].Value,
                        NumberStyles.Any,
                        CultureInfo.InvariantCulture,
                        out limit
                    )
                )
                {
                    return
                        size > limit;
                }
            }

            /*
             * < value
             */
            System.Text.RegularExpressions.Match less =
                System.Text.RegularExpressions.Regex.Match(
                    compact,
                    @"<([0-9]+(?:\.[0-9]+)?)"
                );

            if (less.Success)
            {
                decimal limit;

                if (
                    decimal.TryParse(
                        less.Groups[1].Value,
                        NumberStyles.Any,
                        CultureInfo.InvariantCulture,
                        out limit
                    )
                )
                {
                    return
                        size < limit;
                }
            }

            /*
             * If the workbook contains one plain numeric size,
             * treat it as an exact size.
             */
            List<decimal> numbers =
                System.Text.RegularExpressions.Regex
                    .Matches(
                        compact,
                        @"\d+(?:\.\d+)?"
                    )
                    .Cast<
                        System.Text.RegularExpressions.Match
                    >()
                    .Select(
                        x =>
                        {
                            decimal parsed;

                            return
                                decimal.TryParse(
                                    x.Value,
                                    NumberStyles.Any,
                                    CultureInfo.InvariantCulture,
                                    out parsed
                                )
                                    ? (decimal?)parsed
                                    : null;
                        }
                    )
                    .Where(
                        x =>
                            x.HasValue
                    )
                    .Select(
                        x =>
                            x.Value
                    )
                    .ToList();

            if (
                numbers.Count == 1
            )
            {
                return
                    size == numbers[0];
            }

            /*
             * Unknown size rule:
             * do not match accidentally.
             */
            return false;
        }
        private void ValidateMTCLimit(
            List<string> errors,
            string fieldName,
            decimal? reading,
            string limitText)
        {
            decimal? minimum;
            decimal? maximum;

            /*
             * No limit in Grades Limits.xlsx = no validation.
             * Blank / "--" / unsupported limit text means this field
             * must NOT block MTC generation.
             */
            if (
                !TryParseMTCLimit(
                    limitText,
                    out minimum,
                    out maximum
                )
            )
            {
                return;
            }

            /*
             * A real limit exists, therefore a reading is required.
             */
            if (!reading.HasValue)
            {
                errors.Add(
                    fieldName +
                    " reading is required because a grade limit is defined."
                );

                return;
            }

            if (
                minimum.HasValue &&
                reading.Value < minimum.Value
            )
            {
                errors.Add(
                    fieldName +
                    " " +
                    reading.Value.ToString(
                        "0.###",
                        CultureInfo.InvariantCulture
                    ) +
                    " is below minimum " +
                    minimum.Value.ToString(
                        "0.###",
                        CultureInfo.InvariantCulture
                    )
                );
            }

            if (
                maximum.HasValue &&
                reading.Value > maximum.Value
            )
            {
                errors.Add(
                    fieldName +
                    " " +
                    reading.Value.ToString(
                        "0.###",
                        CultureInfo.InvariantCulture
                    ) +
                    " exceeds maximum " +
                    maximum.Value.ToString(
                        "0.###",
                        CultureInfo.InvariantCulture
                    )
                );
            }
        }


        private bool TryParseMTCLimit(
            string limitText,
            out decimal? minimum,
            out decimal? maximum)
        {
            minimum = null;
            maximum = null;

            if (
                string.IsNullOrWhiteSpace(limitText) ||
                limitText.Trim() == "--"
            )
            {
                return false;
            }

            string textValue =
                limitText
                    .Trim()
                    .ToUpperInvariant();

            System.Text.RegularExpressions.Match minMatch =
                System.Text.RegularExpressions.Regex.Match(
                    textValue,
                    @"MIN\s*\.?\s*([0-9]+(?:\.[0-9]+)?)"
                );

            System.Text.RegularExpressions.Match maxMatch =
                System.Text.RegularExpressions.Regex.Match(
                    textValue,
                    @"MAX\s*\.?\s*([0-9]+(?:\.[0-9]+)?)"
                );

            if (minMatch.Success)
            {
                minimum =
                    Convert.ToDecimal(
                        minMatch.Groups[1].Value,
                        CultureInfo.InvariantCulture
                    );
            }

            if (maxMatch.Success)
            {
                maximum =
                    Convert.ToDecimal(
                        maxMatch.Groups[1].Value,
                        CultureInfo.InvariantCulture
                    );
            }

            if (
                minimum.HasValue ||
                maximum.HasValue
            )
            {
                return true;
            }

            System.Text.RegularExpressions.Match rangeMatch =
                System.Text.RegularExpressions.Regex.Match(
                    textValue,
                    @"([0-9]+(?:\.[0-9]+)?)\s*TO\s*([0-9]+(?:\.[0-9]+)?)"
                );

            if (rangeMatch.Success)
            {
                minimum =
                    Convert.ToDecimal(
                        rangeMatch.Groups[1].Value,
                        CultureInfo.InvariantCulture
                    );

                maximum =
                    Convert.ToDecimal(
                        rangeMatch.Groups[2].Value,
                        CultureInfo.InvariantCulture
                    );

                return true;
            }

            System.Text.RegularExpressions.Match comparatorMatch =
                System.Text.RegularExpressions.Regex.Match(
                    textValue,
                    @"(<=|>=|<|>|≤|≥)\s*([0-9]+(?:\.[0-9]+)?)"
                );

            if (comparatorMatch.Success)
            {
                decimal compareValue =
                    Convert.ToDecimal(
                        comparatorMatch.Groups[2].Value,
                        CultureInfo.InvariantCulture
                    );

                string operation =
                    comparatorMatch.Groups[1].Value;

                if (
                    operation == "<=" ||
                    operation == "<" ||
                    operation == "≤"
                )
                {
                    maximum = compareValue;
                }
                else
                {
                    minimum = compareValue;
                }

                return true;
            }

            return false;
        }


        private string NormalizeMTCKey(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "";

            return
                System.Text.RegularExpressions.Regex.Replace(
                    value.ToUpperInvariant(),
                    @"[^A-Z0-9]",
                    ""
                );
        }


        // ============================================================
        // EDIT BILLET
        // ============================================================
        [HttpGet]
        public ActionResult EditBillet(int id)
        {
            try
            {
                if (id <= 0)
                {
                    TempData["ErrorMessage"] = "Invalid billet boarding record.";
                    return RedirectToAction("BilletBoard");
                }

                BilletBoardBLL model = repo.GetBilletDetails(id);

                if (model == null)
                {
                    TempData["ErrorMessage"] = "Billet boarding record not found.";
                    return RedirectToAction("BilletBoard");
                }

                return View("EditBillet", model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("BilletBoard");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditBillet(BilletBoardBLL model)
        {
            try
            {
                if (model == null || model.ID <= 0)
                    throw new Exception("Invalid billet boarding record.");

                model.UpdatedBy = GetCurrentUser();
                model.UpdatedDate = DateTime.Now;
                repo.UpdateBillet(model);

                TempData["SuccessMessage"] = "Billet boarding record updated successfully.";
                return RedirectToAction("BilletBoard");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View("EditBillet", model);
            }
        }

        // ============================================================
        // ADD BILLET POST
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddBillet(BilletBoardBLL data)
        {
            bool isEdit = data != null && data.ID > 0;

            try
            {
                if (data == null)
                    return BilletSaveError("Invalid data.", false, 0);

                data.BilletBoarding = CleanRMText(data.BilletBoarding);

                if (string.IsNullOrWhiteSpace(data.BilletBoarding))
                    return BilletSaveError("Billet Boarding number is required.", isEdit, data.ID);

                var chemistry = (data.Chemistry ?? new List<RMChemicalAnalysisBLL>())
                    .Where(x => x != null && !string.IsNullOrWhiteSpace(x.HeatNo))
                    .ToList();

                if (!chemistry.Any())
                    return BilletSaveError("At least one Chemical Analysis Heat No is required.", isEdit, data.ID);

                foreach (var item in chemistry)
                    item.HeatNo = item.HeatNo.Trim();

                var formDuplicateHeats = chemistry
                    .GroupBy(x => x.HeatNo, StringComparer.OrdinalIgnoreCase)
                    .Where(x => x.Count() > 1)
                    .Select(x => x.Key)
                    .ToList();

                if (formDuplicateHeats.Any())
                    return BilletSaveError(
                        "Duplicate Heat No(s) entered: " + string.Join(", ", formDuplicateHeats),
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
                    oldChemistry = repo.GetBilletChemistryForEdit(data.ID)
                        ?? new List<RMChemicalAnalysisBLL>();

                    oldChemistry = oldChemistry
                        .Where(x => x != null && x.StatusID != 3)
                        .ToList();

                    var allowedIDs = new HashSet<int>(oldChemistry.Select(x => x.ID));

                    bool invalidIDPosted = chemistry.Any(x => x.ID > 0 && !allowedIDs.Contains(x.ID));
                    if (invalidIDPosted)
                        return BilletSaveError("Invalid Chemical Analysis record submitted.", true, data.ID);
                }

                bool boardingDuplicate = !isEdit
                    ? repo.IsBilletBoardingExists(data.BilletBoarding)
                    : repo.IsBilletBoardingExistsForEdit(data.BilletBoarding, data.ID);

                if (boardingDuplicate)
                    return BilletSaveError("This Billet Boarding number already exists.", isEdit, data.ID);

                var excludedIDs = isEdit
                    ? oldChemistry.Select(x => x.ID).ToList()
                    : new List<int>();

                var heatNosForDatabaseCheck = !isEdit
                    ? heatNos
                    : chemistry
                        .Where(x => x.ID <= 0 || oldChemistry.Any(old =>
                            old.ID == x.ID &&
                            !string.Equals(
                                CleanRMText(old.HeatNo),
                                x.HeatNo,
                                StringComparison.OrdinalIgnoreCase
                            )))
                        .Select(x => x.HeatNo)
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToList();

                var databaseDuplicateHeats = heatNosForDatabaseCheck.Any()
                    ? repo.GetDuplicateHeatNosExcludingIDs(heatNosForDatabaseCheck, excludedIDs)
                        ?? new List<string>()
                    : new List<string>();

                if (databaseDuplicateHeats.Any())
                    return BilletSaveError(
                        "These Heat No(s) already exist in active Chemical Analysis: "
                        + string.Join(", ", databaseDuplicateHeats),
                        isEdit,
                        data.ID
                    );

                CalculateBilletWeight(data);

                string currentUser = GetCurrentUser();
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
                        repo.InsertBilletBoarding(CreateBilletHeatRow(
                            data,
                            item.HeatNo,
                            currentUser,
                            now
                        ));
                    }

                    TempData["SuccessMessage"] = "Billet Boarding and Chemical Analysis added successfully.";
                    return RedirectToAction("BilletBoard");
                }

                data.StatusID = 1;
                data.UpdatedBy = currentUser;
                data.UpdatedDate = now;
                repo.UpdateBilletBoarding(data);

                var postedExistingIDs = new HashSet<int>(
                    chemistry.Where(x => x.ID > 0).Select(x => x.ID)
                );

                var removedChemistry = oldChemistry
                    .Where(x => !postedExistingIDs.Contains(x.ID))
                    .ToList();

                foreach (var removed in removedChemistry)
                    repo.DeactivateChemicalAnalysisRM(removed.ID, currentUser, now);

                for (int index = 0; index < chemistry.Count; index++)
                {
                    var item = chemistry[index];
                    int srNo = index + 1;

                    if (item.ID > 0)
                    {
                        var oldItem = oldChemistry.First(x => x.ID == item.ID);
                        string oldHeatNo = CleanRMText(oldItem.HeatNo);

                        if (!oldHeatNo.Equals(item.HeatNo, StringComparison.OrdinalIgnoreCase))
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
                        repo.UpdateChemicalAnalysisRM(item, srNo);
                    }
                    else
                    {
                        item.StatusID = 1;
                        item.CreatedBy = currentUser;
                        item.CreatedDate = now;

                        repo.InsertChemicalAnalysisRM(item, srNo);
                        repo.InsertBilletBoarding(CreateBilletHeatRow(
                            data,
                            item.HeatNo,
                            currentUser,
                            now
                        ));
                    }
                }

                TempData["SuccessMessage"] = "Billet Boarding and Chemical Analysis updated successfully.";
                return RedirectToAction("BilletBoard");
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
                var normalizedHeatNos = (heatNos ?? new List<string>())
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

                excludedIDs = (excludedIDs ?? new List<int>())
                    .Where(x => x > 0)
                    .Distinct()
                    .ToList();

                var duplicates = repo.GetDuplicateHeatNosExcludingIDs(
                    normalizedHeatNos,
                    excludedIDs
                ) ?? new List<string>();

                return Json(new { success = true, duplicates = duplicates });
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

        private ActionResult BilletSaveError(string message, bool isEdit, int id)
        {
            TempData["ErrorMessage"] = message;
            return RedirectToAction("AddBillet", isEdit ? new { id = id } : null);
        }

        private void CalculateBilletWeight(BilletBoardBLL data)
        {
            decimal billetLength = 0M;
            decimal.TryParse(data.BilletLength, out billetLength);

            decimal billetWeight = 0M;

            if (!string.IsNullOrWhiteSpace(data.CrossSection))
            {
                string[] parts = data.CrossSection
                    .Replace(" ", string.Empty)
                    .ToLowerInvariant()
                    .Split('x');

                int width;
                int height;

                if (parts.Length == 2
                    && int.TryParse(parts[0], out width)
                    && int.TryParse(parts[1], out height)
                    && width == 150
                    && height == 150)
                {
                    billetWeight = 175M * billetLength / 1000M;
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

        // ============================================================
        // COMMON HELPERS
        // ============================================================
        private string GetCurrentUser()
        {
            string currentUser = Convert.ToString(Session["UserName"]);

            if (string.IsNullOrWhiteSpace(currentUser)
                && User != null
                && User.Identity != null)
            {
                currentUser = User.Identity.Name;
            }

            return string.IsNullOrWhiteSpace(currentUser)
                ? "System"
                : currentUser.Trim();
        }

        private string MakeSafeFileName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "UnknownHeat";

            foreach (char invalidCharacter in Path.GetInvalidFileNameChars())
                value = value.Replace(invalidCharacter, '_');

            return value.Trim();
        }
    }
}
