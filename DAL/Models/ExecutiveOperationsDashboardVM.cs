using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class ExecutiveOperationsDashboardVM
    {
        public ExecutiveOperationsDashboardVM()
        {
            Summary = new ExecutiveSummaryVM();
            SelectedProduction = new ProductionPeriodVM();
            MTDProduction = new ProductionPeriodVM();
            YTDProduction = new ProductionPeriodVM();
            SupplyChain = new SupplyChainSnapshotVM();
            ProductionTrend = new List<ProductionTrendVM>();
            Utilities = new List<UtilityComplianceVM>();
            Departments = new List<DepartmentPerformanceVM>();
            TopRisks = new List<OperationalRiskVM>();
            ManagementActions = new List<ManagementActionVM>();
            ExecutiveTrend = new List<ExecutiveTrendVM>();
            DataValidationIssues = new List<DataValidationIssueVM>();
        }

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public DateTime GeneratedOn { get; set; }

        public ExecutiveSummaryVM Summary { get; set; }
        public ProductionPeriodVM SelectedProduction { get; set; }
        public ProductionPeriodVM MTDProduction { get; set; }
        public ProductionPeriodVM YTDProduction { get; set; }
        public SupplyChainSnapshotVM SupplyChain { get; set; }

        public List<ProductionTrendVM> ProductionTrend { get; set; }
        public List<UtilityComplianceVM> Utilities { get; set; }
        public List<DepartmentPerformanceVM> Departments { get; set; }
        public List<OperationalRiskVM> TopRisks { get; set; }
        public List<ManagementActionVM> ManagementActions { get; set; }
        public List<ExecutiveTrendVM> ExecutiveTrend { get; set; }
        public List<DataValidationIssueVM> DataValidationIssues { get; set; }
    }

    public class ExecutiveSummaryVM
    {
        public decimal TotalProduction { get; set; }
        public decimal SMPYield { get; set; }
        public decimal SMPAvailability { get; set; }
        public decimal DispatchMTD { get; set; }
        public decimal DispatchMTDTarget { get; set; }
        public decimal PowerConsumption { get; set; }
        public decimal PowerTarget { get; set; }
        public decimal TotalDowntimeMinutes { get; set; }
        public decimal MaintenanceClosure { get; set; }
        public decimal HealthScore { get; set; }
        public decimal SMPPlanVariancePercentage { get; set; }
        public decimal DataQualityScore { get; set; }
        public int DataWarningCount { get; set; }
        public string DataQualityStatus { get; set; }
    }

    public class ProductionPeriodVM
    {
        public string PeriodName { get; set; }
        public decimal SMP { get; set; }
        public decimal RM1 { get; set; }
        public decimal RM2 { get; set; }
        public decimal SMPPlan { get; set; }

        public decimal Total
        {
            get { return SMP + RM1 + RM2; }
        }

        public decimal SMPPlanVariancePercentage
        {
            get
            {
                return SMPPlan > 0
                    ? ((SMP - SMPPlan) / SMPPlan) * 100m
                    : 0m;
            }
        }
    }

    public class ProductionTrendVM
    {
        public DateTime MonthStart { get; set; }
        public string MonthLabel { get; set; }
        public decimal? SMP { get; set; }
        public decimal? RM1 { get; set; }
        public decimal? RM2 { get; set; }
        public decimal? Total { get; set; }
    }

    public class SupplyChainSnapshotVM
    {
        public DateTime? ReportDate { get; set; }
        public decimal RawMaterialStock { get; set; }
        public decimal FinishedGoodsStock { get; set; }
        public decimal DailyDispatch { get; set; }
        public decimal DailyDispatchTarget { get; set; }
        public decimal DailyTruck { get; set; }
        public decimal DailyTruckTarget { get; set; }
        public decimal MTDDispatch { get; set; }
        public decimal MTDDispatchTarget { get; set; }
        public decimal MTDTruck { get; set; }
        public decimal MTDTruckTarget { get; set; }
    }

    public class UtilityComplianceVM
    {
        public string UtilityName { get; set; }
        public string Unit { get; set; }
        public decimal Actual { get; set; }
        public decimal Target { get; set; }
        public decimal VariancePercentage { get; set; }
        public string Status { get; set; }
    }

    public class DepartmentPerformanceVM
    {
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public decimal Score { get; set; }
        public string MetricLabel { get; set; }
        public bool IsBenchmarkBased { get; set; }
        public string PrimaryText { get; set; }
        public string SecondaryText { get; set; }
    }

    public class OperationalRiskVM
    {
        public int Rank { get; set; }
        public string RiskTitle { get; set; }
        public string RiskDetail { get; set; }
        public string RiskValue { get; set; }
        public decimal RiskScore { get; set; }
        public string Severity { get; set; }
    }

    public class ManagementActionVM
    {
        public string Priority { get; set; }
        public string Action { get; set; }
        public string Owner { get; set; }
        public DateTime? DueDate { get; set; }
        public string Plant { get; set; }
    }

    public class ExecutiveTrendVM
    {
        public DateTime MonthStart { get; set; }
        public string MonthLabel { get; set; }
        public decimal? ProductionScore { get; set; }
        public decimal? AvailabilityScore { get; set; }
        public decimal? MaintenanceScore { get; set; }
        public decimal? SupplyChainScore { get; set; }
    }

    public class DataValidationIssueVM
    {
        public int IssueID { get; set; }
        public string SourceName { get; set; }
        public string FieldName { get; set; }
        public string IssueMessage { get; set; }
        public int AffectedRows { get; set; }
        public string Severity { get; set; }
    }
}
