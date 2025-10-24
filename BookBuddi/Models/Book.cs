using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BookBuddi.Models.Enums;

namespace BookBuddi.Models
{
    public class Book
    {
        [Key]
        public int BookId { get; set; }

        [Required]
        [MaxLength(300)]
        public string BookTitle { get; set; } = string.Empty;

        [Required]
        [MaxLength(13)]
        public string ISBN { get; set; } = string.Empty; // International Standard Book Number

        [MaxLength(200)]
        public string? Publisher { get; set; }

        public DateTime? PublicationDate { get; set; }

        [MaxLength(2000)]
        public string? Description { get; set; }

        [MaxLength(500)]
        public string? CoverImageUrl { get; set; } // URL or path to book cover image if any

        public int? PageCount { get; set; }

        [Required]
        public int AvailableCopies { get; set; }

        [Required]
        public BookStatus Status { get; set; } = BookStatus.Available;

        public DateTime DateAdded { get; set; } = DateTime.Now;
        public DateTime? LastModified { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public virtual Category Category { get; set; } = null!;

        [Required]
        public int GenreId { get; set; }

        [ForeignKey(nameof(GenreId))]
        public virtual Genre Genre { get; set; } = null!;

        public virtual ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();

        public virtual ICollection<BorrowTransaction> BorrowTransactions { get; set; } = new List<BorrowTransaction>();
    }
}