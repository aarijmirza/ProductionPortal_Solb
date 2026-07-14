using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class RawStockBLL
    {
        public int ID { get; set; }

        public DateTime? Date { get; set; }

        public string ItemCode { get; set; }

        public string Material { get; set; }

        public decimal? CurrentStock { get; set; }

        public decimal? AverageDailyIssuance { get; set; }

        public string ProductionDays { get; set; }

        public string LeadTime { get; set; }

        public decimal? CurrentOrderQty { get; set; }

        public string Remarks { get; set; }

        public int? StatusID { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

    }
}
