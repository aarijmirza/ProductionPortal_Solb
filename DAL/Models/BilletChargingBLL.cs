using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class BilletChargingBLL
    {
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public string HeatNo { get; set; }
        public string Shift { get; set; }
        public string BoardingNo { get; set; }
        public string SteelGrade { get; set; }
        public string BilletSize { get; set; }
        public string Profile { get; set; }
        public string ProductSpecs { get; set; }
        public string BilletLength { get; set; }
        public decimal? TotalBillet { get; set; }
        public decimal? TotalWeight { get; set; }
        public decimal? Weight { get; set; }
        public int HeatSequence { get; set; }
        public int HeatStatus { get; set; }
        public int StatusID { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }
}
