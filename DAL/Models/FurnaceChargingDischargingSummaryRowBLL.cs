using System;

namespace DAL.Models
{
    public class FurnaceChargingDischargingSummaryRowBLL
    {
        public DateTime? Date { get; set; }
        public string Plant { get; set; }
        public string Shift { get; set; }

        // Physical Check
        public string BilletBoardingNo { get; set; }
        public string PhysicalHeatNo { get; set; }
        public decimal BoardingBillets { get; set; }
        public decimal ActualBillets { get; set; }

        // Charged
        public string ChargedHeatNo { get; set; }
        public string ChargedGrade { get; set; }
        public string ChargedBilletSize { get; set; }
        public decimal ChargedTotalPcs { get; set; }

        // Discharged
        public string DischargedHeatNo { get; set; }
        public string DischargedGrade { get; set; }
        public string DischargedBilletSize { get; set; }
        public decimal DischargedTotalPcs { get; set; }

        // Balance in Furnace
        public string BalanceHeatNo { get; set; }
        public string BalanceGrade { get; set; }
        public string BalanceBilletSize { get; set; }
        public decimal BalanceTotalPcs { get; set; }
    }
}
