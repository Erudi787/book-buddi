using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Services.Configuration;
using Microsoft.Extensions.Options;

namespace BookBuddi.Pages.Account
{
    public class DiagEmailModel : PageModel
    {
        private readonly EmailSettings _emailSettings;
        private readonly ApplicationSettings _appSettings;
        private readonly IWebHostEnvironment _env;

        public DiagEmailModel(
            IOptions<EmailSettings> emailSettings,
            IOptions<ApplicationSettings> appSettings,
            IWebHostEnvironment env)
        {
            _emailSettings = emailSettings.Value;
            _appSettings = appSettings.Value;
            _env = env;
        }

        public string Environment => _env.EnvironmentName;
        public string SmtpServer => _emailSettings.SmtpServer;
        public int SmtpPort => _emailSettings.SmtpPort;
        public string SmtpUsername => _emailSettings.SmtpUsername;
        public bool UseDevelopmentMode => _emailSettings.UseDevelopmentMode;
        public bool EmailVerificationRequired => _appSettings.EmailVerificationRequired;
        public string ApplicationUrl => _appSettings.ApplicationUrl;

        public void OnGet()
        {
        }
    }
}
