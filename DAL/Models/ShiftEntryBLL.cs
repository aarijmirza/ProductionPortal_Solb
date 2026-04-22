using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class ShiftEntryBLL
    {
        public string Plant { get; set; }
        public string Shift { get; set; }
        public string Team { get; set; }
        public string ShiftIncharge { get; set; }
        public DateTime EntryDate { get; set; }
    }
}
