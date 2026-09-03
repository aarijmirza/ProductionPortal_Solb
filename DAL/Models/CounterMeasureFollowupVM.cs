using System;

namespace DAL.Models
{
    public class CounterMeasureFollowupVM
    {
        public int SrNo { get; set; }

        public int DelayID { get; set; }

        public DateTime? DelayDate { get; set; }

        public string Plant { get; set; }

        public string Agency { get; set; }

        public string EquipmentName { get; set; }

        public string DelayDescription { get; set; }

        public int CounterMeasureID { get; set; }

        public string Countermeasure { get; set; }

        public string SubOrderNumber { get; set; }

        public string Responsible { get; set; }

        public DateTime? TargetDate { get; set; }

        public string EvidenceAttachmentLink { get; set; }

        public string EvidenceFileName { get; set; }

        public string Status { get; set; }

        public string ReasonForNotClosing { get; set; }
    }
}
