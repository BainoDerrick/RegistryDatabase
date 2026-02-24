using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RegistryAPI.Data;

namespace RegistryAPI.Pages.DocumentType
{
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;

        public CreateModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public RegistryAPI.Models.DocumentType DocType { get; set; } = new RegistryAPI.Models.DocumentType();

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            DocType.CreateDate = DateTime.UtcNow;
            DocType.UpdateDate = DateTime.UtcNow;
            DocType.CreatedBy ??= "system";
            DocType.UpdatedBy ??= "system";

            _context.DocumentTypes.Add(DocType);
            await _context.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}
