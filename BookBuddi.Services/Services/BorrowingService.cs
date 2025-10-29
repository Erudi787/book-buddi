using AutoMapper;
using BookBuddi.Data.Interfaces;
using BookBuddi.Data.Models;
using BookBuddi.Resources.Constants;
using BookBuddi.Services.Interfaces;
using BookBuddi.Services.ServiceModels;

namespace BookBuddi.Services.Services
{
    public class BorrowingService : IBorrowingService
    {
        private readonly IBorrowTransactionRepository _transactionRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly IFineRepository _fineRepository;
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;

        public BorrowingService(
            IBorrowTransactionRepository transactionRepository,
            IBookRepository bookRepository,
            IMemberRepository memberRepository,
            IFineRepository fineRepository,
            INotificationService notificationService,
            IMapper mapper)
        {
            _transactionRepository = transactionRepository;
            _bookRepository = bookRepository;
            _memberRepository = memberRepository;
            _fineRepository = fineRepository;
            _notificationService = notificationService;
            _mapper = mapper;
        }

        public IEnumerable<BorrowTransactionViewModel> GetAllTransactions()
        {
            var transactions = _transactionRepository.GetTransactions().ToList();
            return _mapper.Map<IEnumerable<BorrowTransactionViewModel>>(transactions);
        }

        public BorrowTransactionViewModel? GetTransactionById(int transactionId)
        {
            var transaction = _transactionRepository.GetTransactionById(transactionId);
            return transaction != null ? _mapper.Map<BorrowTransactionViewModel>(transaction) : null;
        }

        public IEnumerable<BorrowTransactionViewModel> GetTransactionsByMember(int memberId)
        {
            var transactions = _transactionRepository.GetTransactionsByMember(memberId);
            return _mapper.Map<IEnumerable<BorrowTransactionViewModel>>(transactions);
        }

        public IEnumerable<BorrowTransactionViewModel> GetTransactionsByBook(int bookId)
        {
            var transactions = _transactionRepository.GetTransactionsByBook(bookId);
            return _mapper.Map<IEnumerable<BorrowTransactionViewModel>>(transactions);
        }

        public IEnumerable<BorrowTransactionViewModel> GetTransactionsByStatus(TransactionStatus status)
        {
            var transactions = _transactionRepository.GetTransactionsByStatus(status);
            return _mapper.Map<IEnumerable<BorrowTransactionViewModel>>(transactions);
        }

        public IEnumerable<BorrowTransactionViewModel> GetActiveTransactionsByMember(int memberId)
        {
            var transactions = _transactionRepository.GetActiveTransactionsByMember(memberId);
            return _mapper.Map<IEnumerable<BorrowTransactionViewModel>>(transactions);
        }

        public IEnumerable<BorrowTransactionViewModel> GetOverdueTransactions()
        {
            var transactions = _transactionRepository.GetOverdueTransactions();
            return _mapper.Map<IEnumerable<BorrowTransactionViewModel>>(transactions);
        }

        public bool CanBorrowBook(int memberId, int bookId, out string errorMessage)
        {
            errorMessage = string.Empty;

            // Check member exists and status
            var member = _memberRepository.GetMemberById(memberId);
            if (member == null)
            {
                errorMessage = "Member not found";
                return false;
            }

            if (member.Status != MemberStatus.Active)
            {
                errorMessage = "Member account is not active";
                return false;
            }

            // Check borrowing limit
            if (member.CurrentBorrowedCount >= member.BorrowingLimit)
            {
                errorMessage = $"Member has reached borrowing limit of {member.BorrowingLimit} books";
                return false;
            }

            // Check for unpaid fines
            var unpaidFines = _fineRepository.GetTotalUnpaidFinesByMember(memberId);
            if (unpaidFines > 0)
            {
                errorMessage = $"Member has unpaid fines totaling ${unpaidFines:F2}";
                return false;
            }

            // Check book exists and availability
            var book = _bookRepository.GetBookById(bookId);
            if (book == null)
            {
                errorMessage = "Book not found";
                return false;
            }

            if (book.AvailableCopies <= 0)
            {
                errorMessage = "Book is not available for borrowing";
                return false;
            }

            // Check if member already has this book borrowed
            if (_transactionRepository.HasActiveBorrow(memberId, bookId))
            {
                errorMessage = "Member already has this book borrowed";
                return false;
            }

            return true;
        }

