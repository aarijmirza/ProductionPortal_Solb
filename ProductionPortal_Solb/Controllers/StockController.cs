using BAL.Repositories;
using DAL.Models;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProductionPortal_Solb.Controllers
{
    public class StockController : Controller
    {
        StockRepository repo;

        // GET: Stock
        public ActionResult add()
        {
            return View();
        }
        public ActionResult stockadd()
        {
            var vm = GetDefaultSupplyChainStockVM();
            vm.ReportDate = DateTime.Today;

            return View(vm);
        }


        public ActionResult Stocklist(DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                var vm = new SupplyChainStockVM();

                vm.HeaderList = repo.GetSupplyChainStockHeaderList(fromDate, toDate);

                ViewBag.FromDate = fromDate.HasValue ? fromDate.Value.ToString("yyyy-MM-dd") : "";
                ViewBag.ToDate = toDate.HasValue ? toDate.Value.ToString("yyyy-MM-dd") : "";

                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error: " + ex.Message;
                return View(new SupplyChainStockVM());
            }
        }
        [HttpGet]
        public ActionResult Add()
        {
            var vm = GetDefaultSupplyChainStockVM();
            vm.ReportDate = DateTime.Today;

            return View(vm);
        }

        // ==============================
        // SAVE DATA
        // ==============================
        [HttpPost]
        public ActionResult Add(SupplyChainStockVM model)
        {
            try
            {
                if (model == null)
                {
                    TempData["ErrorMessage"] = "Invalid data.";
                    return RedirectToAction("Add");
                }

                if (!model.ReportDate.HasValue)
                {
                    TempData["ErrorMessage"] = "Please select report date.";
                    return RedirectToAction("Add");
                }

                model.DispatchDetails = model.DispatchDetails ?? new List<DispatchDetailBLL>();
                model.RebarStocks = model.RebarStocks ?? new List<RebarStockBLL>();
                model.WireRodStocks = model.WireRodStocks ?? new List<WireRodStockBLL>();
                model.BilletStocks = model.BilletStocks ?? new List<BilletStockBLL>();
                model.RawMaterialStocks = model.RawMaterialStocks ?? new List<RawMaterialStockBLL>();

                string createdBy = "System";

                if (User != null && User.Identity != null && User.Identity.IsAuthenticated)
                {
                    createdBy = User.Identity.Name;
                }

                int result = repo.InsertSupplyChainStock(model, createdBy);

                if (result < 0)
                {
                    TempData["SuccessMessage"] = "Supply chain stock details saved successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Stock details not saved.";
                }

                return RedirectToAction("Add");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error: " + ex.Message;
                return RedirectToAction("Add");
            }
        }

        // ==============================
        // DEFAULT ROWS FOR FORM
        // ==============================
        private SupplyChainStockVM GetDefaultSupplyChainStockVM()
        {
            var vm = new SupplyChainStockVM();

            // Dispatch Details
            vm.DispatchDetails = new List<DispatchDetailBLL>
            {
                new DispatchDetailBLL { Material = "Rebar" },
                new DispatchDetailBLL { Material = "Plain Rebar" },
                new DispatchDetailBLL { Material = "Rebar Epoxy" },
                new DispatchDetailBLL { Material = "Wire Rod" },
                new DispatchDetailBLL { Material = "Rebar In Coil" },
                new DispatchDetailBLL { Material = "Billet" }
            };

            // Rebar Stock
            vm.RebarStocks = new List<RebarStockBLL>
            {
                new RebarStockBLL { Size = "8mm" },
                new RebarStockBLL { Size = "10mm" },
                new RebarStockBLL { Size = "12mm" },
                new RebarStockBLL { Size = "14mm" },
                new RebarStockBLL { Size = "16mm" },
                new RebarStockBLL { Size = "18mm" },
                new RebarStockBLL { Size = "20mm" },
                new RebarStockBLL { Size = "25mm" },
                new RebarStockBLL { Size = "28mm" },
                new RebarStockBLL { Size = "32mm" },
                new RebarStockBLL { Size = "36mm" },
                new RebarStockBLL { Size = "P32mm" },
                new RebarStockBLL { Size = "P40mm" }
            };

            // Wire Rod & Rebar Coil Stock
            vm.WireRodStocks = new List<WireRodStockBLL>
            {
                new WireRodStockBLL { Size = "5.5mm", Grade = "SAE 1008" },
                new WireRodStockBLL { Size = "5.5mm", Grade = "SAE 1006" },
                new WireRodStockBLL { Size = "6.5mm", Grade = "SAE 1008" },
                new WireRodStockBLL { Size = "6.5mm", Grade = "SAE 1012" },
                new WireRodStockBLL { Size = "7mm", Grade = "SAE 1008" },
                new WireRodStockBLL { Size = "7.5mm", Grade = "SAE 1008" },
                new WireRodStockBLL { Size = "8mm", Grade = "SAE 1008" },
                new WireRodStockBLL { Size = "9mm", Grade = "SAE 1012" },
                new WireRodStockBLL { Size = "9mm", Grade = "SAE 1008" },
                new WireRodStockBLL { Size = "10mm", Grade = "SAE 1008" },
                new WireRodStockBLL { Size = "11mm", Grade = "SAE 1006" },
                new WireRodStockBLL { Size = "11mm", Grade = "SAE 1008" },
                new WireRodStockBLL { Size = "11mm", Grade = "SAE 1080" },
                new WireRodStockBLL { Size = "12mm", Grade = "SAE 1012" },
                new WireRodStockBLL { Size = "14mm", Grade = "SAE 1008" },
                new WireRodStockBLL { Size = "RC8mm", Grade = "GR60" },
                new WireRodStockBLL { Size = "RC10mm", Grade = "GR60" },
                new WireRodStockBLL { Size = "RC12mm", Grade = "GR60" },
                new WireRodStockBLL { Size = "RC16mm", Grade = "GR60" },
                new WireRodStockBLL { Size = "RC16mm", Grade = "GR80" }
            };

            // Billet Stock
            vm.BilletStocks = new List<BilletStockBLL>
            {
                new BilletStockBLL { Grade = "ASTM615 G60" },
                new BilletStockBLL { Grade = "ASTM15 G60 - High Rh %" },
                new BilletStockBLL { Grade = "BS4449" },
                new BilletStockBLL { Grade = "BS4449: 005 B500B" },
                new BilletStockBLL { Grade = "ASTM A706 G80" },
                new BilletStockBLL { Grade = "ASTM 706 G60" },
                new BilletStockBLL { Grade = "ASTM615 G40 - Qaryan" },
                new BilletStockBLL { Grade = "AISI SAE 1008" },
                new BilletStockBLL { Grade = "AISI SAE 1080" },
                new BilletStockBLL { Grade = "AISI SAE 1012" },
                new BilletStockBLL { Grade = "AISI SAE 1042" },
                new BilletStockBLL { Grade = "Short Billet (1008)" },
                new BilletStockBLL { Grade = "ASTM 615 for RIC" }
            };

            // Raw + Sub Raw Material Stock
            vm.RawMaterialStocks = new List<RawMaterialStockBLL>
            {
                // Raw Material Stock
                new RawMaterialStockBLL { MaterialDescription = "HBI", StockCategory = "Raw Material Stock" },
                new RawMaterialStockBLL { MaterialDescription = "DRI", StockCategory = "Raw Material Stock" },
                new RawMaterialStockBLL { MaterialDescription = "Pig Iron", StockCategory = "Raw Material Stock" },
                new RawMaterialStockBLL { MaterialDescription = "Scrap", StockCategory = "Raw Material Stock" },

                // Sub Raw Material Stock
                new RawMaterialStockBLL { MaterialDescription = "Graphite Electrodes for EAF", StockCategory = "Sub Raw Material Stock" },
                new RawMaterialStockBLL { MaterialDescription = "Graphite Electrodes for LF", StockCategory = "Sub Raw Material Stock" },
                new RawMaterialStockBLL { MaterialDescription = "Rice Husk", StockCategory = "Sub Raw Material Stock" },
                new RawMaterialStockBLL { MaterialDescription = "Charge Coal", StockCategory = "Sub Raw Material Stock" },
                new RawMaterialStockBLL { MaterialDescription = "Calcined Carbon", StockCategory = "Sub Raw Material Stock" },
                new RawMaterialStockBLL { MaterialDescription = "Aluminum Sticks", StockCategory = "Sub Raw Material Stock" },
                new RawMaterialStockBLL { MaterialDescription = "Aluminum Wire", StockCategory = "Sub Raw Material Stock" },
                new RawMaterialStockBLL { MaterialDescription = "RECARBURIZER BB", StockCategory = "Sub Raw Material Stock" },
                new RawMaterialStockBLL { MaterialDescription = "Ferro Silicon75%", StockCategory = "Sub Raw Material Stock" },
                new RawMaterialStockBLL { MaterialDescription = "Ferro silicon Manganese", StockCategory = "Sub Raw Material Stock" },
                new RawMaterialStockBLL { MaterialDescription = "Ferro Manganese HC", StockCategory = "Sub Raw Material Stock" },
                new RawMaterialStockBLL { MaterialDescription = "Ferro Vanadium", StockCategory = "Sub Raw Material Stock" },
                new RawMaterialStockBLL { MaterialDescription = "Flourspar (CaF2)", StockCategory = "Sub Raw Material Stock" },
                new RawMaterialStockBLL { MaterialDescription = "Lime (5-50 mm)", StockCategory = "Sub Raw Material Stock" },
                new RawMaterialStockBLL { MaterialDescription = "Lime (3-12 mm)", StockCategory = "Sub Raw Material Stock" },
                new RawMaterialStockBLL { MaterialDescription = "Dolime (8-50 mm)", StockCategory = "Sub Raw Material Stock" },
                new RawMaterialStockBLL { MaterialDescription = "Dolime (3-12 mm)", StockCategory = "Sub Raw Material Stock" },
                new RawMaterialStockBLL { MaterialDescription = "MAGNESITE (MgO C93 CR)", StockCategory = "Sub Raw Material Stock" },
                new RawMaterialStockBLL { MaterialDescription = "CaSi wire", StockCategory = "Sub Raw Material Stock" },
                new RawMaterialStockBLL { MaterialDescription = "CALCIUM-FERRUM (Ca-Fe)", StockCategory = "Sub Raw Material Stock" },
                new RawMaterialStockBLL { MaterialDescription = "Tundish Cover Powder", StockCategory = "Sub Raw Material Stock" },
                new RawMaterialStockBLL { MaterialDescription = "Aluminum Lump", StockCategory = "Sub Raw Material Stock" }
            };

            return vm;
        }
    }
}