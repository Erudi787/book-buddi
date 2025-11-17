namespace BookBuddi.Services.Interfaces
{
    public interface IEmailService
    {
        /// <summary>
        /// Sends an email verification link and code to a member
        /// </summary>
        Task<bool> SendMemberVerificationEmailAsync(string email, string firstName, string verificationToken, string verificationCode);

        /// <summary>
        /// Sends an email verification link to an admin
        /// </summary>
        Task<bool> SendAdminVerificationEmailAsync(string email, string firstName, string verificationToken);

        /// <summary>
        /// Sends a password reset email with reset link
        /// </summary>
        Task<bool> SendPasswordResetEmailAsync(string email, string firstName, string resetUrl);

        /// <summary>
        /// Sends a book due reminder notification
        /// </summary>
        Task<bool> SendBookDueReminderAsync(string email, string memberName, string bookTitle, DateTime dueDate);

        /// <summary>
        /// Sends an overdue book alert notification
        /// </summary>
        Task<bool> SendOverdueAlertAsync(string email, string memberName, string bookTitle, DateTime dueDate, int daysOverdue);

        /// <summary>
        /// Sends a fine issued notification
        /// </summary>
        Task<bool> SendFineIssuedNotificationAsync(string email, string memberName, decimal fineAmount, string reason);

        /// <summary>
        /// Sends a book request status update notification
        /// </summary>
        Task<bool> SendBookRequestUpdateAsync(string email, string memberName, string bookTitle, string status);

        /// <summary>
        /// Sends a membership expiry reminder
        /// </summary>
        Task<bool> SendMembershipExpiryReminderAsync(string email, string memberName, DateTime expiryDate);

        /// <summary>
        /// Sends a book return confirmation
        /// </summary>
        Task<bool> SendBookReturnConfirmationAsync(string email, string memberName, string bookTitle, DateTime returnDate);

        /// <summary>
        /// Sends a book borrow confirmation
        /// </summary>
        Task<bool> SendBookBorrowConfirmationAsync(string email, string memberName, string bookTitle, DateTime dueDate);

        /// <summary>
        /// Generic method to send custom emails
        /// </summary>
        Task<bool> SendEmailAsync(string toEmail, string toName, string subject, string htmlBody, string plainTextBody = "");
    }
}
