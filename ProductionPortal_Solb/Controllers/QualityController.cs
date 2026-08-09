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
        //public ActionResult AddBillet()
        //{
        //    var heat = repo.GetAllChemistry()
        //        .Where(x => x.Area == "Rolling Mill 1" || x.Area == "Rolling Mill 2")
        //        .Select(x => new { x.HeatNo, x.Area })
        //        .Distinct()
        //        .ToList();

        //    ViewBag.HeatNo = new SelectList(heat);
        //    var BilletGradeList = repo.GetBilletGrade();
        //    ViewBag.BilletGrade = new SelectList(BilletGradeList, "ProductID", "SpecGrade");
        //    ViewBag.GradeDataJson = JsonConvert.SerializeObject(BilletGradeList);

        //    return View("~/Views/Quality/BilletBoard/AddBillet.cshtml");
        //}

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
        [ValidateAntiForgeryToken]
        public ActionResult AddBillet(
    BilletBoardBLL data)
        {
            try
            {
                if (data == null)
                {
                    TempData["ErrorMessage"] =
                        "Invalid data.";

                    return RedirectToAction(
                        "AddBillet"
                    );
                }

                bool isEdit =
                    data.ID > 0;

                if (
                    string.IsNullOrWhiteSpace(
                        data.BilletBoarding
                    )
                )
                {
                    TempData["ErrorMessage"] =
                        "Billet Boarding number is required.";

                    return RedirectToAction(
                        "AddBillet",
                        isEdit
                            ? new { id = data.ID }
                            : null
                    );
                }

                if (
                    data.Chemistry == null ||
                    !data.Chemistry.Any()
                )
                {
                    TempData["ErrorMessage"] =
                        "Chemistry data not found.";

                    return RedirectToAction(
                        "AddBillet",
                        isEdit
                            ? new { id = data.ID }
                            : null
                    );
                }

                var validChemistry =
                    data.Chemistry
                        .Where(
                            x =>
                                x != null &&
                                !string.IsNullOrWhiteSpace(
                                    x.HeatNo
                                )
                        )
                        .ToList();

                if (!validChemistry.Any())
                {
                    TempData["ErrorMessage"] =
                        "Heat No not found in chemistry.";

                    return RedirectToAction(
                        "AddBillet",
                        isEdit
                            ? new { id = data.ID }
                            : null
                    );
                }

                /*
                 * Unique Heat Nos from submitted form
                 */
                var uniqueHeatNos =
                    validChemistry
                        .Select(
                            x => x.HeatNo.Trim()
                        )
                        .Distinct(
                            StringComparer.OrdinalIgnoreCase
                        )
                        .ToList();


                /*
                 * Duplicate Heat No inside same submitted form
                 */
                int postedHeatCount =
                    validChemistry
                        .Select(
                            x => x.HeatNo.Trim()
                        )
                        .Count();

                if (
                    uniqueHeatNos.Count !=
                    postedHeatCount
                )
                {
                    TempData["ErrorMessage"] =
                        "Duplicate Heat No found in submitted data.";

                    return RedirectToAction(
                        "AddBillet",
                        isEdit
                            ? new { id = data.ID }
                            : null
                    );
                }


                /* ==============================================
                   DUPLICATE CHECKS
                   ============================================== */

                if (!isEdit)
                {
                    /*
                     * ADD MODE
                     */
                    bool isBilletBoardingExists =
                        repo.IsBilletBoardingExists(
                            data.BilletBoarding.Trim()
                        );

                    if (isBilletBoardingExists)
                    {
                        TempData["ErrorMessage"] =
                            "This Billet Boarding number already exists.";

                        return RedirectToAction(
                            "AddBillet"
                        );
                    }

                    var duplicateHeatNos =
                        repo.GetDuplicateHeatNos(
                            uniqueHeatNos
                        );

                    if (
                        duplicateHeatNos != null &&
                        duplicateHeatNos.Any()
                    )
                    {
                        TempData["ErrorMessage"] =
                            "These Heat No(s) already exist: "
                            + string.Join(
                                ", ",
                                duplicateHeatNos
                            );

                        return RedirectToAction(
                            "AddBillet"
                        );
                    }
                }
                else
                {
                    /*
                     * EDIT MODE

                     * Current boarding ko duplicate check se
                     * exclude karenge.
                     */
                    bool boardingExists =
                        repo.IsBilletBoardingExistsForEdit(
                            data.BilletBoarding.Trim(),
                            data.ID
                        );

                    if (boardingExists)
                    {
                        TempData["ErrorMessage"] =
                            "This Billet Boarding number already exists.";

                        return RedirectToAction(
                            "AddBillet",
                            new
                            {
                                id = data.ID
                            }
                        );
                    }

                    var duplicateHeatNos =
                        repo.GetDuplicateHeatNosForEdit(
                            uniqueHeatNos,
                            data.ID
                        );

                    if (
                        duplicateHeatNos != null &&
                        duplicateHeatNos.Any()
                    )
                    {
                        TempData["ErrorMessage"] =
                            "These Heat No(s) already exist in another Billet Boarding: "
                            + string.Join(
                                ", ",
                                duplicateHeatNos
                            );

                        return RedirectToAction(
                            "AddBillet",
                            new
                            {
                                id = data.ID
                            }
                        );
                    }
                }


                /* ==============================================
                   BILLET WEIGHT
                   ============================================== */

                decimal billetLength = 0;

                if (
                    !string.IsNullOrWhiteSpace(
                        data.BilletLength
                    )
                )
                {
                    decimal.TryParse(
                        data.BilletLength,
                        out billetLength
                    );
                }

                decimal billetWeight = 0;

                if (
                    !string.IsNullOrWhiteSpace(
                        data.CrossSection
                    )
                )
                {
                    var parts =
                        data.CrossSection
                            .Replace(
                                " ",
                                ""
                            )
                            .ToLower()
                            .Split('x');

                    if (parts.Length == 2)
                    {
                        int w = 0;
                        int h = 0;

                        int.TryParse(
                            parts[0],
                            out w
                        );

                        int.TryParse(
                            parts[1],
                            out h
                        );

                        if (
                            w == 150 &&
                            h == 150
                        )
                        {
                            billetWeight =
                                175m
                                * billetLength
                                / 1000;
                        }
                    }
                }

                data.BilletWeight =
                    billetWeight;

                data.StatusID = 1;


                /* ==============================================
                   INSERT MODE
                   ============================================== */

                if (!isEdit)
                {
                    data.CreatedDate =
                        DateTime.Now;

                    data.CreatedBy =
                        User.Identity.Name;


                    /*
                     * Insert chemistry
                     */
                    foreach (
                        var chem
                        in validChemistry
                    )
                    {
                        var chemData =
                            new RMChemicalAnalysisBLL
                            {
                                HeatNo =
                                    chem.HeatNo.Trim(),

                                StatusID = 1,

                                CreatedDate =
                                    DateTime.Now,

                                CreatedBy =
                                    User.Identity.Name,

                                C = chem.C,
                                Mn = chem.Mn,
                                Si = chem.Si,
                                P = chem.P,
                                S = chem.S,
                                N = chem.N,
                                Ceq = chem.Ceq,

                                HeatStatus =
                                    chem.HeatStatus,

                                NoOfBillets =
                                    chem.NoOfBillets
                            };

                        repo.InsertChemicalAnalysisRM(
                            chemData
                        );
                    }


                    /*
                     * Insert one BilletBoard row
                     * per unique Heat No.
                     */
                    foreach (
                        var heatNo
                        in uniqueHeatNos
                    )
                    {
                        var billetData =
                            new BilletBoardBLL
                            {
                                HeatNo =
                                    heatNo,

                                Date =
                                    data.Date,

                                BilletLength =
                                    data.BilletLength,

                                CrossSection =
                                    data.CrossSection,

                                BilletWeight =
                                    data.BilletWeight,

                                StatusID = 1,

                                CreatedDate =
                                    DateTime.Now,

                                CreatedBy =
                                    User.Identity.Name,

                                PlantName =
                                    data.PlantName,

                                Shift =
                                    data.Shift,

                                SteelGrade =
                                    data.SteelGrade,

                                Profile =
                                    data.Profile,

                                Size =
                                    data.Size,

                                BilletBoarding =
                                    data.BilletBoarding
                                        .Trim(),

                                ProductSpecs =
                                    data.ProductSpecs,

                                Remarks =
                                    data.Remarks
                            };

                        repo.InsertBilletBoarding(
                            billetData
                        );
                    }

                    TempData["SuccessMessage"] =
                        "Billet Boarding inserted successfully against all Heat Numbers.";

                    return RedirectToAction(
                        "AddBillet"
                    );
                }


                /* ==============================================
                   EDIT MODE
                   ============================================== */

                else
                {
                    data.UpdatedDate =
                        DateTime.Now;

                    data.UpdatedBy =
                        User.Identity.Name;

                    int boardingNo =
                        data.ID;


                    /*
                     * 1. Update common BilletBoard fields
                     * for complete boarding.
                     */
                    repo.UpdateBilletBoarding(
                        data
                    );


                    /*
                     * 2. Existing chemistry/detail records
                     * deactivate/remove for this boarding.
                     */
                    repo.DeactivateBilletChemistry(
                        boardingNo,
                        User.Identity.Name
                    );


                    /*
                     * 3. Existing BilletBoard Heat rows
                     * deactivate.
                     *
                     * Then updated submitted HeatNos
                     * re-insert karenge.
                     */
                    repo.DeactivateBilletBoardHeatRows(
                        boardingNo,
                        User.Identity.Name
                    );


                    /*
                     * 4. Insert updated chemistry
                     */
                    foreach (
                        var chem
                        in validChemistry
                    )
                    {
                        var chemData =
                            new RMChemicalAnalysisBLL
                            {
                                HeatNo =
                                    chem.HeatNo.Trim(),

                                StatusID = 1,

                                CreatedDate =
                                    DateTime.Now,

                                CreatedBy =
                                    User.Identity.Name,

                                C = chem.C,
                                Mn = chem.Mn,
                                Si = chem.Si,
                                P = chem.P,
                                S = chem.S,
                                N = chem.N,
                                Ceq = chem.Ceq,

                                HeatStatus =
                                    chem.HeatStatus,

                                NoOfBillets =
                                    chem.NoOfBillets
                            };

                        repo.InsertChemicalAnalysisRM(
                            chemData
                        );
                    }


                    /*
                     * 5. Re-create BilletBoard Heat rows
                     */
                    foreach (
                        string heatNo
                        in uniqueHeatNos
                    )
                    {
                        BilletBoardBLL billetData =
                            new BilletBoardBLL
                            {
                                HeatNo =
                                    heatNo,

                                Date =
                                    data.Date,

                                BilletLength =
                                    data.BilletLength,

                                CrossSection =
                                    data.CrossSection,

                                BilletWeight =
                                    data.BilletWeight,

                                StatusID = 1,

                                CreatedDate =
                                    DateTime.Now,

                                CreatedBy =
                                    User.Identity.Name,

                                PlantName =
                                    data.PlantName,

                                Shift =
                                    data.Shift,

                                SteelGrade =
                                    data.SteelGrade,

                                Profile =
                                    data.Profile,

                                Size =
                                    data.Size,

                                ID =
                                    boardingNo,

                                ProductSpecs =
                                    data.ProductSpecs,

                                Remarks =
                                    data.Remarks
                            };

                        repo.InsertBilletBoarding(
                            billetData
                        );
                    }


                    TempData["SuccessMessage"] =
                        "Billet Boarding updated successfully.";

                    return RedirectToAction(
                        "AddBillet",
                        new
                        {
                            id = data.ID
                        }
                    );
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] =
                    "Error while saving Billet Boarding: "
                    + ex.Message;

                if (
                    data != null &&
                    data.ID > 0
                )
                {
                    return RedirectToAction(
                        "AddBillet",
                        new
                        {
                            id = data.ID
                        }
                    );
                }

                return RedirectToAction(
                    "AddBillet"
                );
            }
        }

        //[HttpPost]
        //public ActionResult AddBillet(BilletBoardBLL data)
        //{
        //    try
        //    {
        //        if (data == null)
        //        {
        //            TempData["ErrorMessage"] = "Invalid data.";
        //            return RedirectToAction("AddBillet");
        //        }

        //        if (string.IsNullOrWhiteSpace(data.BilletBoarding))
        //        {
        //            TempData["ErrorMessage"] = "Billet Boarding number is required.";
        //            return RedirectToAction("AddBillet");
        //        }

        //        if (data.Chemistry == null || !data.Chemistry.Any())
        //        {
        //            TempData["ErrorMessage"] = "Chemistry data not found.";
        //            return RedirectToAction("AddBillet");
        //        }

        //        var validChemistry = data.Chemistry
        //            .Where(x => x != null && !string.IsNullOrWhiteSpace(x.HeatNo))
        //            .ToList();

        //        if (!validChemistry.Any())
        //        {
        //            TempData["ErrorMessage"] = "Heat No not found in chemistry.";
        //            return RedirectToAction("AddBillet");
        //        }

        //        // Unique HeatNos from posted chemistry
        //        var uniqueHeatNos = validChemistry
        //            .Select(x => x.HeatNo.Trim())
        //            .Distinct(StringComparer.OrdinalIgnoreCase)
        //            .ToList();

        //        // 1) Check duplicate HeatNo inside same submitted form
        //        if (uniqueHeatNos.Count != validChemistry.Select(x => x.HeatNo.Trim()).Count())
        //        {
        //            TempData["ErrorMessage"] = "Duplicate Heat No found in submitted data.";
        //            return RedirectToAction("AddBillet");
        //        }

        //        // 2) Check duplicate Billet Boarding from DB
        //        bool isBilletBoardingExists = repo.IsBilletBoardingExists(data.BilletBoarding.Trim());

        //        if (isBilletBoardingExists)
        //        {
        //            TempData["ErrorMessage"] = "This Billet Boarding number already exists.";
        //            return RedirectToAction("AddBillet");
        //        }

        //        // 3) Check duplicate HeatNos from DB
        //        var duplicateHeatNos = repo.GetDuplicateHeatNos(uniqueHeatNos);

        //        if (duplicateHeatNos != null && duplicateHeatNos.Any())
        //        {
        //            TempData["ErrorMessage"] = "These Heat No(s) already exist: " + string.Join(", ", duplicateHeatNos);
        //            return RedirectToAction("AddBillet");
        //        }

        //        decimal billetLength = 0;
        //        if (!string.IsNullOrWhiteSpace(data.BilletLength))
        //        {
        //            decimal.TryParse(data.BilletLength, out billetLength);
        //        }

        //        decimal billetWeight = 0;
        //        if (!string.IsNullOrEmpty(data.CrossSection))
        //        {
        //            var parts = data.CrossSection.Replace(" ", "").ToLower().Split('x');

        //            if (parts.Length == 2)
        //            {
        //                int w = 0;
        //                int h = 0;

        //                int.TryParse(parts[0], out w);
        //                int.TryParse(parts[1], out h);

        //                if (w == 150 && h == 150)
        //                {
        //                    billetWeight = 175m * billetLength / 1000;
        //                }
        //            }
        //        }

        //        data.BilletWeight = billetWeight;
        //        data.StatusID = 1;
        //        data.CreatedDate = DateTime.Now;
        //        data.CreatedBy = User.Identity.Name;

        //        int rtn = 0;

        //        // 4) Insert chemistry rows
        //        foreach (var chem in validChemistry)
        //        {
        //            var chemData = new RMChemicalAnalysisBLL
        //            {
        //                HeatNo = chem.HeatNo.Trim(),
        //                StatusID = 1,
        //                CreatedDate = DateTime.Now,
        //                CreatedBy = User.Identity.Name,

        //                C = chem.C,
        //                Mn = chem.Mn,
        //                Si = chem.Si,
        //                P = chem.P,
        //                S = chem.S,
        //                N = chem.N,
        //                Ceq = chem.Ceq,
        //                HeatStatus = chem.HeatStatus,
        //                NoOfBillets = chem.NoOfBillets
        //            };

        //            repo.InsertChemicalAnalysisRM(chemData);
        //        }

        //        // 5) Insert BilletBoarding once per unique HeatNo
        //        foreach (var heatNo in uniqueHeatNos)
        //        {
        //            var billetData = new BilletBoardBLL
        //            {
        //                HeatNo = heatNo,
        //                Date = data.Date,
        //                BilletLength = data.BilletLength,
        //                CrossSection = data.CrossSection,
        //                BilletWeight = data.BilletWeight,
        //                StatusID = 1,
        //                CreatedDate = DateTime.Now,
        //                CreatedBy = User.Identity.Name,

        //                PlantName = data.PlantName,
        //                Shift = data.Shift,
        //                SteelGrade = data.SteelGrade,
        //                Profile = data.Profile,
        //                Size = data.Size,
        //                BilletBoarding = data.BilletBoarding.Trim(),
        //                ProductSpecs = data.ProductSpecs,
        //                Remarks = data.Remarks
        //            };

        //            rtn = repo.InsertBilletBoarding(billetData);
        //        }

        //        TempData["SuccessMessage"] = "Billet Boarding inserted successfully against all Heat Numbers.";
        //        return RedirectToAction("AddBillet");
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["ErrorMessage"] = "Error while saving Billet Boarding: " + ex.Message;
        //        return RedirectToAction("AddBillet");
        //    }
        //}

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
        public JsonResult GetMTCRowsJson(
    string heatNo = null)
        {
            try
            {
                var rows =
                    repo.GetMTCRows(heatNo)
                    ?? new List<QCMTCRowBLL>();

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
                        message = ex.Message
                    },
                    JsonRequestBehavior.AllowGet
                );
            }
        }

        //[HttpGet]
        //public JsonResult GetMTCDetails(
        //    string heatNo)
        //{
        //    try
        //    {
        //        var data =
        //            repo.GetMTCDetail(
        //                heatNo
        //            );

        //        return Json(
        //            new
        //            {
        //                success =
        //                    data != null,

        //                data =
        //                    data
        //            },
        //            JsonRequestBehavior.AllowGet
        //        );
        //    }
        //    catch (Exception ex)
        //    {
        //        Response.StatusCode =
        //            500;

        //        return Json(
        //            new
        //            {
        //                success =
        //                    false,

        //                message =
        //                    ex.Message
        //            },
        //            JsonRequestBehavior.AllowGet
        //        );
        //    }
        //}


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
    }
}