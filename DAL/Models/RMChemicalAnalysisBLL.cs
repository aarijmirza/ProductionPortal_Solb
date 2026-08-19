using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class RMChemicalAnalysisBLL
    {
        public int ID { get; set; }

        public string HeatNo { get; set; }

        public int? NoOfBillets { get; set; }

        public decimal? C { get; set; }

        public decimal? Si { get; set; }

        public decimal? Mn { get; set; }

        public decimal? S { get; set; }

        public decimal? P { get; set; }

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
