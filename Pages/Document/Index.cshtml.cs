using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RegistryAPI.Data;

namespace RegistryAPI.Pages.Document
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public IList<RegistryAPI.Models.Document> Documents { get; set; } = new List<RegistryAPI.Models.Document>();

        public string? Search { get; set; }
        public string? DocumentNameFilter { get; set; }
        public string? DescriptionFilter { get; set; }
        public int? DocTypeIdFilter { get; set; }
        public int? FileIdFilter { get; set; }
        public bool? ScannedFilter { get; set; }

        public async Task OnGetAsync(string? search = null, string? documentName = null, string? description = null, int? docTypeId = null, int? fileId = null, bool? scanned = null)
        {
            Search = search;
            DocumentNameFilter = documentName;
            DescriptionFilter = description;
            DocTypeIdFilter = docTypeId;
            FileIdFilter = fileId;
            ScannedFilter = scanned;

            IQueryable<RegistryAPI.Models.Document> query = _context.Documents.Include(d => d.DocumentType).Include(d => d.File).AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var t = search.Trim().ToLower();
                query = query.Where(x => (x.DocumentName != null && x.DocumentName.ToLower().Contains(t))
                                       || (x.Description != null && x.Description.ToLower().Contains(t))
                                       || (x.FileId != null && x.FileId.ToString().Contains(t))
                                       || (x.DocTypeId != null && x.DocTypeId.ToString().Contains(t)));
            }

            if (!string.IsNullOrWhiteSpace(documentName)) { var t = documentName.Trim().ToLower(); query = query.Where(x => x.DocumentName != null && x.DocumentName.ToLower().Contains(t)); }
            if (!string.IsNullOrWhiteSpace(description)) { var t = description.Trim().ToLower(); query = query.Where(x => x.Description != null && x.Description.ToLower().Contains(t)); }
            if (docTypeId.HasValue) query = query.Where(x => x.DocTypeId == docTypeId.Value);
            if (fileId.HasValue) query = query.Where(x => x.FileId == fileId.Value);
            if (scanned.HasValue) query = query.Where(x => x.Scanned == scanned.Value);

            Documents = await query.ToListAsync();
        }
    }
}
