using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class RMMechanicalTestBLL
    {
        public int ID { get; set; }

        public string HeatNo { get; set; }

        public decimal? BarSize { get; set; }

        public decimal? YieldStrength { get; set; }

        public decimal? TensileStrength { get; set; }

        public decimal? TensileYieldRatio { get; set; }

        public decimal? Elongation { get; set; }

        public decimal? GaugeLength { get; set; }

        public int? NoOfBundles { get; set; }

        public bool BendTestObserved { get; set; }

        public string Remarks { get; set; }

        public int? StatusID { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }
}
