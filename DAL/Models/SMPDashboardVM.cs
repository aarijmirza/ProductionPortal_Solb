using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class SMPDashboardVM
    {
        public SMPDashboardVM()
        {
            TopDelays =
                new List<SMPDelayItemBLL>();

            DailyProduction =
                new List<SMPDailyProductionPointBLL>();
        }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public decimal HeatCount { get; set; }

        public decimal HeatTarget { get; set; }

        public decimal BilletOutput { get; set; }

        public decimal Yield { get; set; }

        public decimal Productivity { get; set; }

        public decimal PowerConsumption { get; set; }

        public decimal TapToTap { get; set; }

        public decimal PowerOnTime { get; set; }

        public decimal HeatWeight { get; set; }

        public decimal Performance { get; set; }

        public decimal Availability { get; set; }

        public decimal QualityYield { get; set; }

        public decimal DailyActual { get; set; }

        public decimal DailyTarget { get; set; }

        public decimal PeriodTarget { get; set; }

        public decimal MTDActual { get; set; }

        public decimal MTDTarget { get; set; }

        public decimal YTDActual { get; set; }

        public decimal YTDTarget { get; set; }

        public decimal YearlyTarget { get; set; }

        public decimal YearlyAchievement { get; set; }

        public decimal FeSi { get; set; }

        public decimal SiMn { get; set; }

        public decimal Fluorspar { get; set; }

        public decimal CalcinedCarbon { get; set; }

        public decimal ChargeCoal { get; set; }

        public decimal RiceHusk { get; set; }

        public decimal Lime { get; set; }

        public decimal DoloLime { get; set; }

        public decimal LPG { get; set; }

        public decimal Oxygen { get; set; }

        public decimal Argon { get; set; }

        public decimal Nitrogen { get; set; }

        public decimal DRIShare { get; set; }

        public decimal HBIShare { get; set; }

        public decimal ScrapShare { get; set; }

        public decimal AvailableShare { get; set; }

        public decimal OperationShare { get; set; }

        public decimal MechanicalShare { get; set; }

        public decimal ElectricalShare { get; set; }

        public decimal RefractoryShare { get; set; }

        public decimal OtherShare { get; set; }

        public List<SMPDelayItemBLL> TopDelays { get; set; }

        public List<SMPDailyProductionPointBLL> DailyProduction { get; set; }
    }

    public class SMPDelayItemBLL
    {
        public string DelayName { get; set; }

        public decimal Minutes { get; set; }

        public string Area { get; set; }
    }

    public class SMPDailyProductionPointBLL
    {
        public DateTime ProductionDate { get; set; }

        public string DateLabel { get; set; }

        public decimal Actual { get; set; }

        public decimal Plan { get; set; }
    }
}
