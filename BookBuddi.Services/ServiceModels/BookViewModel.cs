using System.ComponentModel.DataAnnotations;
using BookBuddi.Resources.Constants;

namespace BookBuddi.Services.ServiceModels
{
    public class BookViewModel
    {
        public int BookId { get; set; }

        [Required(ErrorMessage = "Book title is required")]
        [StringLength(300, ErrorMessage = "Book title cannot exceed 300 characters")]
        public string BookTitle { get; set; } = string.Empty;

        [Required(ErrorMessage = "ISBN is required")]
        [StringLength(13, ErrorMessage = "ISBN cannot exceed 13 characters")]
        public string ISBN { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Publisher cannot exceed 200 characters")]
        public string? Publisher { get; set; }

        public DateTime? PublicationDate { get; set; }

        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
        public string? Description { get; set; }

        [StringLength(500, ErrorMessage = "Cover image URL cannot exceed 500 characters")]
        public string? CoverImageUrl { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Page count must be greater than 0")]
        public int? PageCount { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Available copies cannot be negative")]
        public int AvailableCopies { get; set; }

        public BookStatus Status { get; set; } = BookStatus.Available;

        [Required(ErrorMessage = "Category is required")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Genre is required")]
        public int GenreId { get; set; }

        // Display properties
        public string? CategoryName { get; set; }
        public string? GenreName { get; set; }
        public string? AuthorNames { get; set; }

        // Audit fields
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedTime { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
        public DateTime UpdatedTime { get; set; }
    }
}
