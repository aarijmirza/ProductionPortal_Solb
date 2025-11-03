using BAL.Repositories;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProductionPortal_Solb.Controllers
{
    public class UserRoleController : Controller
    {
        UserRoleRepository repo;
        public UserRoleController()
        {
            repo = new UserRoleRepository();
        }
        // GET: UserRole
        public ActionResult list()
        {
            var data = repo.GetAll();
            return View(data);
        }
        [HttpGet]
        public ActionResult add(int? id)
        {
            return View();
        }
        [HttpPost]
        public ActionResult Save(UserRoleBLL model)
        {
            if (model.ID == 0 || model.ID == null)  // New record
            {
                model.CreatedDate = DateTime.Now;
                model.StatusID = 1;
                int result = repo.Insert(model);

                if (result > 0)
                {
                    TempData["Success"] = "Role saved successfully!";
                }
                else
                {
                    TempData["Error"] = "Failed to save the role.";
                }

                // redirect to list page (or to Add if you prefer)
                return RedirectToAction("list");
            }
            else  // Update
            {
                model.CreatedDate = DateTime.Now;

                ////int result = repo.Update(model);

                //if (result > 0)
                //{
                //    TempData["Success"] = "Role updated successfully!";
                //}
                //else
                //{
                //    TempData["Error"] = "Failed to update the role.";
                //}

                return RedirectToAction("list");
            }
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            int rtn = repo.Delete(id);
            return RedirectToAction("list");
        }
    }
}