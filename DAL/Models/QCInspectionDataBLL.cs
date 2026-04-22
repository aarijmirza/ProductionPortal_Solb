using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class QCInspectionDataBLL
    {
        public int ID { get; set; }
        public DateTime? InspectionDate { get; set; }
        public string HeatNo { get; set; }
        public string CrossSection { get; set; }
        public string Lenght { get; set; }
        public string SteelGrade { get; set; }
        public string Shift { get; set; }
        public int BilletQty { get; set; }
        public int Mixed { get; set; }
        public int Rework { get; set; }
        public int AcceptedForSales { get; set; }
        public int RejectQty { get; set; }
        public int AcceptedForRolling { get; set; }
        public int OnHoldQty { get; set; }
        public string Remarks { get; set; }
        public int ScrapLength { get; set; }
        public int MRBNo { get; set; }
        public string ReasonForRejection { get; set; }
        public int RejectedByQC { get; set; }
        public int RejectedByProcess { get; set; }
        public int StatusID { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }
}
