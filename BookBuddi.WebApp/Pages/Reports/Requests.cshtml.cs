using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BookBuddi.Data;
using BookBuddi.Resources.Constants;
using BookBuddi.Data.Models;
using System.Linq;

namespace BookBuddi.WebApp.Pages
{
    public class RequestsModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public RequestsModel(ApplicationDbContext db) => _db = db;

        public List<RequestDTO> RequestList { get; set; } = new();

        public int TotalRequests { get; set; }
        public int PendingRequests { get; set; }
        public int ApprovedRequests { get; set; }
        public int RejectedRequests { get; set; }

         public string CurrentFilter { get; set; } = "All";

        public async Task<IActionResult> OnGet()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
                return RedirectToPage(string.IsNullOrEmpty(userRole) ? "/Account/Login" : "/Admin/AccessDenied");

            var requests = await _db.BookRequests.ToListAsync();
            var members = await _db.Members.ToListAsync();

            TotalRequests = requests.Count;
            PendingRequests = requests.Count(r => r.Status == RequestStatus.Pending);
            ApprovedRequests = requests.Count(r => r.Status == RequestStatus.Approved);
            RejectedRequests = requests.Count(r => r.Status == RequestStatus.Rejected);

            RequestList = requests.Select(r => new RequestDTO
            {
                RequestId = r.RequestId,
                BookTitle = r.BookTitle,
                MemberName = members.FirstOrDefault(m => m.MemberId == r.MemberId)?.FirstName + " " +
                             members.FirstOrDefault(m => m.MemberId == r.MemberId)?.LastName ?? "-",
                RequestDate = r.RequestDate,
                StatusText = r.Status.ToString(),
                StatusClass = r.Status == RequestStatus.Pending ? "status-pending" :
                              r.Status == RequestStatus.Approved ? "status-approved" : "status-rejected",
                IsPending = r.Status == RequestStatus.Pending
            }).ToList();

            return Page();
        }

        public class RequestDTO
        {
            public int RequestId { get; set; }
            public string? BookTitle { get; set; }
            public string? MemberName { get; set; }
            public DateTime RequestDate { get; set; }
            public string? StatusText { get; set; }
            public string? StatusClass { get; set; }
            public bool IsPending { get; set; }
        }
    }
}
