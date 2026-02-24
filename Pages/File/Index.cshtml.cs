using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RegistryAPI.Data;

namespace RegistryAPI.Pages.File
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public IList<RegistryAPI.Models.File> Files { get; set; } = new List<RegistryAPI.Models.File>();

        public string? Search { get; set; }
        public string? StaffIdFilter { get; set; }
        public string? FileStatusFilter { get; set; }

        public async Task OnGetAsync(string? search = null, string? staffId = null, string? fileStatus = null)
        {
            Search = search;
            StaffIdFilter = staffId;
            FileStatusFilter = fileStatus;

            IQueryable<RegistryAPI.Models.File> query = _context.Files.Include(f => f.Staff).AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var t = search.Trim().ToLower();
                query = query.Where(x => (x.FileId.ToString().Contains(t)) || (x.StaffId != null && x.StaffId.ToLower().Contains(t)) || (x.FileStatus != null && x.FileStatus.ToLower().Contains(t)));
            }

            if (!string.IsNullOrWhiteSpace(staffId)) { var t = staffId.Trim().ToLower(); query = query.Where(x => x.StaffId != null && x.StaffId.ToLower() == t); }
            if (!string.IsNullOrWhiteSpace(fileStatus)) { var t = fileStatus.Trim().ToLower(); query = query.Where(x => x.FileStatus != null && x.FileStatus.ToLower().Contains(t)); }

            Files = await query.ToListAsync();
        }
    }
}
