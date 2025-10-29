using BookBuddi.Data.Interfaces;
using BookBuddi.Data.Models;
using BookBuddi.Resources.Constants;
using Microsoft.EntityFrameworkCore;

namespace BookBuddi.Data.Repositories
{
    public class FineRepository : BaseRepository, IFineRepository
    {
        public FineRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IQueryable<Fine> GetFines()
        {
            return this.GetDbSet<Fine>().OrderByDescending(f => f.IssueDate);
        }

        public Fine? GetFineById(int fineId)
        {
            return this.GetDbSet<Fine>().FirstOrDefault(f => f.FineId == fineId);
        }

        public IEnumerable<Fine> GetFinesByMember(int memberId)
        {
            return this.GetDbSet<Fine>()
                .Where(f => f.MemberId == memberId)
                .OrderByDescending(f => f.IssueDate)
                .ToList();
        }

        public IEnumerable<Fine> GetFinesByStatus(FineStatus status)
        {
            return this.GetDbSet<Fine>()
                .Where(f => f.Status == status)
                .OrderByDescending(f => f.IssueDate)
                .ToList();
        }

        public IEnumerable<Fine> GetUnpaidFinesByMember(int memberId)
        {
            return this.GetDbSet<Fine>()
                .Where(f => f.MemberId == memberId && f.Status == FineStatus.Unpaid)
                .OrderByDescending(f => f.IssueDate)
                .ToList();
        }

        public decimal GetTotalUnpaidFinesByMember(int memberId)
        {
            return this.GetDbSet<Fine>()
                .Where(f => f.MemberId == memberId && f.Status == FineStatus.Unpaid)
                .Sum(f => (decimal?)f.Amount) ?? 0;
        }

        public void AddFine(Fine fine)
        {
            this.GetDbSet<Fine>().Add(fine);
            UnitOfWork.SaveChanges();
        }

        public void UpdateFine(Fine fine)
        {
            SetEntityState(fine, EntityState.Modified);
            UnitOfWork.SaveChanges();
        }

        public void DeleteFine(Fine fine)
        {
            this.GetDbSet<Fine>().Remove(fine);
            UnitOfWork.SaveChanges();
        }
    }
}
