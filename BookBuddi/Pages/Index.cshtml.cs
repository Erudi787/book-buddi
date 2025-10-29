using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Services;

namespace BookBuddi.Pages;

public class IndexModel : PageModel
{
    private readonly ReportService _reportService;

    public IndexModel(ReportService reportService)
    {
        _reportService = reportService;
    }

    public int TotalBooks { get; set; }
    public int AvailableBooks { get; set; }
    public int TotalMembers { get; set; }
    public int ActiveTransactions { get; set; }
    public int UnpaidFines { get; set; }

    public async Task OnGetAsync()
    {
        TotalBooks = await _reportService.GetTotalBooksCountAsync();
        AvailableBooks = await _reportService.GetAvailableBooksCountAsync();
        TotalMembers = await _reportService.GetTotalMembersCountAsync();
        ActiveTransactions = await _reportService.GetActiveTransactionsCountAsync();
        UnpaidFines = await _reportService.GetUnpaidFinesCountAsync();
    }
}
