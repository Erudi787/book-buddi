using BookBuddi.Data.Interfaces;
using BookBuddi.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BookBuddi.Data.Repositories
{
    public class RatingRepository : BaseRepository, IRatingRepository
    {
        public RatingRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<Rating?> GetRatingByMemberAndBookAsync(int memberId, int bookId)
        {
            return await GetDbSet<Rating>()
                .Include(r => r.Book)
                .Include(r => r.Member)
                .FirstOrDefaultAsync(r => r.MemberId == memberId && r.BookId == bookId);
        }

        public async Task<double> GetAverageRatingAsync(int bookId)
        {
            var ratings = await GetDbSet<Rating>()
                .Where(r => r.BookId == bookId)
                .ToListAsync();

            if (!ratings.Any())
                return 0;

            return Math.Round(ratings.Average(r => r.Score), 1);
        }

        public async Task<int> GetRatingCountAsync(int bookId)
        {
            return await GetDbSet<Rating>()
                .Where(r => r.BookId == bookId)
                .CountAsync();
        }

        public async Task<List<Rating>> GetRatingsByMemberAsync(int memberId)
        {
            return await GetDbSet<Rating>()
                .Include(r => r.Book)
                .Include(r => r.Member)
                .Where(r => r.MemberId == memberId)
                .OrderByDescending(r => r.CreatedTime)
                .ToListAsync();
        }

        public async Task<List<Rating>> GetAllRatingsAsync()
        {
            return await GetDbSet<Rating>()
                .Include(r => r.Book)
                .Include(r => r.Member)
                .OrderByDescending(r => r.CreatedTime)
                .ToListAsync();
        }

        public async Task<bool> HasUserRatedBookAsync(int memberId, int bookId)
        {
            return await GetDbSet<Rating>()
                .AnyAsync(r => r.MemberId == memberId && r.BookId == bookId);
        }

        public async Task AddAsync(Rating rating)
        {
            await GetDbSet<Rating>().AddAsync(rating);
        }

        public async Task UpdateAsync(Rating rating)
        {
            GetDbSet<Rating>().Update(rating);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(int ratingId)
        {
            var rating = await GetDbSet<Rating>().FindAsync(ratingId);
            if (rating != null)
            {
                GetDbSet<Rating>().Remove(rating);
            }
        }
    }
}
