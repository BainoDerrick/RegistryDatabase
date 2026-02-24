using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RegistryAPI.Data;

namespace RegistryAPI.Pages.Document
{
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;

        public EditModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public RegistryAPI.Models.Document Document { get; set; } = new RegistryAPI.Models.Document();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var doc = await _context.Documents.FindAsync(id);
            if (doc == null) return NotFound();
            Document = doc;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var existing = await _context.Documents.FindAsync(Document.DocumentId);
            if (existing == null) return NotFound();

            _context.Entry(existing).CurrentValues.SetValues(Document);
            existing.UpdateDate = DateTime.UtcNow;
            existing.UpdatedBy = Document.UpdatedBy ?? "system";

            // Validate FKs
            if (Document.FileId.HasValue && !await _context.Files.AnyAsync(f => f.FileId == Document.FileId))
            {
                ModelState.AddModelError("Document.FileId", "File does not exist.");
                return Page();
            }
            if (Document.DocTypeId.HasValue && !await _context.DocumentTypes.AnyAsync(dt => dt.DocTypeId == Document.DocTypeId))
            {
                ModelState.AddModelError("Document.DocTypeId", "Document type does not exist.");
                return Page();
            }

            await _context.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}
