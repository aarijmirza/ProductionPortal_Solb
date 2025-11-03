using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class ViewModel
    {
        public class Rsp
        {
            public string description { get; set; }
            public int status { get; set; }
        }
        public class GradeBLL
        {
            public string Plant_ID { get; set; }
            public string GRADE_ID { get; set; }
            public GradeBLL Grade { get; set; }
        }
        public class DelaySectionBLL
        {
            public int GROUP_ORD { get; set; }
            public string GROUP_NAME { get; set; }
        }
        public class EAFMeltShopVM
        {
            public string GRADE_ID { get; set; }
            public GradeBLL Grade { get; set; }
            public DelaySectionBLL DelaySection { get; set; }

            public List<GradeBLL> Grades { get; set; }
            public List<DelaySectionBLL> DelaySections { get; set; }

            public EAFMeltShopVM()
            {
                Grades = new List<GradeBLL>();
                DelaySections = new List<DelaySectionBLL>();
            }
        }
        public class LFMeltShopVM
        {
            public string GRADE_ID { get; set; }
            public GradeBLL Grade { get; set; }
            public DelaySectionBLL DelaySection { get; set; }

            public List<GradeBLL> Grades { get; set; }
            public List<DelaySectionBLL> DelaySections { get; set; }

            public LFMeltShopVM()
            {
                Grades = new List<GradeBLL>();
                DelaySections = new List<DelaySectionBLL>();
            }
        }
    }
}
