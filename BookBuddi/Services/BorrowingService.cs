using BookBuddi.Interfaces;
using BookBuddi.Models;
using BookBuddi.Models.Enums;

namespace BookBuddi.Services
{
    public class BorrowingService
    {
        private readonly IBorrowTransactionRepository _transactionRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IFineRepository _fineRepository;

        public BorrowingService(
            IBorrowTransactionRepository transactionRepository,
            IBookRepository bookRepository,
            IMemberRepository memberRepository,
            INotificationRepository notificationRepository,
            IFineRepository fineRepository)
        {
            _transactionRepository = transactionRepository;
            _bookRepository = bookRepository;
            _memberRepository = memberRepository;
            _notificationRepository = notificationRepository;
            _fineRepository = fineRepository;
        }

        public async Task<BorrowTransaction> BorrowBookAsync(int memberId, int bookId, int borrowDays = 14)
        {
            // Validate member
            var member = await _memberRepository.GetMemberIdAsync(memberId);
            if (member == null)
                throw new InvalidOperationException("Member not found.");

            if (member.Status != MemberStatus.Active)
                throw new InvalidOperationException("Member account is not active.");

            if (member.CurrentBorrowedCount >= member.BorrowingLimit)
                throw new InvalidOperationException("Member has reached borrowing limit.");

            // Check for unpaid fines
            var unpaidFines = await _fineRepository.GetUnpaidFinesByMemberIdAsync(memberId);
            if (unpaidFines.Any())
                throw new InvalidOperationException("Member has unpaid fines.");

            // Validate book
            var book = await _bookRepository.GetBookByIdAsync(bookId);
            if (book == null)
                throw new InvalidOperationException("Book not found.");

            if (book.AvailableCopies <= 0)
                throw new InvalidOperationException("Book is not available.");

            // Check if member already has this book borrowed
            if (await _transactionRepository.HasActiveBorrowAsync(memberId, bookId))
                throw new InvalidOperationException("Member already has this book borrowed.");

            // Create transaction
            var transaction = new BorrowTransaction
            {
                BookId = bookId,
                MemberId = memberId,
                BorrowDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(borrowDays),
                Status = TransactionStatus.Active
            };

            await _transactionRepository.AddTransactionAsync(transaction);

            // Update book availability
            await _bookRepository.UpdateBookAvailablityAsync(bookId, -1);

            // Update member's borrowed count
            await _memberRepository.UpdateBorrowedCountAsync(memberId, 1);

            // Create due date reminder notification
            await CreateDueDateNotificationAsync(transaction);

            return transaction;
        }

        public async Task<BorrowTransaction> ReturnBookAsync(int transactionId)
        {
            var transaction = await _transactionRepository.GetTransactionByIdAsync(transactionId);
            if (transaction == null)
                throw new InvalidOperationException("Transaction not found.");

            if (transaction.Status == TransactionStatus.Returned)
                throw new InvalidOperationException("Book has already been returned.");

            // Update transaction
            transaction.ReturnDate = DateTime.Now;
            transaction.Status = TransactionStatus.Returned;

            // Check if book is overdue and create fine if necessary
            if (transaction.ReturnDate > transaction.DueDate)
            {
                await CreateOverdueFineAsync(transaction);
            }

            await _transactionRepository.UpdateTransactionAsync(transaction);

            // Update book availability
            await _bookRepository.UpdateBookAvailablityAsync(transaction.BookId, 1);

            // Update member's borrowed count
            await _memberRepository.UpdateBorrowedCountAsync(transaction.MemberId, -1);

            return transaction;
        }

        public async Task<IEnumerable<BorrowTransaction>> GetMemberTransactionsAsync(int memberId)
        {
            return await _transactionRepository.GetTransactionsByMemberIdAsync(memberId);
        }

        public async Task<IEnumerable<BorrowTransaction>> GetActiveTransactionsAsync()
        {
            return await _transactionRepository.GetActiveTransactionsAsync();
        }

        public async Task<IEnumerable<BorrowTransaction>> GetOverdueTransactionsAsync()
        {
            return await _transactionRepository.GetOverdueTransactionsAsync();
        }

        public async Task ProcessOverdueTransactionsAsync()
        {
            var overdueTransactions = await GetOverdueTransactionsAsync();

            foreach (var transaction in overdueTransactions)
            {
                // Update transaction status
                if (transaction.Status == TransactionStatus.Active)
                {
                    transaction.Status = TransactionStatus.Overdue;
                    await _transactionRepository.UpdateTransactionAsync(transaction);

                    // Send overdue notification
                    await CreateOverdueNotificationAsync(transaction);
                }
            }
        }

        private async Task CreateDueDateNotificationAsync(BorrowTransaction transaction)
        {
            var notification = new Notification
            {
                MemberId = transaction.MemberId,
                Type = NotificationType.DueReminder,
                Title = "Book Due Soon",
                Message = $"Your borrowed book is due on {transaction.DueDate:MMMM dd, yyyy}.",
                RelatedEntityId = transaction.TransactionId,
                RelatedEntityType = "Transaction"
            };

            await _notificationRepository.AddNotificationAsync(notification);
        }

        private async Task CreateOverdueNotificationAsync(BorrowTransaction transaction)
        {
            var notification = new Notification
            {
                MemberId = transaction.MemberId,
                Type = NotificationType.OverdueAlert,
                Title = "Book Overdue",
                Message = $"Your borrowed book was due on {transaction.DueDate:MMMM dd, yyyy}. Please return it as soon as possible.",
                RelatedEntityId = transaction.TransactionId,
                RelatedEntityType = "Transaction"
            };

            await _notificationRepository.AddNotificationAsync(notification);
        }

        private async Task CreateOverdueFineAsync(BorrowTransaction transaction)
        {
            // Calculate fine (e.g., $1 per day overdue)
            var daysOverdue = (transaction.ReturnDate!.Value - transaction.DueDate).Days;
            var fineAmount = daysOverdue * 1.00m;

            var fine = new Fine
            {
                TransactionId = transaction.TransactionId,
                MemberId = transaction.MemberId,
                Amount = fineAmount,
                Reason = FineReason.Overdue,
                Status = FineStatus.Unpaid
            };

            await _fineRepository.AddFineAsync(fine);

            // Create notification
            var notification = new Notification
            {
                MemberId = transaction.MemberId,
                Type = NotificationType.FineIssued,
                Title = "Fine Issued",
                Message = $"A fine of ${fineAmount:F2} has been issued for an overdue book.",
                RelatedEntityId = fine.FineId,
                RelatedEntityType = "Fine"
            };

            await _notificationRepository.AddNotificationAsync(notification);
        }
    }
}
