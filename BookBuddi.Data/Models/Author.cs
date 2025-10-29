namespace BookBuddi.Data.Models
{
    public partial class Author
    {
        public int AuthorId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Biography { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Nationality { get; set; }

        // Audit Trail
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedTime { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
        public DateTime UpdatedTime { get; set; }
    }
}
