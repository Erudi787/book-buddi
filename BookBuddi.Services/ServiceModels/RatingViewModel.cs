using System;
using System.ComponentModel.DataAnnotations;

namespace BookBuddi.Services.ServiceModels
{
    public class RatingViewModel
    {
        public int RatingId { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public string? BookCoverUrl { get; set; }
        public int MemberId { get; set; }
        public string MemberName { get; set; } = string.Empty;
        public int Score { get; set; }
        public DateTime RatedDate { get; set; }
    }

    public class BookRatingStatsViewModel
    {
        public double AverageRating { get; set; }
        public int TotalRatings { get; set; }
        public int FiveStarCount { get; set; }
        public int FourStarCount { get; set; }
        public int ThreeStarCount { get; set; }
        public int TwoStarCount { get; set; }
        public int OneStarCount { get; set; }
        public double FiveStarPercent { get; set; }
        public double FourStarPercent { get; set; }
        public double ThreeStarPercent { get; set; }
        public double TwoStarPercent { get; set; }
        public double OneStarPercent { get; set; }
    }

    public class CreateRatingModel
    {
        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5 stars")]
        public int Score { get; set; }

        [Required]
        public int BookId { get; set; }

        [Required]
        public int MemberId { get; set; }
    }

    public class UpdateRatingModel
    {
        [Required]
        public int RatingId { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5 stars")]
        public int Score { get; set; }
    }
}
