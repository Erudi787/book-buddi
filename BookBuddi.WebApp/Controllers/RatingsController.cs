using Microsoft.AspNetCore.Mvc;
using BookBuddi.Services.Interfaces;
using BookBuddi.Services.ServiceModels;

namespace BookBuddi.WebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RatingsController : ControllerBase
    {
        private readonly IRatingService _ratingService;

        public RatingsController(IRatingService ratingService)
        {
            _ratingService = ratingService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRating([FromBody] CreateRatingModel model)
        {
            // Get MemberId from session
            var memberId = HttpContext.Session.GetInt32("MemberId");
            if (!memberId.HasValue)
            {
                return Unauthorized("User not logged in");
            }

            model.MemberId = memberId.Value;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentUser = HttpContext.Session.GetString("MemberEmail") ?? "Unknown";
            var result = await _ratingService.CreateRatingAsync(model, currentUser);

            if (result)
            {
                return Ok(new { message = "Rating created successfully" });
            }

            return BadRequest("Failed to create rating. You may have already rated this book.");
        }

        [HttpPut("{ratingId}")]
        public async Task<IActionResult> UpdateRating(int ratingId, [FromBody] UpdateRatingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            model.RatingId = ratingId;
            var currentUser = HttpContext.Session.GetString("MemberEmail") ?? "Unknown";
            var result = await _ratingService.UpdateRatingAsync(model, currentUser);

            if (result)
            {
                return Ok(new { message = "Rating updated successfully" });
            }

            return BadRequest("Failed to update rating");
        }

        [HttpDelete("{ratingId}")]
        public async Task<IActionResult> DeleteRating(int ratingId)
        {
            var result = await _ratingService.DeleteRatingAsync(ratingId);

            if (result)
            {
                return Ok(new { message = "Rating deleted successfully" });
            }

            return BadRequest("Failed to delete rating");
        }

        [HttpGet("book/{bookId}")]
        public async Task<IActionResult> GetBookRatings(int bookId)
        {
            var stats = await _ratingService.GetBookRatingStatsAsync(bookId);
            return Ok(stats);
        }
    }
}
