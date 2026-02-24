using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RegistryAPI.Data;

namespace RegistryAPI.Pages.Document
{
    public class DeleteModel : PageModel
    {
        private readonly AppDbContext _context;

        public DeleteModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public RegistryAPI.Models.Document DocumentItem { get; set; } = new RegistryAPI.Models.Document();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var doc = await _context.Documents.FirstOrDefaultAsync(d => d.DocumentId == id);
            if (doc == null) return NotFound();
            DocumentItem = doc;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var doc = await _context.Documents.FindAsync(DocumentItem.DocumentId);
            if (doc == null) return NotFound();

            _context.Documents.Remove(doc);
            await _context.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}
