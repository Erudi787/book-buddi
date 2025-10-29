using BookBuddi.Services.ServiceModels;
using BookBuddi.Resources.Constants;

namespace BookBuddi.Services.Interfaces
{
    public interface IBorrowingService
    {
        IEnumerable<BorrowTransactionViewModel> GetAllTransactions();
        BorrowTransactionViewModel? GetTransactionById(int transactionId);
        IEnumerable<BorrowTransactionViewModel> GetTransactionsByMember(int memberId);
        IEnumerable<BorrowTransactionViewModel> GetTransactionsByBook(int bookId);
        IEnumerable<BorrowTransactionViewModel> GetTransactionsByStatus(TransactionStatus status);
        IEnumerable<BorrowTransactionViewModel> GetActiveTransactionsByMember(int memberId);
        IEnumerable<BorrowTransactionViewModel> GetOverdueTransactions();
        bool CanBorrowBook(int memberId, int bookId, out string errorMessage);
        void BorrowBook(int memberId, int bookId, string createdBy);
        void ReturnBook(int transactionId, string updatedBy);
        void ProcessOverdueTransactions();
    }
}
