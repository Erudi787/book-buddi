using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Services.Interfaces;
using BookBuddi.Services.ServiceModels;

namespace BookBuddi.Pages.Authors
{
    public class IndexModel : PageModel
    {
        private readonly IAuthorService _authorService;

        public IndexModel(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        public IEnumerable<AuthorViewModel> Authors { get; set; } = new List<AuthorViewModel>();

        public void OnGet()
        {
            Authors = _authorService.GetAllAuthors();
        }
    }
}
