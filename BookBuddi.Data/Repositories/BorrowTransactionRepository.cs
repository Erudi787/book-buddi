using BookBuddi.Data.Interfaces;
using BookBuddi.Data.Models;
using BookBuddi.Resources.Constants;
using Microsoft.EntityFrameworkCore;

namespace BookBuddi.Data.Repositories
{
    public class BorrowTransactionRepository : BaseRepository, IBorrowTransactionRepository
    {
        public BorrowTransactionRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IQueryable<BorrowTransaction> GetTransactions()
        {
            return this.GetDbSet<BorrowTransaction>().OrderByDescending(t => t.BorrowDate);
        }

        public BorrowTransaction? GetTransactionById(int transactionId)
        {
            return this.GetDbSet<BorrowTransaction>()
                .FirstOrDefault(t => t.TransactionId == transactionId);
        }

        public IEnumerable<BorrowTransaction> GetTransactionsByMember(int memberId)
        {
            return this.GetDbSet<BorrowTransaction>()
                .Where(t => t.MemberId == memberId)
                .OrderByDescending(t => t.BorrowDate)
                .ToList();
        }

        public IEnumerable<BorrowTransaction> GetTransactionsByBook(int bookId)
        {
            return this.GetDbSet<BorrowTransaction>()
                .Where(t => t.BookId == bookId)
                .OrderByDescending(t => t.BorrowDate)
                .ToList();
        }

        public IEnumerable<BorrowTransaction> GetTransactionsByStatus(TransactionStatus status)
        {
            return this.GetDbSet<BorrowTransaction>()
                .Where(t => t.Status == status)
                .OrderByDescending(t => t.BorrowDate)
                .ToList();
        }

        public IEnumerable<BorrowTransaction> GetActiveTransactionsByMember(int memberId)
        {
            return this.GetDbSet<BorrowTransaction>()
                .Where(t => t.MemberId == memberId && t.Status == TransactionStatus.Active)
                .ToList();
        }

        public IEnumerable<BorrowTransaction> GetOverdueTransactions()
        {
            var today = DateTime.Today;
            return this.GetDbSet<BorrowTransaction>()
                .Where(t => t.Status == TransactionStatus.Active && t.DueDate < today)
                .ToList();
        }

        public bool HasActiveBorrow(int memberId, int bookId)
        {
            return this.GetDbSet<BorrowTransaction>()
                .Any(t => t.MemberId == memberId &&
                         t.BookId == bookId &&
                         t.Status == TransactionStatus.Active);
        }

        public void AddTransaction(BorrowTransaction transaction)
        {
            this.GetDbSet<BorrowTransaction>().Add(transaction);
            UnitOfWork.SaveChanges();
        }

        public void UpdateTransaction(BorrowTransaction transaction)
        {
            SetEntityState(transaction, EntityState.Modified);
            UnitOfWork.SaveChanges();
        }

        public void DeleteTransaction(BorrowTransaction transaction)
        {
            this.GetDbSet<BorrowTransaction>().Remove(transaction);
            UnitOfWork.SaveChanges();
        }
    }
}
