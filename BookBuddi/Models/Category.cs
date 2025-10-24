using System.ComponentModel.DataAnnotations;

namespace BookBuddi.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { set; get; }

        [Required]
        [MaxLength(100)]
        public string CategoryName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public virtual ICollection<Book> Books { get; set; } = new List<Book>();
    }
}