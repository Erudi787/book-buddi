using BookBuddi.Interfaces;
using BookBuddi.Models;
using BookBuddi.Models.Enums;

namespace BookBuddi.Services
{
    public class FineService
    {
        private readonly IFineRepository _fineRepository;
        private readonly INotificationRepository _notificationRepository;

        public FineService(IFineRepository fineRepository, INotificationRepository notificationRepository)
        {
            _fineRepository = fineRepository;
            _notificationRepository = notificationRepository;
        }

        public async Task<IEnumerable<Fine>> GetAllFinesAsync()
        {
            return await _fineRepository.GetAllFinesAsync();
        }

        public async Task<Fine?> GetFineByIdAsync(int fineId)
        {
            return await _fineRepository.GetFineByIdAsync(fineId);
        }

        public async Task<IEnumerable<Fine>> GetMemberFinesAsync(int memberId)
        {
            return await _fineRepository.GetFinesByMemberIdAsync(memberId);
        }

        public async Task<IEnumerable<Fine>> GetUnpaidFinesAsync()
        {
            return await _fineRepository.GetFinesByStatusAsync(FineStatus.Unpaid);
        }

        public async Task<IEnumerable<Fine>> GetMemberUnpaidFinesAsync(int memberId)
        {
            return await _fineRepository.GetUnpaidFinesByMemberIdAsync(memberId);
        }

        public async Task<decimal> GetMemberTotalUnpaidFinesAsync(int memberId)
        {
            return await _fineRepository.GetTotalUnpaidFinesForMemberAsync(memberId);
        }

        public async Task<Fine> CreateFineAsync(
            int transactionId,
            int memberId,
            decimal amount,
            FineReason reason,
            string? notes = null)
        {
            var fine = new Fine
            {
                TransactionId = transactionId,
                MemberId = memberId,
                Amount = amount,
                Reason = reason,
                Status = FineStatus.Unpaid,
                IssueDate = DateTime.Now,
                Notes = notes
            };

            var createdFine = await _fineRepository.AddFineAsync(fine);

            // Create notification
            var notification = new Notification
            {
                MemberId = memberId,
                Type = NotificationType.FineIssued,
                Title = "Fine Issued",
                Message = $"A fine of ${amount:F2} has been issued. Reason: {reason}",
                RelatedEntityId = createdFine.FineId,
                RelatedEntityType = "Fine"
            };

            await _notificationRepository.AddNotificationAsync(notification);

            return createdFine;
        }

        public async Task MarkFineAsPaidAsync(int fineId)
        {
            var fine = await _fineRepository.GetFineByIdAsync(fineId);
            if (fine == null)
            {
                throw new InvalidOperationException("Fine not found.");
            }

            if (fine.Status == FineStatus.Paid)
            {
                throw new InvalidOperationException("Fine has already been paid.");
            }

            fine.Status = FineStatus.Paid;
            fine.PaymentDate = DateTime.Now;

            await _fineRepository.UpdateFineAsync(fine);
        }

        public async Task WaiveFineAsync(int fineId, string reason)
        {
            var fine = await _fineRepository.GetFineByIdAsync(fineId);
            if (fine == null)
            {
                throw new InvalidOperationException("Fine not found.");
            }

            if (fine.Status == FineStatus.Paid)
            {
                throw new InvalidOperationException("Cannot waive a paid fine.");
            }

            fine.Status = FineStatus.Waived;
            fine.Notes = $"Waived: {reason}";

            await _fineRepository.UpdateFineAsync(fine);
        }
    }
}
