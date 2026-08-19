using System;
using System.Collections.Generic;

namespace DAL.Models
{
    public class CMDPerformanceDashboardVM
    {
        public CMDPerformanceDashboardVM()
        {
            DailyProduction =
                new ProductionSummaryVM();

            MTDProduction =
                new ProductionSummaryVM();

            YTDProduction =
                new ProductionSummaryVM();

            Downtime =
                new List<DowntimeSummaryVM>();

            EquipmentFailures =
                new List<TopFailureVM>();

            RCAFailures =
                new List<TopFailureVM>();

            ClosureRates =
                new List<ClosureRateVM>();

            TopDelays =
                new List<CMDTopDelayVM>();
        }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public ProductionSummaryVM DailyProduction { get; set; }

        public ProductionSummaryVM MTDProduction { get; set; }

        public ProductionSummaryVM YTDProduction { get; set; }

        public List<DowntimeSummaryVM> Downtime { get; set; }

        public List<TopFailureVM> EquipmentFailures { get; set; }

        public List<TopFailureVM> RCAFailures { get; set; }

        public List<ClosureRateVM> ClosureRates { get; set; }

        public List<CMDTopDelayVM> TopDelays { get; set; }
    }

    public class ProductionSummaryVM
    {
        public decimal SMP { get; set; }

        public decimal RM1 { get; set; }

        public decimal RM2 { get; set; }

        public decimal ComparisonPercentage { get; set; }
    }

    public class DowntimeSummaryVM
    {
        public string Plant { get; set; }

        public decimal DTDMechanical { get; set; }

        public decimal DTDElectrical { get; set; }

        public decimal DTDCranes { get; set; }

        public decimal DTDUtilities { get; set; }

        public decimal MTDMechanical { get; set; }

        public decimal MTDElectrical { get; set; }

        public decimal MTDCranes { get; set; }

        public decimal MTDUtilities { get; set; }
    }

    public class TopFailureVM
    {
        public string Plant { get; set; }

        public string Name { get; set; }

        public decimal DelayHours { get; set; }

        public string FailureType { get; set; }
    }

    public class ClosureRateVM
    {
        public string Plant { get; set; }

        public string Department { get; set; }

        public string MonthName { get; set; }

        public int MonthNumber { get; set; }

        public decimal ClosurePercentage { get; set; }
    }

    public class CMDTopDelayVM
    {
        public string Plant { get; set; }

        public string Shift { get; set; }

        public string AgencyName { get; set; }

        public decimal TotalDuration { get; set; }

        public string DelayCode { get; set; }

        public string EquipmentCode { get; set; }

        public string Description { get; set; }

        public string ReasonForOccurrence { get; set; }

        public string ActionTaken { get; set; }
    }
}
