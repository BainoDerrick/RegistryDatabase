using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RegistryAPI.Data;

namespace RegistryAPI.Pages.DocumentType
{
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;

        public EditModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public RegistryAPI.Models.DocumentType DocType { get; set; } = new RegistryAPI.Models.DocumentType();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var dt = await _context.DocumentTypes.FindAsync(id);
            if (dt == null) return NotFound();
            DocType = dt;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var existing = await _context.DocumentTypes.FindAsync(DocType.DocTypeId);
            if (existing == null) return NotFound();

            _context.Entry(existing).CurrentValues.SetValues(DocType);
            existing.UpdateDate = DateTime.UtcNow;
            existing.UpdatedBy = DocType.UpdatedBy ?? "system";

            await _context.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}
