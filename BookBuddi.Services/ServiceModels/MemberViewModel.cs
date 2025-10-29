using System.ComponentModel.DataAnnotations;
using BookBuddi.Resources.Constants;

namespace BookBuddi.Services.ServiceModels
{
    public class MemberViewModel
    {
        public int MemberId { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters")]
        public string LastName { get; set; } = string.Empty;

        public string FullName => $"{FirstName} {LastName}";

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(200, ErrorMessage = "Email cannot exceed 200 characters")]
        public string Email { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "Phone cannot exceed 20 characters")]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string? Phone { get; set; }

        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
        public string? Address { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public DateTime MembershipDate { get; set; }

        public DateTime MembershipExpiryDate { get; set; }

        public MemberStatus Status { get; set; } = MemberStatus.Active;

        [Range(1, 10, ErrorMessage = "Borrowing limit must be between 1 and 10")]
        public int BorrowingLimit { get; set; } = 5;

        public int CurrentBorrowedCount { get; set; } = 0;

        // Audit fields
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedTime { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
        public DateTime UpdatedTime { get; set; }
    }
}
