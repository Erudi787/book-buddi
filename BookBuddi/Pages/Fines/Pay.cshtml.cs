using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookBuddi.Models;
using BookBuddi.Services;
using BookBuddi.Interfaces;

namespace BookBuddi.Pages.Fines
{
    public class PayModel : PageModel
    {
        private readonly FineService _fineService;
        private readonly IFineRepository _fineRepository;

        public PayModel(FineService fineService, IFineRepository fineRepository)
        {
            _fineService = fineService;
            _fineRepository = fineRepository;
        }

        public Fine? Fine { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Fine = await _fineRepository.GetFineByIdAsync(id);

            if (Fine == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            try
            {
                await _fineService.MarkFineAsPaidAsync(id);
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                Fine = await _fineRepository.GetFineByIdAsync(id);
                return Page();
            }
        }
    }
}
