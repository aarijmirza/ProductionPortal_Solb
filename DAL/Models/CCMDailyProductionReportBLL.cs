using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class CCMDailyProductionReportBLL
    {
        public int ID { get; set; }

        [Required]
        [StringLength(50)]
        public string ReportNo { get; set; }

        [Required]
        public DateTime ReportDate { get; set; }

        [Required]
        [StringLength(50)]
        public string Shift { get; set; }

        [StringLength(100)]
        public string Team { get; set; }

        [StringLength(150)]
        public string CCMForeman { get; set; }

        [StringLength(150)]
        public string BilletYardOperator { get; set; }

        public string HeatNo { get; set; }

        public int TotalBillets { get; set; }

        public int PrimeBillets { get; set; }

        public int ShortBillets { get; set; }

        public int StatusID { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public List<CCMDailyProductionReportDetailBLL>
            Details
        {
            get;
            set;
        }

        public CCMDailyProductionReportBLL()
        {
            ReportDate = DateTime.Today;
            StatusID = 1;

            Details =
                new List<
                    CCMDailyProductionReportDetailBLL
                >
                {
                    new CCMDailyProductionReportDetailBLL()
                };
        }
    }

    public class CCMDailyProductionReportDetailBLL
    {
        public int ID { get; set; }

        public int ReportID { get; set; }

        public int SequenceNo { get; set; }

        [StringLength(50)]
        public string HeatNo { get; set; }

        [StringLength(100)]
        public string Grade { get; set; }

        public int Billet14M { get; set; }

        public int Billet13M { get; set; }

        public int Billet12M { get; set; }

        public int Billet11M { get; set; }

        public int GoodBillets { get; set; }

        public int ShortBillets { get; set; }

        public int Bend { get; set; }

        public int TotalBillets { get; set; }

        /*
            Prime billet total length:
            14 × Qty14 + 13 × Qty13 +
            12 × Qty12 + 11 × Qty11
        */
        public decimal? TotalLength { get; set; }

        public decimal? ShortBilletTotalLength
        {
            get;
            set;
        }

        public decimal? ShortBilletAvgLength
        {
            get;
            set;
        }

        /*
            The existing View property name is retained.
            It represents Per Unit Weight.
        */
        public decimal? PerCoilBundleWeight
        {
            get;
            set;
        }

        public decimal? PrimeBilletWeight
        {
            get;
            set;
        }

        public decimal? ShortBilletWeight
        {
            get;
            set;
        }

        public decimal? TotalWeight { get; set; }

        public string Remarks { get; set; }

        public int StatusID { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }
}