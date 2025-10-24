using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BookBuddi.Models.Enums;

namespace BookBuddi.Models
{
    public class Fine
    {
        [Key]
        public int FineId { get; set; }

        [Required]
        public int TransactionId { get; set; }

        [ForeignKey(nameof(TransactionId))]
        public virtual BorrowTransaction BorrowTransaction { get; set; } = null!;

        [Required]
        public int MemberId { get; set; }

        [ForeignKey(nameof(MemberId))]
        public virtual Member Member { get; set; } = null!;

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Amount { get; set; }

        [Required]
        public FineStatus Status { get; set; } = FineStatus.Unpaid;

        [Required]
        public FineReason Reason { get; set; }

        [Required]
        public DateTime IssueDate { get; set; } = DateTime.Now;

        public DateTime? PaymentDate { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }
    }
}