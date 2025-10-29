using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Models;
using BookBuddi.Interfaces;

namespace BookBuddi.Pages.Fines
{
    public class IndexModel : PageModel
    {
        private readonly IFineRepository _fineRepository;

        public IndexModel(IFineRepository fineRepository)
        {
            _fineRepository = fineRepository;
        }

        public IEnumerable<Fine> Fines { get; set; } = new List<Fine>();

        public async Task<IActionResult> OnGetAsync()
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            // Require login
            if (string.IsNullOrEmpty(userRole))
            {
                return RedirectToPage("/Account/Login");
            }

            if (userRole == "Member")
            {
                // Show only the member's fines
                var memberId = HttpContext.Session.GetInt32("MemberId");
                if (memberId.HasValue)
                {
                    Fines = await _fineRepository.GetFinesByMemberIdAsync(memberId.Value);
                }
            }
            else
            {
                // Admin sees all fines
                Fines = await _fineRepository.GetAllFinesAsync();
            }

            return Page();
        }
    }
}
