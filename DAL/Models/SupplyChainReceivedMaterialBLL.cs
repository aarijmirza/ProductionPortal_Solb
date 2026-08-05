using System;
using System.Collections.Generic;

namespace DAL.Models
{
    public class SupplyChainReceivedMaterialBLL
    {
        public int ID { get; set; }

        public int SupplyChainDailyID { get; set; }

        public string MaterialType { get; set; }

        public string ItemName { get; set; }

        public decimal Quantity { get; set; }

        public int StatusID { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }
    }
}