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

        [HttpGet]
        public ActionResult SMPDailyDashboard(
            DateTime? fromDate,
            DateTime? toDate)
        {
            DateTime startDate =
                fromDate.HasValue
                    ? fromDate.Value.Date
                    : DateTime.Today;

            DateTime endDate =
                toDate.HasValue
                    ? toDate.Value.Date
                    : startDate;


            if (startDate > endDate)
            {
                DateTime temp =
                    startDate;

                startDate =
                    endDate;

                endDate =
                    temp;
            }


            SMPDashboardVM model =
                repo.GetSMPDashboard(
                    startDate,
                    endDate
                );


            return View(
                model
            );
        }
    }
}