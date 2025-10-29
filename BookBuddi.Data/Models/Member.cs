using BookBuddi.Resources.Constants;

namespace BookBuddi.Data.Models
{
    public partial class Member
    {
        public int MemberId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime MembershipDate { get; set; }
        public DateTime MembershipExpiryDate { get; set; }
        public MemberStatus Status { get; set; } = MemberStatus.Active;
        public int BorrowingLimit { get; set; } = 5;
        public int CurrentBorrowedCount { get; set; } = 0;

        // Audit Trail
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedTime { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
        public DateTime UpdatedTime { get; set; }
    }
}
