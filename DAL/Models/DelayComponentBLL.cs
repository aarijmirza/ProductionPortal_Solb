using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class DelayComponentBLL
    {
        public int ComponentID { get; set; }

        public string Code { get; set; }

        public string ComponentGroup { get; set; }

        public string Description { get; set; }

        public int? StatusID { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string CreatedBy { get; set; }

    }
}
