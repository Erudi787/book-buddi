using System.ComponentModel.DataAnnotations;

namespace BookBuddi.Services.ServiceModels
{
    public class GenreViewModel
    {
        public int GenreId { get; set; }

        [Required(ErrorMessage = "Genre name is required")]
        [StringLength(100, ErrorMessage = "Genre name cannot exceed 100 characters")]
        public string GenreName { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? GenreDescription { get; set; }

        // Audit fields
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedTime { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
        public DateTime UpdatedTime { get; set; }
    }
}
