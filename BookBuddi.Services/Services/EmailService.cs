using BookBuddi.Services.Configuration;
using BookBuddi.Services.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace BookBuddi.Services.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ApplicationSettings _appSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(
            IOptions<EmailSettings> emailSettings,
            IOptions<ApplicationSettings> appSettings,
            ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _appSettings = appSettings.Value;
            _logger = logger;

            // Log configuration on startup
            _logger.LogInformation(
                "EmailService initialized - UseDevelopmentMode: {UseDevelopmentMode}, SmtpServer: {SmtpServer}, SmtpUsername: {SmtpUsername}, ApplicationUrl: {ApplicationUrl}",
                _emailSettings.UseDevelopmentMode,
                _emailSettings.SmtpServer,
                _emailSettings.SmtpUsername,
                _appSettings.ApplicationUrl);
        }

        public async Task<bool> SendMemberVerificationEmailAsync(string email, string firstName, string verificationToken, string verificationCode)
        {
            var verificationUrl = $"{_appSettings.ApplicationUrl}/Account/VerifyEmail?token={verificationToken}";
            var subject = "Verify Your Email - BookBuddi Library";

            var htmlBody = GetEmailVerificationTemplate(firstName, verificationCode, verificationUrl);
            var plainTextBody = $"Hi {firstName},\n\nYour verification code is: {verificationCode}\n\nEnter this code at: {_appSettings.ApplicationUrl}/Account/EnterVerificationCode\n\nOr click this link: {verificationUrl}\n\nThis code will expire in {_appSettings.TokenExpirationHours} hours.\n\nThank you,\nBookBuddi Library System";

            return await SendEmailAsync(email, firstName, subject, htmlBody, plainTextBody);
        }

        public async Task<bool> SendAdminVerificationEmailAsync(string email, string firstName, string verificationToken)
        {
            var verificationUrl = $"{_appSettings.ApplicationUrl}/Account/VerifyEmail?token={verificationToken}";
            var verificationCode = "N/A"; // Admins don't use code verification
            var subject = "Verify Your Admin Email - BookBuddi Library";

            var htmlBody = GetEmailVerificationTemplate(firstName, verificationCode, verificationUrl, isAdmin: true);
            var plainTextBody = $"Hi {firstName},\n\nPlease verify your admin email by visiting: {verificationUrl}\n\nThis link will expire in {_appSettings.TokenExpirationHours} hours.\n\nThank you,\nBookBuddi Library System";

            return await SendEmailAsync(email, firstName, subject, htmlBody, plainTextBody);
        }

        public async Task<bool> SendPasswordResetEmailAsync(string email, string firstName, string resetUrl)
        {
            var subject = "Reset Your Password - BookBuddi Library";

            var htmlBody = GetPasswordResetTemplate(firstName, resetUrl);
            var plainTextBody = $"Hi {firstName},\n\nYou requested to reset your password. Click the link below to reset it:\n{resetUrl}\n\nThis link will expire in 1 hour.\n\nIf you didn't request this, please ignore this email.\n\nThank you,\nBookBuddi Library System";

            return await SendEmailAsync(email, firstName, subject, htmlBody, plainTextBody);
        }

        public async Task<bool> SendBookDueReminderAsync(string email, string memberName, string bookTitle, DateTime dueDate)
        {
            var subject = "Book Due Reminder - BookBuddi Library";
            var daysUntilDue = (dueDate - DateTime.Now).Days;

            var htmlBody = GetBookDueReminderTemplate(memberName, bookTitle, dueDate, daysUntilDue);
            var plainTextBody = $"Hi {memberName},\n\nThis is a reminder that the following book is due soon:\n\nBook: {bookTitle}\nDue Date: {dueDate:MMMM dd, yyyy}\nDays Until Due: {daysUntilDue}\n\nPlease return it on time to avoid late fees.\n\nThank you,\nBookBuddi Library System";

            return await SendEmailAsync(email, memberName, subject, htmlBody, plainTextBody);
        }

        public async Task<bool> SendOverdueAlertAsync(string email, string memberName, string bookTitle, DateTime dueDate, int daysOverdue)
        {
            var subject = "Overdue Book Alert - BookBuddi Library";

            var htmlBody = GetOverdueAlertTemplate(memberName, bookTitle, dueDate, daysOverdue);
            var plainTextBody = $"Hi {memberName},\n\nThe following book is now OVERDUE:\n\nBook: {bookTitle}\nDue Date: {dueDate:MMMM dd, yyyy}\nDays Overdue: {daysOverdue}\n\nPlease return it as soon as possible. Late fees may apply.\n\nThank you,\nBookBuddi Library System";

            return await SendEmailAsync(email, memberName, subject, htmlBody, plainTextBody);
        }

        public async Task<bool> SendFineIssuedNotificationAsync(string email, string memberName, decimal fineAmount, string reason)
        {
            var subject = "Fine Issued - BookBuddi Library";

            var htmlBody = GetFineIssuedTemplate(memberName, fineAmount, reason);
            var plainTextBody = $"Hi {memberName},\n\nA fine has been issued to your account:\n\nAmount: ₱{fineAmount:N2}\nReason: {reason}\n\nPlease settle this fine at your earliest convenience.\n\nThank you,\nBookBuddi Library System";

            return await SendEmailAsync(email, memberName, subject, htmlBody, plainTextBody);
        }

        public async Task<bool> SendBookRequestUpdateAsync(string email, string memberName, string bookTitle, string status)
        {
            var subject = $"Book Request {status} - BookBuddi Library";

            var htmlBody = GetBookRequestUpdateTemplate(memberName, bookTitle, status);
            var plainTextBody = $"Hi {memberName},\n\nYour book request has been updated:\n\nBook: {bookTitle}\nStatus: {status}\n\nThank you,\nBookBuddi Library System";

            return await SendEmailAsync(email, memberName, subject, htmlBody, plainTextBody);
        }

        public async Task<bool> SendMembershipExpiryReminderAsync(string email, string memberName, DateTime expiryDate)
        {
            var subject = "Membership Expiry Reminder - BookBuddi Library";
            var daysUntilExpiry = (expiryDate - DateTime.Now).Days;

            var htmlBody = GetMembershipExpiryTemplate(memberName, expiryDate, daysUntilExpiry);
            var plainTextBody = $"Hi {memberName},\n\nYour membership will expire soon:\n\nExpiry Date: {expiryDate:MMMM dd, yyyy}\nDays Remaining: {daysUntilExpiry}\n\nPlease renew your membership to continue enjoying our services.\n\nThank you,\nBookBuddi Library System";

            return await SendEmailAsync(email, memberName, subject, htmlBody, plainTextBody);
        }

        public async Task<bool> SendBookReturnConfirmationAsync(string email, string memberName, string bookTitle, DateTime returnDate)
        {
            var subject = "Book Return Confirmation - BookBuddi Library";

            var htmlBody = GetBookReturnConfirmationTemplate(memberName, bookTitle, returnDate);
            var plainTextBody = $"Hi {memberName},\n\nThank you for returning:\n\nBook: {bookTitle}\nReturn Date: {returnDate:MMMM dd, yyyy}\n\nThank you,\nBookBuddi Library System";

            return await SendEmailAsync(email, memberName, subject, htmlBody, plainTextBody);
        }

        public async Task<bool> SendBookBorrowConfirmationAsync(string email, string memberName, string bookTitle, DateTime dueDate)
        {
            var subject = "Book Borrowed Successfully - BookBuddi Library";

            var htmlBody = GetBookBorrowConfirmationTemplate(memberName, bookTitle, dueDate);
            var plainTextBody = $"Hi {memberName},\n\nYou have successfully borrowed:\n\nBook: {bookTitle}\nDue Date: {dueDate:MMMM dd, yyyy}\n\nPlease return it on time.\n\nThank you,\nBookBuddi Library System";

            return await SendEmailAsync(email, memberName, subject, htmlBody, plainTextBody);
        }

        public async Task<bool> SendEmailAsync(string toEmail, string toName, string subject, string htmlBody, string plainTextBody = "")
        {
            try
            {
                _logger.LogInformation("Attempting to send email to {Email}. UseDevelopmentMode: {UseDevelopmentMode}",
                    toEmail, _emailSettings.UseDevelopmentMode);

                // In development mode, log the email instead of sending
                if (_emailSettings.UseDevelopmentMode)
                {
                    _logger.LogInformation(
                        "DEV MODE - Email would be sent:\n" +
                        "To: {ToEmail} ({ToName})\n" +
                        "Subject: {Subject}\n" +
                        "Body: {PlainTextBody}",
                        toEmail, toName, subject, plainTextBody);
                    return true;
                }

                _logger.LogInformation("Sending actual email to {Email} via {SmtpServer}:{SmtpPort}",
                    toEmail, _emailSettings.SmtpServer, _emailSettings.SmtpPort);

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
                message.To.Add(new MailboxAddress(toName, toEmail));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = htmlBody,
                    TextBody = string.IsNullOrEmpty(plainTextBody) ? ConvertHtmlToPlainText(htmlBody) : plainTextBody
                };

                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();

                _logger.LogInformation("Connecting to SMTP server {SmtpServer}:{SmtpPort}...",
                    _emailSettings.SmtpServer, _emailSettings.SmtpPort);

                await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort,
                    _emailSettings.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);

                _logger.LogInformation("Connected. Authenticating as {Username}...", _emailSettings.SmtpUsername);

                if (!string.IsNullOrEmpty(_emailSettings.SmtpUsername) && !string.IsNullOrEmpty(_emailSettings.SmtpPassword))
                {
                    await client.AuthenticateAsync(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword);
                }

                _logger.LogInformation("Authenticated. Sending message...");
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation("Email sent successfully to {Email}", toEmail);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}. Subject: {Subject}. Error: {ErrorMessage}",
                    toEmail, subject, ex.Message);
                return false;
            }
        }

        #region Email Templates

        private string GetEmailVerificationTemplate(string firstName, string verificationCode, string verificationUrl, bool isAdmin = false)
        {
            var role = isAdmin ? "Admin" : "Member";
            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #641B2E 0%, #8B4049 100%); color: white; padding: 20px; text-align: center; }}
        .logo {{ font-size: 28px; font-weight: bold; color: #FBDB93; margin-bottom: 10px; }}
        .logo-icon {{ font-size: 32px; }}
        .content {{ background-color: #f9f9f9; padding: 30px; border-radius: 5px; margin-top: 20px; }}
        .code-box {{ background-color: #fff; border: 3px dashed #FBDB93; padding: 20px; text-align: center; margin: 30px 0; border-radius: 10px; }}
        .code {{ font-size: 36px; font-weight: bold; letter-spacing: 8px; color: #8B4049; font-family: 'Courier New', monospace; }}
        .button {{ display: inline-block; padding: 12px 30px; background: linear-gradient(135deg, #641B2E 0%, #8B4049 100%); color: white; text-decoration: none; border-radius: 5px; margin: 20px 0; }}
        .divider {{ text-align: center; margin: 30px 0; color: #999; }}
        .footer {{ text-align: center; margin-top: 30px; font-size: 12px; color: #666; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <div class='logo'>
                <span class='logo-icon'>📚</span> BookBuddi
            </div>
            <h1>Welcome to BookBuddi Library!</h1>
        </div>
        <div class='content'>
            <h2>Hello {firstName},</h2>
            <p>Thank you for registering as a {role} with BookBuddi Library System.</p>
            <p><strong>Your verification code is:</strong></p>

            <div class='code-box'>
                <div class='code'>{verificationCode}</div>
            </div>

            <p style='text-align: center;'>Enter this code on the verification page to activate your account.</p>

            <div class='divider'>
                <p>── OR ──</p>
            </div>

            <p style='text-align: center;'>Click the button below to verify automatically:</p>
            <p style='text-align: center;'>
                <a href='{verificationUrl}' class='button'>Verify Email Address</a>
            </p>

            <p style='font-size: 12px; color: #666;'>This verification code will expire in {_appSettings.TokenExpirationHours} hours.</p>
            <p style='font-size: 12px; color: #666;'>If you didn't create this account, please ignore this email.</p>
        </div>
        <div class='footer'>
            <p>&copy; 2025 BookBuddi Library System. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";
        }

        private string GetPasswordResetTemplate(string firstName, string resetUrl)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #641B2E 0%, #8B4049 100%); color: white; padding: 20px; text-align: center; }}
        .logo {{ font-size: 28px; font-weight: bold; color: #FBDB93; margin-bottom: 10px; }}
        .logo-icon {{ font-size: 32px; }}
        .content {{ background-color: #f9f9f9; padding: 30px; border-radius: 5px; margin-top: 20px; }}
        .button {{ display: inline-block; padding: 12px 30px; background: linear-gradient(135deg, #641B2E 0%, #8B4049 100%); color: white; text-decoration: none; border-radius: 5px; margin: 20px 0; }}
        .footer {{ text-align: center; margin-top: 30px; font-size: 12px; color: #666; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <div class='logo'>
                <span class='logo-icon'>📚</span> BookBuddi
            </div>
            <h1>Password Reset Request</h1>
        </div>
        <div class='content'>
            <h2>Hello {firstName},</h2>
            <p>You requested to reset your password for your BookBuddi Library account.</p>
            <p>Click the button below to reset your password:</p>
            <p style='text-align: center;'>
                <a href='{resetUrl}' class='button'>Reset Password</a>
            </p>
            <p>Or copy and paste this link into your browser:</p>
            <p style='word-break: break-all;'>{resetUrl}</p>
            <p>This link will expire in 1 hour for security reasons.</p>
            <p><strong>If you didn't request this password reset, please ignore this email.</strong> Your password will remain unchanged.</p>
        </div>
        <div class='footer'>
            <p>&copy; 2025 BookBuddi Library System. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";
        }

        private string GetBookDueReminderTemplate(string memberName, string bookTitle, DateTime dueDate, int daysUntilDue)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #641B2E 0%, #8B4049 100%); color: white; padding: 20px; text-align: center; }}
        .logo {{ font-size: 28px; font-weight: bold; color: #FBDB93; margin-bottom: 10px; }}
        .logo-icon {{ font-size: 32px; }}
        .content {{ background-color: #f9f9f9; padding: 30px; border-radius: 5px; margin-top: 20px; }}
        .book-info {{ background-color: white; padding: 15px; border-left: 4px solid #FBDB93; margin: 20px 0; }}
        .footer {{ text-align: center; margin-top: 30px; font-size: 12px; color: #666; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <div class='logo'>
                <span class='logo-icon'>📚</span> BookBuddi
            </div>
            <h1>Book Due Reminder</h1>
        </div>
        <div class='content'>
            <h2>Hello {memberName},</h2>
            <p>This is a friendly reminder that the following book is due soon:</p>
            <div class='book-info'>
                <p><strong>Book Title:</strong> {bookTitle}</p>
                <p><strong>Due Date:</strong> {dueDate:MMMM dd, yyyy}</p>
                <p><strong>Days Until Due:</strong> {daysUntilDue} day(s)</p>
            </div>
            <p>Please return the book on time to avoid late fees.</p>
            <p>Thank you for using BookBuddi Library!</p>
        </div>
        <div class='footer'>
            <p>&copy; 2025 BookBuddi Library System. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";
        }

        private string GetOverdueAlertTemplate(string memberName, string bookTitle, DateTime dueDate, int daysOverdue)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #641B2E 0%, #8B4049 100%); color: white; padding: 20px; text-align: center; }}
        .logo {{ font-size: 28px; font-weight: bold; color: #FBDB93; margin-bottom: 10px; }}
        .logo-icon {{ font-size: 32px; }}
        .content {{ background-color: #f9f9f9; padding: 30px; border-radius: 5px; margin-top: 20px; }}
        .book-info {{ background-color: #ffebee; padding: 15px; border-left: 4px solid #F44336; margin: 20px 0; }}
        .footer {{ text-align: center; margin-top: 30px; font-size: 12px; color: #666; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <div class='logo'>
                <span class='logo-icon'>📚</span> BookBuddi
            </div>
            <h1>⚠️ Overdue Book Alert</h1>
        </div>
        <div class='content'>
            <h2>Hello {memberName},</h2>
            <p>The following book is now <strong>OVERDUE</strong>:</p>
            <div class='book-info'>
                <p><strong>Book Title:</strong> {bookTitle}</p>
                <p><strong>Due Date:</strong> {dueDate:MMMM dd, yyyy}</p>
                <p><strong>Days Overdue:</strong> {daysOverdue} day(s)</p>
            </div>
            <p>Please return this book as soon as possible. Late fees may apply.</p>
            <p>If you have any questions, please contact the library staff.</p>
        </div>
        <div class='footer'>
            <p>&copy; 2025 BookBuddi Library System. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";
        }

        private string GetFineIssuedTemplate(string memberName, decimal fineAmount, string reason)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #641B2E 0%, #8B4049 100%); color: white; padding: 20px; text-align: center; }}
        .logo {{ font-size: 28px; font-weight: bold; color: #FBDB93; margin-bottom: 10px; }}
        .logo-icon {{ font-size: 32px; }}
        .content {{ background-color: #f9f9f9; padding: 30px; border-radius: 5px; margin-top: 20px; }}
        .fine-info {{ background-color: #FFF9F0; padding: 15px; border-left: 4px solid #FBDB93; margin: 20px 0; }}
        .footer {{ text-align: center; margin-top: 30px; font-size: 12px; color: #666; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <div class='logo'>
                <span class='logo-icon'>📚</span> BookBuddi
            </div>
            <h1>Fine Issued Notification</h1>
        </div>
        <div class='content'>
            <h2>Hello {memberName},</h2>
            <p>A fine has been issued to your library account:</p>
            <div class='fine-info'>
                <p><strong>Fine Amount:</strong> ₱{fineAmount:N2}</p>
                <p><strong>Reason:</strong> {reason}</p>
            </div>
            <p>Please settle this fine at your earliest convenience to avoid account suspension.</p>
            <p>You can pay at the library counter during operating hours.</p>
        </div>
        <div class='footer'>
            <p>&copy; 2025 BookBuddi Library System. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";
        }

        private string GetBookRequestUpdateTemplate(string memberName, string bookTitle, string status)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #641B2E 0%, #8B4049 100%); color: white; padding: 20px; text-align: center; }}
        .logo {{ font-size: 28px; font-weight: bold; color: #FBDB93; margin-bottom: 10px; }}
        .logo-icon {{ font-size: 32px; }}
        .content {{ background-color: #f9f9f9; padding: 30px; border-radius: 5px; margin-top: 20px; }}
        .request-info {{ background-color: white; padding: 15px; border-left: 4px solid #FBDB93; margin: 20px 0; }}
        .footer {{ text-align: center; margin-top: 30px; font-size: 12px; color: #666; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <div class='logo'>
                <span class='logo-icon'>📚</span> BookBuddi
            </div>
            <h1>Book Request Update</h1>
        </div>
        <div class='content'>
            <h2>Hello {memberName},</h2>
            <p>Your book request has been updated:</p>
            <div class='request-info'>
                <p><strong>Book Title:</strong> {bookTitle}</p>
                <p><strong>Status:</strong> {status}</p>
            </div>
            <p>Thank you for your patience!</p>
        </div>
        <div class='footer'>
            <p>&copy; 2025 BookBuddi Library System. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";
        }

        private string GetMembershipExpiryTemplate(string memberName, DateTime expiryDate, int daysUntilExpiry)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #641B2E 0%, #8B4049 100%); color: white; padding: 20px; text-align: center; }}
        .logo {{ font-size: 28px; font-weight: bold; color: #FBDB93; margin-bottom: 10px; }}
        .logo-icon {{ font-size: 32px; }}
        .content {{ background-color: #f9f9f9; padding: 30px; border-radius: 5px; margin-top: 20px; }}
        .membership-info {{ background-color: white; padding: 15px; border-left: 4px solid #FBDB93; margin: 20px 0; }}
        .footer {{ text-align: center; margin-top: 30px; font-size: 12px; color: #666; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <div class='logo'>
                <span class='logo-icon'>📚</span> BookBuddi
            </div>
            <h1>Membership Expiry Reminder</h1>
        </div>
        <div class='content'>
            <h2>Hello {memberName},</h2>
            <p>Your BookBuddi Library membership will expire soon:</p>
            <div class='membership-info'>
                <p><strong>Expiry Date:</strong> {expiryDate:MMMM dd, yyyy}</p>
                <p><strong>Days Remaining:</strong> {daysUntilExpiry} day(s)</p>
            </div>
            <p>Please renew your membership to continue enjoying our library services.</p>
            <p>Visit the library or contact us to renew your membership.</p>
        </div>
        <div class='footer'>
            <p>&copy; 2025 BookBuddi Library System. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";
        }

        private string GetBookReturnConfirmationTemplate(string memberName, string bookTitle, DateTime returnDate)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #641B2E 0%, #8B4049 100%); color: white; padding: 20px; text-align: center; }}
        .content {{ background-color: #f9f9f9; padding: 30px; border-radius: 5px; margin-top: 20px; }}
        .book-info {{ background-color: white; padding: 15px; border-left: 4px solid #FBDB93; margin: 20px 0; }}
        .footer {{ text-align: center; margin-top: 30px; font-size: 12px; color: #666; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>✓ Book Return Confirmation</h1>
        </div>
        <div class='content'>
            <h2>Hello {memberName},</h2>
            <p>Thank you for returning the following book:</p>
            <div class='book-info'>
                <p><strong>Book Title:</strong> {bookTitle}</p>
                <p><strong>Return Date:</strong> {returnDate:MMMM dd, yyyy}</p>
            </div>
            <p>We hope you enjoyed reading it!</p>
            <p>Visit us again soon to borrow more books.</p>
        </div>
        <div class='footer'>
            <p>&copy; 2025 BookBuddi Library System. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";
        }

        private string GetBookBorrowConfirmationTemplate(string memberName, string bookTitle, DateTime dueDate)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #641B2E 0%, #8B4049 100%); color: white; padding: 20px; text-align: center; }}
        .content {{ background-color: #f9f9f9; padding: 30px; border-radius: 5px; margin-top: 20px; }}
        .book-info {{ background-color: white; padding: 15px; border-left: 4px solid #FBDB93; margin: 20px 0; }}
        .footer {{ text-align: center; margin-top: 30px; font-size: 12px; color: #666; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Book Borrowed Successfully</h1>
        </div>
        <div class='content'>
            <h2>Hello {memberName},</h2>
            <p>You have successfully borrowed the following book:</p>
            <div class='book-info'>
                <p><strong>Book Title:</strong> {bookTitle}</p>
                <p><strong>Due Date:</strong> {dueDate:MMMM dd, yyyy}</p>
            </div>
            <p>Please return this book on or before the due date to avoid late fees.</p>
            <p>Enjoy your reading!</p>
        </div>
        <div class='footer'>
            <p>&copy; 2025 BookBuddi Library System. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";
        }

        private string ConvertHtmlToPlainText(string html)
        {
            // Simple HTML to plain text conversion
            var text = System.Text.RegularExpressions.Regex.Replace(html, "<[^>]+>", " ");
            text = System.Text.RegularExpressions.Regex.Replace(text, @"\s+", " ");
            return text.Trim();
        }

        #endregion
    }
}
