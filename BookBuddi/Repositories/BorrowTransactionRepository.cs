using Microsoft.EntityFrameworkCore;
using BookBuddi.Data;
using BookBuddi.Interfaces;
using BookBuddi.Models;
using BookBuddi.Models.Enums;

namespace BookBuddi.Repositories
{
    public class BorrowTransactionRepository : IBorrowTransactionRepository
    {
        private readonly ApplicationDbContext _context;

        public BorrowTransactionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BorrowTransaction>> GetAllTransactionsAsync()
        {
            return await _context.BorrowTransactions
                .Include(bt => bt.Book)
                .Include(bt => bt.Member)
                .OrderByDescending(bt => bt.BorrowDate)
                .ToListAsync();
        }

        public async Task<BorrowTransaction?> GetTransactionByIdAsync(int transactionId)
        {
            return await _context.BorrowTransactions
                .Include(bt => bt.Book)
                .Include(bt => bt.Member)
                .Include(bt => bt.Fine)
                .FirstOrDefaultAsync(bt => bt.TransactionId == transactionId);
        }

        public async Task<IEnumerable<BorrowTransaction>> GetTransactionsByMemberIdAsync(int memberId)
        {
            return await _context.BorrowTransactions
                .Include(bt => bt.Book)
                .Where(bt => bt.MemberId == memberId)
                .OrderByDescending(bt => bt.BorrowDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<BorrowTransaction>> GetTransactionsByBookIdAsync(int bookId)
        {
            return await _context.BorrowTransactions
                .Include(bt => bt.Member)
                .Where(bt => bt.BookId == bookId)
                .OrderByDescending(bt => bt.BorrowDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<BorrowTransaction>> GetActiveTransactionsAsync()
        {
            return await _context.BorrowTransactions
                .Include(bt => bt.Book)
                .Include(bt => bt.Member)
                .Where(bt => bt.Status == TransactionStatus.Active)
                .OrderBy(bt => bt.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<BorrowTransaction>> GetOverdueTransactionsAsync()
        {
            var today = DateTime.Now.Date;
            return await _context.BorrowTransactions
                .Include(bt => bt.Book)
                .Include(bt => bt.Member)
                .Where(bt => bt.Status == TransactionStatus.Active && bt.DueDate < today)
                .OrderBy(bt => bt.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<BorrowTransaction>> GetTransactionsByStatusAsync(TransactionStatus status)
        {
            return await _context.BorrowTransactions
                .Include(bt => bt.Book)
                .Include(bt => bt.Member)
                .Where(bt => bt.Status == status)
                .OrderByDescending(bt => bt.BorrowDate)
                .ToListAsync();
        }

        public async Task<BorrowTransaction> AddTransactionAsync(BorrowTransaction transaction)
        {
            _context.BorrowTransactions.Add(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task UpdateTransactionAsync(BorrowTransaction transaction)
        {
            _context.BorrowTransactions.Update(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTransactionAsync(int transactionId)
        {
            var transaction = await _context.BorrowTransactions.FindAsync(transactionId);
            if (transaction != null)
            {
                _context.BorrowTransactions.Remove(transaction);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> HasActiveBorrowAsync(int memberId, int bookId)
        {
            return await _context.BorrowTransactions
                .AnyAsync(bt => bt.MemberId == memberId &&
                               bt.BookId == bookId &&
                               bt.Status == TransactionStatus.Active);
        }
    }
}
