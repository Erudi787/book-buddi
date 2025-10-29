using AutoMapper;
using BookBuddi.Data.Interfaces;
using BookBuddi.Data.Models;
using BookBuddi.Resources.Constants;
using BookBuddi.Services.Interfaces;
using BookBuddi.Services.ServiceModels;

namespace BookBuddi.Services.Services
{
    public class BookRequestService : IBookRequestService
    {
        private readonly IBookRequestRepository _requestRepository;
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;

        public BookRequestService(IBookRequestRepository requestRepository, INotificationService notificationService, IMapper mapper)
        {
            _requestRepository = requestRepository;
            _notificationService = notificationService;
            _mapper = mapper;
        }

        public IEnumerable<BookRequestViewModel> GetAllRequests()
        {
            var requests = _requestRepository.GetRequests().ToList();
            return _mapper.Map<IEnumerable<BookRequestViewModel>>(requests);
        }

        public BookRequestViewModel? GetRequestById(int requestId)
        {
            var request = _requestRepository.GetRequestById(requestId);
            return request != null ? _mapper.Map<BookRequestViewModel>(request) : null;
        }

        public IEnumerable<BookRequestViewModel> GetRequestsByMember(int memberId)
        {
            var requests = _requestRepository.GetRequestsByMember(memberId);
            return _mapper.Map<IEnumerable<BookRequestViewModel>>(requests);
        }

        public IEnumerable<BookRequestViewModel> GetRequestsByStatus(RequestStatus status)
        {
            var requests = _requestRepository.GetRequestsByStatus(status);
            return _mapper.Map<IEnumerable<BookRequestViewModel>>(requests);
        }

        public void AddRequest(BookRequestViewModel model, string createdBy)
        {
            var request = _mapper.Map<BookRequest>(model);
            request.RequestDate = DateTime.Now;
            request.Status = RequestStatus.Pending;
            request.CreatedBy = createdBy;
            request.CreatedTime = DateTime.Now;
            request.UpdatedBy = createdBy;
            request.UpdatedTime = DateTime.Now;

            _requestRepository.AddRequest(request);
        }

        public void ApproveRequest(int requestId, string adminNotes, string updatedBy)
        {
            var request = _requestRepository.GetRequestById(requestId);
            if (request == null)
                throw new InvalidOperationException("Request not found");

            if (request.Status != RequestStatus.Pending)
                throw new InvalidOperationException("Request has already been processed");

            request.Status = RequestStatus.Approved;
            request.AdminNotes = adminNotes;
            request.StatusUpdateDate = DateTime.Now;
            request.UpdatedBy = updatedBy;
            request.UpdatedTime = DateTime.Now;

            _requestRepository.UpdateRequest(request);

            // Notify member
            _notificationService.CreateRequestUpdateNotification(request.MemberId, requestId, RequestStatus.Approved);
        }

        public void RejectRequest(int requestId, string adminNotes, string updatedBy)
        {
            var request = _requestRepository.GetRequestById(requestId);
            if (request == null)
                throw new InvalidOperationException("Request not found");

            if (request.Status != RequestStatus.Pending)
                throw new InvalidOperationException("Request has already been processed");

            request.Status = RequestStatus.Rejected;
            request.AdminNotes = adminNotes;
            request.StatusUpdateDate = DateTime.Now;
            request.UpdatedBy = updatedBy;
            request.UpdatedTime = DateTime.Now;

            _requestRepository.UpdateRequest(request);

            // Notify member
            _notificationService.CreateRequestUpdateNotification(request.MemberId, requestId, RequestStatus.Rejected);
        }

        public void CancelRequest(int requestId, string updatedBy)
        {
            var request = _requestRepository.GetRequestById(requestId);
            if (request == null)
                throw new InvalidOperationException("Request not found");

            if (request.Status != RequestStatus.Pending)
                throw new InvalidOperationException("Only pending requests can be cancelled");

            request.Status = RequestStatus.Cancelled;
            request.StatusUpdateDate = DateTime.Now;
            request.UpdatedBy = updatedBy;
            request.UpdatedTime = DateTime.Now;

            _requestRepository.UpdateRequest(request);
        }
    }
}
