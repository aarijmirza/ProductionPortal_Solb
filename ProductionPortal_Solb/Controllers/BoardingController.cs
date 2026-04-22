using BAL.Repositories;
using DAL.Models;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProductionPortal_Solb.Controllers
{
    public class BoardingController : Controller
    {
        BoardingRepository repo;
        public BoardingController()
        {
            repo = new BoardingRepository();
        }
        [Route("BilletBoarding")]
        // GET: Boarding
        public ActionResult BilletBoard()
        {
            var billets = repo.GetAllBoarding();
            return View("~/Views/Boarding/BilletBoard/BilletBoard.cshtml", billets);
        }
        [Route("AddBilletBoarding")]
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

            return View("~/Views/Boarding/BilletBoard/AddBillet.cshtml");
        }
        [HttpPost]
        public ActionResult AddBillet(RollingMillMasterBLL data)
        {
            decimal billetLength = data.BilletLength ?? 0;

            decimal billetWeight = 0;

            if (!string.IsNullOrWhiteSpace(data.CrossSection))
            {
                var parts = data.CrossSection.Replace(" ", "").Split('x');

                if (parts.Length == 2
                    && int.TryParse(parts[0], out int w)
                    && int.TryParse(parts[1], out int h))
                {
                    // Rule: 150 x 150
                    if (w == 150 && h == 150)
                    {
                        billetWeight = 175m * billetLength;
                    }
                }
            }

            data.BilletWeight = billetWeight;

            data.StatusID = 1;
            data.CreatedDate = DateTime.Now;
            data.CreatedBy = User.Identity.Name;
            if (data.Chemistry != null && data.Chemistry.Any())
            {
                foreach (var chem in data.Chemistry)
                {
                    chem.StatusID = 1;
                    chem.CreatedDate = DateTime.Now;
                    chem.CreatedBy = User.Identity.Name;
                    chem.HeatNo = data.HeatNo;
                    // Save each chemical analysis
                    // repo.InsertChemicalAnalysisRM(chem);
                }
            }
            int rtn = repo.InsertBilletBoarding(data);
            return View("list");
        }
    }
}