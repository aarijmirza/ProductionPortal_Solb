using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class ElectrodeBLL
    {
        public int ID { get; set; }
        public int HeatID { get; set; }
        public int PlantID { get; set; }
        public string PlantName { get; set; }
        public string ElectrodeAddition { get; set; }
        public int Adjusted { get; set; } // Yes/No
        public int Break { get; set; } // Yes/No
        public int StubEndLoss { get; set; } // Yes/No
        public int StatusID { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
