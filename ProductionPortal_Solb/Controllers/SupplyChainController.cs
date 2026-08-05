using BAL.Repositories;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProductionPortal_Solb.Controllers
{
    public class SupplyChainController : Controller
    {
        SupplyChainRepository repo = new SupplyChainRepository();
        // GET: SupplyChain

        [HttpGet]
        public ActionResult List(DateTime? fromDate, DateTime? toDate)
        {
            DateTime from = fromDate ?? DateTime.Today;
            DateTime to = toDate ?? DateTime.Today;

            var data = repo.GetSupplyChainDailyList(from, to);

            ViewBag.FromDate = from.ToString("yyyy-MM-dd");
            ViewBag.ToDate = to.ToString("yyyy-MM-dd");

            return View(data);
        }

        [HttpGet]
        public ActionResult Add(int? id)
        {
            if (id.HasValue && id.Value > 0)
            {
                var model = repo.GetSupplyChainDailyByID(id.Value);

                if (model == null || model.ID <= 0)
                {
                    TempData["ErrorMessage"] = "Record not found.";
                    return RedirectToAction("List");
                }

                return View(model);
            }

            var newModel = new SupplyChainDailyBLL
            {
                ReportDate = DateTime.Today,
                ReportTime = DateTime.Now.TimeOfDay
            };

            return View(newModel);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult add(SupplyChainDailyBLL model)
        //{
        //    try
        //    {
        //        if (model == null)
        //        {
        //            TempData["ErrorMessage"] = "Invalid data submitted.";
        //            return RedirectToAction("Add");
        //        }

        //        if (!model.ReportDate.HasValue)
        //        {
        //            TempData["ErrorMessage"] = "Please select report date.";
        //            return View(model);
        //        }

        //        if (!model.ReportTime.HasValue)
        //        {
        //            model.ReportTime = DateTime.Now.TimeOfDay;
        //        }

        //        if (model.ID > 0)
        //        {
        //            model.UpdatedBy = User.Identity.Name;
        //            model.UpdatedDate = DateTime.Now;
        //        }
        //        else
        //        {
        //            model.StatusID = 1;
        //            model.CreatedBy = User.Identity.Name;
        //            model.CreatedDate = DateTime.Now;
        //        }

        //        int result = repo.SaveSupplyChainDaily(model);

        //        if (result < 0)
        //        {
        //            TempData["SuccessMessage"] = "Supply Chain dashboard data saved successfully.";
        //            return RedirectToAction("Add");
        //        }

        //        TempData["ErrorMessage"] = "Data not saved. Please try again.";
        //        return View(model);
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["ErrorMessage"] = "Error: " + ex.Message;
        //        return View(model);
        //    }
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(
    SupplyChainDailyBLL data)
        {
            if (data == null)
            {
                TempData["ErrorMessage"] =
                    "Invalid data submitted.";

                return RedirectToAction(
                    "Add"
                );
            }

            try
            {
                data.ReceivedMaterials =
                    data.ReceivedMaterials
                    ?? new List<SupplyChainReceivedMaterialBLL>();

                /*
                    Remove empty rows.
                */
                data.ReceivedMaterials =
                    data.ReceivedMaterials
                        .Where(x =>
                            x != null &&
                            !string.IsNullOrWhiteSpace(
                                x.ItemName
                            )
                        )
                        .ToList();

                /*
                    Recalculate parent totals from dynamic rows.
                */
                data.RawMaterialsReceived =
                    data.ReceivedMaterials
                        .Where(x =>
                            x.MaterialType ==
                                "Raw Material"
                        )
                        .Sum(x =>
                            x.Quantity
                        );

                data.SubRawMaterialsReceived =
                    data.ReceivedMaterials
                        .Where(x =>
                            x.MaterialType ==
                                "Sub Raw Material"
                        )
                        .Sum(x =>
                            x.Quantity
                        );

                string userName =
                    User != null &&
                    User.Identity != null
                        ? User.Identity.Name
                        : "";

                int supplyChainDailyID;

                if (data.ID > 0)
                {
                    data.UpdatedBy =
                        userName;

                    data.UpdatedDate =
                        DateTime.Now;

                    //int updated =
                    //    repo.Update(
                    //        data
                    //    );

                    //if (updated <= 0)
                    //{
                    //    TempData["ErrorMessage"] =
                    //        "Supply Chain report was not updated.";

                    //    return RedirectToAction(
                    //        "Add",
                    //        new
                    //        {
                    //            id = data.ID
                    //        }
                    //    );
                    //}

                    supplyChainDailyID =
                        data.ID;

                    /*
                        Delete old child rows and insert current rows.
                        This is simple and reliable for dynamic editable rows.
                    */
                    repo.DeleteReceivedMaterials(
                        supplyChainDailyID
                    );
                }
                else
                {
                    data.CreatedBy =
                        userName;

                    data.CreatedDate =
                        DateTime.Now;

                    supplyChainDailyID =
                        repo.SaveSupplyChainDaily(
                            data
                        );

                    if (supplyChainDailyID <= 0)
                    {
                        TempData["ErrorMessage"] =
                            "Supply Chain report was not saved.";

                        return RedirectToAction(
                            "Add"
                        );
                    }
                }

                foreach (
                    SupplyChainReceivedMaterialBLL item
                    in data.ReceivedMaterials
                )
                {
                    item.SupplyChainDailyID =
                        supplyChainDailyID;

                    item.StatusID =
                        1;

                    item.CreatedBy =
                        userName;

                    item.CreatedDate =
                        DateTime.Now;

                    repo.InsertReceivedMaterial(
                        item
                    );
                }

                TempData["SuccessMessage"] =
                    data.ID > 0
                        ? "Supply Chain report updated successfully."
                        : "Supply Chain report saved successfully.";

                return RedirectToAction(
                    "Add",
                    new
                    {
                        id = supplyChainDailyID
                    }
                );
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] =
                    "Unable to save report. " +
                    ex.Message;

                return RedirectToAction(
                    "Add",
                    new
                    {
                        id =
                            data.ID > 0
                                ? (int?)data.ID
                                : null
                    }
                );
            }
        }


        /*
            In GET Add/Edit action, after loading the main record:
        */
        //public ActionResult Add(
        //    int? id)
        //{
        //    SupplyChainDailyBLL model =
        //        id.HasValue &&
        //        id.Value > 0
        //            ? repo.GetByID(
        //                id.Value
        //            )
        //            : new SupplyChainDailyBLL
        //            {
        //                ReportDate =
        //                    DateTime.Today
        //            };

        //    if (model == null)
        //    {
        //        model =
        //            new SupplyChainDailyBLL
        //            {
        //                ReportDate =
        //                    DateTime.Today
        //            };
        //    }

        //    model.ReceivedMaterials =
        //        model.ID > 0
        //            ? repo.GetReceivedMaterials(
        //                model.ID
        //            )
        //            : new List<SupplyChainReceivedMaterialBLL>();

        //    return View(
        //        model
        //    );
        //}

    }
}