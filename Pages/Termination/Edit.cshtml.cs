using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RegistryAPI.Data;
using RegistryAPI.Models;

namespace RegistryAPI.Pages.Termination
{
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;

        public EditModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public RegistryAPI.Models.Termination TerminationItem { get; set; } = new RegistryAPI.Models.Termination();

        public SelectList StaffSelect { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var item = await _context.Terminations.FindAsync(id);
            if (item == null) return NotFound();
            TerminationItem = item;
            StaffSelect = new SelectList(_context.Staff.OrderBy(s => s.FirstName).ThenBy(s => s.LastName).Select(s => new { s.StaffId, Name = s.FirstName + " " + s.LastName }), "StaffId", "Name");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var existing = await _context.Terminations.FindAsync(TerminationItem.TerminationId);
            if (existing == null) return NotFound();

            existing.StaffId = TerminationItem.StaffId;
            existing.Reason = TerminationItem.Reason;
            existing.UpdatedBy = User?.Identity?.Name ?? "system";
            existing.UpdateDate = DateTime.UtcNow;

            // ensure staff termination date and status updated
            if (!string.IsNullOrEmpty(existing.StaffId))
            {
                var staff = await _context.Staff.FindAsync(existing.StaffId);
                if (staff != null)
                {
                    staff.TerminationDate = existing.UpdateDate;
                    staff.Status = "Terminated";
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}
