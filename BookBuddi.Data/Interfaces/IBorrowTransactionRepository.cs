using BookBuddi.Data.Models;
using BookBuddi.Resources.Constants;

namespace BookBuddi.Data.Interfaces
{
    public interface IBorrowTransactionRepository
    {
        IQueryable<BorrowTransaction> GetTransactions();
        BorrowTransaction? GetTransactionById(int transactionId);
        IEnumerable<BorrowTransaction> GetTransactionsByMember(int memberId);
        IEnumerable<BorrowTransaction> GetTransactionsByBook(int bookId);
        IEnumerable<BorrowTransaction> GetTransactionsByStatus(TransactionStatus status);
        IEnumerable<BorrowTransaction> GetActiveTransactionsByMember(int memberId);
        IEnumerable<BorrowTransaction> GetOverdueTransactions();
        bool HasActiveBorrow(int memberId, int bookId);
        void AddTransaction(BorrowTransaction transaction);
        void UpdateTransaction(BorrowTransaction transaction);
        void DeleteTransaction(BorrowTransaction transaction);
    }
}
