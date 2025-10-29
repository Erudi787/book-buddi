using BookBuddi.Interfaces;
using BookBuddi.Models.Enums;

namespace BookBuddi.Services
{
    public class ReportService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly IBorrowTransactionRepository _transactionRepository;
        private readonly IFineRepository _fineRepository;
        private readonly IBookRequestRepository _requestRepository;

        public ReportService(
            IBookRepository bookRepository,
            IMemberRepository memberRepository,
            IBorrowTransactionRepository transactionRepository,
            IFineRepository fineRepository,
            IBookRequestRepository requestRepository)
        {
            _bookRepository = bookRepository;
            _memberRepository = memberRepository;
            _transactionRepository = transactionRepository;
            _fineRepository = fineRepository;
            _requestRepository = requestRepository;
        }

        // Inventory Reports
        public async Task<int> GetTotalBooksCountAsync()
        {
            return await _bookRepository.GetTotalBooksCountAsync();
        }

        public async Task<int> GetAvailableBooksCountAsync()
        {
            var availableBooks = await _bookRepository.GetAvailableBooksAsync();
            return availableBooks.Count();
        }

        public async Task<int> GetBorrowedBooksCountAsync()
        {
            var activeTransactions = await _transactionRepository.GetActiveTransactionsAsync();
            return activeTransactions.Count();
        }

        // Member Reports
        public async Task<int> GetTotalMembersCountAsync()
        {
            var members = await _memberRepository.GetAllMembersAsync();
            return members.Count();
        }

        public async Task<int> GetActiveMembersCountAsync()
        {
            var activeMembers = await _memberRepository.GetMembersByStatusAsync(MemberStatus.Active);
            return activeMembers.Count();
        }

        // Transaction Reports
        public async Task<int> GetActiveTransactionsCountAsync()
        {
            var activeTransactions = await _transactionRepository.GetActiveTransactionsAsync();
            return activeTransactions.Count();
        }

        public async Task<int> GetOverdueTransactionsCountAsync()
        {
            var overdueTransactions = await _transactionRepository.GetOverdueTransactionsAsync();
            return overdueTransactions.Count();
        }

        // Fine Reports
        public async Task<decimal> GetTotalUnpaidFinesAmountAsync()
        {
            var unpaidFines = await _fineRepository.GetFinesByStatusAsync(FineStatus.Unpaid);
            return unpaidFines.Sum(f => f.Amount);
        }

        public async Task<int> GetUnpaidFinesCountAsync()
        {
            var unpaidFines = await _fineRepository.GetFinesByStatusAsync(FineStatus.Unpaid);
            return unpaidFines.Count();
        }

        // Request Reports
        public async Task<int> GetPendingRequestsCountAsync()
        {
            var pendingRequests = await _requestRepository.GetPendingRequestsAsync();
            return pendingRequests.Count();
        }

        public async Task<int> GetTotalRequestsCountAsync()
        {
            var requests = await _requestRepository.GetAllRequestsAsync();
            return requests.Count();
        }

        // Dashboard Summary
        public async Task<Dictionary<string, object>> GetDashboardSummaryAsync()
        {
            return new Dictionary<string, object>
            {
                { "TotalBooks", await GetTotalBooksCountAsync() },
                { "AvailableBooks", await GetAvailableBooksCountAsync() },
                { "BorrowedBooks", await GetBorrowedBooksCountAsync() },
                { "TotalMembers", await GetTotalMembersCountAsync() },
                { "ActiveMembers", await GetActiveMembersCountAsync() },
                { "ActiveTransactions", await GetActiveTransactionsCountAsync() },
                { "OverdueTransactions", await GetOverdueTransactionsCountAsync() },
                { "UnpaidFinesAmount", await GetTotalUnpaidFinesAmountAsync() },
                { "UnpaidFinesCount", await GetUnpaidFinesCountAsync() },
                { "PendingRequests", await GetPendingRequestsCountAsync() }
            };
        }
    }
}
