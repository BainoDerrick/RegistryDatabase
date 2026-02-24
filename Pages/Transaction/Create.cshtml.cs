using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RegistryAPI.Data;

namespace RegistryAPI.Pages.Transaction
{
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;

        public CreateModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public RegistryAPI.Models.Transaction TransactionItem { get; set; } = new RegistryAPI.Models.Transaction();

        public async Task<IActionResult> OnPostAsync()
        {
            // Manual transaction creation has been disabled. Transactions are created automatically by the system.
            return Forbid();
        }
    }
}
