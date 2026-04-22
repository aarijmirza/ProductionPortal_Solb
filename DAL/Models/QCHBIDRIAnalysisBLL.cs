using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class QCHBIDRIAnalysisBLL
    {
        public int ID { get; set; }

        public string Material { get; set; }

        public string ShipmentCodeNo { get; set; }

        public string Supplier { get; set; }

        public DateTime? ReceivingDate { get; set; }

        public int? Quantity { get; set; }

        public DateTime? AnalysisDate { get; set; }

        public string ReferenceNo { get; set; }

        public string ReceivedQuantity { get; set; }

        public string PhysicalAnalysis { get; set; }

        public int? StatusID { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public string UpdatedBy { get; set; }

        // ✅ ADD THIS
        public List<SampleHBIDRIBLL> Samples { get; set; }

        public QCHBIDRIAnalysisBLL()
        {
            Samples = new List<SampleHBIDRIBLL>();
        }
    }

}
