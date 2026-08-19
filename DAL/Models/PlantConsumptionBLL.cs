using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class PlantConsumptionBLL
    {
        public int ID { get; set; }

        public DateTime? Date { get; set; }

        public decimal? TotalProductBillet { get; set; }

        public decimal? LPG { get; set; }

        public decimal? Oxygen { get; set; }

        public decimal? Nitrogen { get; set; }

        public decimal? Argon { get; set; }

        public decimal? WaterConsumption { get; set; }

        public decimal? PowerConsumption { get; set; }

        public decimal? LPGm3ton { get; set; }

        public decimal? LPGNm3tonTarget { get; set; }

        public decimal? OxygenNm3ton { get; set; }

        public decimal? OxygenNm3tonTarget { get; set; }

        public decimal? NitrogenNm3ton { get; set; }

        public decimal? NitrogenNm3tonTarget { get; set; }

        public decimal? ArgonNm3ton { get; set; }

        public decimal? ArgonNm3tonTarget { get; set; }

        public decimal? PowerConsumptionKWHton { get; set; }

        public decimal? PowerConsumptionKWHtarget { get; set; }

        public decimal? WaterConsumptionM3 { get; set; }

        public decimal? WaterConsumptionTarget { get; set; }

        public string Plant { get; set; }

        public decimal? FuelConsumption { get; set; }

        public decimal? FuelConsumptionTarget { get; set; }

        public int? StatusID { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

    }
}
