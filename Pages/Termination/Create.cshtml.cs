using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RegistryAPI.Data;
using RegistryAPI.Models;

namespace RegistryAPI.Pages.Termination
{
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;

        public CreateModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public RegistryAPI.Models.Termination TerminationItem { get; set; } = new RegistryAPI.Models.Termination();

        public SelectList StaffSelect { get; set; } = null!;

        public void OnGet()
        {
            StaffSelect = new SelectList(_context.Staff
                .Where(s => !((s.Status != null && s.Status.ToLower() == "terminated") || s.TerminationDate.HasValue))
                .OrderBy(s => s.FirstName).ThenBy(s => s.LastName)
                .Select(s => new { s.StaffId, Name = s.FirstName + " " + s.LastName }), "StaffId", "Name");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                OnGet();
                return Page();
            }

            TerminationItem.CreateDate = DateTime.UtcNow;
            TerminationItem.CreatedBy = User?.Identity?.Name ?? "system";

            _context.Terminations.Add(TerminationItem);

            // update staff termination date
            if (!string.IsNullOrEmpty(TerminationItem.StaffId))
            {
                var staff = await _context.Staff.FindAsync(TerminationItem.StaffId);
                if (staff != null)
                {
                    staff.TerminationDate = TerminationItem.CreateDate;
                    // mark staff as terminated so they no longer appear in active staff lists
                    staff.Status = "Terminated";
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}
