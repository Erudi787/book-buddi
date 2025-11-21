using BookBuddi.Data.Models;

namespace BookBuddi.Data.Interfaces
{
    public interface IRatingRepository
    {
        Task<Rating?> GetRatingByMemberAndBookAsync(int memberId, int bookId);
        Task<double> GetAverageRatingAsync(int bookId);
        Task<int> GetRatingCountAsync(int bookId);
        Task<List<Rating>> GetRatingsByMemberAsync(int memberId);
        Task<List<Rating>> GetAllRatingsAsync();
        Task<bool> HasUserRatedBookAsync(int memberId, int bookId);
        Task AddAsync(Rating rating);
        Task UpdateAsync(Rating rating);
        Task DeleteAsync(int ratingId);
    }
}
