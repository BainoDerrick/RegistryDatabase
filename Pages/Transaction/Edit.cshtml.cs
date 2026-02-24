using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RegistryAPI.Data;

namespace RegistryAPI.Pages.Transaction
{
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;

        public EditModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public RegistryAPI.Models.Transaction TransactionItem { get; set; } = new RegistryAPI.Models.Transaction();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var tr = await _context.Transactions.FindAsync(id);
            if (tr == null) return NotFound();
            TransactionItem = tr;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Manual editing disabled
            return Forbid();
        }
    }
}
