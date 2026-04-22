using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class ScrapyardBLL
    {
        public int ScrapID { get; set; }
        public DateTime? Date { get; set; }
        public TimeSpan? Time { get; set; }
        public string HeatNo { get; set; }
        public string Bucket { get; set; }
        public decimal? LightScrap { get; set; }
        public decimal? HMS { get; set; }
        public decimal? ReturnMetal { get; set; }
        public decimal? ReturnBar { get; set; }
        public decimal? MetalSkull { get; set; }
        public decimal? DRI { get; set; }
        public decimal? Coal { get; set; }
        public decimal? Lime { get; set; }
        public decimal? Dololime { get; set; }
        public int? StatusID { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
    public class ScrapyardInputModel
    {
        // --- Header Fields (From the top of the form) ---
        public DateTime? date { get; set; }
        public TimeSpan? time { get; set; }
        public string heatno { get; set; }
        // --- Dynamic Table Fields ---
        // The list of individual ScrapyardBLL records submitted from the table.
        public List<ScrapyardBLL> data { get; set; }
    }
}
