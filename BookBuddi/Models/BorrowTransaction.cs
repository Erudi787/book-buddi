using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Transactions;
using BookBuddi.Models.Enums;
using TransactionStatus = BookBuddi.Models.Enums.TransactionStatus;

namespace BookBuddi.Models
{
    public class BorrowTransaction
    {
        [Key]
        public int TransactionId { get; set; }

        [Required]
        public int BookId { get; set; }

        [ForeignKey(nameof(BookId))]
        public virtual Book Book { get; set; } = null!;

        [Required]
        public int MemberId { get; set; }

        [ForeignKey(nameof(MemberId))]
        public virtual Member Member { get; set; } = null!;

        [Required]
        public DateTime BorrowDate { get; set; } = DateTime.Now;

        public DateTime? ReturnDate { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        [Required]
        public TransactionStatus Status { get; set; } = TransactionStatus.Active;

        [MaxLength(1000)]
        public string? Notes { get; set; }

        public virtual Fine? Fine { get; set; }
    }
}