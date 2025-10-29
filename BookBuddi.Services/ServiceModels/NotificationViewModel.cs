using System.ComponentModel.DataAnnotations;
using BookBuddi.Resources.Constants;

namespace BookBuddi.Services.ServiceModels
{
    public class NotificationViewModel
    {
        public int NotificationId { get; set; }

        public int MemberId { get; set; }

        public NotificationType Type { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Message is required")]
        [StringLength(1000, ErrorMessage = "Message cannot exceed 1000 characters")]
        public string Message { get; set; } = string.Empty;

        public DateTime DateCreated { get; set; }

        public DateTime? DateRead { get; set; }

        public bool IsRead { get; set; } = false;

        public int? RelatedEntityId { get; set; }

        [StringLength(50)]
        public string? RelatedEntityType { get; set; }

        // Audit fields
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedTime { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
        public DateTime UpdatedTime { get; set; }
    }
}
