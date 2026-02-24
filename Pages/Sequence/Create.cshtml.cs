using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RegistryAPI.Data;

namespace RegistryAPI.Pages.Sequence
{
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;

        public CreateModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public RegistryAPI.Models.Sequence Sequence { get; set; } = new RegistryAPI.Models.Sequence();

        public async Task<IActionResult> OnPostAsync()
        {
            // manual creation disabled
            return Forbid();
        }
    }
}
