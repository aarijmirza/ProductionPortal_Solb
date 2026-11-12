using System;

namespace DAL.Models
{
    public class RMChemicalAnalysisBLL
    {
        public int ID { get; set; }

        public string HeatNo { get; set; }

        public int? NoOfBillets { get; set; }

        // Exact MTC / Chemical Analysis sequence:
        // C, Si, Mn, P, S, Cu, Ni, Cr, Mo, V, B, N, CE%
        public decimal? C { get; set; }
        public decimal? Si { get; set; }
        public decimal? Mn { get; set; }
        public decimal? P { get; set; }
        public decimal? S { get; set; }
        public decimal? Cu { get; set; }
        public decimal? Ni { get; set; }
        public decimal? Cr { get; set; }
        public decimal? Mo { get; set; }
        public decimal? V { get; set; }
        public decimal? B { get; set; }
        public decimal? N { get; set; }
        public decimal? Ceq { get; set; }

        public int? HeatStatus { get; set; }
        public int? StatusID { get; set; }

        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
