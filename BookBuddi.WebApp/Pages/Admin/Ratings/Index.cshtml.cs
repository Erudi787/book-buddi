using BookBuddi.Services.Interfaces;
using BookBuddi.Services.ServiceModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookBuddi.WebApp.Pages.Admin.Ratings
{
    public class IndexModel : PageModel
    {
        private readonly IRatingService _ratingService;

        public IndexModel(IRatingService ratingService)
        {
            _ratingService = ratingService;
        }

        public List<RatingViewModel> AllRatings { get; set; } = new List<RatingViewModel>();
        public BookRatingStatsViewModel OverallStats { get; set; } = new BookRatingStatsViewModel();

        public async Task<IActionResult> OnGetAsync()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                return RedirectToPage("/Account/Login");
            }

            AllRatings = await _ratingService.GetAllRatingsAsync();
            OverallStats = await _ratingService.GetOverallRatingStatsAsync();

            return Page();
        }
    }
}
