using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class RMShiftDetailsBLL
    {
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public string Plant { get; set; }
        public string Shift { get; set; }
        public string Team { get; set; }
        public string ShiftIncharge { get; set; }
        public int StatusID { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
