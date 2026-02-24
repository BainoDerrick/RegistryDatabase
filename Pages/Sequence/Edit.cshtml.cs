using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RegistryAPI.Data;

namespace RegistryAPI.Pages.Sequence
{
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;

        public EditModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public RegistryAPI.Models.Sequence Sequence { get; set; } = new RegistryAPI.Models.Sequence();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();
            var seq = await _context.Sequences.FindAsync(id);
            if (seq == null) return NotFound();
            Sequence = seq;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // manual editing disabled
            return Forbid();
        }
    }
}
