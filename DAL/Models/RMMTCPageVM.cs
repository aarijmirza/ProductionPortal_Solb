
using System;
using System.Collections.Generic;

namespace DAL.Models
{
    public class RMMTCPageVM
    {
        public RMMTCHeaderBLL Header { get; set; }
        public List<RMMTCEligibleHeatBLL> EligibleHeats { get; set; }
        public List<RMMTCDetailBLL> SelectedHeats { get; set; }
        public List<RMMTCListRowBLL> MTCList { get; set; }

        public RMMTCPageVM()
        {
            Header = new RMMTCHeaderBLL();
            EligibleHeats = new List<RMMTCEligibleHeatBLL>();
            SelectedHeats = new List<RMMTCDetailBLL>();
            MTCList = new List<RMMTCListRowBLL>();
        }
    }

    public class RMMTCHeaderBLL
    {
        public int ID { get; set; }
        public string MTCNo { get; set; }
        public DateTime IssueDate { get; set; }
        public string CustomerName { get; set; }
        public string ShippingAddress { get; set; }
        public string DNNo { get; set; }
        public string CustomerNo { get; set; }
        public string SalesOrderNo { get; set; }
        public string CustomerOrderNo { get; set; }
        public bool IsWireRodMTC { get; set; }
        public bool IsFinal { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }

        public RMMTCHeaderBLL()
        {
            IssueDate = DateTime.Today;
        }
    }

    public class RMMTCEligibleHeatBLL
    {
        public int QCInspectionRMID { get; set; }
        public string HeatNo { get; set; }
        public string SteelGrade { get; set; }
        public string Specification { get; set; }
        public string ProductType { get; set; }
        public decimal? BarSize { get; set; }
        public int? NoOfBundles { get; set; }
        public bool IsWireRodOrCoil { get; set; }
        public decimal? YieldStrength { get; set; }
        public decimal? TensileStrength { get; set; }
        public decimal? TensileYieldRatio { get; set; }
    }

    public class RMMTCDetailBLL
    {
        public int ID { get; set; }
        public int MTCID { get; set; }
        public int QCInspectionRMID { get; set; }

        public string HeatNo { get; set; }
        public string SteelGrade { get; set; }
        public string Specification { get; set; }
        public string ProductType { get; set; }

        public decimal? BarSize { get; set; }
        public int? ActualNoOfBundles { get; set; }
        public int SortOrder { get; set; }

        public decimal? Length { get; set; }
        public decimal? WeightPerBundle { get; set; }
        public decimal? NominalWeight { get; set; }
        public decimal? CrossSectionArea { get; set; }
        public int? NoOfBarsPerBundle { get; set; }
        public int? NoOfBundles { get; set; }
        public bool BendTestObserved { get; set; }

        public decimal? YieldStrength { get; set; }
        public decimal? TensileStrength { get; set; }
        public decimal? TensileYieldRatio { get; set; }
        public decimal? Elongation { get; set; }
        public decimal? GaugeLength { get; set; }

        public decimal? C { get; set; }
        public decimal? Si { get; set; }
        public decimal? Mn { get; set; }
        public decimal? P { get; set; }
        public decimal? S { get; set; }
        public decimal? Cu { get; set; }
        public decimal? Ni { get; set; }
        public decimal? Cr { get; set; }
        public decimal? Mo { get; set; }
        public decimal? V { get; set; }
        public decimal? B { get; set; }
        public decimal? N { get; set; }
        public decimal? Ceq { get; set; }
    }

    public class RMMTCListRowBLL
    {
        public int ID { get; set; }
        public string MTCNo { get; set; }
        public DateTime IssueDate { get; set; }
        public string CustomerName { get; set; }
        public string CreatedBy { get; set; }
        public bool IsFinal { get; set; }
        public int HeatCount { get; set; }
    }
}
