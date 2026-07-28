using System;

namespace DAL.Models
{
    public class CCMDailyProductionReportDetailBLL
    {
        public int ID { get; set; }

        public int ReportID { get; set; }

        public int SequenceNo { get; set; }

        public string HeatNo { get; set; }

        public string Grade { get; set; }

        public int Billet14M { get; set; }

        public int Billet13M { get; set; }

        public int Billet12M { get; set; }

        public int Billet11M { get; set; }

        public int Billet10M { get; set; }

        public int Billet09M { get; set; }

        public int Billet08M { get; set; }

        public int Billet07M { get; set; }

        public int Billet06M { get; set; }

        public int Billet05M { get; set; }

        public int Billet04M { get; set; }

        public int BilletBelow4M { get; set; }

        public int CropEndStart { get; set; }

        public int Bend { get; set; }

        public int GoodBillets { get; set; }

        public string Remarks { get; set; }

        public int StatusID { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public decimal? PrimeBilletWeight { get; set; }

        public decimal? ShortBilletWeight { get; set; }

        public decimal? TotalWeight { get; set; }

        public decimal? PerCoilBundleWeight { get; set; }

        public int TotalBillets
        {
            get
            {
                return
                    Billet14M +
                    Billet13M +
                    Billet12M +
                    Billet11M +
                    Billet10M +
                    Billet09M +
                    Billet08M +
                    Billet07M +
                    Billet06M +
                    Billet05M +
                    Billet04M +
                    BilletBelow4M +
                    CropEndStart +
                    Bend;
            }
        }

        public string UpdatedBy { get; set; }
    }
}