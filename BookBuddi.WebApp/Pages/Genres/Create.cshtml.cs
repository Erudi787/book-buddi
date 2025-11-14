using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Services.Interfaces;
using BookBuddi.Services.ServiceModels;

namespace BookBuddi.Pages.Genres
{
    public class CreateModel : PageModel
    {
        private readonly IGenreService _genreService;

        public CreateModel(IGenreService genreService)
        {
            _genreService = genreService;
        }

        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

        public IActionResult OnGet()
        {
            // Admin-only check
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                return RedirectToPage("/Admin/Login");
            }

            return Page();
        }

        public IActionResult OnPost(string genreName, string? genreDescription)
        {
            var isAdmin = HttpContext.Session.GetString("UserRole") == "Admin";
            if (!isAdmin)
            {
                return RedirectToPage("/Admin/Login");
            }

            try
            {
                var genre = new GenreViewModel
                {
                    GenreName = genreName,
                    GenreDescription = genreDescription
                };

                var adminName = HttpContext.Session.GetString("AdminName") ?? "Admin";
                _genreService.AddGenre(genre, adminName);
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
