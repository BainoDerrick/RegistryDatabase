using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RegistryAPI.Data;
using RegistryAPI.Models;

namespace RegistryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SequenceController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SequenceController(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // GET: api/sequence
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Sequence>>> GetSequences()
        {
            return await _context.Sequences.ToListAsync();
        }

        // GET: api/sequence/SEQ001
        [HttpGet("{id}")]
        public async Task<ActionResult<Sequence>> GetSequence(string id)
        {
            var sequence = await _context.Sequences.FindAsync(id);

            if (sequence == null)
            {
                return NotFound(); // Returns NotFoundResult
            }

            return sequence; // Returns Sequence object
        }

        // POST: api/sequence
        [HttpPost]
        public async Task<ActionResult<Sequence>> CreateSequence(Sequence sequence)
        {
            return StatusCode(405, "Manual creation of sequences is disabled.");
        }

        // PUT: api/sequence/SEQ001
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSequence(string id, Sequence sequence)
        {
            return StatusCode(405, "Manual editing of sequences is disabled.");
        }

        // DELETE: api/sequence/SEQ001
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSequence(string id)
        {
            return StatusCode(405, "Deleting sequences via API is disabled.");
        }

        private bool SequenceExists(string id)
        {
            return _context.Sequences.Any(e => e.SequenceId == id);
        }
    }
}