using BAL.Repositories;
using ClosedXML.Excel;
using DAL.Models;
using Newtonsoft.Json;
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
using static DAL.Models.ViewModel.RollingMillChargeVM;

namespace ProductionPortal_Solb.Controllers
{
    [SessionState(SessionStateBehavior.Required)]
    public class QualityController : Controller
    {
        private readonly QualityRepository repo;

        public QualityController()
        {
            repo = new QualityRepository();
        }

        // ============================================================
        // BILLET BOARD
        // ============================================================
        public ActionResult BilletBoard()
        {
            var billets = repo.GetAllBoarding();
            return View("~/Views/Quality/BilletBoard/BilletBoard.cshtml", billets);
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
        public JsonResult GetMTCDetails(int mtcID)
        {
            try
            {
                if (mtcID <= 0)
                {
                    Response.StatusCode = 400;
                    return Json(new
                    {
                        success = false,
                        message = "A valid MTC record must be selected."
                    }, JsonRequestBehavior.AllowGet);
                }

                var data = repo.GetMTCDetails(mtcID);

                if (data == null)
                {
                    Response.StatusCode = 404;
                    return Json(new
                    {
                        success = false,
                        message = "The selected MTC record was not found."
                    }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = true, data = data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new
                {
                    success = false,
                    message = "The selected MTC data could not be loaded. " + ex.Message
                }, JsonRequestBehavior.AllowGet);
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
        public ActionResult castmillCertificate()
        {
            var billets = repo.GetAllBoarding();
            return View("~/Views/Quality/CastMillCertificate/castmillCertificate.cshtml", billets);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GenerateMTC(QCInspectionRMDetailBLL model)
        {
            try
            {
                if (model == null)
                    throw new Exception("MTC data was not received.");

                if (model.MTCID <= 0)
                    throw new Exception("Please select an MTC record.");

                if (string.IsNullOrWhiteSpace(model.HeatNo))
                    throw new Exception("Heat number is required.");

                string templatePath = Server.MapPath("~/Templates/MTCTemplate.xlsx");

                if (!System.IO.File.Exists(templatePath))
                    throw new FileNotFoundException("MTC Excel template was not found.", templatePath);

                using (XLWorkbook workbook = new XLWorkbook(templatePath))
                {
                    IXLWorksheet sheet = workbook.Worksheet(1);

                    sheet.Cell("B7").Value = model.Specification ?? "";
                    sheet.Cell("E7").Value = model.SteelGrade ?? "";
                    sheet.Cell("N10").Value = DateTime.Today;
                    sheet.Cell("N10").Style.DateFormat.Format = "dd-MMM-yyyy";

                    int rowNo = 16;
                    sheet.Cell(rowNo, 2).Value = model.BarSize;
                    sheet.Cell(rowNo, 3).Value = model.NominalWeight;
                    sheet.Cell(rowNo, 4).Value = model.IsWireRodOrCoil ? "Wire Rod / Coil" : "Deformed Steel Bar";
                    sheet.Cell(rowNo, 5).Value = model.HeatNo ?? "";
                    sheet.Cell(rowNo, 6).Value = model.YieldStrength;
                    sheet.Cell(rowNo, 7).Value = model.TensileStrength;
                    sheet.Cell(rowNo, 8).Value = model.TensileYieldRatio;
                    sheet.Cell(rowNo, 9).Value = model.Elongation;
                    sheet.Cell(rowNo, 10).Value = model.BendTestObserved ? "Satisfactory" : "";
                    sheet.Cell(rowNo, 11).Value = model.C;
                    sheet.Cell(rowNo, 12).Value = model.Si;
                    sheet.Cell(rowNo, 13).Value = model.Mn;
                    sheet.Cell(rowNo, 14).Value = model.P;
                    sheet.Cell(rowNo, 15).Value = model.S;
                    sheet.Cell(rowNo, 16).Value = "";
                    sheet.Cell(rowNo, 17).Value = "";
                    sheet.Cell(rowNo, 18).Value = "";
                    sheet.Cell(rowNo, 19).Value = model.N;
                    sheet.Cell(rowNo, 20).Value = model.Ceq;

                    sheet.Cell("B49").Value = "MTC generated using : "
                        + GetCurrentUser()
                        + "  "
                        + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);

                    string safeHeatNo = MakeSafeFileName(model.HeatNo);
                    string fileName = "MTC_" + safeHeatNo + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";

                    using (MemoryStream stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        return File(
                            stream.ToArray(),
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            fileName
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("QCInspectionRMadd", new
                {
                    rm = model != null ? model.RollingMill : "RM1"
                });
            }
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
