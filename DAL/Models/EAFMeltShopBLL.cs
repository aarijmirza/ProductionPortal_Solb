using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class EAFMeltShopBLL
    {
        public int ID { get; set; }
        public DateTime? Date { get; set; }
        public string Shift { get; set; }
        public string GRADE_ID { get; set; }
        public string HeatNo { get; set; }
        public int NoOfHeat { get; set; }
        public int SequenceHeat { get; set; }
        public TimeSpan? PrevTappTime { get; set; }
        public int EAFShellNo { get; set; }
        public TimeSpan? HeatStartTime { get; set; }
        public TimeSpan? HeatEndTime { get; set; }
        public int TaptoTapTime { get; set; }
        public int NetArcingTime { get; set; }
        public int TurnAroundTime { get; set; }
        public int LiningLife { get; set; }
        public int BottomLife { get; set; }
        public int EBTLife { get; set; }
        public int DeltaLife { get; set; }

        // Charges
        public decimal BucketChargingWeight { get; set; }
        public decimal ISACChargeWeight { get; set; }
        public decimal LimeISAC { get; set; }
        public decimal DoloISAC { get; set; }
        public decimal CokeISAC { get; set; }
        public decimal ISACFluxWeight { get; set; } // readonly (calculated)
        public decimal TotalChargeWeight { get; set; } // readonly (calculated)

        // Process Values
        public decimal LMTAPWTApp { get; set; }
        public decimal TappingTemp { get; set; }
        public decimal OpeningCarbon { get; set; }
        public int TappingDuration { get; set; }
        public decimal PowerConsumption { get; set; }

        // Consumables
        public decimal FeSi { get; set; }
        public decimal SiMn { get; set; }
        public decimal FeMn { get; set; }
        public decimal AlBAR { get; set; }
        public decimal LIME { get; set; }
        public decimal DOLOLIME { get; set; }
        public decimal DOLOMITE { get; set; }
        public decimal INJECTIONCARBON { get; set; }
        public decimal HARD_COKE { get; set; }
        public decimal GUNNING { get; set; }
        public decimal FETTLING { get; set; }
        public decimal TEMP_TIPS { get; set; }
        public decimal LOLLYPOP_SAMPLER { get; set; }
        public decimal TOTAL_OXY { get; set; }
        public decimal CELOXO2_PROBE { get; set; }
        public decimal EBT_FILLER { get; set; }
        public decimal O2LANCING_PIPE { get; set; }
        public decimal CERAMIC_COATED_PIPE { get; set; }
        public int StatusID { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; } = null;
        public DateTime? UpdatedDate { get; set; }

        // Navigation Properties
        public List<ElectrodeBLL> Electrodes { get; set; }
        public List<DelaysBLL> Delays { get; set; }

        public EAFMeltShopBLL()
        {
            Electrodes = new List<ElectrodeBLL>();
            Delays = new List<DelaysBLL>();
        }
    }
}
