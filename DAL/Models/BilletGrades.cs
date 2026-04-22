using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class BilletGrades
    {
        public int ProductID { get; set; }
        public string SpecGrade { get; set; }
        public string HeatNo { get; set; }
        public string SteelGrade { get; set; }
        public string Length { get; set; }
        public string Width { get; set; }
        public string BilletLength { get; set; }
        public int? StatusID { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }

    //    public List<BilletChemistryBLL> Chemistry = new List<BilletChemistryBLL>();
    }
}
