using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RegistryAPI.Data;

namespace RegistryAPI.Pages.File
{
    public class DeleteModel : PageModel
    {
        private readonly AppDbContext _context;

        public DeleteModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public RegistryAPI.Models.File FileItem { get; set; } = new RegistryAPI.Models.File();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var file = await _context.Files.FirstOrDefaultAsync(f => f.FileId == id);
            if (file == null) return NotFound();
            FileItem = file;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var file = await _context.Files.FindAsync(FileItem.FileId);
            if (file == null) return NotFound();
            _context.Files.Remove(file);
            await _context.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}
