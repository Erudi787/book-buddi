using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Models;
using BookBuddi.Interfaces;

namespace BookBuddi.Pages.Authors
{
    public class IndexModel : PageModel
    {
        private readonly IAuthorRepository _authorRepository;

        public IndexModel(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }

        public IEnumerable<Author> Authors { get; set; } = new List<Author>();

        public async Task OnGetAsync()
        {
            Authors = await _authorRepository.GetAllAuthorsAsync();
        }
    }
}
