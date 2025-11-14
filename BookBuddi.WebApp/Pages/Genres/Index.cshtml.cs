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
            // Admin-only check
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                return RedirectToPage("/Index");
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
