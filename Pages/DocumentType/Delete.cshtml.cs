using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RegistryAPI.Data;

namespace RegistryAPI.Pages.DocumentType
{
    public class DeleteModel : PageModel
    {
        private readonly AppDbContext _context;

        public DeleteModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public RegistryAPI.Models.DocumentType DocType { get; set; } = new RegistryAPI.Models.DocumentType();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var dt = await _context.DocumentTypes.FirstOrDefaultAsync(d => d.DocTypeId == id);
            if (dt == null) return NotFound();
            DocType = dt;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var dt = await _context.DocumentTypes.FindAsync(DocType.DocTypeId);
            if (dt == null) return NotFound();
            _context.DocumentTypes.Remove(dt);
            await _context.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}
