using System.ComponentModel.DataAnnotations;

namespace BookBuddi.Services.ServiceModels
{
    public class AuthorViewModel
    {
        public int AuthorId { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters")]
        public string LastName { get; set; } = string.Empty;

        public string FullName => $"{FirstName} {LastName}";

        [StringLength(2000, ErrorMessage = "Biography cannot exceed 2000 characters")]
        public string? Biography { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [StringLength(100, ErrorMessage = "Nationality cannot exceed 100 characters")]
        public string? Nationality { get; set; }

        // Audit fields
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedTime { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
        public DateTime UpdatedTime { get; set; }
    }
}
