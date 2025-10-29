using System.ComponentModel.DataAnnotations;
using BookBuddi.Resources.Constants;

namespace BookBuddi.Services.ServiceModels
{
    public class FineViewModel
    {
        public int FineId { get; set; }

        public int TransactionId { get; set; }

        public int MemberId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        public FineStatus Status { get; set; } = FineStatus.Unpaid;

        public FineReason Reason { get; set; }

        public DateTime IssueDate { get; set; }

        public DateTime? PaymentDate { get; set; }

        [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
        public string? Notes { get; set; }

        // Display properties
        public string? MemberName { get; set; }
        public string? BookTitle { get; set; }

        // Audit fields
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedTime { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
        public DateTime UpdatedTime { get; set; }
    }
}
