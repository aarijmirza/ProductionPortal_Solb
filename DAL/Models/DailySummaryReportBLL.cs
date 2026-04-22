using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class DailySummaryReportBLL
    {
        public DateTime Date { get; set; }
        public string Heats { get; set; }
        public string DRI { get; set; }
        public string Scrap { get; set; }
        public string LiquidSteel { get; set; }
        public string CastedWeight { get; set; }
        public string TapToTap { get; set; }

        public string AvailabilityT { get; set; }
        public string PerformanceT { get; set; }
        public string YieldT { get; set; }
        public string QualityYieldT { get; set; }
        public string EAFTPH_T { get; set; }
        public string CCMTPH_T { get; set; }

        public string AvailabilityA { get; set; }
        public string PerformanceA { get; set; }
        public string YieldA { get; set; }
        public string QualityYieldA { get; set; }
        public string EAFTPH_A { get; set; }
        public string CCMTPH_A { get; set; }

        public string DRIT { get; set; }
        public string ScrapT { get; set; }
        public string FeSiT { get; set; }
        public string FeSiMnT { get; set; }
        public string FeMnT { get; set; }
        public string RiceT { get; set; }

        public string DRIA { get; set; }
        public string ScrapA { get; set; }
        public string FeSiA { get; set; }
        public string FeSiMnA { get; set; }
        public string FeMnA { get; set; }
        public string RiceA { get; set; }
    }
}
