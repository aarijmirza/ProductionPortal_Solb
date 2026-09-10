using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class QCLimeReactivityReportBLL
    {
        public int ID { get; set; }

        [Display(Name = "Analysis Date (QC)")]
        [DataType(DataType.Date)]
        public DateTime? AnalysisDateQC { get; set; }

        [Display(Name = "Reference No.")]
        [StringLength(100)]
        public string ReferenceNo { get; set; }

        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        // Accepted / MRB
        [Display(Name = "Final Status")]
        [StringLength(30)]
        public string FinalStatus { get; set; }

        [Display(Name = "MRB No.")]
        [StringLength(100)]
        public string MRBNo { get; set; }

        public int StatusID { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }

        public List<QCLimeReactivitySampleBLL> Samples { get; set; }
            = new List<QCLimeReactivitySampleBLL>();
    }

    public class QCLimeReactivitySampleBLL
    {
        public int ID { get; set; }
        public int ReportID { get; set; }
        public int SrNo { get; set; }

        [Display(Name = "Shipment Code")]
        [StringLength(100)]
        public string ShipmentCode { get; set; }

        [Display(Name = "Received Date")]
        [DataType(DataType.Date)]
        public DateTime? ReceivedDate { get; set; }

        [Display(Name = "Supplier")]
        [StringLength(200)]
        public string Supplier { get; set; }

        [Display(Name = "Quantity (Tons)")]
        public decimal? QuantityTons { get; set; }

        [Display(Name = "Sample No.")]
        [StringLength(100)]
        public string SampleNo { get; set; }

        [Display(Name = "Active CaO %")]
        public decimal? ActiveCaO { get; set; }

        [Display(Name = "Total CaO %")]
        public decimal? TotalCaO { get; set; }

        [Display(Name = "MgO %")]
        public decimal? MgO { get; set; }

        [Display(Name = "LOI %")]
        public decimal? LOI { get; set; }

        // Fresh Lime Temperature (°C)
        public decimal? Temp0Sec { get; set; }
        public decimal? Temp30Sec { get; set; }
        public decimal? Temp60Sec { get; set; }
        public decimal? Temp90Sec { get; set; }
        public decimal? Temp120Sec { get; set; }
        public decimal? Temp150Sec { get; set; }
        public decimal? Temp180Sec { get; set; }
        public decimal? Temp240Sec { get; set; }
        public decimal? Temp360Sec { get; set; }
        public decimal? Temp600Sec { get; set; }

        [Display(Name = "Status")]
        [StringLength(50)]
        public string TestStatus { get; set; }

        public int StatusID { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
