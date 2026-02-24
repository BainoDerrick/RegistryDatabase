using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RegistryAPI.Data;

namespace RegistryAPI.Pages.DocumentType
{
    public class DetailsModel : PageModel
    {
        private readonly AppDbContext _context;

        public DetailsModel(AppDbContext context)
        {
            _context = context;
        }

        public RegistryAPI.Models.DocumentType DocType { get; set; } = new RegistryAPI.Models.DocumentType();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var dt = await _context.DocumentTypes.FirstOrDefaultAsync(d => d.DocTypeId == id);
            if (dt == null) return NotFound();
            DocType = dt;
            return Page();
        }
    }
}
