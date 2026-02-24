using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RegistryAPI.Data;

namespace RegistryAPI.Pages.Department
{
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;

        public CreateModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public RegistryAPI.Models.Department Department { get; set; } = new RegistryAPI.Models.Department();

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            Department.CreateDate = DateTime.UtcNow;
            Department.UpdateDate = DateTime.UtcNow;
            Department.CreatedBy ??= "system";
            Department.UpdatedBy ??= "system";

            _context.Departments.Add(Department);
            await _context.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}
