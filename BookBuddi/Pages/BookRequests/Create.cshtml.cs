using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BookBuddi.Models;
using BookBuddi.Models.Enums;
using BookBuddi.Interfaces;
using BookBuddi.Data;

namespace BookBuddi.Pages.BookRequests
{
    public class CreateModel : PageModel
    {
        private readonly IBookRequestRepository _requestRepository;
        private readonly ApplicationDbContext _context;

        public CreateModel(IBookRequestRepository requestRepository, ApplicationDbContext context)
        {
            _requestRepository = requestRepository;
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
                var request = new BookRequest
                {
                    MemberId = memberId,
                    BookTitle = bookTitle,
                    AuthorName = author ?? string.Empty,
                    ISBN = isbn,
                    MemberNotes = notes,
                    RequestDate = DateTime.Now,
                    Status = RequestStatus.Pending
                };

                await _requestRepository.AddRequestAsync(request);
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
