using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class RMBundlingQCRowBLL
    {
        public int ID { get; set; }

        public DateTime? ProductionDate { get; set; }

        public string Shift { get; set; }

        public string Plant { get; set; }

        public string HeatNo { get; set; }

        public string Size { get; set; }

        public string Profile { get; set; }

        public string Product { get; set; }

        public string SteelGrade { get; set; }

        public int? TotalBundleProduced { get; set; }

        public int? OnHold { get; set; }

        public int? Rejected { get; set; }

        public int? Accepted { get; set; }

        public string BundleSeriesOnHold { get; set; }

        public string DefectCode { get; set; }

        public string MRBNo { get; set; }

        public string Remarks { get; set; }
    }
}
