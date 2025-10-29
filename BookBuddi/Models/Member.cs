using System.ComponentModel.DataAnnotations;
using BookBuddi.Models.Enums;

namespace BookBuddi.Models
{
    public class Member
    {
        [Key]
        public int MemberId { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        public string FullName => $"{FirstName} {LastName}";

        [Required]
        [MaxLength(200)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string PasswordHash { get; set; } = string.Empty;

        [MaxLength(20)]
        [Phone]
        public string? Phone { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [Required]
        public DateTime MembershipDate { get; set; } = DateTime.Now;

        public DateTime MembershipExpiryDate { get; set; }

        [Required]
        public MemberStatus Status { get; set; } = MemberStatus.Active;

        [Required]
        public int BorrowingLimit { get; set; } = 5;

        public int CurrentBorrowedCount { get; set; } = 0;

        public virtual ICollection<BorrowTransaction> BorrowTransactions { get; set; } = new List<BorrowTransaction>();

        public virtual ICollection<BookRequest> BookRequests { get; set; } = new List<BookRequest>();

        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

        public virtual ICollection<Fine> Fines { get; set; } = new List<Fine>();
    }
}