using AutoMapper;
using BookBuddi.Data;
using BookBuddi.Data.Interfaces;
using BookBuddi.Data.Models;
using BookBuddi.Data.Repositories;
using BookBuddi.Services.Interfaces;
using BookBuddi.Services.ServiceModels;

namespace BookBuddi.Services.Services
{
    public class RatingService : IRatingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRatingRepository _ratingRepository;
        private readonly IMapper _mapper;

        public RatingService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _ratingRepository = new RatingRepository(unitOfWork);
            _mapper = mapper;
        }

        public async Task<RatingViewModel?> GetRatingAsync(int ratingId)
        {
            var ratings = await _ratingRepository.GetAllRatingsAsync();
            var rating = ratings.FirstOrDefault(r => r.RatingId == ratingId);
            return rating != null ? _mapper.Map<RatingViewModel>(rating) : null;
        }

        public async Task<RatingViewModel?> GetMemberRatingForBookAsync(int memberId, int bookId)
        {
            var rating = await _ratingRepository.GetRatingByMemberAndBookAsync(memberId, bookId);
            return rating != null ? _mapper.Map<RatingViewModel>(rating) : null;
        }

        public async Task<List<RatingViewModel>> GetMemberRatingsAsync(int memberId)
        {
            var ratings = await _ratingRepository.GetRatingsByMemberAsync(memberId);
            return _mapper.Map<List<RatingViewModel>>(ratings);
        }

        public async Task<BookRatingStatsViewModel> GetBookRatingStatsAsync(int bookId)
        {
            var averageRating = await _ratingRepository.GetAverageRatingAsync(bookId);
            var totalRatings = await _ratingRepository.GetRatingCountAsync(bookId);
            var allRatings = await _ratingRepository.GetAllRatingsAsync();
            var bookRatings = allRatings.Where(r => r.BookId == bookId).ToList();

            var stats = new BookRatingStatsViewModel
            {
                AverageRating = averageRating,
                TotalRatings = totalRatings,
                FiveStarCount = bookRatings.Count(r => r.Score == 5),
                FourStarCount = bookRatings.Count(r => r.Score == 4),
                ThreeStarCount = bookRatings.Count(r => r.Score == 3),
                TwoStarCount = bookRatings.Count(r => r.Score == 2),
                OneStarCount = bookRatings.Count(r => r.Score == 1)
            };

            if (totalRatings > 0)
            {
                stats.FiveStarPercent = Math.Round((double)stats.FiveStarCount / totalRatings * 100, 1);
                stats.FourStarPercent = Math.Round((double)stats.FourStarCount / totalRatings * 100, 1);
                stats.ThreeStarPercent = Math.Round((double)stats.ThreeStarCount / totalRatings * 100, 1);
                stats.TwoStarPercent = Math.Round((double)stats.TwoStarCount / totalRatings * 100, 1);
                stats.OneStarPercent = Math.Round((double)stats.OneStarCount / totalRatings * 100, 1);
            }

            return stats;
        }

        public async Task<bool> CreateRatingAsync(CreateRatingModel model, string currentUser)
        {
            try
            {
                // Check if user has already rated this book
                var existingRating = await _ratingRepository.GetRatingByMemberAndBookAsync(model.MemberId, model.BookId);
                if (existingRating != null)
                {
                    return false; // User has already rated this book
                }

                var rating = _mapper.Map<Rating>(model);
                rating.CreatedBy = currentUser;
                rating.CreatedTime = DateTime.Now;

                var dbSet = _unitOfWork.Database.Set<Rating>();
                await dbSet.AddAsync(rating);
                await _unitOfWork.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateRatingAsync(UpdateRatingModel model, string currentUser)
        {
            try
            {
                var ratings = await _ratingRepository.GetAllRatingsAsync();
                var rating = ratings.FirstOrDefault(r => r.RatingId == model.RatingId);
                
                if (rating == null)
                {
                    return false;
                }

                rating.Score = model.Score;
                rating.UpdatedBy = currentUser;
                rating.UpdatedTime = DateTime.Now;

                var dbSet = _unitOfWork.Database.Set<Rating>();
                dbSet.Update(rating);
                await _unitOfWork.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteRatingAsync(int ratingId)
        {
            try
            {
                var ratings = await _ratingRepository.GetAllRatingsAsync();
                var rating = ratings.FirstOrDefault(r => r.RatingId == ratingId);
                
                if (rating == null)
                {
                    return false;
                }

                var dbSet = _unitOfWork.Database.Set<Rating>();
                dbSet.Remove(rating);
                await _unitOfWork.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> HasUserRatedBookAsync(int memberId, int bookId)
        {
            return await _ratingRepository.HasUserRatedBookAsync(memberId, bookId);
        }

        public async Task<List<RatingViewModel>> GetAllRatingsAsync()
        {
            var ratings = await _ratingRepository.GetAllRatingsAsync();
            return _mapper.Map<List<RatingViewModel>>(ratings);
        }

        public async Task<BookRatingStatsViewModel> GetOverallRatingStatsAsync()
        {
            var allRatings = await _ratingRepository.GetAllRatingsAsync();
            var totalRatings = allRatings.Count;

            var stats = new BookRatingStatsViewModel
            {
                TotalRatings = totalRatings,
                FiveStarCount = allRatings.Count(r => r.Score == 5),
                FourStarCount = allRatings.Count(r => r.Score == 4),
                ThreeStarCount = allRatings.Count(r => r.Score == 3),
                TwoStarCount = allRatings.Count(r => r.Score == 2),
                OneStarCount = allRatings.Count(r => r.Score == 1)
            };

            if (totalRatings > 0)
            {
                stats.AverageRating = Math.Round(allRatings.Average(r => r.Score), 1);
                stats.FiveStarPercent = Math.Round((double)stats.FiveStarCount / totalRatings * 100, 1);
                stats.FourStarPercent = Math.Round((double)stats.FourStarCount / totalRatings * 100, 1);
                stats.ThreeStarPercent = Math.Round((double)stats.ThreeStarCount / totalRatings * 100, 1);
                stats.TwoStarPercent = Math.Round((double)stats.TwoStarCount / totalRatings * 100, 1);
                stats.OneStarPercent = Math.Round((double)stats.OneStarCount / totalRatings * 100, 1);
            }

            return stats;
        }
    }
}
