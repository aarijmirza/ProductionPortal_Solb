using BAL.Repositories;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProductionPortal_Solb.Controllers
{
    public class EAFMeltShopController : Controller
    {
        EAFMeltShopRepository repo;
        public EAFMeltShopController()
        {
            repo = new EAFMeltShopRepository();
        }
        // GET: EAFMeltShop
        public ActionResult list()
        {
            var requests = repo.GetAllRecord();
            return View(requests);
        }
        public ActionResult add(int? id)
        {
            if(id != null) 
            {
                var requests = repo.GetRecordByID(id);
                return View(requests);
            }
            else
            {
                var Grade = repo.GetAllGrade();
                ViewBag.Grade = new SelectList(Grade, "GRADE_ID", "GRADE_ID");
                var Delay = repo.GetAllDelays();
                ViewBag.Delay = new SelectList(Delay, "GROUP_NAME", "GROUP_NAME"); 
                return View();
            }
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult add(EAFMeltShopBLL model)
        {
            try
            {
                //if (ModelState.IsValid)
                //{
                model.StatusID = 1;
                model.CreatedDate = DateTime.Now;
                model.CreatedBy = User.Identity.Name;
                int rtn = repo.Insert(model);
                // If electrodes are passed from form
                if (model.Electrodes != null && model.Electrodes.Any())
                {
                    foreach (var electrode in model.Electrodes)
                    {
                        repo.AddElectrode(new ElectrodeBLL
                        {
                            HeatID = rtn,
                            StatusID = 1,
                            CreatedDate = DateTime.Now,
                            CreatedBy = User.Identity.Name,
                            PlantID = 1,
                            PlantName = "EAF Meltshop",
                            ElectrodeAddition = electrode.ElectrodeAddition,
                            Adjusted = electrode.Adjusted,
                            Break = electrode.Break,
                            StubEndLoss = electrode.StubEndLoss
                        });                        
                    }
                }
                // If delays are passed from form
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
                            PlantName = "EAF Meltshop",
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