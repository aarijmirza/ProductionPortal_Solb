using System;
using System.Collections.Generic;

namespace DAL.Models
{
    public class QCCokeCoalReportBLL
    {
        public int ID { get; set; }

        public string Supplier { get; set; }
        public decimal? Quantity { get; set; }
        public string ShipmentCodeNo { get; set; }
        public DateTime? ReceivingDate { get; set; }
        public DateTime? AnalysisDate { get; set; }

        public string Material { get; set; }
        public string Description { get; set; }
        public string DeliveryPO { get; set; }
        public string LotSize { get; set; }
        public string CertificateNo { get; set; }

        public string FinalStatus { get; set; }
        public string MRBNo { get; set; }
        public string DecisionDetails { get; set; }
        public string Comments { get; set; }

        public int StatusID { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }

        public List<QCCokeCoalSampleBLL> Samples { get; set; }

        public QCCokeCoalReportBLL()
        {
            Samples = new List<QCCokeCoalSampleBLL>();
        }
    }

    public class QCCokeCoalSampleBLL
    {
        public int ID { get; set; }
        public int ReportID { get; set; }
        public int SrNo { get; set; }

        public string SampleCode { get; set; }
        public decimal? Ash { get; set; }
        public decimal? Moisture { get; set; }
        public decimal? Volatile { get; set; }
        public decimal? S { get; set; }
        public decimal? FixedCarbon { get; set; }
        public string GrainSizeMM { get; set; }
        public string Remark { get; set; }

        public int StatusID { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
