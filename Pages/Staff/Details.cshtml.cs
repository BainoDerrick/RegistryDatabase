using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RegistryAPI.Data;
using RegistryAPI.Models;

namespace RegistryAPI.Pages.Staff
{
    public class DetailsModel : PageModel
    {
        private readonly AppDbContext _context;

        public DetailsModel(AppDbContext context)
        {
            _context = context;
        }

        public RegistryAPI.Models.Staff StaffItem { get; set; } = new RegistryAPI.Models.Staff();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var staff = await _context.Staff.Include(s => s.Department).FirstOrDefaultAsync(s => s.StaffId == id);
            if (staff == null) return NotFound();

            StaffItem = staff;
            return Page();
        }
    }
}
