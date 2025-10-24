using System.ComponentModel.DataAnnotations;

namespace BookBuddi.Models
{
    public class Genre
    {
        [Key]
        public int GenreId { get; set; }

        [Required]
        [MaxLength(100)]
        public string GenreName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? GenreDescription { get; set; }

        public virtual ICollection<Book> Books { get; set; } = new List<Book>();
    }
}