using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class FailureAnalysisBLL
    {
        public int ID { get; set; }

        public int? DelayID { get; set; }

        public DateTime? LastPMDate { get; set; }

        public string FailureReportStatus { get; set; }

        public string IncreaseMTBF { get; set; }

        public string DecreaseMTTR { get; set; }

        public string SAPBreakdownOrder { get; set; }

        public string FailureCategory1Component { get; set; }

        public string FailureCategory2RootCause { get; set; }

        public int? StatusID { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

    }
}
