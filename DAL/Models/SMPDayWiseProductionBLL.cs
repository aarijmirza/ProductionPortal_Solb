using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class SMPDayWiseProductionBLL
    {
        public int ID { get; set; }

        public DateTime Date { get; set; }
        public string Month { get; set; }

        public int? NoOfHeats { get; set; }
        public decimal? ProductionPlan { get; set; }
        public decimal? TotalCastedTon { get; set; }
        public decimal? TLSTon { get; set; }

        public decimal? ScrapCharge { get; set; }
        public decimal? DRI_OLD_DRI { get; set; }
        public decimal? HBI { get; set; }
        public decimal? AverageHeatWeight { get; set; }
        public decimal? CCMProductivity { get; set; }
        public decimal? PerformanceRate { get; set; }
        public decimal? SMPMaterialYield { get; set; }
        public decimal? Availability { get; set; }
        public decimal? QualityYield { get; set; }
        public decimal? PowerOnTime { get; set; }
        public decimal? NetTapToTap { get; set; }
        public decimal? AverageCastingTime { get; set; }

        public string LengthOfSequence { get; set; }

        public decimal? Electrical { get; set; }
        public decimal? EAFLF { get; set; }
        public decimal? LPG { get; set; }
        public decimal? O2 { get; set; }
        public decimal? Argon { get; set; }
        public decimal? N2 { get; set; }

        public decimal? DRI_HBI { get; set; }
        public decimal? ScrapConsumption { get; set; }
        public decimal? FeSi { get; set; }
        public decimal? SiMn { get; set; }
        public decimal? EAFElectrode { get; set; }
        public decimal? LRFElectrode { get; set; }
        public decimal? Flourspar { get; set; }
        public decimal? CalcinedCarbon { get; set; }
        public decimal? ChargeCoal { get; set; }
        public decimal? RiceHusk { get; set; }
        public decimal? Lime { get; set; }
        public decimal? LFLime { get; set; }
        public decimal? DoloLime { get; set; }

        public decimal? ElectricalDelayEM { get; set; }
        public decimal? MechanicalDelayMM { get; set; }
        public decimal? RefractoryDelayRF { get; set; }
        public decimal? OperationDelayO { get; set; }
        public decimal? UtilityDelayU { get; set; }
        public decimal? CranesDelayCR { get; set; }
        public decimal? MaterialHandlingRMH { get; set; }
        public decimal? ProcurementPR { get; set; }
        public decimal? CCMOperationO { get; set; }
        public decimal? OutsideOS { get; set; }

        public decimal? PlannedMaintenance { get; set; }
        public decimal? ScheduleTime { get; set; }
        public decimal? UtilizedTime { get; set; }
        public decimal? TotalDelayTime { get; set; }

        public int StatusID { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
