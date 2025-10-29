using BookBuddi.Data.Models;
using BookBuddi.Resources.Constants;

namespace BookBuddi.Data.Interfaces
{
    public interface IBookRequestRepository
    {
        IQueryable<BookRequest> GetRequests();
        BookRequest? GetRequestById(int requestId);
        IEnumerable<BookRequest> GetRequestsByMember(int memberId);
        IEnumerable<BookRequest> GetRequestsByStatus(RequestStatus status);
        void AddRequest(BookRequest request);
        void UpdateRequest(BookRequest request);
        void DeleteRequest(BookRequest request);
    }
}
