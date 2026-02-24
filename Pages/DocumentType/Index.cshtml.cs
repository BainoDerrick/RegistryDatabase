using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RegistryAPI.Data;

namespace RegistryAPI.Pages.DocumentType
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public IList<RegistryAPI.Models.DocumentType> DocumentTypes { get; set; } = new List<RegistryAPI.Models.DocumentType>();
        public string? Search { get; set; }
        public string? TypeNameFilter { get; set; }
        public string? DescriptionFilter { get; set; }

        public async Task OnGetAsync(string? search = null, string? typeName = null, string? description = null)
        {
            Search = search;
            TypeNameFilter = typeName;
            DescriptionFilter = description;

            IQueryable<RegistryAPI.Models.DocumentType> query = _context.DocumentTypes.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var t = search.Trim().ToLower();
                query = query.Where(x => (x.TypeName != null && x.TypeName.ToLower().Contains(t)) || (x.Description != null && x.Description.ToLower().Contains(t)));
            }

            if (!string.IsNullOrWhiteSpace(typeName)) { var t = typeName.Trim().ToLower(); query = query.Where(x => x.TypeName != null && x.TypeName.ToLower().Contains(t)); }
            if (!string.IsNullOrWhiteSpace(description)) { var t = description.Trim().ToLower(); query = query.Where(x => x.Description != null && x.Description.ToLower().Contains(t)); }

            DocumentTypes = await query.ToListAsync();
        }
    }
}
