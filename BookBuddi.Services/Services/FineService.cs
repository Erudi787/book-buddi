using AutoMapper;
using BookBuddi.Data.Interfaces;
using BookBuddi.Data.Models;
using BookBuddi.Resources.Constants;
using BookBuddi.Services.Interfaces;
using BookBuddi.Services.ServiceModels;

namespace BookBuddi.Services.Services
{
    public class FineService : IFineService
    {
        private readonly IFineRepository _fineRepository;
        private readonly IBorrowTransactionRepository _transactionRepository;
        private readonly IMapper _mapper;

        public FineService(IFineRepository fineRepository, IBorrowTransactionRepository transactionRepository, IMapper mapper)
        {
            _fineRepository = fineRepository;
            _transactionRepository = transactionRepository;
            _mapper = mapper;
        }

        public IEnumerable<FineViewModel> GetAllFines()
        {
            var fines = _fineRepository.GetFines().ToList();
            return _mapper.Map<IEnumerable<FineViewModel>>(fines);
        }

        public FineViewModel? GetFineById(int fineId)
        {
            var fine = _fineRepository.GetFineById(fineId);
            return fine != null ? _mapper.Map<FineViewModel>(fine) : null;
        }

        public IEnumerable<FineViewModel> GetFinesByMember(int memberId)
        {
            var fines = _fineRepository.GetFinesByMember(memberId);
            return _mapper.Map<IEnumerable<FineViewModel>>(fines);
        }

        public IEnumerable<FineViewModel> GetFinesByStatus(FineStatus status)
        {
            var fines = _fineRepository.GetFinesByStatus(status);
            return _mapper.Map<IEnumerable<FineViewModel>>(fines);
        }

        public IEnumerable<FineViewModel> GetUnpaidFinesByMember(int memberId)
        {
            var fines = _fineRepository.GetUnpaidFinesByMember(memberId);
            return _mapper.Map<IEnumerable<FineViewModel>>(fines);
        }

        public decimal GetTotalUnpaidFinesByMember(int memberId)
        {
            return _fineRepository.GetTotalUnpaidFinesByMember(memberId);
        }

        public void AddFine(FineViewModel model, string createdBy)
        {
            var fine = _mapper.Map<Fine>(model);
            fine.IssueDate = DateTime.Now;
            fine.CreatedBy = createdBy;
            fine.CreatedTime = DateTime.Now;
            fine.UpdatedBy = createdBy;
            fine.UpdatedTime = DateTime.Now;

            _fineRepository.AddFine(fine);
        }

        public void PayFine(int fineId, string updatedBy)
        {
            var fine = _fineRepository.GetFineById(fineId);
            if (fine == null)
                throw new InvalidOperationException("Fine not found");

            if (fine.Status == FineStatus.Paid)
                throw new InvalidOperationException("Fine has already been paid");

            fine.Status = FineStatus.Paid;
            fine.PaymentDate = DateTime.Now;
            fine.UpdatedBy = updatedBy;
            fine.UpdatedTime = DateTime.Now;

            _fineRepository.UpdateFine(fine);
        }

        public void WaiveFine(int fineId, string updatedBy)
        {
            var fine = _fineRepository.GetFineById(fineId);
            if (fine == null)
                throw new InvalidOperationException("Fine not found");

            fine.Status = FineStatus.Waived;
            fine.UpdatedBy = updatedBy;
            fine.UpdatedTime = DateTime.Now;

            _fineRepository.UpdateFine(fine);
        }

        public void CalculateAndCreateOverdueFine(int transactionId, string createdBy)
        {
            var transaction = _transactionRepository.GetTransactionById(transactionId);
            if (transaction == null)
                throw new InvalidOperationException("Transaction not found");

            if (transaction.Status != TransactionStatus.Active && transaction.Status != TransactionStatus.Overdue)
                throw new InvalidOperationException("Transaction is not active or overdue");

            var daysOverdue = (DateTime.Now - transaction.DueDate).Days;
            if (daysOverdue <= 0)
                throw new InvalidOperationException("Transaction is not overdue");

            var fineAmount = daysOverdue * Const.DefaultOverdueFinePerDay;

            var fine = new Fine
            {
                TransactionId = transactionId,
                MemberId = transaction.MemberId,
                Amount = fineAmount,
                Status = FineStatus.Unpaid,
                Reason = FineReason.Overdue,
                IssueDate = DateTime.Now,
                Notes = $"Overdue by {daysOverdue} days",
                CreatedBy = createdBy,
                CreatedTime = DateTime.Now,
                UpdatedBy = createdBy,
                UpdatedTime = DateTime.Now
            };

            _fineRepository.AddFine(fine);
        }
    }
}
