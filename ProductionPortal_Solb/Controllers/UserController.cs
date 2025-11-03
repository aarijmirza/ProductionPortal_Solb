using BAL.Repositories;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProductionPortal_Solb.Controllers
{
    public class UserController : Controller
    {
        UserRepository repo;
        UserRoleRepository role;
        public UserController()
        {
            repo = new UserRepository();
            role = new UserRoleRepository();
        }
        // GET: User
        public ActionResult list()
        {
            var data = repo.GetAll();
            return View(data);
        }
        [HttpGet]
        public ActionResult add(int? id)
        {
            var Role = role.GetAll();
            ViewBag.Role = new SelectList(Role, "ID", "RoleName");
            return View();
        }
        [HttpPost]
        public ActionResult Save(UserBLL data)
        {
            if (data.ID == 0 || data.ID == null)  // New record
            {
                data.CreatedDate = DateTime.Now;
                data.StatusID = 1;
                int result = repo.Insert(data);

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
                data.CreatedDate = DateTime.Now;

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
    }
}