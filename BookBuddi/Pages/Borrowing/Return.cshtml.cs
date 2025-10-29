using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Models;
using BookBuddi.Services;
using BookBuddi.Interfaces;

namespace BookBuddi.Pages.Borrowing
{
    public class ReturnModel : PageModel
    {
        private readonly BorrowingService _borrowingService;
        private readonly IBorrowTransactionRepository _transactionRepository;

        public ReturnModel(BorrowingService borrowingService, IBorrowTransactionRepository transactionRepository)
        {
            _borrowingService = borrowingService;
            _transactionRepository = transactionRepository;
        }

        public BorrowTransaction? Transaction { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Transaction = await _transactionRepository.GetTransactionByIdAsync(id);

            if (Transaction == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            try
            {
                await _borrowingService.ReturnBookAsync(id);
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                Transaction = await _transactionRepository.GetTransactionByIdAsync(id);
                return Page();
            }
        }
    }
}
