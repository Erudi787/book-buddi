using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BookBuddi.Models.Enums;

namespace BookBuddi.Models
{
    public class BookRequest
    {
        [Key]
        public int RequestId { get; set; }

        [Required]
        public int MemberId { get; set; }

        [ForeignKey(nameof(MemberId))]
        public virtual Member Member { get; set; } = null!;

        [Required]
        [MaxLength(300)]
        public string BookTitle { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string AuthorName { get; set; } = string.Empty;

        public string? ISBN { get; set; }

        [Required]
        public DateTime RequestDate { get; set; } = DateTime.Now;

        [Required]
        public RequestStatus Status { get; set; } = RequestStatus.Pending;

        [MaxLength(1000)]
        public string? AdminNotes { get; set; }

        [MaxLength(1000)]
        public string? MemberNotes { get; set; }

        public DateTime? StatusUpdateDate { get; set; }
    }
}