using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RegistryAPI.Data;

namespace RegistryAPI.Pages.File
{
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;

        private readonly RegistryAPI.Services.SequenceService _seqService;

        public CreateModel(AppDbContext context, RegistryAPI.Services.SequenceService seqService)
        {
            _context = context;
            _seqService = seqService;
        }

        [BindProperty]
        public RegistryAPI.Models.File FileItem { get; set; } = new RegistryAPI.Models.File();

        public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> StaffOptions { get; set; } = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();
        public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> ExistingFiles { get; set; } = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

        public async Task<IActionResult> OnGetAsync()
        {
            // populate staff dropdown with id and name for easy identification
            var staff = await _context.Staff.OrderBy(s => s.StaffId).ToListAsync();
            StaffOptions = staff.Select(s => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = s.StaffId,
                Text = string.IsNullOrWhiteSpace(s.FirstName) && string.IsNullOrWhiteSpace(s.LastName) ? s.StaffId : $"{s.StaffId} - {s.FirstName} {s.LastName}"
            }).ToList();

            var files = await _context.Files.Include(f => f.Staff).OrderBy(f => f.FileId).ToListAsync();
            ExistingFiles = files.Select(f => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = f.FileId.ToString(),
                Text = f.Staff != null ? $"{f.FileId} - {f.Staff.FirstName} {f.Staff.LastName}" : f.FileId.ToString()
            }).ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            if (!string.IsNullOrEmpty(FileItem.StaffId) && !await _context.Staff.AnyAsync(s => s.StaffId == FileItem.StaffId))
            {
                ModelState.AddModelError("FileItem.StaffId", "Staff does not exist.");
                return Page();
            }

            FileItem.CreateDate = DateTime.UtcNow;
            FileItem.UpdateDate = DateTime.UtcNow;
            FileItem.CreatedBy ??= "system";
            FileItem.UpdatedBy ??= "system";

            _context.Files.Add(FileItem);
            await _context.SaveChangesAsync();

            // Automatically create a transaction for file creation
            try
            {
                var dept = FileItem.StaffId != null ? (await _context.Staff.FindAsync(FileItem.StaffId))?.DeptCode ?? "GEN" : "GEN";
                var year = DateTime.UtcNow.Year;
                var seq = await _seqService.GetNextAsync(dept ?? "GEN", year, FileItem.CreatedBy ?? "system");
                var display = $"{dept}{year % 100:D2}-{seq:D4}";
                var tx = new RegistryAPI.Models.Transaction
                {
                    StaffId = FileItem.StaffId ?? "",
                    FileId = FileItem.FileId,
                    TransactionType = "FileCreated",
                    TransactionDate = DateTime.UtcNow,
                    CreatedBy = FileItem.CreatedBy ?? "system",
                    CreatedDate = DateTime.UtcNow,
                    SequenceNumber = seq,
                    SequenceId = $"{dept}-{year}",
                    DisplayNumber = display
                };
                _context.Transactions.Add(tx);
                await _context.SaveChangesAsync();
            }
            catch
            {
                // do not block file creation if sequence/transaction fails; log later if needed
            }
            return RedirectToPage("Index");
        }
    }
}
