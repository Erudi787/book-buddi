using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Interfaces;
using BookBuddi.Models.Enums;

namespace BookBuddi.Pages.BookRequests
{
    public class ApproveModel : PageModel
    {
        private readonly IBookRequestRepository _requestRepository;

        public ApproveModel(IBookRequestRepository requestRepository)
        {
            _requestRepository = requestRepository;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var request = await _requestRepository.GetRequestByIdAsync(id);
            if (request != null)
            {
                request.Status = RequestStatus.Approved;
                request.StatusUpdateDate = DateTime.Now;
                await _requestRepository.UpdateRequestAsync(request);
            }
            return RedirectToPage("./Index");
        }
    }
}
