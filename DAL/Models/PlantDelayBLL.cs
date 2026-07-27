using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DAL.Models.ViewModel;

namespace DAL.Models
{
    public class PlantDelayBLL
    {
        public int ID { get; set; }
        public DateTime? Date { get; set; }
        public string Delaycode { get; set; }
        public string Area { get; set; }
        public string Plant { get; set; }
        public string ProductSize { get; set; }
        public string Shift { get; set; }
        public string Team { get; set; }
        public string ShiftIncharge { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public int? TotalDuration { get; set; }
        public int? Cobble { get; set; }
        public int? HotOut { get; set; }
        public string DelayType { get; set; }
        public string Department { get; set; }
        public string AgencyName { get; set; }
        public string AgencyCode { get; set; }
        public string Component { get; set; }
        public string Equipments { get; set; }
        public string Reason { get; set; }
        public string DelayDescription { get; set; }
        public string DelayDescription1 { get; set; }
        public string ReasonForOccurence { get; set; }
        public string ReasonForOccurence1 { get; set; }
        public string ActionTaken { get; set; }
        public string ActionTaken1 { get; set; }

        public DateTime? LastPMDate { get; set; }

        public string AnalysisCode { get; set; }

        public string FailureReportStatus { get; set; }

        public string IncreaseMTBF { get; set; }

        public string IncreaseMTBF1 { get; set; }

        public string DecreaseMTTR { get; set; }

        public string DecreaseMTTR1 { get; set; }

        public string ActionType { get; set; }

        public string ActionRemarks { get; set; }

        public string SAPBreakdownOrder { get; set; }

        public string FailureCategory1Component { get; set; }

        public string FailureCategory2RootCause { get; set; }

        public int StatusID { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }

    }
    public class DelaysVM
    {
        public DateTime? Date { get; set; }
        public string Plant { get; set; }
        public string Shift { get; set; }
        public string Team { get; set; }
        public string ShiftIncharge { get; set; }
        public List<DelayAgencyBLL> Agency { get; set; }
        public List<DelayEquipmentBLL> Equipments { get; set; }
        public List<DelayComponentBLL> Components { get; set; }
        public DelaysVM()
        {
            Agency = new List<DelayAgencyBLL>();
            Equipments = new List<DelayEquipmentBLL>();
            Components = new List<DelayComponentBLL>();
        }
    }

}
