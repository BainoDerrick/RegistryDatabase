using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RegistryAPI.Data;

namespace RegistryAPI.Pages.Department
{
    public class DetailsModel : PageModel
    {
        private readonly AppDbContext _context;

        public DetailsModel(AppDbContext context)
        {
            _context = context;
        }

        public RegistryAPI.Models.Department DepartmentItem { get; set; } = new RegistryAPI.Models.Department();

        public List<RegistryAPI.Models.Staff> DepartmentStaff { get; set; } = new List<RegistryAPI.Models.Staff>();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var dept = await _context.Departments
                .Include(d => d.StaffMembers)
                .FirstOrDefaultAsync(d => d.DeptId == id);
            if (dept == null) return NotFound();
            DepartmentItem = dept;

            // Load staff list explicitly by DeptCode to avoid relying on navigation mapping
            if (!string.IsNullOrEmpty(dept.DeptCode))
            {
                DepartmentStaff = await _context.Staff.Where(s => s.DeptCode == dept.DeptCode).ToListAsync();
            }
            else
            {
                DepartmentStaff = dept.StaffMembers?.ToList() ?? new List<RegistryAPI.Models.Staff>();
            }
            return Page();
        }
    }
}
