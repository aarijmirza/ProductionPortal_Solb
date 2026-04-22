using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class SlagByProductAnalysisBLL
    {
        public int ID { get; set; }

        public DateTime? Date { get; set; }

        public string HeatNo { get; set; }

        public string CertificateNo { get; set; }

        public string ByProductType { get; set; }

        public DateTime? DateOfProduction { get; set; }

        public DateTime? DateOfAnalysis { get; set; }

        public int? StatusID { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string CreatedBy { get; set; }
        
        public DateTime? UpdatedDate { get; set; }

        public string UpdatedBy { get; set; }

        // ✅ ADD THIS
        public List<SlagSampleAnalysisBLL> Samples { get; set; }

        public SlagByProductAnalysisBLL()
        {
            Samples = new List<SlagSampleAnalysisBLL>();
        }
    }
}
