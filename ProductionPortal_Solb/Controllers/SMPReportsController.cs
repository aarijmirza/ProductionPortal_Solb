using BAL.Repositories;
using DAL.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace ProductionPortal_Solb.Controllers
{
    public class SMPReportsController : Controller
    {
        SMPReportsRepository repo;
        public SMPReportsController()
        {
            repo = new SMPReportsRepository();
        }

        public ActionResult list()
        {
            return View();
        }
        public ActionResult ProductionSummary()
        {
            return View();
        }

        public ActionResult DailySummary()
        {
            return View();
        }
        public ActionResult IntegratedReport()
        {
            return View();
        }
        public ActionResult StatisticReport()
        {
            return View();
        }
        public ActionResult SMPDailyDashboard(DateTime? fromDate,
            DateTime? toDate)
        {
            DateTime selectedToDate =
                toDate ?? DateTime.Today;

            DateTime selectedFromDate =
                fromDate ?? selectedToDate;

            // Agar From Date, To Date se bari ho
            // to dates automatically swap ho jayengi.
            if (selectedFromDate > selectedToDate)
            {
                DateTime temp =
                    selectedFromDate;

                selectedFromDate =
                    selectedToDate;

                selectedToDate =
                    temp;
            }

            SMPDashboardVM model =
                repo.GetDashboard(
                    selectedFromDate,
                    selectedToDate
                );

            return View(model);
        }
    }
}