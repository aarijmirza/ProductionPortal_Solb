using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DAL.Models.ViewModel;

namespace DAL.Models
{
    public class CCMYeildBLL
    {
        public int ID { get; set; }

        public DateTime Date { get; set; }

        public string HeatNo { get; set; }

        public decimal? TundishSkull { get; set; }

        public decimal? ProcessRejectedBillet { get; set; }

        public decimal? ShortBillet6m { get; set; }

        public decimal? HeadTail { get; set; }

        public string Comment { get; set; }

        public int? StatusID { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

    }
}
