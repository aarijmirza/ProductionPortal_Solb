using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class SMPDailyPerformanceReportBLL
    {
        public DateTime ReportDate { get; set; }
        public int NumberOfHeats { get; set; }
        public decimal DRI { get; set; }
        public decimal Scrap { get; set; }
        public decimal LiquidSteel { get; set; }
        public decimal CastedWeight { get; set; }
        public decimal TapToTap { get; set; }
        public decimal Availability { get; set; }
        public decimal Performance { get; set; }
        public decimal Yield { get; set; }
        public decimal QualityYield { get; set; }
        public decimal EAFProductivity { get; set; }
        public decimal CCMProductivity { get; set; }
        public decimal DRIKgPerTon { get; set; }
        public decimal ScrapKgPerTon { get; set; }
        public decimal FeSiKgPerTon { get; set; }
        public decimal FeSiMnKgPerTon { get; set; }
        public decimal FeMnKgPerTon { get; set; }
        public decimal RiceHuskKgPerTon { get; set; }
        public decimal LimeKgPerTon { get; set; }
        public decimal DoloLimeKgPerTon { get; set; }
        public decimal ChargeCoalKgPerTon { get; set; }
        public decimal FluorsparKgPerTon { get; set; }
        public decimal CalcinedCarbonKgPerTon { get; set; }
        public decimal AluminiumKgPerTon { get; set; }
        public decimal PowerKwhPerTon { get; set; }
        public decimal LPGNm3PerTon { get; set; }
        public decimal OxygenNm3PerTon { get; set; }
        public decimal ArgonNm3PerTon { get; set; }
        public decimal NitrogenNm3PerTon { get; set; }
        public decimal WaterM3 { get; set; }
        public decimal MechanicalDelay { get; set; }
        public decimal ElectricalDelay { get; set; }
        public decimal OperationDelay { get; set; }
        public decimal RefractoryDelay { get; set; }
        public decimal UtilityDelay { get; set; }
        public decimal CraneDelay { get; set; }
        public string Remarks { get; set; }
    }
}