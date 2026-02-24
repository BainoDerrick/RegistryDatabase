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
        // Chart data
        public List<string> DocumentsByMonthLabels { get; set; } = new();
        public List<int> DocumentsByMonthCounts { get; set; } = new();
        public List<string> DocumentsByTypeLabels { get; set; } = new();
        public List<int> DocumentsByTypeCounts { get; set; } = new();
        public List<string> StaffByDeptLabels { get; set; } = new();
        public List<int> StaffByDeptCounts { get; set; } = new();

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

            // Documents per month (last 12 months)
            var lastYear = DateTime.UtcNow.AddMonths(-11);
            var docsByMonth = await _context.Documents
                .Where(d => d.CreateDate >= lastYear)
                .GroupBy(d => new { Year = d.CreateDate.Value.Year, Month = d.CreateDate.Value.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
                .ToListAsync();

            // build labels for last 12 months
            for (int i = 0; i < 12; i++)
            {
                var dt = DateTime.UtcNow.AddMonths(-11 + i);
                var label = dt.ToString("MMM yyyy");
                DocumentsByMonthLabels.Add(label);
                var found = docsByMonth.FirstOrDefault(x => x.Year == dt.Year && x.Month == dt.Month);
                DocumentsByMonthCounts.Add(found?.Count ?? 0);
            }

            // Documents by type
            var docsByType = await _context.Documents
                .Where(d => d.DocTypeId != null)
                .GroupBy(d => d.DocTypeId)
                .Select(g => new { DocTypeId = g.Key, Count = g.Count() })
                .ToListAsync();

            foreach (var item in docsByType)
            {
                var type = await _context.DocumentTypes.FindAsync(item.DocTypeId);
                DocumentsByTypeLabels.Add(type?.TypeName ?? $"Type {item.DocTypeId}");
                DocumentsByTypeCounts.Add(item.Count);
            }

            // Staff by department (top 8)
            var staffByDept = await _context.Staff
                .GroupBy(s => s.DeptCode)
                .Select(g => new { Dept = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(8)
                .ToListAsync();

            foreach (var sd in staffByDept)
            {
                StaffByDeptLabels.Add(sd.Dept ?? "Unassigned");
                StaffByDeptCounts.Add(sd.Count);
            }
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