using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RegistryAPI.Data;

namespace RegistryAPI.Pages.Transaction
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public IList<RegistryAPI.Models.Transaction> Transactions { get; set; } = new List<RegistryAPI.Models.Transaction>();

        public string? Search { get; set; }
        public string? StaffIdFilter { get; set; }
        public string? TransactionTypeFilter { get; set; }
        public string? StatusFilter { get; set; }
        public string? DetailsFilter { get; set; }
        public string? SequenceIdFilter { get; set; }
        public string? DisplayNumberFilter { get; set; }

        public async Task OnGetAsync(string? search = null, string? staffId = null, string? transactionType = null, string? status = null, string? details = null, string? sequenceId = null, string? displayNumber = null)
        {
            Search = search;
            StaffIdFilter = staffId;
            TransactionTypeFilter = transactionType;
            StatusFilter = status;
            DetailsFilter = details;
            SequenceIdFilter = sequenceId;
            DisplayNumberFilter = displayNumber;

            IQueryable<RegistryAPI.Models.Transaction> query = _context.Transactions.Include(t => t.Staff).Include(t => t.File).AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var t = search.Trim().ToLower();
                query = query.Where(x => (x.TransactionId.ToString().Contains(t))
                                       || (x.StaffId != null && x.StaffId.ToLower().Contains(t))
                                       || (x.TransactionType != null && x.TransactionType.ToLower().Contains(t))
                                       || (x.Status != null && x.Status.ToLower().Contains(t))
                                       || (x.Details != null && x.Details.ToLower().Contains(t))
                                       || (x.SequenceId != null && x.SequenceId.ToLower().Contains(t))
                                       || (x.DisplayNumber != null && x.DisplayNumber.ToLower().Contains(t)));
            }

            if (!string.IsNullOrWhiteSpace(staffId)) { var t = staffId.Trim().ToLower(); query = query.Where(x => x.StaffId != null && x.StaffId.ToLower() == t); }
            if (!string.IsNullOrWhiteSpace(transactionType)) { var t = transactionType.Trim().ToLower(); query = query.Where(x => x.TransactionType != null && x.TransactionType.ToLower().Contains(t)); }
            if (!string.IsNullOrWhiteSpace(status)) { var t = status.Trim().ToLower(); query = query.Where(x => x.Status != null && x.Status.ToLower().Contains(t)); }
            if (!string.IsNullOrWhiteSpace(details)) { var t = details.Trim().ToLower(); query = query.Where(x => x.Details != null && x.Details.ToLower().Contains(t)); }
            if (!string.IsNullOrWhiteSpace(sequenceId)) { var t = sequenceId.Trim().ToLower(); query = query.Where(x => x.SequenceId != null && x.SequenceId.ToLower().Contains(t)); }
            if (!string.IsNullOrWhiteSpace(displayNumber)) { var t = displayNumber.Trim().ToLower(); query = query.Where(x => x.DisplayNumber != null && x.DisplayNumber.ToLower().Contains(t)); }

            Transactions = await query.ToListAsync();
        }
    }
}
