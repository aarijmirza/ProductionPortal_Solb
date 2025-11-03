using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class DelaysBLL
    {
        public int ID { get; set; }
        public int PlantID { get; set; }
        public string PlantName { get; set; }
        public string HeatNo { get; set; } = null;
        public int HeatID { get; set; }
        public int OperationID { get; set; } // FK → EAFOperation
        public string GROUP_ORD { get; set; } // Section/Delay type
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public int TotalDuration { get; set; }
        public string Reason { get; set; }
        public int StatusID { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
