using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RegistryAPI.Data;

namespace RegistryAPI.Pages.Sequence
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public IList<RegistryAPI.Models.Sequence> Sequences { get; set; } = new List<RegistryAPI.Models.Sequence>();

        public string? Search { get; set; }
        public string? SequenceIdFilter { get; set; }

        public async Task OnGetAsync(string? search = null, string? sequenceId = null)
        {
            Search = search;
            SequenceIdFilter = sequenceId;

            IQueryable<RegistryAPI.Models.Sequence> query = _context.Sequences.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var t = search.Trim().ToLower();
                query = query.Where(x => (x.SequenceId != null && x.SequenceId.ToLower().Contains(t))
                                       || (x.DeptCode != null && x.DeptCode.ToLower().Contains(t)));
            }

            if (!string.IsNullOrWhiteSpace(sequenceId)) { var t = sequenceId.Trim().ToLower(); query = query.Where(x => x.SequenceId != null && x.SequenceId.ToLower().Contains(t)); }

            Sequences = await query.ToListAsync();
        }
    }
}
