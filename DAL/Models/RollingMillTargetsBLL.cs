using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class RollingMillTargetsBLL
    {
        public int ID { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }

        public string Size { get; set; }
        public string Profile { get; set; }
        public decimal YeildPercentageRBTarget { get; set; }
        public decimal YeildPercentageWRTarget { get; set; }
        public decimal YeildPercentagePBTarget { get; set; }
        public decimal YeildPercentageRICTarget { get; set; }
        public decimal RRRPercentageTarget { get; set; }
        public decimal TonperhourTarget { get; set; }
        public decimal FuelOilConsumption { get; set; }
        public decimal FuelOilTarget { get; set; }
        public decimal ElectricityTarget { get; set; }
        public decimal WaterTarget { get; set; }
        public decimal ProductionTarget { get; set; }
        public decimal GuidePassTarget { get; set; }
        public decimal RollShopTarget { get; set; }
        public decimal ElectricalTarget { get; set; }
        public decimal MechanicalTarget { get; set; }
        public decimal CraneTarget { get; set; }
        public decimal DispatchTarget { get; set; }
        public decimal QualityTarget { get; set; }
        public decimal UtilityTarget { get; set; }
        public decimal OthersTarget { get; set; }
        public decimal SizeChangeTarget { get; set; }
        public decimal DownDayTarget { get; set; }
        public decimal PowerFailureTarget { get; set; }
        public decimal NoBilletTarget { get; set; }
        public decimal AnnualShutdownTarget { get; set; }
        public int StatusID { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
