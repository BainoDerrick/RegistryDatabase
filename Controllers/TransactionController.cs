using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RegistryAPI.Data;
using RegistryAPI.Models;

namespace RegistryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TransactionsController(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // GET: api/transactions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactions(
            [FromQuery] string? search = null,
            [FromQuery] string? staffId = null,
            [FromQuery] string? transactionType = null,
            [FromQuery] string? status = null,
            [FromQuery] string? details = null,
            [FromQuery] string? sequenceId = null,
            [FromQuery] string? displayNumber = null)
        {
            try
            {
                IQueryable<Transaction> query = _context.Transactions
                    .Include(t => t.Staff)
                    .Include(t => t.File);

                if (!string.IsNullOrEmpty(search))
                {
                    var t = search.Trim().ToLower();
                    query = query.Where(x => (x.TransactionId.ToString().Contains(t))
                                           || (x.StaffId != null && x.StaffId.ToLower().Contains(t))
                                           || (x.TransactionType != null && x.TransactionType.ToLower().Contains(t))
                                           || (x.Status != null && x.Status.ToLower().Contains(t))
                                           || (x.Details != null && x.Details.ToLower().Contains(t))
                                           || (x.SequenceId != null && x.SequenceId.ToLower().Contains(t))
                                           || (x.DisplayNumber != null && x.DisplayNumber.ToLower().Contains(t)));
                }

                if (!string.IsNullOrEmpty(staffId)) { var t = staffId.Trim().ToLower(); query = query.Where(x => x.StaffId != null && x.StaffId.ToLower() == t); }
                if (!string.IsNullOrEmpty(transactionType)) { var t = transactionType.Trim().ToLower(); query = query.Where(x => x.TransactionType != null && x.TransactionType.ToLower().Contains(t)); }
                if (!string.IsNullOrEmpty(status)) { var t = status.Trim().ToLower(); query = query.Where(x => x.Status != null && x.Status.ToLower().Contains(t)); }
                if (!string.IsNullOrEmpty(details)) { var t = details.Trim().ToLower(); query = query.Where(x => x.Details != null && x.Details.ToLower().Contains(t)); }
                if (!string.IsNullOrEmpty(sequenceId)) { var t = sequenceId.Trim().ToLower(); query = query.Where(x => x.SequenceId != null && x.SequenceId.ToLower().Contains(t)); }
                if (!string.IsNullOrEmpty(displayNumber)) { var t = displayNumber.Trim().ToLower(); query = query.Where(x => x.DisplayNumber != null && x.DisplayNumber.ToLower().Contains(t)); }

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/transactions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Transaction>> GetTransaction(int id)
        {
            try
            {
                var transaction = await _context.Transactions
                    .Include(t => t.Staff)
                    .Include(t => t.File)
                    .FirstOrDefaultAsync(t => t.TransactionId == id);

                if (transaction == null)
                {
                    return NotFound();
                }

                return transaction;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/transactions
        [HttpPost]
        public async Task<ActionResult<Transaction>> CreateTransaction(Transaction transaction)
        {
            // Manual creation via API disabled; transactions are created automatically by system events.
            return StatusCode(405, "Manual creation of transactions is disabled.");
        }

        // PUT: api/transactions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTransaction(int id, Transaction transaction)
        {
            // Manual updates disabled
            return StatusCode(405, "Manual editing of transactions is disabled.");
        }

        // DELETE: api/transactions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            return StatusCode(405, "Deleting transactions via API is disabled.");
        }
    }
}