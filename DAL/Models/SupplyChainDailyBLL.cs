using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class SupplyChainDailyBLL
    {
        public int ID { get; set; }

        public DateTime? ReportDate { get; set; }
        public TimeSpan? ReportTime { get; set; }

        public decimal? Scrap { get; set; }
        public decimal? DRI { get; set; }
        public decimal? HBI { get; set; }

        public decimal? Billet { get; set; }
        public decimal? Rebar { get; set; }
        public decimal? WireRodCoil { get; set; }
        public decimal? RebarInCoil { get; set; }
        public decimal? EpoxyRebar { get; set; }
        public decimal? ShortBar { get; set; }

        public decimal? DailyDispatch { get; set; }
        public decimal? DailyDispatchTarget { get; set; }
        public int? DailyTruck { get; set; }
        public int? DailyTruckTarget { get; set; }

        public decimal? WTDDispatch { get; set; }
        public decimal? WTDDispatchTarget { get; set; }
        public decimal? MTDDispatch { get; set; }
        public decimal? MTDDispatchTarget { get; set; }

        public decimal? RawMaterialsReceived { get; set; }
        public decimal? SubRawMaterialsReceived { get; set; }
        public decimal? RefractoryMaterialsReceived { get; set; }
        public decimal? FuelOilReceived { get; set; }
        public decimal? OtherReceived { get; set; }

        public decimal? MillScale { get; set; }
        public decimal? Slag { get; set; }
        public decimal? Dust { get; set; }
        public decimal? Sludge { get; set; }

        public int? StatusID { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
