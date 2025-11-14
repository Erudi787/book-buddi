namespace BookBuddi.Data.Models
{
    public partial class PasswordResetToken
    {
        public int TokenId { get; set; }
        public int MemberId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime CreatedTime { get; set; }
        public DateTime ExpiryTime { get; set; }
        public bool IsUsed { get; set; } = false;

        // Navigation property
        public virtual Member Member { get; set; } = null!;
    }
}
