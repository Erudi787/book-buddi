using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Services.Interfaces;
using BookBuddi.Resources.Constants;

namespace BookBuddi.Pages.BookRequests
{
    public class RejectModel : PageModel
    {
        private readonly IBookRequestService _requestService;

        public RejectModel(IBookRequestService requestService)
        {
            _requestService = requestService;
        }

        public IActionResult OnGet(int id)
        {
            var adminName = HttpContext.Session.GetString("AdminName") ?? "Admin";
            _requestService.RejectRequest(id, "Rejected by admin", adminName);
            return RedirectToPage("./Index");
        }
    }
}
