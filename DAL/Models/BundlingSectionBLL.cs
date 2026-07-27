using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class BundlingSectionBLL
    {
        public int ID { get; set; }

        public DateTime? Date { get; set; }

        public string Shift { get; set; }   
        
        public string Plant { get; set; }    

        public string Size { get; set; }  
        
        public string Profile { get; set; }    

        public string HeatNo { get; set; }

        public string BilletBoardingNo { get; set; }

        public string PONumber { get; set; }

        public string SteelGrade { get; set; }

        public string ProductCode { get; set; }

        public decimal? TotalBundleProduced { get; set; }

        public decimal? PerCoilWeight { get; set; }

        public decimal? TotalActualWeight { get; set; }

        public decimal? NoOfShortBundle { get; set; }

        public decimal? ShortBundleActualWeight { get; set; }

        public string BundleSerialNo { get; set; }

        public decimal? TheoriticalWeight { get; set; }

        public decimal? TotalBundleOnHold { get; set; }

        public decimal? AcceptedBundle { get; set; }

        public decimal? TotalRejectedBundle { get; set; }

        public string DefectCode { get; set; }

        public string MRBNo { get; set; }

        public string Remarks { get; set; }

        public int? HeatStatus { get; set; }

        public bool? IsPOComplete { get; set; }

        public int? StatusID { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public string UpdatedBy { get; set; }

    }
}
