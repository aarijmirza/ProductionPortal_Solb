using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class QCInspectionDataRMBLL
    {
        public int ID { get; set; }
        public DateTime? Date { get; set; }
        public string HeatNo { get; set; }
        public string SteelGrade { get; set; }
        public string Barsize { get; set; }
        public int TotalBundlesProduced { get; set; }
        public int TotalBundleOnHold { get; set; }
        public int TotalRejectedBundle { get; set; }
        public int AcceptedBundle { get; set; }
        public string BundleSeriesOnHold { get; set; }
        public string DefectCodes { get; set; }
        public string MRBNo { get; set; }
        public int HeatStatus { get; set; }
        public string Remarks { get; set; }
        public int StatusID { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }
}
