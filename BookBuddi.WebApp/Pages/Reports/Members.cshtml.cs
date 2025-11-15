using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BookBuddi.Data;
using BookBuddi.Resources.Constants;
using BookBuddi.Data.Models;
using System.Linq;

namespace BookBuddi.WebApp.Pages
{
    public class MembersModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public MembersModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public List<MemberDTO> MemberList { get; set; } = new();

        // Summary counts
        public int TotalMembers { get; set; }
        public int ActiveMembers { get; set; }
        public int InactiveMembers { get; set; }
        public int TotalBooksBorrowed { get; set; }

        public async Task<IActionResult> OnGet()
        {
            // Admin authorization check
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                TempData["ErrorMessage"] = "You must be logged in as an administrator to access this page.";
                return RedirectToPage("/Account/Login");
            }

            var members = await _db.Members.ToListAsync();

            // Summary calculations
            TotalMembers = members.Count;
            ActiveMembers = members.Count(m => m.Status == MemberStatus.Active);
            InactiveMembers = members.Count(m => m.Status != MemberStatus.Active);
            TotalBooksBorrowed = members.Sum(m => m.CurrentBorrowedCount);

            // Map data into DTO
            MemberList = members.Select(m => new MemberDTO
            {
                MemberId = m.MemberId,
                MemberName = m.FirstName + " " + m.LastName,
                Email = m.Email,
                TotalBooksBorrowed = m.CurrentBorrowedCount,
                StatusText = m.Status.ToString(),
                StatusClass = m.Status == MemberStatus.Active ? "status-active" : "status-inactive"
            }).ToList();

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