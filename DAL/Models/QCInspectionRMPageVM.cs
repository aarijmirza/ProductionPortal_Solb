using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class QCInspectionRMPageVM
    {
        public string SelectedRM { get; set; }

        public List<QCBilletBoardingRowBLL>
            BilletBoardingRows
        { get; set; }

        public List<QCMTCRowBLL>
            MTCRows
        { get; set; }

        public QCInspectionRMDetailBLL
            Detail
        { get; set; }

        public QCInspectionRMPageVM()
        {
            BilletBoardingRows =
                new List<QCBilletBoardingRowBLL>();

            MTCRows =
                new List<QCMTCRowBLL>();

            Detail =
                new QCInspectionRMDetailBLL();
        }
    }


    public class QCBilletBoardingRowBLL
    {
        public int ID { get; set; }
        public string Site { get; set; }
        public string BoardingNo { get; set; }
        public int SerialNo { get; set; }
        public string HeatNo { get; set; }
        public string SteelGrade { get; set; }
        public string BarSize { get; set; }
        public int BarsPerBundle { get; set; }
        public int ActualBundleCount { get; set; }
        public string YardInspection { get; set; }
        public string YardInspectionRemarks { get; set; }
    }


    public class QCMTCRowBLL
    {
        public int ID { get; set; }
        public string HeatNo { get; set; }
        public string SteelGrade { get; set; }
        public decimal BarSize { get; set; }
        public decimal YieldStress { get; set; }
        public decimal TensileStress { get; set; }
        public int NoOfBundles { get; set; }
        public decimal YSTSRatio { get; set; }
    }


    public class QCMTCDetailBLL
    {
        public string HeatNo { get; set; }
        public decimal YieldStrength { get; set; }
        public decimal TensileStrength { get; set; }
        public decimal TensileYieldRatio { get; set; }
        public decimal Elongation { get; set; }
        public decimal GaugeLength { get; set; }
        public decimal C { get; set; }
        public decimal Si { get; set; }
        public decimal Mn { get; set; }
        public decimal P { get; set; }
        public decimal S { get; set; }
        public decimal N { get; set; }
        public decimal Ceq { get; set; }
    }


    public class QCInspectionRMDetailBLL
    {
        public int ID { get; set; }
        public int BilletBoardingID { get; set; }
        public int MTCID { get; set; }

        public string Site { get; set; }
        public string ProductionShift { get; set; }
        public DateTime? ProductionDateValue { get; set; }

        public string HeatNo { get; set; }
        public string Specification { get; set; }
        public string SteelGrade { get; set; }

        public decimal? LengthValue { get; set; }
        public decimal? NominalWeightValue { get; set; }
        public decimal? CrossSectionAreaValue { get; set; }

        public bool BendTestObserved { get; set; }
        public decimal? BarSizeValue { get; set; }
        public decimal? WeightPerBundleValue { get; set; }
        public int? NoOfBarsPerBundleValue { get; set; }
        public int? NoOfBundlesValue { get; set; }
        public bool IsWireRodOrCoil { get; set; }

        public decimal? YieldStrengthValue { get; set; }
        public decimal? TensileStrengthValue { get; set; }
        public decimal? TensileYieldRatioValue { get; set; }
        public decimal? ElongationValue { get; set; }
        public decimal? GaugeLengthValue { get; set; }

        public decimal? CValue { get; set; }
        public decimal? SiValue { get; set; }
        public decimal? MnValue { get; set; }
        public decimal? PValue { get; set; }
        public decimal? SValue { get; set; }
        public decimal? NValue { get; set; }
        public decimal? CeqValue { get; set; }

        public int StatusID { get; set; }
        public string RollingMill { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }

        // Display-compatible aliases used by the current View.
        public string ProductionDate
        {
            get
            {
                return ProductionDateValue.HasValue
                    ? ProductionDateValue.Value.ToString(
                        "dd-MM-yyyy"
                    )
                    : "";
            }
            set
            {
                DateTime parsed;

                if (
                    DateTime.TryParse(
                        value,
                        out parsed
                    )
                )
                {
                    ProductionDateValue =
                        parsed;
                }
            }
        }

        public string DatabaseServer { get; set; }

        public string Length
        {
            get { return Format(LengthValue); }
            set { LengthValue = ParseDecimal(value); }
        }

        public string NominalWeight
        {
            get { return Format(NominalWeightValue); }
            set { NominalWeightValue = ParseDecimal(value); }
        }

        public string CrossSectionArea
        {
            get { return Format(CrossSectionAreaValue); }
            set { CrossSectionAreaValue = ParseDecimal(value); }
        }

        public string BarSize
        {
            get { return Format(BarSizeValue); }
            set { BarSizeValue = ParseDecimal(value); }
        }

        public string WeightPerBundle
        {
            get { return Format(WeightPerBundleValue); }
            set { WeightPerBundleValue = ParseDecimal(value); }
        }

        public string NoOfBarsPerBundle
        {
            get
            {
                return NoOfBarsPerBundleValue.HasValue
                    ? NoOfBarsPerBundleValue.Value.ToString()
                    : "";
            }
            set
            {
                int parsed;

                NoOfBarsPerBundleValue =
                    int.TryParse(value, out parsed)
                        ? (int?)parsed
                        : null;
            }
        }

        public string NoOfBundles
        {
            get
            {
                return NoOfBundlesValue.HasValue
                    ? NoOfBundlesValue.Value.ToString()
                    : "";
            }
            set
            {
                int parsed;

                NoOfBundlesValue =
                    int.TryParse(value, out parsed)
                        ? (int?)parsed
                        : null;
            }
        }

        public string YieldStrength
        {
            get { return Format(YieldStrengthValue); }
            set { YieldStrengthValue = ParseDecimal(value); }
        }

        public string TensileStrength
        {
            get { return Format(TensileStrengthValue); }
            set { TensileStrengthValue = ParseDecimal(value); }
        }

        public string TensileYieldRatio
        {
            get { return Format(TensileYieldRatioValue); }
            set { TensileYieldRatioValue = ParseDecimal(value); }
        }

        public string Elongation
        {
            get { return Format(ElongationValue); }
            set { ElongationValue = ParseDecimal(value); }
        }

        public string GaugeLength
        {
            get { return Format(GaugeLengthValue); }
            set { GaugeLengthValue = ParseDecimal(value); }
        }

        public string C
        {
            get { return Format(CValue); }
            set { CValue = ParseDecimal(value); }
        }

        public string Si
        {
            get { return Format(SiValue); }
            set { SiValue = ParseDecimal(value); }
        }

        public string Mn
        {
            get { return Format(MnValue); }
            set { MnValue = ParseDecimal(value); }
        }

        public string P
        {
            get { return Format(PValue); }
            set { PValue = ParseDecimal(value); }
        }

        public string S
        {
            get { return Format(SValue); }
            set { SValue = ParseDecimal(value); }
        }

        public string N
        {
            get { return Format(NValue); }
            set { NValue = ParseDecimal(value); }
        }

        public string Ceq
        {
            get { return Format(CeqValue); }
            set { CeqValue = ParseDecimal(value); }
        }


        private static string Format(
            decimal? value)
        {
            return value.HasValue
                ? value.Value.ToString(
                    "0.###",
                    CultureInfo.InvariantCulture
                )
                : "";
        }


        private static decimal? ParseDecimal(
            string value)
        {
            decimal parsed;

            return decimal.TryParse(
                value,
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out parsed
            )
                ? (decimal?)parsed
                : null;
        }
    }
}