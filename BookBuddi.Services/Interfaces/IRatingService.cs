using BookBuddi.Services.ServiceModels;

namespace BookBuddi.Services.Interfaces
{
    public interface IRatingService
    {
        Task<RatingViewModel?> GetRatingAsync(int ratingId);
        Task<RatingViewModel?> GetMemberRatingForBookAsync(int memberId, int bookId);
        Task<List<RatingViewModel>> GetMemberRatingsAsync(int memberId);
        Task<BookRatingStatsViewModel> GetBookRatingStatsAsync(int bookId);
        Task<bool> CreateRatingAsync(CreateRatingModel model, string currentUser);
        Task<bool> UpdateRatingAsync(UpdateRatingModel model, string currentUser);
        Task<bool> DeleteRatingAsync(int ratingId);
        Task<bool> HasUserRatedBookAsync(int memberId, int bookId);
        Task<List<RatingViewModel>> GetAllRatingsAsync();
        Task<BookRatingStatsViewModel> GetOverallRatingStatsAsync();
    }
}
