using System;
using System.ComponentModel.DataAnnotations;

namespace BookBuddi.Data.Models
{
    public class Rating
    {
        [Key]
        public int RatingId { get; set; }

        [Required]
        public int BookId { get; set; }

        [Required]
        public int MemberId { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5 stars")]
        public int Score { get; set; }

        // Audit trail
        [Required]
        [MaxLength(100)]
        public string CreatedBy { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedTime { get; set; }

        [MaxLength(100)]
        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedTime { get; set; }

        // Navigation properties
        public virtual Book Book { get; set; } = null!;
        public virtual Member Member { get; set; } = null!;
    }
}
