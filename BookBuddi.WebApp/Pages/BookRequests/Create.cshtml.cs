using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BookBuddi.Services.Interfaces;
using BookBuddi.Services.ServiceModels;
using BookBuddi.Data;
using BookBuddi.Data.Models;
using BookBuddi.Resources.Constants;

namespace BookBuddi.Pages.BookRequests
{
    public class CreateModel : PageModel
    {
        private readonly IBookRequestService _requestService;
        private readonly ApplicationDbContext _context;

        public CreateModel(IBookRequestService requestService, ApplicationDbContext context)
        {
            _requestService = requestService;
            _context = context;
        }

        public List<Member> Members { get; set; } = new List<Member>();
        public string? ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            Members = await _context.Members.Where(m => m.Status == MemberStatus.Active).ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync(int memberId, string bookTitle, string? author, string? isbn, string? notes)
        {
            try
            {
                var request = new BookRequestViewModel
                {
                    MemberId = memberId,
                    BookTitle = bookTitle,
                    AuthorName = author ?? string.Empty,
                    ISBN = isbn,
                    MemberNotes = notes,
                    RequestDate = DateTime.Now,
                    Status = RequestStatus.Pending
                };

                var createdBy = HttpContext.Session.GetString("MemberName") ?? "System";
                _requestService.AddRequest(request, createdBy);
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                Members = await _context.Members.Where(m => m.Status == MemberStatus.Active).ToListAsync();
                return Page();
            }
        }
    }
}
