using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RegistryAPI.Data;
using RegistryAPI.Models;

namespace RegistryAPI.Pages.Termination
{
    public class DetailsModel : PageModel
    {
        private readonly AppDbContext _context;

        public DetailsModel(AppDbContext context)
        {
            _context = context;
        }

        public RegistryAPI.Models.Termination TerminationItem { get; set; } = new RegistryAPI.Models.Termination();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var item = await _context.Terminations.Include(t => t.Staff).FirstOrDefaultAsync(t => t.TerminationId == id);
            if (item == null) return NotFound();
            TerminationItem = item;
            return Page();
        }
    }
}