        public void BorrowBook(int memberId, int bookId, string createdBy)
        {
            if (!CanBorrowBook(memberId, bookId, out string errorMessage))
                throw new InvalidOperationException(errorMessage);

            // Create transaction
            var transaction = new BorrowTransaction
            {
                MemberId = memberId,
                BookId = bookId,
                BorrowDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(Const.DefaultBorrowPeriodDays),
                Status = TransactionStatus.Active,
                CreatedBy = createdBy,
                CreatedTime = DateTime.Now,
                UpdatedBy = createdBy,
                UpdatedTime = DateTime.Now
            };

            _transactionRepository.AddTransaction(transaction);

            // Update book availability
            _bookRepository.UpdateBookAvailability(bookId, -1);

            // Update member borrow count
            _memberRepository.UpdateBorrowCount(memberId, 1);

            // Create due date reminder notification
            _notificationService.CreateDueReminderNotification(memberId, transaction.TransactionId, transaction.DueDate);
        }

        public void ReturnBook(int transactionId, string updatedBy)
        {
            var transaction = _transactionRepository.GetTransactionById(transactionId);
            if (transaction == null)
                throw new InvalidOperationException("Transaction not found");

            if (transaction.Status != TransactionStatus.Active)
                throw new InvalidOperationException("Book has already been returned");

            // Update transaction
            transaction.ReturnDate = DateTime.Now;
            transaction.Status = TransactionStatus.Returned;
            transaction.UpdatedBy = updatedBy;
            transaction.UpdatedTime = DateTime.Now;

            // Check if overdue and create fine
            if (transaction.ReturnDate > transaction.DueDate)
            {
                var daysOverdue = (transaction.ReturnDate.Value - transaction.DueDate).Days;
                var fineAmount = daysOverdue * Const.DefaultOverdueFinePerDay;

                var fine = new Fine
                {
                    TransactionId = transactionId,
                    MemberId = transaction.MemberId,
                    Amount = fineAmount,
                    Status = FineStatus.Unpaid,
                    Reason = FineReason.Overdue,
                    IssueDate = DateTime.Now,
                    Notes = $"Overdue by {daysOverdue} days",
                    CreatedBy = updatedBy,
                    CreatedTime = DateTime.Now,
                    UpdatedBy = updatedBy,
                    UpdatedTime = DateTime.Now
                };

                _fineRepository.AddFine(fine);

                // Notify member about fine
                _notificationService.CreateFineIssuedNotification(transaction.MemberId, fine.FineId, fineAmount);
            }

            _transactionRepository.UpdateTransaction(transaction);

            // Update book availability
            _bookRepository.UpdateBookAvailability(transaction.BookId, 1);

            // Update member borrow count
            _memberRepository.UpdateBorrowCount(transaction.MemberId, -1);
        }

        public void ProcessOverdueTransactions()
        {
            var overdueTransactions = _transactionRepository.GetOverdueTransactions();

            foreach (var transaction in overdueTransactions)
            {
                // Update status to overdue
                transaction.Status = TransactionStatus.Overdue;
                transaction.UpdatedBy = "System";
                transaction.UpdatedTime = DateTime.Now;
                _transactionRepository.UpdateTransaction(transaction);

                // Send overdue alert notification
                _notificationService.CreateOverdueAlertNotification(transaction.MemberId, transaction.TransactionId);
            }
        }
    }
}
