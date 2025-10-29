namespace BookBuddi.Data.Models
{
    public partial class Genre
    {
        public int GenreId { get; set; }
        public string GenreName { get; set; } = string.Empty;
        public string? GenreDescription { get; set; }

        // Audit Trail
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedTime { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
        public DateTime UpdatedTime { get; set; }
    }
}
