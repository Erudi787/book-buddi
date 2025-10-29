using BookBuddi.Resources.Constants;

namespace BookBuddi.Data.Models
{
    public partial class Fine
    {
        public int FineId { get; set; }
        public int TransactionId { get; set; }
        public int MemberId { get; set; }
        public decimal Amount { get; set; }
        public FineStatus Status { get; set; } = FineStatus.Unpaid;
        public FineReason Reason { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string? Notes { get; set; }

        // Audit Trail
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedTime { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
        public DateTime UpdatedTime { get; set; }
    }
}
