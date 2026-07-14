using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class FailureAnalysisActionBLL
    {
        public int ID { get; set; }

        public string ActionCode { get; set; }

        public int DelayID { get; set; }

        public int? AnalysisID { get; set; }

        // IncreaseMTBF / DecreaseMTTR
        public string ActionType { get; set; }

        public string ActionRemarks { get; set; }

        public int? StatusID { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }
}
