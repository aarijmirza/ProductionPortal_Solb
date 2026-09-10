using System;
using System.Collections.Generic;

namespace DAL.Models
{
    public class QCFerroAlloyReportBLL
    {
        public int ID { get; set; }

        public string Supplier { get; set; }
        public decimal? Quantity { get; set; }
        public string ShipmentCodeNo { get; set; }
        public DateTime? ReceivingDate { get; set; }
        public DateTime? AnalysisDate { get; set; }

        public string Material { get; set; }
        public string ReferenceNo { get; set; }
        public string DeliveryPO { get; set; }
        public string LotSize { get; set; }
        public string CertificateNo { get; set; }

        public string FinalStatus { get; set; }
        public string MRBNo { get; set; }
        public string Remarks { get; set; }
        public string Comments { get; set; }

        public int StatusID { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }

        public List<QCFerroAlloySampleBLL> Samples { get; set; }

        public QCFerroAlloyReportBLL()
        {
            Samples = new List<QCFerroAlloySampleBLL>();
        }
    }

    public class QCFerroAlloySampleBLL
    {
        public int ID { get; set; }
        public int ReportID { get; set; }
        public int SrNo { get; set; }

        public string SampleNo { get; set; }
        public decimal? Si { get; set; }
        public decimal? Mn { get; set; }
        public decimal? P { get; set; }
        public decimal? S { get; set; }
        public decimal? Al { get; set; }
        public decimal? Ca { get; set; }
        public decimal? V { get; set; }
        public string Comment { get; set; }

        public int StatusID { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
