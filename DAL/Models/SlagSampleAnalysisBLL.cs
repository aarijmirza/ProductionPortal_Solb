using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class SlagSampleAnalysisBLL
    {
        public int ID { get; set; }

        public int SlagID { get; set; }

        public string SampleCode { get; set; }

        public TimeSpan? SampleTime { get; set; }

        public decimal? CaO { get; set; }

        public decimal? MgO { get; set; }

        public decimal? SiO2 { get; set; }

        public decimal? Al2O3 { get; set; }

        public decimal? Fe2O3 { get; set; }

        public decimal? S { get; set; }

        public decimal? MnO { get; set; }

        public decimal? Cr2O3 { get; set; }

        public decimal? P2O5 { get; set; }

        public decimal? V2O5 { get; set; }

        public decimal? TiO2 { get; set; }

        public decimal? ZnO { get; set; }

        public decimal? TotalFe { get; set; }

        public decimal? Basicity4 { get; set; }

        public string Comment { get; set; }

        public int? StatusID { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string CreatedBy { get; set; }

    }
}
