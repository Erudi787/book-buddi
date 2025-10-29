using BookBuddi.Data.Models;
using BookBuddi.Resources.Constants;

namespace BookBuddi.Data.Interfaces
{
    public interface IFineRepository
    {
        IQueryable<Fine> GetFines();
        Fine? GetFineById(int fineId);
        IEnumerable<Fine> GetFinesByMember(int memberId);
        IEnumerable<Fine> GetFinesByStatus(FineStatus status);
        IEnumerable<Fine> GetUnpaidFinesByMember(int memberId);
        decimal GetTotalUnpaidFinesByMember(int memberId);
        void AddFine(Fine fine);
        void UpdateFine(Fine fine);
        void DeleteFine(Fine fine);
    }
}
