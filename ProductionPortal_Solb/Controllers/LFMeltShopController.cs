using BAL.Repositories;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProductionPortal_Solb.Controllers
{
    public class LFMeltShopController : Controller
    {
        LFMeltShopRepository repo;
        public LFMeltShopController()
        {
            repo = new LFMeltShopRepository();
        }
        // GET: LFMeltShop
        public ActionResult list()
        {
            var records = repo.GetAllRecord();
            return View(records);
        }
        public ActionResult add(int? id)
        {
            var Grade = repo.GetAllGrade();
            ViewBag.Grade = new SelectList(Grade, "GRADE_ID", "GRADE_ID");
            var Delay = repo.GetAllDelays();
            ViewBag.Delay = new SelectList(Delay, "GROUP_NAME", "GROUP_NAME");
            return View();
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult add(LFMeltShopBLL model)
        {
            try
            {
                model.StatusID = 1;
                model.CreatedDate = DateTime.Now;
                model.CreatedBy = User.Identity.Name;
                int rtn = repo.InsertLF(model);
                if (model.Delays != null && model.Delays.Any())
                {
                    foreach (var delay in model.Delays)
                    {
                        repo.AddDelays(new DelaysBLL
                        {
                            HeatID = rtn,
                            HeatNo = model.HeatNo,
                            StatusID = 1,
                            CreatedDate = DateTime.Now,
                            CreatedBy = User.Identity.Name,
                            PlantID = 1,
                            PlantName = "LF Meltshop",
                            StartTime = delay.StartTime,
                            EndTime = delay.EndTime,
                            TotalDuration = delay.TotalDuration,
                            Reason = delay.Reason
                        });
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return RedirectToAction("list");
        }
    }
}