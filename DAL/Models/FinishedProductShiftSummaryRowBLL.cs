using System;

namespace DAL.Models
{
    public class FinishedProductShiftSummaryRowBLL
    {
        public DateTime? Date { get; set; }
        public string Plant { get; set; }
        public string Shift { get; set; }

        public string HeatNo { get; set; }
        public string Size { get; set; }

        public decimal BilletsRolled { get; set; }
        public decimal RolledBilletWeight { get; set; }
        public decimal FinishedProductWeight { get; set; }
        public decimal YieldPercentage { get; set; }

        public int PrimeBundles { get; set; }

        public string BundleCoilSerialNos { get; set; }
        public int HoldRejectedBundles { get; set; }
        public string HoldRejectedReason { get; set; }
        public string PrimeLocation { get; set; }
        public string VerifiedByQC { get; set; }
        public string VerifiedByShipping { get; set; }
        public string Remarks { get; set; }
    }
}
