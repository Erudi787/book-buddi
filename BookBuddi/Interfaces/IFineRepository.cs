using BookBuddi.Models;
using BookBuddi.Models.Enums;

namespace BookBuddi.Interfaces
{
    public interface IFineRepository
    {
        Task<IEnumerable<Fine>> GetAllFinesAsync();
        Task<Fine?> GetFineByIdAsync(int fineId);
        Task<IEnumerable<Fine>> GetFinesByMemberIdAsync(int memberId);
        Task<IEnumerable<Fine>> GetFinesByStatusAsync(FineStatus status);
        Task<IEnumerable<Fine>> GetUnpaidFinesByMemberIdAsync(int memberId);
        Task<decimal> GetTotalUnpaidFinesForMemberAsync(int memberId);

        Task<Fine> AddFineAsync(Fine fine);
        Task UpdateFineAsync(Fine fine);
        Task DeleteFineAsync(int fineId);
    }
}