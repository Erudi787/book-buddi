using BookBuddi.Services.Interfaces;
using BookBuddi.Services.ServiceModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookBuddi.WebApp.Pages.Ratings
{
    public class MyRatingsModel : PageModel
    {
        private readonly IRatingService _ratingService;
        private readonly IMemberService _memberService;

        public MyRatingsModel(IRatingService ratingService, IMemberService memberService)
        {
            _ratingService = ratingService;
            _memberService = memberService;
        }

        public List<RatingViewModel> MyRatings { get; set; } = new List<RatingViewModel>();
        public string MemberName { get; set; } = string.Empty;
        public double AverageRating { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Member")
            {
                return RedirectToPage("/Account/Login");
            }

            var memberId = HttpContext.Session.GetInt32("MemberId");
            if (!memberId.HasValue)
            {
                return RedirectToPage("/Account/Login");
            }

            var memberName = HttpContext.Session.GetString("MemberName");
            MemberName = memberName ?? "Member";

            MyRatings = await _ratingService.GetMemberRatingsAsync(memberId.Value);

            if (MyRatings.Any())
            {
                AverageRating = Math.Round(MyRatings.Average(r => r.Score), 1);
            }

            return Page();
        }
    }
}
