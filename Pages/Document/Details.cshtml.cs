using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RegistryAPI.Data;

namespace RegistryAPI.Pages.Document
{
    public class DetailsModel : PageModel
    {
        private readonly AppDbContext _context;

        public DetailsModel(AppDbContext context)
        {
            _context = context;
        }

        public RegistryAPI.Models.Document DocumentItem { get; set; } = new RegistryAPI.Models.Document();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var doc = await _context.Documents
                .Include(d => d.DocumentType)
                .Include(d => d.File)
                .FirstOrDefaultAsync(d => d.DocumentId == id);
            if (doc == null) return NotFound();

            DocumentItem = doc;
            return Page();
        }
    }
}
