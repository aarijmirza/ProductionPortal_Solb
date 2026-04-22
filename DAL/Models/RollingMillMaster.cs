using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class RollingMillMasterBLL
    {
        public int ID { get; set; }

        public DateTime? Date { get; set; }

        public DateTime? ChargingDate { get; set; }

        public DateTime? DischargingDate { get; set; }

        public string HeatNo { get; set; }

        public string BoardingNo { get; set; }

        public string Plant { get; set; }

        public string Shift { get; set; }

        public string ProductSpecs { get; set; }

        public string SteelGrade { get; set; }

        public decimal? BilletLength { get; set; }

        public string CrossSection { get; set; }

        public decimal? BilletWeight { get; set; }

        public decimal? TotalBillet { get; set; }

        public string TotalWeight { get; set; }

        public string Size { get; set; }

        public string Profile { get; set; }

        public string Remarks { get; set; }

        public int? ChargingHeatSeq { get; set; }

        public int? DischargingHeatSeq { get; set; }

        public int? HeatStatus { get; set; }

        public string NewSteelGrade { get; set; }

        public string PONumber { get; set; }

        public decimal? Cobble { get; set; }

        public decimal? HotOut { get; set; }

        public decimal? TotalDischargeBillet { get; set; }

        public decimal? TotalDischargeWeight { get; set; }

        public int? StatusID { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public string UpdatedBy { get; set; }

        public List<RMChemicalAnalysisBLL> Chemistry { get; set; }

    }
}
