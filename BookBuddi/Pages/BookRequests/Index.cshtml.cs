using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Models;
using BookBuddi.Interfaces;

namespace BookBuddi.Pages.BookRequests
{
    public class IndexModel : PageModel
    {
        private readonly IBookRequestRepository _requestRepository;

        public IndexModel(IBookRequestRepository requestRepository)
        {
            _requestRepository = requestRepository;
        }

        public IEnumerable<BookRequest> Requests { get; set; } = new List<BookRequest>();

        public async Task OnGetAsync()
        {
            Requests = await _requestRepository.GetAllRequestsAsync();
        }
    }
}
