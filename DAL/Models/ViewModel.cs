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
            public string HeatNo { get; set; }
            public BilletChargingBLL Form { get; set; }
            public List<BundlingSectionBLL> SubmittedHeat { get; set; }
        
            public RollingMillChargeVM()
            {
                Form = new BilletChargingBLL();
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
    }
}
