using BookBuddi.Services.ServiceModels;
using BookBuddi.Resources.Constants;

namespace BookBuddi.Services.Interfaces
{
    public interface IBookRequestService
    {
        IEnumerable<BookRequestViewModel> GetAllRequests();
        BookRequestViewModel? GetRequestById(int requestId);
        IEnumerable<BookRequestViewModel> GetRequestsByMember(int memberId);
        IEnumerable<BookRequestViewModel> GetRequestsByStatus(RequestStatus status);
        void AddRequest(BookRequestViewModel model, string createdBy);
        void ApproveRequest(int requestId, string adminNotes, string updatedBy);
        void RejectRequest(int requestId, string adminNotes, string updatedBy);
        void CancelRequest(int requestId, string updatedBy);
    }
}
