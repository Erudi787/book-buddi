using BookBuddi.Models;
using BookBuddi.Models.Enums;

namespace BookBuddi.Interfaces
{
    public interface IBookRequestRepository
    {
        Task<IEnumerable<BookRequest>> GetAllRequestsAsync();
        Task<BookRequest?> GetRequestByIdAsync(int requestId);
        Task<IEnumerable<BookRequest>> GetRequestsByMemberIdAsync(int memberId);
        Task<IEnumerable<BookRequest>> GetRequestsByStatusAsync(RequestStatus status);
        Task<IEnumerable<BookRequest>> GetPendingRequestsAsync();

        Task<BookRequest> AddRequestAsync(BookRequest request);
        Task UpdateRequestAsync(BookRequest request);
        Task DeleteRequestAsync(int requestId);
    }
}