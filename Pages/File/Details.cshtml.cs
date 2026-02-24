using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RegistryAPI.Data;

namespace RegistryAPI.Pages.File
{
    public class DetailsModel : PageModel
    {
        private readonly AppDbContext _context;

        public DetailsModel(AppDbContext context)
        {
            _context = context;
        }

        public RegistryAPI.Models.File FileItem { get; set; } = new RegistryAPI.Models.File();
        public List<RegistryAPI.Models.Document> Documents { get; set; } = new List<RegistryAPI.Models.Document>();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var file = await _context.Files.Include(f => f.Staff).FirstOrDefaultAsync(f => f.FileId == id);
            if (file == null) return NotFound();
            FileItem = file;
            Documents = await _context.Documents
                .Where(d => d.FileId == id)
                .Include(d => d.DocumentType)
                .OrderByDescending(d => d.CreateDate)
                .ToListAsync();
            return Page();
        }
    }
}
