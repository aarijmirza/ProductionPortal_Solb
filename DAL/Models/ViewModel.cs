using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class ViewModel
    {
        public class Rsp
        {
            public string description { get; set; }
            public int status { get; set; }
        }
        public class GradeBLL
        {
            public string Plant_ID { get; set; }
            public string GRADE_ID { get; set; }
            public GradeBLL Grade { get; set; }
        }
        public class DelaySectionBLL
        {
            public int GROUP_ORD { get; set; }
            public string GROUP_NAME { get; set; }
        }
        public class EAFMeltShopVM
        {
            public string GRADE_ID { get; set; }
            public GradeBLL Grade { get; set; }
            public DelaySectionBLL DelaySection { get; set; }

            public List<GradeBLL> Grades { get; set; }
            public List<DelaySectionBLL> DelaySections { get; set; }

            public EAFMeltShopVM()
            {
                Grades = new List<GradeBLL>();
                DelaySections = new List<DelaySectionBLL>();
            }
        }
        public class LFMeltShopVM
        {
            public string GRADE_ID { get; set; }
            public GradeBLL Grade { get; set; }
            public DelaySectionBLL DelaySection { get; set; }

            public List<GradeBLL> Grades { get; set; }
            public List<DelaySectionBLL> DelaySections { get; set; }

            public LFMeltShopVM()
            {
                Grades = new List<GradeBLL>();
                DelaySections = new List<DelaySectionBLL>();
            }

        }
        public class ShiftProductionReportVM
        {
            public List<PlantDelayBLL> Delays { get; set; }
            public List<BilletDischargingBLL> DischargedHeats { get; set; }

            public ShiftProductionReportVM()
            {
                Delays = new List<PlantDelayBLL>();
                DischargedHeats = new List<BilletDischargingBLL>();
            }
        }

        public class EAFReportVM
        {
            public List<ElectricArcFurnaceBLL> EAFdata { get; set; }
                = new List<ElectricArcFurnaceBLL>();

            // 🔹 ADD THESE
            public DateTime FromDate { get; set; }
            public DateTime ToDate { get; set; }

            public string Shift { get; set; }
            public string Group { get; set; }
        }

        public class LFReportVM
        {
            public List<LaddleFurnaceBLL> LFdata { get; set; }
                = new List<LaddleFurnaceBLL>();

            // 🔹 ADD THESE
            public DateTime FromDate { get; set; }
            public DateTime ToDate { get; set; }

            public string Shift { get; set; }
            public string Group { get; set; }
        }

        public class CCMMeltShopVM
        {
            public string HeatNo { get; set; }
            // ✅ Single record for Add/Edit
            public CCMBLL Master { get; set; } = new CCMBLL();

            // ✅ Optional: list for grid / history
            public List<CCMBLL> CCM { get; set; }

            public List<LaddleFurnaceBLL> LaddleFurnaces { get; set; }

            public CCMMeltShopVM()
            {
                CCM = new List<CCMBLL>();
                LaddleFurnaces = new List<LaddleFurnaceBLL>();
            }
        }

        public class RollingMillChargeVM
        {

            public DateTime? Date { get; set; }
            public string HeatNo { get; set; }
            public string Shift { get; set; }
            public string Plant { get; set; }
            public string Team { get; set; }
            public string ShiftIncharge { get; set; }
            public BilletDischargingBLL Form { get; set; }
            public List<BundlingSectionBLL> SubmittedHeat { get; set; }

            public RollingMillChargeVM()
            {
                Form = new BilletDischargingBLL();
                SubmittedHeat = new List<BundlingSectionBLL>();
            }

        }

        public class RMChargingVM
        {
            public DateTime? Date { get; set; }
            public string HeatNo { get; set; }
            public string Shift { get; set; }
            public string Plant { get; set; }
            public string Team { get; set; }
            public string ShiftIncharge { get; set; }
            public BilletBoardBLL Form { get; set; }
            public List<BilletChargingBLL> SubmittedHeat { get; set; }

            public RMChargingVM()
            {
                Form = new BilletBoardBLL();
                SubmittedHeat = new List<BilletChargingBLL>();
            }

        }
        public class RMDischargingVM
        {
            public DateTime? Date { get; set; }
            public string HeatNo { get; set; }
            public string Shift { get; set; }
            public string Plant { get; set; }
            public string Team { get; set; }
            public string ShiftIncharge { get; set; }
            public BilletChargingBLL Form { get; set; }
            public List<BilletDischargingBLL> SubmittedHeat { get; set; }

            public RMDischargingVM()
            {
                Form = new BilletChargingBLL();
                SubmittedHeat = new List<BilletDischargingBLL>();
            }

        }

        public class SlagByProductPDFVM
        {
            public List<SlagByProductAnalysisBLL> SlagData { get; set; }
                = new List<SlagByProductAnalysisBLL>();

            public List<SlagSampleAnalysisBLL> Samples { get; set; }
                = new List<SlagSampleAnalysisBLL>();

            public DateTime FromDate { get; set; }
            public DateTime ToDate { get; set; }
        }

        public class HBIDRIAnalysisPDFVM
        {
            public List<QCHBIDRIAnalysisBLL> HBIDRIData { get; set; }
                = new List<QCHBIDRIAnalysisBLL>();

            public List<SampleHBIDRIBLL> Samples { get; set; }
                = new List<SampleHBIDRIBLL>();

            public DateTime FromDate { get; set; }
            public DateTime ToDate { get; set; }
        }

        public class BilletBoardingPDFVM
        {
            public List<BilletBoardBLL> BilletBoards { get; set; }
                = new List<BilletBoardBLL>();

            public List<HeatChemistryBLL> Samples { get; set; }
                = new List<HeatChemistryBLL>();

            public DateTime FromDate { get; set; }
            public DateTime ToDate { get; set; }
        }

        public class RMHourlyDischargeVM
        {
            public DateTime? Date { get; set; }
            public string HeatNo { get; set; }
            public string Shift { get; set; }
            public string Plant { get; set; }
            public string Team { get; set; }
            public string ShiftIncharge { get; set; }

            public string TimeFrom { get; set; }

            public string TimeTo { get; set; }

            public string NoofBillets { get; set; }

            public decimal? NoofCobble { get; set; }

            public int? Reject { get; set; }

            public string BilletHeatNo { get; set; }

            public string SafetyIssueShift { get; set; }

            public string MessageShift { get; set; }

            public string FuelConsumptionStart { get; set; }

            public string FuelConsumptionEnd { get; set; }

            public string TotalConsumption { get; set; }

            public string ElectricityConsumption { get; set; }

            public int? StatusID { get; set; }

            public string CreatedBy { get; set; }

            public DateTime? CreatedDate { get; set; }

            public string UpdatedBy { get; set; }

            public DateTime? UpdatedDate { get; set; }

            public BilletDischargingBLL Form { get; set; }
            public List<RMHourlyDischargeBLL> RMHourlyDischarge { get; set; }

            public RMHourlyDischargeVM()
            {
                Form = new BilletDischargingBLL();
                RMHourlyDischarge = new List<RMHourlyDischargeBLL>();
            }
        }
        public class RMShiftDetailsVM
        {
            public RMShiftDetailsBLL Form { get; set; }
            public List<RMShiftDetailsBLL> List { get; set; }

            public RMShiftDetailsVM()
            {
                Form = new RMShiftDetailsBLL();
                List = new List<RMShiftDetailsBLL>();
            }
        }
    }
    public class SupplyChainStockVM
    {
        public int ID { get; set; }
        public DateTime? ReportDate { get; set; }

        public List<SupplyChainStockHeaderBLL> HeaderList { get; set; }
        public List<DispatchDetailBLL> DispatchDetails { get; set; }
        public List<RebarStockBLL> RebarStocks { get; set; }
        public List<WireRodStockBLL> WireRodStocks { get; set; }
        public List<BilletStockBLL> BilletStocks { get; set; }
        public List<RawMaterialStockBLL> RawMaterialStocks { get; set; }

        public SupplyChainStockVM()
        {
            HeaderList = new List<SupplyChainStockHeaderBLL>();
            DispatchDetails = new List<DispatchDetailBLL>();
            RebarStocks = new List<RebarStockBLL>();
            WireRodStocks = new List<WireRodStockBLL>();
            BilletStocks = new List<BilletStockBLL>();
            RawMaterialStocks = new List<RawMaterialStockBLL>();

        }
    }

    public class SupplyChainStockHeaderBLL
    {
        public int ID { get; set; }
        public DateTime? ReportDate { get; set; }
        public int StatusID { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }

    public class DispatchDetailBLL
    {
        public int ID { get; set; }
        public int HeaderID { get; set; }

        public string Material { get; set; }
        public decimal Trucks { get; set; }
        public decimal Tons { get; set; }
        public decimal MTD { get; set; }
    }

    public class RebarStockBLL
    {
        public int ID { get; set; }
        public int HeaderID { get; set; }

        public string Size { get; set; }
        public decimal Prime { get; set; }
        public decimal Discolored { get; set; }
        public decimal Epoxy { get; set; }
        public decimal ShortBars { get; set; }
    }

    public class WireRodStockBLL
    {
        public int ID { get; set; }
        public int HeaderID { get; set; }

        public string Size { get; set; }
        public string Grade { get; set; }
        public decimal Prime { get; set; }
    }

    public class BilletStockBLL
    {
        public int ID { get; set; }
        public int HeaderID { get; set; }

        public string Grade { get; set; }
        public decimal QtyTon { get; set; }
    }

    public class RawMaterialStockBLL
    {
        public int ID { get; set; }
        public int HeaderID { get; set; }

        public string MaterialDescription { get; set; }
        public decimal QtyTon { get; set; }

        // Raw Material Stock / Sub Raw Material Stock
        public string StockCategory { get; set; }
    }
    public class UtilityDailyReportVM
    {
        public DateTime ReportDate { get; set; }

        public PlantConsumptionBLL SMP { get; set; }

        public PlantConsumptionBLL RM1 { get; set; }

        public PlantConsumptionBLL RM2 { get; set; }

        public List<PlantConsumptionBLL> Records { get; set; }

        public UtilityDailyReportVM()
        {
            ReportDate = DateTime.Today;
            SMP = new PlantConsumptionBLL { Plant = "SMP" };
            RM1 = new PlantConsumptionBLL { Plant = "RM1" };
            RM2 = new PlantConsumptionBLL { Plant = "RM2" };
            Records = new List<PlantConsumptionBLL>();
        }
    }
}
