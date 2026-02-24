using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RegistryAPI.Data;
using RegistryAPI.Models;

namespace RegistryAPI.Pages.Termination
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public IList<RegistryAPI.Models.Termination> Terminations { get; set; } = new List<RegistryAPI.Models.Termination>();

        public string? Search { get; set; }
        public string? StaffIdFilter { get; set; }

        public async Task OnGetAsync(string? search = null, string? staffId = null)
        {
            Search = search;
            StaffIdFilter = staffId;

            IQueryable<RegistryAPI.Models.Termination> query = _context.Terminations.Include(t => t.Staff).AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var t = search.Trim().ToLower();
                query = query.Where(x => (x.TerminationId.ToString().Contains(t)) || (x.StaffId != null && x.StaffId.ToLower().Contains(t)) || (x.Reason != null && x.Reason.ToLower().Contains(t)));
            }

            if (!string.IsNullOrWhiteSpace(staffId)) { var t = staffId.Trim().ToLower(); query = query.Where(x => x.StaffId != null && x.StaffId.ToLower().Contains(t)); }

            Terminations = await query.ToListAsync();
        }
    }
}
