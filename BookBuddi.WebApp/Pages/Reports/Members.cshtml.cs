using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BookBuddi.Data;
using BookBuddi.Resources.Constants;
using BookBuddi.Data.Models;

namespace BookBuddi.WebApp.Pages
{
    public class MembersModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public MembersModel(ApplicationDbContext db) => _db = db;

        public List<MemberDTO> MemberList { get; set; } = new();
        public int TotalMembers { get; set; }
        public int ActiveMembers { get; set; }
        public int InactiveMembers { get; set; }
        public int TotalBooksBorrowed { get; set; }
        public string CurrentFilter { get; set; } = "All";

        public async Task<IActionResult> OnGet(string filter, string search)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
                return RedirectToPage(string.IsNullOrEmpty(userRole)? "/Account/Login":"/Admin/AccessDenied");

            CurrentFilter = string.IsNullOrEmpty(filter) ? "All" : filter;

            var members = await _db.Members.ToListAsync();

            TotalMembers = members.Count;
            ActiveMembers = members.Count(m => m.Status == MemberStatus.Active);
            InactiveMembers = members.Count(m => m.Status != MemberStatus.Active);
            TotalBooksBorrowed = members.Sum(m => m.CurrentBorrowedCount);

            MemberList = members.Select(m => new MemberDTO
            {
                MemberId = m.MemberId,
                MemberName = $"{m.FirstName} {m.LastName}",
                Email = m.Email,
                TotalBooksBorrowed = m.CurrentBorrowedCount,
                StatusText = m.Status == MemberStatus.Active ? "Active" : "Inactive",
                StatusClass = m.Status == MemberStatus.Active ? "status-active" : "status-inactive"
            }).ToList();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                MemberList = MemberList
                    .Where(m => (m.MemberName ?? "").ToLower().Contains(search) ||
                                (m.Email ?? "").ToLower().Contains(search))
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(filter) && filter != "All")
            {
                MemberList = MemberList
                    .Where(m => m.StatusText == filter)
                    .ToList();
            }

            return Page();
        }

        public class MemberDTO
        {
            public int MemberId { get; set; }
            public string? MemberName { get; set; }
            public string? Email { get; set; }
            public int TotalBooksBorrowed { get; set; }
            public string? StatusText { get; set; }
            public string? StatusClass { get; set; }
        }
    }
}
