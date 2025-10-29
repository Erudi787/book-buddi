using BookBuddi.Services.ServiceModels;
using BookBuddi.Resources.Constants;

namespace BookBuddi.Services.Interfaces
{
    public interface IFineService
    {
        IEnumerable<FineViewModel> GetAllFines();
        FineViewModel? GetFineById(int fineId);
        IEnumerable<FineViewModel> GetFinesByMember(int memberId);
        IEnumerable<FineViewModel> GetFinesByStatus(FineStatus status);
        IEnumerable<FineViewModel> GetUnpaidFinesByMember(int memberId);
        decimal GetTotalUnpaidFinesByMember(int memberId);
        void AddFine(FineViewModel model, string createdBy);
        void PayFine(int fineId, string updatedBy);
        void WaiveFine(int fineId, string updatedBy);
        void CalculateAndCreateOverdueFine(int transactionId, string createdBy);
    }
}
