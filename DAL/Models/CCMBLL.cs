using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DAL.Models.ViewModel;

namespace DAL.Models
{
    public class CCMBLL
    {
        public int ID { get; set; }

        public DateTime Date { get; set; }

        public string Shift { get; set; }

        public string GRADE_ID { get; set; }

        public string Grade { get; set; }

        public string HeatNo { get; set; }

        public decimal? SequenceHeat { get; set; }

        public decimal? LadleNo { get; set; }
        public decimal? LadleOpen { get; set; }

        public decimal? LadleTemperature { get; set; }

        public decimal? MoltenSteel { get; set; }

        public decimal? TimeOnTurret { get; set; }

        public decimal? PlateLife { get; set; }

        public TimeSpan? OpenTime { get; set; }

        public TimeSpan? CloseTime { get; set; }

        public decimal? TundishNo { get; set; }

        public decimal? TundishLife { get; set; }

        public decimal? ShroudLife { get; set; }

        public decimal? SteelTemperature1 { get; set; }

        public decimal? SteelTemperature2 { get; set; }

        public decimal? SteelTemperature3 { get; set; }

        public decimal? SteelTemperature4 { get; set; }

        public string Strand1 { get; set; }
        public string Strand2 { get; set; }
        public string Strand3 { get; set; }
        public string Strand4 { get; set; }
        public string Strand5 { get; set; }

        public decimal? BilletSize { get; set; }

        public decimal? BilletLength { get; set; }

        public decimal? HotCharging { get; set; }

        public decimal? TOCB { get; set; }

        public decimal? Yield { get; set; }

        public decimal? CastingTime { get; set; }

        public decimal? BilletNumber { get; set; }

        public decimal? BilletTotalWeight { get; set; }

        public decimal? Productivitytonhr { get; set; }

        public decimal? Yeild { get; set; }

        public decimal? LF_C { get; set; }

        public decimal? LF_Si { get; set; }

        public decimal? LF_Mn { get; set; }

        public decimal? LF_S { get; set; }

        public decimal? LF_TE { get; set; }

        public decimal? LF_MnSi { get; set; }

        public decimal? LF_MnS { get; set; }

        public int? StatusID { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public List<CCMChemicalAnalysisBLL> Analysis { get; set; }

        public List<GradeBLL> Grades { get; set; }

        public GradeBLL Gradess { get; set; }

        public decimal? LadleWeightStart { get; set; }
        public decimal? LadleWeightEnd { get; set; }

        public decimal? Prime1Length { get; set; }
        public decimal? Prime1Pcs { get; set; }

        public decimal? Prime2Length { get; set; }
        public decimal? Prime2Pcs { get; set; }

        public decimal? VarLengthPcs { get; set; }
        public decimal? EndCutPcs { get; set; }

        public decimal? Prime1Wt { get; set; }
        public decimal? Prime2Wt { get; set; }
        public decimal? VLenWt { get; set; }
        public decimal? EndWt { get; set; }

    }


}
