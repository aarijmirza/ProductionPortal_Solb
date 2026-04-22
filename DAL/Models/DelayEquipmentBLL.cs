using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class DelayEquipmentBLL
    {
        public int ID { get; set; }

        public string LocationName { get; set; }

        public string PlantArea { get; set; }

        public string Description { get; set; }

        public string Code { get; set; }

        public int? StatusID { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

    }
}
