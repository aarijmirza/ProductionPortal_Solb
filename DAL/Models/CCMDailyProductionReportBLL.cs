using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class CCMDailyProductionReportBLL
    {
        public int ID { get; set; }

        [Required]
        [Display(Name = "Report No.")]
        public string ReportNo { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Report Date")]
        public DateTime ReportDate { get; set; }

        [Required]
        public string Shift { get; set; }

        public string Team { get; set; }

        [Display(Name = "CCM Foreman")]
        public string CCMForeman { get; set; }

        [Display(Name = "Billet Yard Operator")]
        public string BilletYardOperator { get; set; }

        public int TotalBillets { get; set; }

        public int PrimeBillets { get; set; }

        public int StatusID { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public List<CCMDailyProductionReportDetailBLL>
            Details
        { get; set; }

        public CCMDailyProductionReportBLL()
        {
            Details =
                new List<
                    CCMDailyProductionReportDetailBLL
                >();
        }
    }
}