using BookBuddi.Resources.Constants;

namespace BookBuddi.Data.Models
{
    public partial class Book
    {
        public int BookId { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public string? Publisher { get; set; }
        public DateTime? PublicationDate { get; set; }
        public string? Description { get; set; }
        public string? CoverImageUrl { get; set; }
        public int? PageCount { get; set; }
        public int AvailableCopies { get; set; }
        public BookStatus Status { get; set; } = BookStatus.Available;
        public int CategoryId { get; set; }
        public int GenreId { get; set; }

        // Audit Trail
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedTime { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
        public DateTime UpdatedTime { get; set; }
    }
}
