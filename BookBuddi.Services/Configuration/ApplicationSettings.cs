namespace BookBuddi.Services.Configuration
{
    public class ApplicationSettings
    {
        public string ApplicationUrl { get; set; } = string.Empty;
        public bool EmailVerificationRequired { get; set; } = true;
        public int TokenExpirationHours { get; set; } = 24;
    }
}
