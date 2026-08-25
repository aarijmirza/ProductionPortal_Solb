using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class SMPProductionDelayBLL
    {
        public int ID { get; set; }
        public string DelayCode { get; set; }
        public int? PlantDelayID { get; set; }
        public string Plant { get; set; }
        public string ShiftGroup { get; set; }
        public DateTime ProductionDate { get; set; }
        public TimeSpan? DelayStart { get; set; }
        public TimeSpan? DelayFinish { get; set; }
        public int TotalMinutes { get; set; }
        public string Agency { get; set; }
        public string Area { get; set; }
        public string Equipment { get; set; }
        public string DelayDescription { get; set; }
        public string ReasonForOccurrence { get; set; }
        public string ActionTaken { get; set; }
        public DateTime? LastPMDate { get; set; }
        public string FailureReportStatus { get; set; }
        public string IncreaseMTBF { get; set; }
        public string DecreaseMTTR { get; set; }
        public string SAPBreakdownOrder { get; set; }
        public string FailureCategory1Component { get; set; }
        public string FailureCategory2RootCause { get; set; }
        public bool IsFailureAnalysisFilled { get; set; }
        public int StatusID { get; set; }
    }

    public class SMPProductionDelayUploadBLL
    {
        public int ExcelRowNo { get; set; }

        public string Plant { get; set; }
        public string ShiftGroup { get; set; }
        public DateTime ProductionDate { get; set; }
        public TimeSpan? DelayStart { get; set; }
        public TimeSpan? DelayFinish { get; set; }
        public int TotalMinutes { get; set; }

        public string Agency { get; set; }
        public string Area { get; set; }
        public string Equipment { get; set; }
        public string DelayDescription { get; set; }
        public string ReasonForOccurrence { get; set; }
        public string ActionTaken { get; set; }

        public DateTime? LastPMDate { get; set; }
        public string FailureReportStatus { get; set; }
        public string IncreaseMTBF { get; set; }
        public string DecreaseMTTR { get; set; }
        public string SAPBreakdownOrder { get; set; }
        public string FailureCategory1Component { get; set; }
        public string FailureCategory2RootCause { get; set; }
    }

    public class SMPProductionDelayImportResultBLL
    {
        public int ProcessedRows { get; set; }
        public int InsertedPlantDelays { get; set; }
        public int UpdatedPlantDelays { get; set; }
        public int DeactivatedPlantDelays { get; set; }
        public int InsertedSMPProductionDelays { get; set; }
        public int UpdatedSMPProductionDelays { get; set; }
        public int DeactivatedSMPProductionDelays { get; set; }
        public int InsertedFailureAnalyses { get; set; }
        public int UpdatedFailureAnalyses { get; set; }
        public int GeneratedDelayCodes { get; set; }
        public string FirstGeneratedDelayCode { get; set; }
        public string LastGeneratedDelayCode { get; set; }
    }
}


