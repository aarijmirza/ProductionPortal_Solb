using BAL.Repositories;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using OfficeOpenXml;
using System.Globalization;

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
    }
}