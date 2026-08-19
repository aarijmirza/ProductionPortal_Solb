using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class BilletDischargingBLL
    {
        public int ID { get; set; }
        public int DischargingSequence { get; set; }
        public string Shift { get; set; }
        public string HeatNo { get; set; }
        public string BoardingNo { get; set; }
        public string SteelGrade { get; set; }
        public string NewSteelGrade { get; set; }
        public string ProductCode { get; set; }
        public string PONumber { get; set; }
        public decimal? Cobble { get; set; }
        public decimal? HotOut { get; set; }
        public decimal? TotalBillet { get; set; }
        public decimal? FuelConsumption { get; set; }
        public decimal? TotalWeight { get; set; }
        public decimal? TheoriticalWeight { get; set; }
        public decimal? TotalBundleProduced { get; set; }
        public decimal? ShortBundleActualWeight { get; set; }
        public decimal? TotalBundleOnHold { get; set; }
        public decimal? TotalRejectedBundle { get; set; }
        public int StatusID { get; set; }
        public int HeatStatus { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime Date { get; set; }
        public string Plant { get; set; }
        public string PlantName { get; set; }
        public string ProductSpecs { get; set; }
        public string Grade { get; set; }
        public string BilletLength { get; set; }
        public string CrossSection { get; set; }
        public string Size { get; set; }
        public string Profile { get; set; }
        public string Remarks { get; set; }

        public List<BilletDischargingBLL> MonthlyProductionData { get; set; }
    }
}
