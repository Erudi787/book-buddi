using Microsoft.EntityFrameworkCore;
using BookBuddi.Data;
using BookBuddi.Interfaces;
using BookBuddi.Models;
using BookBuddi.Models.Enums;

namespace BookBuddi.Repositories
{
    public class BookRequestRepository : IBookRequestRepository
    {
        private readonly ApplicationDbContext _context;

        public BookRequestRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BookRequest>> GetAllRequestsAsync()
        {
            return await _context.BookRequests
                .Include(br => br.Member)
                .OrderByDescending(br => br.RequestDate)
                .ToListAsync();
        }

        public async Task<BookRequest?> GetRequestByIdAsync(int requestId)
        {
            return await _context.BookRequests
                .Include(br => br.Member)
                .FirstOrDefaultAsync(br => br.RequestId == requestId);
        }

        public async Task<IEnumerable<BookRequest>> GetRequestsByMemberIdAsync(int memberId)
        {
            return await _context.BookRequests
                .Where(br => br.MemberId == memberId)
                .OrderByDescending(br => br.RequestDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<BookRequest>> GetRequestsByStatusAsync(RequestStatus status)
        {
            return await _context.BookRequests
                .Include(br => br.Member)
                .Where(br => br.Status == status)
                .OrderByDescending(br => br.RequestDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<BookRequest>> GetPendingRequestsAsync()
        {
            return await _context.BookRequests
                .Include(br => br.Member)
                .Where(br => br.Status == RequestStatus.Pending)
                .OrderBy(br => br.RequestDate)
                .ToListAsync();
        }

        public async Task<BookRequest> AddRequestAsync(BookRequest request)
        {
            _context.BookRequests.Add(request);
            await _context.SaveChangesAsync();
            return request;
        }

        public async Task UpdateRequestAsync(BookRequest request)
        {
            _context.BookRequests.Update(request);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteRequestAsync(int requestId)
        {
            var request = await _context.BookRequests.FindAsync(requestId);
            if (request != null)
            {
                _context.BookRequests.Remove(request);
                await _context.SaveChangesAsync();
            }
        }
    }
}
