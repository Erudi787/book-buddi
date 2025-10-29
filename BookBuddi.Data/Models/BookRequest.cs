using BookBuddi.Resources.Constants;

namespace BookBuddi.Data.Models
{
    public partial class BookRequest
    {
        public int RequestId { get; set; }
        public int MemberId { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public string? ISBN { get; set; }
        public DateTime RequestDate { get; set; }
        public RequestStatus Status { get; set; } = RequestStatus.Pending;
        public string? AdminNotes { get; set; }
        public string? MemberNotes { get; set; }
        public DateTime? StatusUpdateDate { get; set; }

        // Audit Trail
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedTime { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
        public DateTime UpdatedTime { get; set; }
    }
}
