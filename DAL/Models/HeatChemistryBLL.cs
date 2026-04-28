using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class HeatChemistryBLL // Or whatever you call your final data class
    {
        // Primary Key / Identity
        public int ChemistryID { get; set; }

        // Common Heat/Run Data (These will be the same for all samples of one heat)
        public string PlantName { get; set; }
        public DateTime? Date { get; set; }
        public string HeatNo { get; set; }
        public string SteelGrade { get; set; }
        public decimal? Lenght { get; set; }
        public decimal? CrossSection { get; set; }
        public decimal? Weight { get; set; }
        public string Area { get; set; }
        public string Size { get; set; }
        public TimeSpan? Time { get; set; }
        public string Shift { get; set; }

        // Sample Specific Data
        public string NoOfBillets { get; set; }
        public string SampleNo { get; set; }
        public decimal? C { get; set; }
        public decimal? Si { get; set; }
        public decimal? Mn { get; set; }
        public decimal? P { get; set; }
        public decimal? S { get; set; }
        public decimal? Ni { get; set; }
        public decimal? Cr { get; set; }
        public decimal? Mo { get; set; }
        public decimal? V { get; set; }
        public decimal? Cu { get; set; }
        public decimal? Ti { get; set; }
        public decimal? Sn { get; set; }
        public decimal? Al { get; set; }
        public decimal? Pb { get; set; }
        public decimal? B { get; set; }
        public decimal? Zn { get; set; }
        public decimal? N { get; set; }
        public decimal? MnS { get; set; }
        public decimal? Ceq { get; set; }

        // Audit/Status Data
        public int? StatusID { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }
    // This model binds the inputs in the dynamic table rows (e.g., data[0].C)
    public class HeatChemistrySampleInput
    {
        public string SampleNo { get; set; }
        public decimal? C { get; set; }
        public decimal? Si { get; set; }
        public decimal? Mn { get; set; }
        public decimal? P { get; set; }
        public decimal? S { get; set; }
        public decimal? Ni { get; set; }
        public decimal? Cr { get; set; }
        public decimal? Mo { get; set; }
        public decimal? V { get; set; }
        public decimal? Cu { get; set; }
        public decimal? Ti { get; set; }
        public decimal? Sn { get; set; }
        public decimal? Al { get; set; }
        public decimal? Pb { get; set; }
        public decimal? B { get; set; }
        public decimal? Zn { get; set; }
        public decimal? N { get; set; }
        public decimal? MnS { get; set; }
        public decimal? Ceq { get; set; }
    }

    public class ChemistryInputModel
    {
        // Common Heat/Run Data (Bind from the single set of inputs)
        public string PlantName { get; set; } // bound by the radio buttons
        public DateTime? Date { get; set; }
        public string HeatNo { get; set; }
        public string Grade { get; set; }
        public decimal? Weight { get; set; }
        public string Area { get; set; }
        public string Size { get; set; }
        public TimeSpan? Time { get; set; }
        public string Shift { get; set; }

        // Collection property (MUST be named 'data' to match your controller's iteration loop)
        public List<HeatChemistrySampleInput> data { get; set; } = new List<HeatChemistrySampleInput>();
    }
}
