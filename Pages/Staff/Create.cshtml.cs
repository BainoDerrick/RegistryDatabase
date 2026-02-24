using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RegistryAPI.Data;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using RegistryAPI.Models;

namespace RegistryAPI.Pages.Staff
{
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;

        public CreateModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public RegistryAPI.Models.Staff Staff { get; set; } = new RegistryAPI.Models.Staff();

        public List<SelectListItem> DepartmentOptions { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> StatusOptions { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> EmploymentTypeOptions { get; set; } = new List<SelectListItem>();

        public async Task<IActionResult> OnGetAsync()
        {
            var depts = await _context.Departments.OrderBy(d => d.DeptName).ToListAsync();
            DepartmentOptions = depts.Select(d => new SelectListItem
            {
                Value = d.DeptCode,
                Text = string.IsNullOrWhiteSpace(d.DeptName) ? d.DeptCode : $"{d.DeptCode} - {d.DeptName}"
            }).ToList();

            // Status dropdown options
            StatusOptions = new List<SelectListItem>
            {
                new SelectListItem("Active", "Active"),
                new SelectListItem("Inactive", "Inactive")
            };

            // Employment type dropdown options
            EmploymentTypeOptions = new List<SelectListItem>
            {
                new SelectListItem("Full-time", "Full-time"),
                new SelectListItem("Contract", "Contract"),
                new SelectListItem("Part-time", "Part-time")
            };

            // Also expose via ViewData for Razor compilation compatibility
            ViewData["DepartmentOptions"] = DepartmentOptions;
            ViewData["StatusOptions"] = StatusOptions;
            ViewData["EmploymentTypeOptions"] = EmploymentTypeOptions;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // ensure dropdown is populated when redisplaying the form
                await OnGetAsync();
                return Page();
            }

            if (!string.IsNullOrEmpty(Staff.DeptCode))
            {
                var dept = await _context.Departments.FirstOrDefaultAsync(d => d.DeptCode == Staff.DeptCode);
                if (dept == null)
                {
                    ModelState.AddModelError("Staff.DeptCode", "Department code does not exist.");
                    await OnGetAsync();
                    return Page();
                }
            }

            // Generate Staff ID if not provided
            if (string.IsNullOrWhiteSpace(Staff.StaffId))
            {
                Staff.StaffId = await GenerateStaffIdAsync(Staff.DeptCode);
            }

            // Ensure password is not null to avoid DB NOT NULL constraints (existing DB may require empties)
            Staff.Password ??= string.Empty;

            Staff.CreateDate = DateTime.UtcNow;
            Staff.UpdateDate = DateTime.UtcNow;
            Staff.CreatedBy ??= "system";
            Staff.UpdatedBy ??= "system";

            try
            {
                _context.Staff.Add(Staff);
                await _context.SaveChangesAsync();
                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                // Add model-level error to show to user and redisplay the form
                ModelState.AddModelError(string.Empty, "Unable to create staff: " + ex.Message);
                await OnGetAsync();
                return Page();
            }
        }

        private async Task<string> GenerateStaffIdAsync(string? deptCode)
        {
            var prefix = string.IsNullOrWhiteSpace(deptCode) ? "STF" : deptCode!.ToUpper().Replace(" ", "");

            var ids = await _context.Staff
                .Where(s => s.StaffId != null && s.StaffId.StartsWith(prefix))
                .Select(s => s.StaffId)
                .ToListAsync();

            var max = 0;
            foreach (var id in ids)
            {
                if (id == null) continue;
                if (id.Length <= prefix.Length) continue;
                var numericPart = new string(id.Skip(prefix.Length).Where(char.IsDigit).ToArray());
                if (int.TryParse(numericPart, out var n))
                {
                    if (n > max) max = n;
                }
            }

            var next = max + 1;
            return $"{prefix}{next.ToString("D3")}";
        }
    }
}
