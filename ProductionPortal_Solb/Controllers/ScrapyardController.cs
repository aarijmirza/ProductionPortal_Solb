using BAL.Repositories;
using DAL.Models;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;

namespace ProductionPortal_Solb.Controllers
{
    public class ScrapyardController : Controller
    {
        ScrapyardRepository repo;

        public ScrapyardController()
        {
            repo = new ScrapyardRepository();

        }
        // GET: User
        public ActionResult list()
        {
            var scrap = repo.GetAll();
            return View(scrap);
        }
        public ActionResult add()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Add(ScrapyardInputModel model)
        {
            ScrapyardBLL dal = new ScrapyardBLL();

            foreach (var item in model.data)
            {
                ScrapyardBLL bll = new ScrapyardBLL()
                {
                    Date = model.date,
                    Time = model.time,
                    HeatNo = model.heatno,

                    Bucket = item.Bucket,
                    LightScrap = item.LightScrap ?? 0,
                    HMS = item.HMS ?? 0,
                    ReturnMetal = item.ReturnMetal ?? 0,
                    ReturnBar = item.ReturnBar ?? 0,
                    MetalSkull = item.MetalSkull ?? 0,
                    DRI = item.DRI ?? 0,
                    Coal = item.Coal ?? 0,
                    Lime = item.Lime ?? 0,
                    Dololime = item.Dololime ?? 0,

                    StatusID = 1,
                    CreatedDate = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                };

                repo.AddScrapyard(bll);   // <-- Correct Call
            }

            TempData["msg"] = "Records Saved Successfully!";
            return RedirectToAction("list");
        }

        // Inside ScrapyardController.cs

        public ActionResult detail(string heatNo)
        {
            if (string.IsNullOrEmpty(heatNo))
            {
                // Handle case where no Heat Number is provided
                return RedirectToAction("list");
            }

            // Call the corrected repository method to fetch all bucket records for the heat
            var data = repo.GetScrapHeatDetails(heatNo);

            if (data == null || !data.Any())
            {
                TempData["Error"] = $"No records found for Heat # {heatNo}.";
                return RedirectToAction("list");
            }

            // Pass the list of ScrapyardBLL objects to the view
            return View(data);
        }
        public ActionResult delete(string heatNo)
        {
            var UpdatedBy = User.Identity.Name;
            int rtn = repo.Delete(heatNo, UpdatedBy);
            TempData["SuccessMessage"] = "Data Delete Successfully";

            return RedirectToAction("list");
        }
    }
}