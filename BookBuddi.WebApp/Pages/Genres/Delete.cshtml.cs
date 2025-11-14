using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Services.Interfaces;
using BookBuddi.Services.ServiceModels;

namespace BookBuddi.Pages.Genres
{
    public class DeleteModel : PageModel
    {
        private readonly IGenreService _genreService;

        public DeleteModel(IGenreService genreService)
        {
            _genreService = genreService;
        }

        public GenreViewModel Genre { get; set; } = new GenreViewModel();
        public string? ErrorMessage { get; set; }

        public IActionResult OnGet(int id)
        {
            // Admin-only check
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                return RedirectToPage("/Admin/Login");
            }

            var genre = _genreService.GetGenreById(id);
            if (genre == null)
            {
                return RedirectToPage("./Index");
            }

            Genre = genre;
            return Page();
        }

        public IActionResult OnPost(int id)
        {
            var isAdmin = HttpContext.Session.GetString("UserRole") == "Admin";
            if (!isAdmin)
            {
                return RedirectToPage("/Admin/Login");
            }

            try
            {
                _genreService.DeleteGenre(id);
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                var genre = _genreService.GetGenreById(id);
                if (genre != null)
                {
                    Genre = genre;
                }
                return Page();
            }
        }
    }
}
