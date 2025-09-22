using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RegistryAPI.Data;

namespace RegistryAPI.Pages.Dashboard
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public int DepartmentCount { get; set; }
        public int StaffCount { get; set; }
        public int FileCount { get; set; }
        public int DocumentCount { get; set; }
        public int TerminationCount { get; set; }
        public int TransactionCount { get; set; }
        public int ActiveStaffCount { get; set; }
        public int InactiveStaffCount { get; set; }
        public List<RecentTransaction> RecentTransactions { get; set; } = new List<RecentTransaction>();

        public async Task OnGetAsync()
        {
            DepartmentCount = await _context.Departments.CountAsync();
            StaffCount = await _context.Staff.CountAsync();
            FileCount = await _context.Files.CountAsync();
            DocumentCount = await _context.Documents.CountAsync();
            TerminationCount = await _context.Terminations.CountAsync();
            TransactionCount = await _context.Transactions.CountAsync();

            // Count active vs inactive staff
            ActiveStaffCount = await _context.Staff.CountAsync(s => s.Status == "Active");
            InactiveStaffCount = await _context.Staff.CountAsync(s => s.Status != "Active");

            // Get recent transactions (last 10)
            RecentTransactions = await _context.Transactions
                .Include(t => t.Staff)
                .Include(t => t.File)
                .OrderByDescending(t => t.TransactionDate)
                .Take(10)
                .Select(t => new RecentTransaction
                {
                    TransactionId = t.TransactionId,
                    TransactionType = t.TransactionType,
                    TransactionDate = t.TransactionDate,
                    StaffName = t.Staff != null ? $"{t.Staff.FirstName} {t.Staff.LastName}" : "Unknown",
                    FileId = t.FileId,
                    Status = t.Status
                })
                .ToListAsync();
        }

        public class RecentTransaction
        {
            public int TransactionId { get; set; }
            public string? TransactionType { get; set; }
            public DateTime TransactionDate { get; set; }
            public string? StaffName { get; set; }
            public int? FileId { get; set; }
            public string? Status { get; set; }
        }
    }
}