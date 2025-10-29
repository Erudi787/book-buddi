using BookBuddi.Resources.Constants;

namespace BookBuddi.Data.Models
{
    public partial class Notification
    {
        public int NotificationId { get; set; }
        public int MemberId { get; set; }
        public NotificationType Type { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; }
        public DateTime? DateRead { get; set; }
        public bool IsRead { get; set; } = false;
        public int? RelatedEntityId { get; set; }
        public string? RelatedEntityType { get; set; }

        // Audit Trail
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedTime { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
        public DateTime UpdatedTime { get; set; }
    }
}
