using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class BilletBoardBLL
    {
        public int ID { get; set; }
        public DateTime? Date { get; set; }
        public string HeatNo { get; set; }
        public string BilletBoarding { get; set; }
        public string PlantName { get; set; }
        public string Shift { get; set; }
        public string ProductSpecs { get; set; }
        public string SteelGrade { get; set; }
        public string Grade { get; set; }
        public string BilletLength { get; set; }
        public string CrossSection { get; set; }
        public decimal? BilletWeight { get; set; }
        public string Size { get; set; }
        public string Profile { get; set; }
        public string Remarks { get; set; }
        public int? NoOfBillets { get; set; }
        public int? HeatStatus { get; set; }
        public int? StatusID { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public List<RMChemicalAnalysisBLL> Chemistry { get; set; } = new List<RMChemicalAnalysisBLL>();
        public List<BilletGrades> Grades = new List<BilletGrades>();
    }
}
