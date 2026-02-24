using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RegistryAPI.Data;

namespace RegistryAPI.Pages.Department
{
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;

        public EditModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public RegistryAPI.Models.Department Department { get; set; } = new RegistryAPI.Models.Department();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var dept = await _context.Departments.FindAsync(id);
            if (dept == null) return NotFound();

            Department = dept;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var existing = await _context.Departments.FindAsync(Department.DeptId);
            if (existing == null) return NotFound();

            _context.Entry(existing).CurrentValues.SetValues(Department);
            existing.UpdateDate = DateTime.UtcNow;
            existing.UpdatedBy = Department.UpdatedBy ?? "system";

            await _context.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}
