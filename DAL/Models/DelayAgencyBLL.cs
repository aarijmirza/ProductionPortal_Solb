using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class DelayAgencyBLL
    {
        public int AgencyID { get; set; }
        public string AgencyName { get; set; }
        public string AgencyCode { get; set; }
        public string Department { get; set; }
        public string AgencyType { get; set; }
        public int? StatusID { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
