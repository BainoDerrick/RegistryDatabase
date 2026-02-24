using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RegistryAPI.Data;

namespace RegistryAPI.Pages.Transaction
{
    public class DetailsModel : PageModel
    {
        private readonly AppDbContext _context;

        public DetailsModel(AppDbContext context)
        {
            _context = context;
        }

        public RegistryAPI.Models.Transaction TransactionItem { get; set; } = new RegistryAPI.Models.Transaction();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var tr = await _context.Transactions.Include(t => t.Staff).Include(t => t.File).FirstOrDefaultAsync(t => t.TransactionId == id);
            if (tr == null) return NotFound();
            TransactionItem = tr;
            return Page();
        }
    }
}
