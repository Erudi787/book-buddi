using BookBuddi.Data.Interfaces;
using BookBuddi.Data.Models;
using BookBuddi.Resources.Constants;
using Microsoft.EntityFrameworkCore;

namespace BookBuddi.Data.Repositories
{
    public class BookRequestRepository : BaseRepository, IBookRequestRepository
    {
        public BookRequestRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IQueryable<BookRequest> GetRequests()
        {
            return this.GetDbSet<BookRequest>().OrderByDescending(r => r.RequestDate);
        }

        public BookRequest? GetRequestById(int requestId)
        {
            return this.GetDbSet<BookRequest>().FirstOrDefault(r => r.RequestId == requestId);
        }

        public IEnumerable<BookRequest> GetRequestsByMember(int memberId)
        {
            return this.GetDbSet<BookRequest>()
                .Where(r => r.MemberId == memberId)
                .OrderByDescending(r => r.RequestDate)
                .ToList();
        }

        public IEnumerable<BookRequest> GetRequestsByStatus(RequestStatus status)
        {
            return this.GetDbSet<BookRequest>()
                .Where(r => r.Status == status)
                .OrderByDescending(r => r.RequestDate)
                .ToList();
        }

        public void AddRequest(BookRequest request)
        {
            this.GetDbSet<BookRequest>().Add(request);
            UnitOfWork.SaveChanges();
        }

        public void UpdateRequest(BookRequest request)
        {
            SetEntityState(request, EntityState.Modified);
            UnitOfWork.SaveChanges();
        }

        public void DeleteRequest(BookRequest request)
        {
            this.GetDbSet<BookRequest>().Remove(request);
            UnitOfWork.SaveChanges();
        }
    }
}
