using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class LaddleFurnaceBLL
    {
        public int LFID { get; set; }
        public DateTime? Date { get; set; }
        public string HeatNo { get; set; }
        public string Grade { get; set; }
        public decimal? RecarburizerSilo { get; set; }
        public decimal? RecarburizerBags { get; set; }
        public decimal? FeSi { get; set; }
        public decimal? FeMn { get; set; }
        public decimal? SiMn { get; set; }
        public decimal? TotalAlloys { get; set; }
        public decimal? Lime312mm { get; set; }
        public decimal? Dololime { get; set; }
        public decimal? MgO { get; set; }
        public decimal? FlourSpar { get; set; }
        public decimal? RiceHusk { get; set; }
        public decimal? CaSi { get; set; }
        public decimal? CafeWire { get; set; }
        public decimal? Cac2 { get; set; }
        public decimal? SynSlag { get; set; }
        public decimal? Electrode { get; set; }
        public decimal? KWH { get; set; }
        public decimal? Nitrogen { get; set; }
        public decimal? Argon { get; set; }
        public decimal? LMForCasting { get; set; }
        public string Remarks { get; set; }
        public int? StatusID { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
