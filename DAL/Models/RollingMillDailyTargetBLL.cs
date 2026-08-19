using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class RollingMillDailyTargetBLL
    {
        public int ID { get; set; }

        public DateTime? TargetDate { get; set; }

        public string Plant { get; set; }

        public decimal? DailyProductionTarget { get; set; }

        public decimal? FuelConsumption { get; set; }

        public int? StatusID { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }
}
