using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class LFMeltShopBLL
    {
        public int ID { get; set; }
        public DateTime? Date { get; set; }
        public string Shift { get; set; }
        public string Grade { get; set; }
        public string HeatNo { get; set; }
        public int? NoOfHeat { get; set; }
        public int? SequenceHeat { get; set; }
        public decimal? TemperatureC { get; set; }
        public decimal? LMWeightApp { get; set; }
        public decimal? CarryOverSlag { get; set; }
        public decimal? FreeBoard { get; set; }
        public TimeSpan? HeatStartTime { get; set; }
        public TimeSpan? HeatEndTime { get; set; }
        public decimal? ArcingTime { get; set; }
        public decimal? PowerConsumption { get; set; }
        public decimal? DispatchTemperature { get; set; }
        public decimal? DeltaLife { get; set; }
        public int? LadleNo { get; set; }
        public decimal? Ladlelife { get; set; }
        public decimal? Returned { get; set; }
        public decimal? Preheated { get; set; }
        public decimal? Slagzone { get; set; }
        public decimal? Purging { get; set; }
        public decimal? LadleTemperature { get; set; }
        public decimal? EAFCPC { get; set; }
        public decimal? EAFFeSI { get; set; }
        public decimal? EAFSiMn { get; set; }
        public decimal? EAFFeMn { get; set; }
        public decimal? EAFAlBar { get; set; }
        public decimal? EAFLime { get; set; }
        public decimal? LFCPC { get; set; }
        public decimal? LFGCrush { get; set; }
        public decimal? LFInjectedCoke { get; set; }
        public decimal? LFDOLOLIME { get; set; }
        public decimal? LFDOLOMITE { get; set; }
        public decimal? LFCaF2 { get; set; }
        public decimal? LFRiceHusk { get; set; }
        public decimal? LFArgon { get; set; }
        public decimal? TT900MMImport { get; set; }
        public decimal? TT900MMLocal { get; set; }
        public decimal? TT1200MM { get; set; }
        public decimal? CeloxO2Probe { get; set; }
        public int? StatusID { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public List<DelaysBLL> Delays { get; set; }

        public LFMeltShopBLL()
        {
            Delays = new List<DelaysBLL>();
        }
    }
}
