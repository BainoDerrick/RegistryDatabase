using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RegistryAPI.Data;
using RegistryAPI.Models;

namespace RegistryAPI.Pages.Staff
{
    public class DeleteModel : PageModel
    {
        private readonly AppDbContext _context;

        public DeleteModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public RegistryAPI.Models.Staff StaffItem { get; set; } = new RegistryAPI.Models.Staff();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var staff = await _context.Staff.FirstOrDefaultAsync(s => s.StaffId == id);
            if (staff == null) return NotFound();

            StaffItem = staff;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var staff = await _context.Staff.FindAsync(StaffItem.StaffId);
            if (staff == null) return NotFound();

            _context.Staff.Remove(staff);
            await _context.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}
