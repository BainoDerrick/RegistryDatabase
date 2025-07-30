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
            _context.Sequences.Add(sequence);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSequence), new { id = sequence.SequenceId }, sequence);
        }

        // PUT: api/sequence/SEQ001
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSequence(string id, Sequence sequence)
        {
            if (id != sequence.SequenceId)
            {
                return BadRequest();
            }

            var existingSequence = await _context.Sequences.FindAsync(id);
            if (existingSequence == null)
            {
                return NotFound();
            }

            // Apply the recent fix using SetValues to ensure all properties are updated
            _context.Entry(existingSequence).CurrentValues.SetValues(sequence);

            // Override CreateDate and UpdatedBy if not provided (assuming UpdateDate is missing from model)
            existingSequence.CreateDate = existingSequence.CreateDate ?? DateTime.UtcNow;
            // Note: UpdateDate is not in Sequence.cs; consider adding it to the model if needed
            existingSequence.CreatedBy = sequence.CreatedBy ?? "system";

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SequenceExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/sequence/SEQ001
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSequence(string id)
        {
            var sequence = await _context.Sequences.FindAsync(id);
            if (sequence == null)
            {
                return NotFound();
            }

            _context.Sequences.Remove(sequence);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SequenceExists(string id)
        {
            return _context.Sequences.Any(e => e.SequenceId == id);
        }
    }
}