using System.ComponentModel.DataAnnotations;
using BookBuddi.Resources.Constants;

namespace BookBuddi.Services.ServiceModels
{
    public class BookRequestViewModel
    {
        public int RequestId { get; set; }

        public int MemberId { get; set; }

        [Required(ErrorMessage = "Book title is required")]
        [StringLength(300, ErrorMessage = "Book title cannot exceed 300 characters")]
        public string BookTitle { get; set; } = string.Empty;

        [Required(ErrorMessage = "Author name is required")]
        [StringLength(200, ErrorMessage = "Author name cannot exceed 200 characters")]
        public string AuthorName { get; set; } = string.Empty;

        [StringLength(13, ErrorMessage = "ISBN cannot exceed 13 characters")]
        public string? ISBN { get; set; }

        public DateTime RequestDate { get; set; }

        public RequestStatus Status { get; set; } = RequestStatus.Pending;

        [StringLength(1000, ErrorMessage = "Admin notes cannot exceed 1000 characters")]
        public string? AdminNotes { get; set; }

        [StringLength(1000, ErrorMessage = "Member notes cannot exceed 1000 characters")]
        public string? MemberNotes { get; set; }

        public DateTime? StatusUpdateDate { get; set; }

        // Display properties
        public string? MemberName { get; set; }

        // Audit fields
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedTime { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
        public DateTime UpdatedTime { get; set; }
    }
}
