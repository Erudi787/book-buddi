using Microsoft.EntityFrameworkCore;
using BookBuddi.Data;
using BookBuddi.Interfaces;
using BookBuddi.Models;
using BookBuddi.Models.Enums;

namespace BookBuddi.Repositories
{
    public class FineRepository : IFineRepository
    {
        private readonly ApplicationDbContext _context;

        public FineRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Fine>> GetAllFinesAsync()
        {
            return await _context.Fines
                .Include(f => f.Member)
                .Include(f => f.BorrowTransaction)
                    .ThenInclude(bt => bt.Book)
                .OrderByDescending(f => f.IssueDate)
                .ToListAsync();
        }

        public async Task<Fine?> GetFineByIdAsync(int fineId)
        {
            return await _context.Fines
                .Include(f => f.Member)
                .Include(f => f.BorrowTransaction)
                    .ThenInclude(bt => bt.Book)
                .FirstOrDefaultAsync(f => f.FineId == fineId);
        }

        public async Task<IEnumerable<Fine>> GetFinesByMemberIdAsync(int memberId)
        {
            return await _context.Fines
                .Include(f => f.BorrowTransaction)
                    .ThenInclude(bt => bt.Book)
                .Where(f => f.MemberId == memberId)
                .OrderByDescending(f => f.IssueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Fine>> GetFinesByStatusAsync(FineStatus status)
        {
            return await _context.Fines
                .Include(f => f.Member)
                .Include(f => f.BorrowTransaction)
                    .ThenInclude(bt => bt.Book)
                .Where(f => f.Status == status)
                .OrderByDescending(f => f.IssueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Fine>> GetUnpaidFinesByMemberIdAsync(int memberId)
        {
            return await _context.Fines
                .Include(f => f.BorrowTransaction)
                    .ThenInclude(bt => bt.Book)
                .Where(f => f.MemberId == memberId && f.Status == FineStatus.Unpaid)
                .OrderByDescending(f => f.IssueDate)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalUnpaidFinesForMemberAsync(int memberId)
        {
            return await _context.Fines
                .Where(f => f.MemberId == memberId && f.Status == FineStatus.Unpaid)
                .SumAsync(f => f.Amount);
        }

        public async Task<Fine> AddFineAsync(Fine fine)
        {
            _context.Fines.Add(fine);
            await _context.SaveChangesAsync();
            return fine;
        }

        public async Task UpdateFineAsync(Fine fine)
        {
            _context.Fines.Update(fine);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteFineAsync(int fineId)
        {
            var fine = await _context.Fines.FindAsync(fineId);
            if (fine != null)
            {
                _context.Fines.Remove(fine);
                await _context.SaveChangesAsync();
            }
        }
    }
}
