using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RegistryAPI.Data;
using RegistryAPI.Models;

namespace RegistryAPI.Pages.Termination
{
    public class DeleteModel : PageModel
    {
        private readonly AppDbContext _context;

        public DeleteModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public RegistryAPI.Models.Termination TerminationItem { get; set; } = new RegistryAPI.Models.Termination();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var item = await _context.Terminations.Include(t => t.Staff).FirstOrDefaultAsync(t => t.TerminationId == id);
            if (item == null) return NotFound();
            TerminationItem = item;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var item = await _context.Terminations.FindAsync(TerminationItem.TerminationId);
            if (item == null) return NotFound();

            // clear staff termination date and reactivate staff
            if (!string.IsNullOrEmpty(item.StaffId))
            {
                var staff = await _context.Staff.FindAsync(item.StaffId);
                if (staff != null)
                {
                    staff.TerminationDate = null;
                    staff.Status = "Active";
                }
            }

            _context.Terminations.Remove(item);
            await _context.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}
