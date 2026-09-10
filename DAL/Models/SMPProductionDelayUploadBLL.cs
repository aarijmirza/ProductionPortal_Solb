using System;

namespace DAL.Models
{
    public class SMPProductionDelayBLL
    {
        public int ID { get; set; }
        public int? RowNo { get; set; }
        public string DelayCode { get; set; }
        public string Plant { get; set; }
        public string PlantDescription { get; set; }
        public string ShiftGroup { get; set; }
        public DateTime ProductionDate { get; set; }
        public TimeSpan? DelayStart { get; set; }
        public TimeSpan? DelayFinish { get; set; }
        public int TotalMinutes { get; set; }
        public string Agency { get; set; }
        public string AgencyCode { get; set; }
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
        public int StatusID { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsFailureAnalysisFilled { get; set; }
    }

    public class SMPProductionDelayUploadBLL
    {
        public int ExcelRowNo { get; set; }

        public int? ID { get; set; }
        public int RowNo { get; set; }
        public string DelayCode { get; set; }

        // Excel Plant ID = SMPPlantArea.AreaCode.
        // This value is saved into SMPDelayDaywise.Plant.
        public string Plant { get; set; }

        public string ShiftGroup { get; set; }
        public DateTime ProductionDate { get; set; }
        public TimeSpan? DelayStart { get; set; }
        public TimeSpan? DelayFinish { get; set; }
        public int TotalMinutes { get; set; }
        public string Agency { get; set; }
        public string AgencyCode { get; set; }
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
        public int StatusID { get; set; }

        // Present because Excel follows the table structure.
        // Import SQL does not trust these audit values.
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
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
