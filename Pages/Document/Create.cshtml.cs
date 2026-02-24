using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RegistryAPI.Data;

namespace RegistryAPI.Pages.Document
{
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;
        // Max upload allowed by the application (bytes). Change as needed and update DB settings if larger uploads required.
        private const long MAX_UPLOAD_BYTES = 100 * 1024 * 1024; // 100 MB
        private readonly RegistryAPI.Services.SequenceService _seqService;

        public CreateModel(AppDbContext context, RegistryAPI.Services.SequenceService seqService)
        {
            _context = context;
            _seqService = seqService;
        }

        [BindProperty]
        public RegistryAPI.Models.Document Document { get; set; } = new RegistryAPI.Models.Document();

        [BindProperty]
        public IFormFile? Upload { get; set; }

        public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> FileOptions { get; set; } = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();
        public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> DocumentTypeOptions { get; set; } = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();
        public long MaxUploadBytes => MAX_UPLOAD_BYTES;
        public long MaxUploadMB => MAX_UPLOAD_BYTES / 1024 / 1024;

        public async Task<IActionResult> OnGetAsync()
        {
            // populate file and doc type options
            var files = await _context.Files.Include(f => f.Staff).OrderBy(f => f.FileId).ToListAsync();
            FileOptions = files.Select(f => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = f.FileId.ToString(),
                Text = $"{f.FileId} - {(f.Staff != null ? f.Staff.FirstName + " " + f.Staff.LastName : "Unassigned")}"
            }).ToList();

            var dts = await _context.DocumentTypes.OrderBy(dt => dt.TypeName).ToListAsync();
            DocumentTypeOptions = dts.Select(dt => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = dt.DocTypeId.ToString(),
                Text = dt.TypeName
            }).ToList();

            ViewData["FileOptions"] = FileOptions;
            ViewData["DocumentTypeOptions"] = DocumentTypeOptions;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            // Validate foreign keys
            if (Document.FileId.HasValue)
            {
                var file = await _context.Files.Include(f => f.Staff).FirstOrDefaultAsync(f => f.FileId == Document.FileId);
                if (file == null)
                {
                    ModelState.AddModelError("Document.FileId", "File does not exist.");
                    await OnGetAsync();
                    return Page();
                }
            }
            if (Document.DocTypeId.HasValue && !await _context.DocumentTypes.AnyAsync(dt => dt.DocTypeId == Document.DocTypeId))
            {
                ModelState.AddModelError("Document.DocTypeId", "Document type does not exist.");
                await OnGetAsync();
                return Page();
            }

            if (Upload != null && Upload.Length > 0)
            {
                if (Upload.Length > MAX_UPLOAD_BYTES)
                {
                    ModelState.AddModelError("Upload", $"File is too large. Maximum allowed is {MAX_UPLOAD_BYTES / 1024 / 1024} MB.");
                    await OnGetAsync();
                    return Page();
                }
                using var ms = new MemoryStream();
                await Upload.CopyToAsync(ms);
                Document.ScanFileData = ms.ToArray();
                Document.Scanned = true;
                if (string.IsNullOrEmpty(Document.DocumentName)) Document.DocumentName = Upload.FileName;
            }

            Document.CreateDate = DateTime.UtcNow;
            Document.UpdateDate = DateTime.UtcNow;
            Document.CreatedBy ??= "system";
            Document.UpdatedBy ??= "system";

            _context.Documents.Add(Document);
            await _context.SaveChangesAsync();

            // create transaction for document upload
            try
            {
                var staffId = (await _context.Files.Where(f => f.FileId == Document.FileId).Select(f => f.StaffId).FirstOrDefaultAsync()) ?? "GEN";
                var dept = (await _context.Staff.FindAsync(staffId))?.DeptCode ?? "GEN";
                var year = DateTime.UtcNow.Year;
                var seq = await _seqService.GetNextAsync(dept ?? "GEN", year, Document.CreatedBy ?? "system");
                var display = $"{dept}{year % 100:D2}-{seq:D4}";
                var tx = new RegistryAPI.Models.Transaction
                {
                    StaffId = staffId ?? "",
                    FileId = Document.FileId,
                    TransactionType = "DocumentUploaded",
                    TransactionDate = DateTime.UtcNow,
                    CreatedBy = Document.CreatedBy ?? "system",
                    CreatedDate = DateTime.UtcNow,
                    SequenceNumber = seq,
                    SequenceId = $"{dept}-{year}",
                    DisplayNumber = display
                };
                _context.Transactions.Add(tx);
                await _context.SaveChangesAsync();
            }
            catch { }

            // After save, write physical file copy to wwwroot/uploads/{staffId}
            try
            {
                var staffId = (await _context.Files.Where(f => f.FileId == Document.FileId).Select(f => f.StaffId).FirstOrDefaultAsync()) ?? "unassigned";
                var uploadsRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", staffId);
                Directory.CreateDirectory(uploadsRoot);
                if (Upload != null && Upload.Length > 0)
                {
                    var safeName = Path.GetFileName(Upload.FileName);
                    var physicalName = $"{Document.DocumentId}_{safeName}";
                    var physicalPath = Path.Combine(uploadsRoot, physicalName);
                    using var fs = new FileStream(physicalPath, FileMode.Create);
                    await fs.WriteAsync(Document.ScanFileData ?? Array.Empty<byte>());
                }
            }
            catch
            {
                // ignore disk write failures but DB already contains file bytes
            }

            return RedirectToPage("Index");
        }
    }
}
