using BookBuddi.Models;
using BookBuddi.Models.Enums;

namespace BookBuddi.Interfaces
{
    public interface IBorrowTransactionRepository
    {
        Task<IEnumerable<BorrowTransaction>> GetAllTransactionsAsync();
        Task<BorrowTransaction?> GetTransactionByIdAsync(int transactionId);
        Task<IEnumerable<BorrowTransaction>> GetTransactionsByMemberIdAsync(int memberId);
        Task<IEnumerable<BorrowTransaction>> GetTransactionsByBookIdAsync(int bookId);
        Task<IEnumerable<BorrowTransaction>> GetTransactionsByStatusAsync(TransactionStatus status);
        Task<IEnumerable<BorrowTransaction>> GetActiveTransactionsAsync();
        Task<IEnumerable<BorrowTransaction>> GetOverdueTransactionsAsync();

        Task<BorrowTransaction> AddTransactionAsync(BorrowTransaction transaction);
        Task UpdateTransactionAsync(BorrowTransaction transaction);
        Task DeleteTransactionAsync(int transactionId);

        Task<bool> HasActiveBorrowAsync(int memberId, int bookId);
    }
}