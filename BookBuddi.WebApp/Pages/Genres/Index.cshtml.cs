using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Services.Interfaces;
using BookBuddi.Services.ServiceModels;

namespace BookBuddi.Pages.Genres
{
    public class IndexModel : PageModel
    {
        private readonly IGenreService _genreService;

        public IndexModel(IGenreService genreService)
        {
            _genreService = genreService;
        }

        public IEnumerable<GenreViewModel> Genres { get; set; } = new List<GenreViewModel>();
        public string? SearchTerm { get; set; }

        public IActionResult OnGet(string? searchTerm)
        {
            // Admin authorization check
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                TempData["ErrorMessage"] = "You must be logged in as an administrator to access this page.";
                return RedirectToPage(string.IsNullOrEmpty(userRole) ? "/Admin/Login" : "/Admin/AccessDenied");
            }

            SearchTerm = searchTerm;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                Genres = _genreService.SearchGenres(searchTerm);
            }
            else
            {
                Genres = _genreService.GetAllGenres();
            }

            return Page();
        }
    }
}
