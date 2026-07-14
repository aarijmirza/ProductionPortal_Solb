using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class RMHourlyDischargeBLL
    {
        public int ID { get; set; }

        public DateTime? Date { get; set; }

        public string Shift { get; set; }

        public string Plant { get; set; }

        public string TimeFrom { get; set; }

        public string TimeTo { get; set; }

        public string NoofBillets { get; set; }

        public decimal? NoofCobble { get; set; }

        public int? Reject { get; set; }

        public string BilletHeatNo { get; set; }

        public string SafetyIssueShift { get; set; }

        public string MessageShift { get; set; }

        public string FuelConsumptionStart { get; set; }

        public string FuelConsumptionEnd { get; set; }

        public string TotalConsumption { get; set; }

        public string ElectricityConsumption { get; set; }

        public int? StatusID { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

    }
}
