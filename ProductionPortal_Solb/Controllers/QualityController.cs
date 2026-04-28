using BAL.Repositories;
using DAL.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using Org.BouncyCastle.Asn1.X500;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static DAL.Models.ViewModel;

namespace ProductionPortal_Solb.Controllers
{
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
        public ActionResult AddBillet()
        {
            var heat = repo.GetAllChemistry()
                .Where(x => x.Area == "Rolling Mill 1" || x.Area == "Rolling Mill 2")
                .Select(x => new { x.HeatNo, x.Area })
                .Distinct()
                .ToList();

            ViewBag.HeatNo = new SelectList(heat);
            var BilletGradeList = repo.GetBilletGrade();
            ViewBag.BilletGrade = new SelectList(BilletGradeList, "ProductID", "SpecGrade");
            ViewBag.GradeDataJson = JsonConvert.SerializeObject(BilletGradeList);

            return View("~/Views/Quality/BilletBoard/AddBillet.cshtml");
        }

        [HttpPost]
        public ActionResult AddBillet(BilletBoardBLL data)
        {
            if (data == null)
            {
                TempData["ErrorMessage"] = "Invalid data.";
                return View(data);
            }

            decimal billetLength = 0;
            if (!string.IsNullOrWhiteSpace(data.BilletLength))
            {
                decimal.TryParse(data.BilletLength, out billetLength);
            }

            decimal billetWeight = 0;
            if (!string.IsNullOrEmpty(data.CrossSection))
            {
                var parts = data.CrossSection.Replace(" ", "").ToLower().Split('x');

                if (parts.Length == 2)
                {
                    int w = 0;
                    int h = 0;

                    int.TryParse(parts[0], out w);
                    int.TryParse(parts[1], out h);

                    // Rule: 150x150
                    if (w == 150 && h == 150)
                    {
                        billetWeight = 175m * billetLength / 1000;
                    }
                }
            }

            data.BilletWeight = billetWeight;
            data.StatusID = 1;
            data.CreatedDate = DateTime.Now;
            data.CreatedBy = User.Identity.Name;

            if (data.Chemistry == null || !data.Chemistry.Any())
            {
                TempData["ErrorMessage"] = "Chemistry data not found.";
                return View(data);
            }

            // Valid chemistry rows jahan HeatNo available ho
            var validChemistry = data.Chemistry
                .Where(x => x != null && !string.IsNullOrWhiteSpace(x.HeatNo))
                .ToList();

            if (!validChemistry.Any())
            {
                TempData["ErrorMessage"] = "Heat No not found in chemistry.";
                return View(data);
            }

            int rtn = 0;

            // 1) Pehle sari chemistry rows insert karo
            foreach (var chem in validChemistry)
            {
                var chemData = new RMChemicalAnalysisBLL
                {
                    HeatNo = chem.HeatNo.Trim(),
                    StatusID = 1,
                    CreatedDate = DateTime.Now,
                    CreatedBy = User.Identity.Name,

                    C = chem.C,
                    Mn = chem.Mn,
                    Si = chem.Si,
                    P = chem.P,
                    S = chem.S,
                    N = chem.N,
                    Ceq = chem.Ceq,
                    HeatStatus = chem.HeatStatus,
                    NoOfBillets = chem.NoOfBillets
                };

                repo.InsertChemicalAnalysisRM(chemData);
            }

            // 2) Unique HeatNos ke against billet boarding sirf ek baar insert karo
            var uniqueHeatNos = validChemistry
                .Select(x => x.HeatNo.Trim())
                .Distinct()
                .ToList();

            foreach (var heatNo in uniqueHeatNos)
            {
                var billetData = new BilletBoardBLL
                {
                    HeatNo = heatNo,
                    BilletLength = data.BilletLength,
                    CrossSection = data.CrossSection,
                    BilletWeight = data.BilletWeight,
                    StatusID = 1,
                    CreatedDate = DateTime.Now,
                    CreatedBy = User.Identity.Name,

                    PlantName = data.PlantName,
                    Shift = data.Shift,
                    SteelGrade = data.SteelGrade,
                    Profile = data.Profile,
                    Size = data.Size,
                    BilletBoarding = data.BilletBoarding,
                    ProductSpecs = data.ProductSpecs,
                    Remarks = data.Remarks
                };

                rtn = repo.InsertBilletBoarding(billetData);
            }

            TempData["SuccessMessage"] = "Billet Boarding inserted successfully against all Heat Numbers.";
            return RedirectToAction("BilletBoard");
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
        public ActionResult QCInspectionRMadd()
        {
            return View("~/Views/Quality/RMMechanical/add.cshtml");
        }

        public ActionResult InspectionRMlist()
        {
            return View("~/Views/Quality/QCInspectionRM/InspectionRMlist.cshtml");
        }

        public ActionResult AddInspectionRM()
        {
            return View("~/Views/Quality/QCInspectionRM/AddInspectionRM.cshtml");
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
        public ActionResult AddSlagByProduct(SlagByProductAnalysisBLL data)
        {
            if (data == null)
            {
                TempData["ErrorMessage"] = "Invalid data submitted.";
                return RedirectToAction("SlagByProductList");
            }

            // ✅ Ensure samples list not null
            if (data.Samples == null)
                data.Samples = new List<SlagSampleAnalysisBLL>();

            // ============================
            // ✅ ADD MODE (Insert)
            // ============================
            if (data.ID == 0)
            {
                data.StatusID = 1;
                data.CreatedDate = DateTime.Now;
                data.CreatedBy = User.Identity.Name;

                int newID = repo.InsertSlagByProduct(data);   // returns newly inserted SlagID

                if (newID <= 0)
                {
                    TempData["ErrorMessage"] = "Data not saved. Please try again.";
                    return RedirectToAction("SlagByProductList");
                }

                foreach (var item in data.Samples)
                {
                    // optional: skip empty rows
                    if (string.IsNullOrWhiteSpace(item.SampleCode) &&
                        item.CaO == null && item.MgO == null && item.SiO2 == null)
                        continue;

                    var bll = new SlagSampleAnalysisBLL
                    {
                        SampleCode = item.SampleCode,
                        SampleTime = item.SampleTime,
                        CaO = item.CaO,
                        MgO = item.MgO,
                        SiO2 = item.SiO2,
                        Al2O3 = item.Al2O3,
                        Fe2O3 = item.Fe2O3,
                        S = item.S,
                        MnO = item.MnO,
                        Cr2O3 = item.Cr2O3,
                        P2O5 = item.P2O5,
                        V2O5 = item.V2O5,
                        TiO2 = item.TiO2,
                        ZnO = item.ZnO,
                        TotalFe = item.TotalFe,
                        Basicity4 = item.Basicity4,
                        Comment = item.Comment,

                        SlagID = newID,
                        StatusID = 1,
                        CreatedDate = data.CreatedDate,
                        CreatedBy = data.CreatedBy
                    };

                    repo.InsertSlagSample(bll);
                }

                TempData["SuccessMessage"] = "Data saved successfully";
                return RedirectToAction("SlagByProductList");
            }

            // ============================
            // ✅ EDIT MODE (Update)
            // ============================
            else
            {
                // 1) Fetch existing master
                var existing = repo.GetSlagByID(data.ID);   // ✅ create this repo method
                if (existing == null)
                {
                    TempData["ErrorMessage"] = "Record not found.";
                    return RedirectToAction("SlagByProductList");
                }

                // 2) Update master fields (map only those you allow)
                existing.DateOfProduction = data.DateOfProduction;
                existing.DateOfAnalysis = data.DateOfAnalysis;
                existing.HeatNo = data.HeatNo;
                existing.CertificateNo = data.CertificateNo;

                // ✅ If you store ByProductType as string, set it here
                // existing.ByProductType = data.ByProductType;  // if you have this field

                // OR if you have EAF/LF/TS... columns:
                existing.ByProductType = data.ByProductType;

                existing.StatusID = 1;
                existing.UpdatedDate = DateTime.Now;     // ✅ add in model if not present
                existing.UpdatedBy = User.Identity.Name; // ✅ add in model if not present

                int upd = repo.UpdateSlagByProduct(existing); // ✅ create this repo method

                if (upd != 0)
                {
                    TempData["ErrorMessage"] = "Data not updated. Please try again.";
                    return RedirectToAction("SlagByProductList");
                }
                existing.UpdatedBy = User.Identity.Name;
                // 3) Replace samples
                // ✅ OPTION A: Hard delete previous samples then insert new
                repo.DeleteSlagSamplesBySlagID(existing.ID, existing.UpdatedBy); // ✅ create this method (recommended)

                foreach (var item in data.Samples)
                {
                    // optional: skip empty rows
                    if (string.IsNullOrWhiteSpace(item.SampleCode) &&
                        item.CaO == null && item.MgO == null && item.SiO2 == null)
                        continue;

                    var bll = new SlagSampleAnalysisBLL
                    {
                        SampleCode = item.SampleCode,
                        SampleTime = item.SampleTime,
                        CaO = item.CaO,
                        MgO = item.MgO,
                        SiO2 = item.SiO2,
                        Al2O3 = item.Al2O3,
                        Fe2O3 = item.Fe2O3,
                        S = item.S,
                        MnO = item.MnO,
                        Cr2O3 = item.Cr2O3,
                        P2O5 = item.P2O5,
                        V2O5 = item.V2O5,
                        TiO2 = item.TiO2,
                        ZnO = item.ZnO,
                        TotalFe = item.TotalFe,
                        Basicity4 = item.Basicity4,
                        Comment = item.Comment,

                        SlagID = existing.ID,
                        StatusID = 1,
                        CreatedDate = DateTime.Now,
                        CreatedBy = User.Identity.Name
                    };

                    repo.InsertSlagSample(bll);
                }

                TempData["SuccessMessage"] = "Data updated successfully";
                return RedirectToAction("SlagByProductList");
            }
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
    }
}