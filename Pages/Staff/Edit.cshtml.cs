using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RegistryAPI.Data;
using RegistryAPI.Models;

namespace RegistryAPI.Pages.Staff
{
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;

        public EditModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public RegistryAPI.Models.Staff Staff { get; set; } = new RegistryAPI.Models.Staff();

        public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> DepartmentOptions { get; set; } = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();
        public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> StatusOptions { get; set; } = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();
        public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> EmploymentTypeOptions { get; set; } = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var staff = await _context.Staff.FindAsync(id);
            if (staff == null) return NotFound();

            Staff = staff;
            // populate dropdowns
            var depts = await _context.Departments.OrderBy(d => d.DeptName).ToListAsync();
            DepartmentOptions = depts.Select(d => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = d.DeptCode,
                Text = string.IsNullOrWhiteSpace(d.DeptName) ? d.DeptCode : $"{d.DeptCode} - {d.DeptName}"
            }).ToList();

            StatusOptions = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>
            {
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem("Active","Active"),
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem("Inactive","Inactive")
            };

            EmploymentTypeOptions = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>
            {
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem("Full-time","Full-time"),
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem("Contract","Contract"),
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem("Part-time","Part-time")
            };

            ViewData["DepartmentOptions"] = DepartmentOptions;
            ViewData["StatusOptions"] = StatusOptions;
            ViewData["EmploymentTypeOptions"] = EmploymentTypeOptions;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // repopulate dropdowns before redisplay
                await OnGetAsync(Staff.StaffId);
                return Page();
            }

            var existing = await _context.Staff.FindAsync(Staff.StaffId);
            if (existing == null) return NotFound();

            // If password not provided in the edit form, keep the existing password
            if (string.IsNullOrEmpty(Staff.Password))
            {
                Staff.Password = existing.Password;
            }

            _context.Entry(existing).CurrentValues.SetValues(Staff);
            existing.UpdateDate = DateTime.UtcNow;
            existing.UpdatedBy = Staff.UpdatedBy ?? "system";

            await _context.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}
