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

        public string TotalProductBillet { get; set; }

        public string LPG { get; set; }

        public string Oxygen { get; set; }

        public string Nitrogen { get; set; }

        public string Argon { get; set; }

        public string WaterConsumption { get; set; }

        public string PowerConsumption { get; set; }

        public string LPGm3ton { get; set; }

        public string LPGNm3tonTarget { get; set; }

        public string OxygenNm3ton { get; set; }

        public string OxygenNm3tonTarget { get; set; }

        public string NitrogenNm3ton { get; set; }

        public string NitrogenNm3tonTarget { get; set; }

        public string ArgonNm3ton { get; set; }

        public string ArgonNm3tonTarget { get; set; }

        public string PowerConsumptionKWHton { get; set; }

        public string PowerConsumptionKWHtarget { get; set; }

        public string WaterConsumptionM3 { get; set; }

        public string WaterConsumptionTarget { get; set; }

        public string Plant { get; set; }

        public decimal? FuelConsumption { get; set; }

        public int? StatusID { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

    }
}
