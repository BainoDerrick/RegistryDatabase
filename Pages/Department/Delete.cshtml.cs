using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RegistryAPI.Data;

namespace RegistryAPI.Pages.Department
{
    public class DeleteModel : PageModel
    {
        private readonly AppDbContext _context;

        public DeleteModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public RegistryAPI.Models.Department DepartmentItem { get; set; } = new RegistryAPI.Models.Department();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var dept = await _context.Departments.FirstOrDefaultAsync(d => d.DeptId == id);
            if (dept == null) return NotFound();

            DepartmentItem = dept;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var dept = await _context.Departments.FindAsync(DepartmentItem.DeptId);
            if (dept == null) return NotFound();

            _context.Departments.Remove(dept);
            await _context.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}
