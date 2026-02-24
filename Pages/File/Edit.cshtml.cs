using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RegistryAPI.Data;

namespace RegistryAPI.Pages.File
{
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;

        public EditModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public RegistryAPI.Models.File FileItem { get; set; } = new RegistryAPI.Models.File();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var file = await _context.Files.FindAsync(id);
            if (file == null) return NotFound();
            FileItem = file;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var existing = await _context.Files.FindAsync(FileItem.FileId);
            if (existing == null) return NotFound();

            // Validate staff FK
            if (!string.IsNullOrEmpty(FileItem.StaffId) && !await _context.Staff.AnyAsync(s => s.StaffId == FileItem.StaffId))
            {
                ModelState.AddModelError("FileItem.StaffId", "Staff does not exist.");
                return Page();
            }

            _context.Entry(existing).CurrentValues.SetValues(FileItem);
            existing.UpdateDate = DateTime.UtcNow;
            existing.UpdatedBy = FileItem.UpdatedBy ?? "system";

            await _context.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}
