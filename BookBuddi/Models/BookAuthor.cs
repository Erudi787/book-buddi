using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookBuddi.Models
{
    public class BookAuthor
    {
        [Required]
        public int BookId { get; set; }

        [ForeignKey(nameof(BookId))]
        public virtual Book Book { get; set; } = null!;

        [Required]
        public int AuthorId { get; set; }

        [ForeignKey(nameof(AuthorId))]
        public virtual Author Author { get; set; } = null!;

        public int AuthorOrder { get; set; } = 1; // 1 for primary author, 2+ for co-authors
    }
}