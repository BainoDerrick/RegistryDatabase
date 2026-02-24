using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RegistryAPI.Data;

namespace RegistryAPI.Pages.Department
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public IList<RegistryAPI.Models.Department> DepartmentList { get; set; } = new List<RegistryAPI.Models.Department>();

        public int TotalStaffAssigned { get; set; }

        public string? Search { get; set; }
        public string? DeptIdFilter { get; set; }
        public string? DeptCodeFilter { get; set; }
        public string? DeptNameFilter { get; set; }

        public async Task OnGetAsync(string? search = null, string? deptId = null, string? deptCode = null, string? deptName = null)
        {
            Search = search;
            DeptIdFilter = deptId;
            DeptCodeFilter = deptCode;
            DeptNameFilter = deptName;

            IQueryable<RegistryAPI.Models.Department> query = _context.Departments.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var t = search.Trim().ToLower();
                query = query.Where(x => (x.DeptId != null && x.DeptId.ToLower().Contains(t))
                                       || (x.DeptCode != null && x.DeptCode.ToLower().Contains(t))
                                       || (x.DeptName != null && x.DeptName.ToLower().Contains(t)));
            }

            if (!string.IsNullOrWhiteSpace(deptId)) { var t = deptId.Trim().ToLower(); query = query.Where(x => x.DeptId != null && x.DeptId.ToLower() == t); }
            if (!string.IsNullOrWhiteSpace(deptCode)) { var t = deptCode.Trim().ToLower(); query = query.Where(x => x.DeptCode != null && x.DeptCode.ToLower() == t); }
            if (!string.IsNullOrWhiteSpace(deptName)) { var t = deptName.Trim().ToLower(); query = query.Where(x => x.DeptName != null && x.DeptName.ToLower().Contains(t)); }

            DepartmentList = await query.ToListAsync();

            // Count staff assigned to any department (dept code not null/empty)
            TotalStaffAssigned = await _context.Staff.CountAsync(s => s.DeptCode != null && s.DeptCode != "");
        }
    }
}
