using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class RMQCInspectionBLL
    {
        public DateTime ProductionDate { get; set; }
        public string Shift { get; set; }
        public string HeatNo { get; set; }

        public string SteelGrade { get; set; }
        public string BarSize { get; set; }

        public int TotalBundles { get; set; }
        public int OnHold { get; set; }
        public int Rejected { get; set; }
        public int Accepted { get; set; }

        public string BundleSeriesOnHold { get; set; }
        public string DefectCodes { get; set; }
        public string MRBNo { get; set; }

        public string QCStatus { get; set; }
        public string Remarks { get; set; }

        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public int StatusID { get; set; }
    }
}
