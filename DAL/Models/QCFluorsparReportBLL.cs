using System;
using System.Collections.Generic;

namespace DAL.Models
{
    public class QCFluorsparReportBLL
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
        public List<QCFluorsparSampleBLL> Samples { get; set; }

        public QCFluorsparReportBLL()
        {
            Samples = new List<QCFluorsparSampleBLL>();
        }
    }

    public class QCFluorsparSampleBLL
    {
        public int ID { get; set; }
        public int ReportID { get; set; }
        public int SrNo { get; set; }
        public string SampleNo { get; set; }
        public decimal? CaF2 { get; set; }
        public decimal? SiO2 { get; set; }
        public decimal? P { get; set; }
        public decimal? S { get; set; }
        public decimal? Fe2O3 { get; set; }
        public decimal? Al2O3 { get; set; }
        public decimal? Na2O { get; set; }
        public decimal? K2O { get; set; }
        public decimal? BaO { get; set; }
        public decimal? Pb { get; set; }
        public string Comment { get; set; }
        public int StatusID { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
