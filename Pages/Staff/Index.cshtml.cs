using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RegistryAPI.Data;
using RegistryAPI.Models;

namespace RegistryAPI.Pages.Staff
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public IList<RegistryAPI.Models.Staff> StaffList { get; set; } = new List<RegistryAPI.Models.Staff>();
        public string? Search { get; set; }

        public string? FirstNameFilter { get; set; }
        public string? LastNameFilter { get; set; }
        public string? EmailFilter { get; set; }
        public string? DeptCodeFilter { get; set; }
        public string? StatusFilter { get; set; }
        public string? RoleFilter { get; set; }

        public async Task OnGetAsync(string? search = null, string? firstName = null, string? lastName = null, string? email = null, string? deptCode = null, string? status = null, string? role = null)
        {
            Search = search;
            FirstNameFilter = firstName;
            LastNameFilter = lastName;
            EmailFilter = email;
            DeptCodeFilter = deptCode;
            StatusFilter = status;
            RoleFilter = role;

            IQueryable<RegistryAPI.Models.Staff> query = _context.Staff.Include(s => s.Department).AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var t = search.Trim().ToLower();
                query = query.Where(x => (x.StaffId != null && x.StaffId.ToLower().Contains(t))
                                       || (x.FirstName != null && x.FirstName.ToLower().Contains(t))
                                       || (x.LastName != null && x.LastName.ToLower().Contains(t))
                                       || (x.Email != null && x.Email.ToLower().Contains(t))
                                       || (x.DeptCode != null && x.DeptCode.ToLower().Contains(t))
                                       || (x.Role != null && x.Role.ToLower().Contains(t))
                                       || (x.Title != null && x.Title.ToLower().Contains(t)));
            }

            if (!string.IsNullOrWhiteSpace(firstName)) { var t = firstName.Trim().ToLower(); query = query.Where(x => x.FirstName != null && x.FirstName.ToLower().Contains(t)); }
            if (!string.IsNullOrWhiteSpace(lastName)) { var t = lastName.Trim().ToLower(); query = query.Where(x => x.LastName != null && x.LastName.ToLower().Contains(t)); }
            if (!string.IsNullOrWhiteSpace(email)) { var t = email.Trim().ToLower(); query = query.Where(x => x.Email != null && x.Email.ToLower().Contains(t)); }
            if (!string.IsNullOrWhiteSpace(deptCode)) { var t = deptCode.Trim().ToLower(); query = query.Where(x => x.DeptCode != null && x.DeptCode.ToLower() == t); }
            if (!string.IsNullOrWhiteSpace(status)) { var t = status.Trim().ToLower(); query = query.Where(x => x.Status != null && x.Status.ToLower() == t); }

            // By default exclude terminated staff from the main list unless a status filter is explicitly provided
            if (string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(x => !((x.Status != null && x.Status.ToLower() == "terminated") || x.TerminationDate.HasValue));
            }
            if (!string.IsNullOrWhiteSpace(role)) { var t = role.Trim().ToLower(); query = query.Where(x => x.Role != null && x.Role.ToLower() == t); }

            StaffList = await query.ToListAsync();
        }
    }
}
