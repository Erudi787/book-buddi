using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Services.Interfaces;
using BookBuddi.Services.ServiceModels;

namespace BookBuddi.Pages.Genres
{
    public class EditModel : PageModel
    {
        private readonly IGenreService _genreService;

        public EditModel(IGenreService genreService)
        {
            _genreService = genreService;
        }

        [BindProperty]
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

        public IActionResult OnPost()
        {
            var isAdmin = HttpContext.Session.GetString("UserRole") == "Admin";
            if (!isAdmin)
            {
                return RedirectToPage("/Admin/Login");
            }

            try
            {
                var adminName = HttpContext.Session.GetString("AdminName") ?? "Admin";
                _genreService.UpdateGenre(Genre, adminName);
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return Page();
            }
        }
    }
}
