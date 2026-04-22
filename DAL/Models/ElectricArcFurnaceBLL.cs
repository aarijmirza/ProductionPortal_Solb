using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class ElectricArcFurnaceBLL
    {
        public int EAFID { get; set; }

        public DateTime? Date { get; set; }

        public string HeatNo { get; set; }

        public string Group { get; set; }

        public string Shift { get; set; }

        public string Grade { get; set; }

        public decimal? NoofBaskets { get; set; }

        public decimal? Scrap { get; set; }

        public decimal? DRI { get; set; }

        public decimal? OldHBIDRI { get; set; }

        public decimal? HBI { get; set; }

        public decimal? PB { get; set; }

        public decimal? Dumped { get; set; }

        public decimal? Lime { get; set; }

        public decimal? Dololime { get; set; }

        public decimal? ChargeCoal { get; set; }

        public decimal? TotalChargeWeight { get; set; }

        public decimal? TotalTappingWeight { get; set; }

        public decimal? FeMn { get; set; }

        public decimal? FeSi { get; set; }

        public decimal? SiMn { get; set; }

        public decimal? TotalAlloys { get; set; }

        public decimal? LPG { get; set; }

        public decimal? Oxygen { get; set; }

        public decimal? SpecOxygen { get; set; }

        public decimal? InjCarbon { get; set; }

        public decimal? SpecCarbon { get; set; }

        public decimal? EnergyKWH { get; set; }

        public decimal? SpecEnergy { get; set; }

        public decimal? TaptoTap { get; set; }

        public decimal? TAT { get; set; }

        public decimal? Carbon { get; set; }

        public decimal? FlourSpar { get; set; }

        public decimal? AluminumLamps { get; set; }

        public decimal? Lime312mm { get; set; }

        public decimal? Lime550mm { get; set; }

        public decimal? Coke { get; set; }

        public decimal? Magnisia { get; set; }

        public decimal? Electrode { get; set; }

        public decimal? TotalTls { get; set; }

        public decimal? NetTls { get; set; }

        public TimeSpan? HeatStart { get; set; }

        public TimeSpan? HeatStop { get; set; }

        public decimal? Yeild { get; set; }

        public decimal? PowerOn { get; set; }

        public decimal? TappingC { get; set; }

        public decimal? TapTemp { get; set; }

        public decimal? UnscheduleDelay { get; set; }

        public decimal? EffectiveDelay { get; set; }

        public decimal? TotalDelay { get; set; }

        public decimal? Hearth { get; set; }

        public decimal? Wall { get; set; }

        public decimal? Roof { get; set; }

        public decimal? EBT { get; set; }

        public decimal? Gunning { get; set; }

        public decimal? Fettling { get; set; }

        public decimal? LadleNo { get; set; }

        public decimal? LadleLife { get; set; }

        public int? StatusID { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

    }
}
