using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RegistryAPI.Data;

namespace RegistryAPI.Pages.Sequence
{
    public class DeleteModel : PageModel
    {
        private readonly AppDbContext _context;

        public DeleteModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public RegistryAPI.Models.Sequence SequenceItem { get; set; } = new RegistryAPI.Models.Sequence();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();
            var seq = await _context.Sequences.FirstOrDefaultAsync(s => s.SequenceId == id);
            if (seq == null) return NotFound();
            SequenceItem = seq;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var seq = await _context.Sequences.FindAsync(SequenceItem.SequenceId);
            if (seq == null) return NotFound();
            _context.Sequences.Remove(seq);
            await _context.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}
