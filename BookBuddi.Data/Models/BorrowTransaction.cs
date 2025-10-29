using BookBuddi.Resources.Constants;

namespace BookBuddi.Data.Models
{
    public partial class BorrowTransaction
    {
        public int TransactionId { get; set; }
        public int BookId { get; set; }
        public int MemberId { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public DateTime DueDate { get; set; }
        public TransactionStatus Status { get; set; } = TransactionStatus.Active;
        public string? Notes { get; set; }

        // Audit Trail
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedTime { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
        public DateTime UpdatedTime { get; set; }
    }
}
