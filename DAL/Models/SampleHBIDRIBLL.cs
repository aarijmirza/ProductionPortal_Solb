using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class SampleHBIDRIBLL
    {
        public int ID { get; set; }

        public int? AnalysisID { get; set; }

        public string SampleCode { get; set; }

        public decimal? FeTotal { get; set; }

        public decimal? FeMetallic { get; set; }

        public decimal? Metallization { get; set; }

        public decimal? C { get; set; }

        public decimal? S { get; set; }

        public decimal? P { get; set; }

        public decimal? SiO2 { get; set; }

        public decimal? Al2O3 { get; set; }

        public decimal? MgO { get; set; }

        public decimal? CaO { get; set; }

        public decimal? TotalGangue { get; set; }

        public string GrainSize { get; set; }

        public string Comment { get; set; }

        public int? StatusID { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public string UpdatedBy { get; set; }

    }
}
