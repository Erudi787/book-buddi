using BookBuddi.Data.Interfaces;
using BookBuddi.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace BookBuddi.Services.Services
{
    public class EmailNotificationBackgroundService
    {
        private readonly IBorrowTransactionRepository _borrowingRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IEmailService _emailService;
        private readonly INotificationService _notificationService;
        private readonly ILogger<EmailNotificationBackgroundService> _logger;

        public EmailNotificationBackgroundService(
            IBorrowTransactionRepository borrowingRepository,
            IMemberRepository memberRepository,
            IBookRepository bookRepository,
            IEmailService emailService,
            INotificationService notificationService,
            ILogger<EmailNotificationBackgroundService> logger)
        {
            _borrowingRepository = borrowingRepository;
            _memberRepository = memberRepository;
            _bookRepository = bookRepository;
            _emailService = emailService;
            _notificationService = notificationService;
            _logger = logger;
        }

        /// <summary>
        /// Sends email reminders for books due in 3 days
        /// </summary>
        public async Task SendDueRemindersAsync()
        {
            try
            {
                _logger.LogInformation("Starting due date reminder email job");

                var activeTransactions = _borrowingRepository.GetTransactionsByStatus(Resources.Constants.TransactionStatus.Active);
                var threeDaysFromNow = DateTime.Now.AddDays(3).Date;

                foreach (var transaction in activeTransactions)
                {
                    // Send reminder if book is due in 3 days
                    if (transaction.DueDate.Date == threeDaysFromNow)
                    {
                        var member = _memberRepository.GetMemberById(transaction.MemberId);
                        var book = _bookRepository.GetBookById(transaction.BookId);

                        if (member != null && book != null)
                        {
                            try
                            {
                                await _emailService.SendBookDueReminderAsync(
                                    member.Email,
                                    $"{member.FirstName} {member.LastName}",
                                    book.BookTitle,
                                    transaction.DueDate);

                                // Create in-app notification
                                _notificationService.CreateDueReminderNotification(
                                    transaction.MemberId,
                                    transaction.TransactionId,
                                    transaction.DueDate);

                                _logger.LogInformation(
                                    "Sent due reminder for transaction {TransactionId} to {Email}",
                                    transaction.TransactionId, member.Email);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex,
                                    "Failed to send due reminder for transaction {TransactionId}",
                                    transaction.TransactionId);
                            }
                        }
                    }
                }

                _logger.LogInformation("Completed due date reminder email job");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SendDueRemindersAsync job");
            }
        }

        /// <summary>
        /// Sends email alerts for overdue books
        /// </summary>
        public async Task SendOverdueAlertsAsync()
        {
            try
            {
                _logger.LogInformation("Starting overdue alert email job");

                var activeTransactions = _borrowingRepository.GetTransactionsByStatus(Resources.Constants.TransactionStatus.Active);
                var today = DateTime.Now.Date;

                foreach (var transaction in activeTransactions)
                {
                    // Send alert if book is overdue
                    if (transaction.DueDate.Date < today)
                    {
                        var member = _memberRepository.GetMemberById(transaction.MemberId);
                        var book = _bookRepository.GetBookById(transaction.BookId);

                        if (member != null && book != null)
                        {
                            try
                            {
                                var daysOverdue = (today - transaction.DueDate.Date).Days;

                                await _emailService.SendOverdueAlertAsync(
                                    member.Email,
                                    $"{member.FirstName} {member.LastName}",
                                    book.BookTitle,
                                    transaction.DueDate,
                                    daysOverdue);

                                // Create in-app notification (only once when first overdue)
                                if (daysOverdue == 1)
                                {
                                    _notificationService.CreateOverdueAlertNotification(
                                        transaction.MemberId,
                                        transaction.TransactionId);
                                }

                                _logger.LogInformation(
                                    "Sent overdue alert for transaction {TransactionId} to {Email} (Days overdue: {DaysOverdue})",
                                    transaction.TransactionId, member.Email, daysOverdue);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex,
                                    "Failed to send overdue alert for transaction {TransactionId}",
                                    transaction.TransactionId);
                            }
                        }
                    }
                }

                _logger.LogInformation("Completed overdue alert email job");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SendOverdueAlertsAsync job");
            }
        }

        /// <summary>
        /// Sends membership expiry reminders for memberships expiring in 7 days
        /// </summary>
        public async Task SendMembershipExpiryRemindersAsync()
        {
            try
            {
                _logger.LogInformation("Starting membership expiry reminder email job");

                var members = _memberRepository.GetMembers();
                var sevenDaysFromNow = DateTime.Now.AddDays(7).Date;

                foreach (var member in members)
                {
                    if (member.MembershipExpiryDate.Date == sevenDaysFromNow &&
                        member.Status == Resources.Constants.MemberStatus.Active)
                    {
                        try
                        {
                            await _emailService.SendMembershipExpiryReminderAsync(
                                member.Email,
                                $"{member.FirstName} {member.LastName}",
                                member.MembershipExpiryDate);

                            _logger.LogInformation(
                                "Sent membership expiry reminder to {Email}",
                                member.Email);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex,
                                "Failed to send membership expiry reminder to member {MemberId}",
                                member.MemberId);
                        }
                    }
                }

                _logger.LogInformation("Completed membership expiry reminder email job");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SendMembershipExpiryRemindersAsync job");
            }
        }
    }
}
