using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class DelayCounterMeasureBLL
    {
        public int ID { get; set; }

        public int PlantDelayID { get; set; }

        public string CounterMeasureCode { get; set; }

        public string CounterMeasure { get; set; }

        public string SAPOrderNo { get; set; }

        public string Responsible { get; set; }

        public DateTime? TargetDate { get; set; }

        public string EvidenceForCompletion { get; set; }

        public string CounterMeasureStatus { get; set; }

        public string ReasonForNotClosing { get; set; }

        public int StatusID { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        // Display-only PlantDelay information
        public string DelayCode { get; set; }

        public DateTime? DelayDate { get; set; }

        public string Plant { get; set; }

        public string Area { get; set; }

        public string Equipment { get; set; }

        public string DelayDescription { get; set; }

        public string CounterMeasureA { get; set; }

        public string IncreaseMTBF { get; set; }

        public string DecreaseMTTR { get; set; }

        public string IncreaseMTBF1 { get; set; }

        public string DecreaseMTTR1 { get; set; }

        public string RootCause { get; set; }

        public string SAPBreakdownOrder { get; set; }

        public string FailureReportStatus { get; set; }
    }
}
