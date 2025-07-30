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
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactions()
        {
            try
            {
                return await _context.Transactions
                    .Include(t => t.Staff)
                    .Include(t => t.File)
                    .ToListAsync();
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
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validate required TransactionId
                if (transaction.TransactionId == 0 || transaction.TransactionId < 0)
                {
                    ModelState.AddModelError("transactionId", "Transaction ID is required and must be a positive integer.");
                    return BadRequest(ModelState);
                }

                // Validate required fields
                if (string.IsNullOrEmpty(transaction.StaffId))
                {
                    ModelState.AddModelError("staffId", "Staff ID is required.");
                    return BadRequest(ModelState);
                }
                if (transaction.TransactionDate == default)
                {
                    ModelState.AddModelError("transactionDate", "Transaction date is required.");
                    return BadRequest(ModelState);
                }
                if (string.IsNullOrEmpty(transaction.CreatedBy))
                {
                    ModelState.AddModelError("createdBy", "Created by is required.");
                    return BadRequest(ModelState);
                }

                // Validate foreign keys
                if (!await _context.Staff.AnyAsync(s => s.StaffId == transaction.StaffId))
                {
                    ModelState.AddModelError("staffId", $"Staff with ID {transaction.StaffId} does not exist.");
                }
                if (transaction.FileId.HasValue && !await _context.Files.AnyAsync(f => f.FileId == transaction.FileId))
                {
                    ModelState.AddModelError("fileId", $"File with ID {transaction.FileId} does not exist.");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Check for duplicate TransactionId
                if (await _context.Transactions.AnyAsync(t => t.TransactionId == transaction.TransactionId))
                {
                    ModelState.AddModelError("transactionId", "Transaction ID already exists.");
                    return BadRequest(ModelState);
                }

                transaction.CreatedDate = transaction.CreatedDate ?? DateTime.UtcNow;
                transaction.UpdatedDate = transaction.UpdatedDate ?? DateTime.UtcNow;
                transaction.UpdatedBy = transaction.UpdatedBy ?? "system";

                _context.Transactions.Add(transaction);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetTransaction), new { id = transaction.TransactionId }, transaction);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/transactions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTransaction(int id, Transaction transaction)
        {
            try
            {
                if (id != transaction.TransactionId)
                {
                    return BadRequest("Transaction ID mismatch.");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingTransaction = await _context.Transactions.FindAsync(id);
                if (existingTransaction == null)
                {
                    return NotFound();
                }

                // Apply the recent fix using SetValues to ensure all properties are updated
                _context.Entry(existingTransaction).CurrentValues.SetValues(transaction);

                // Validate required fields
                if (string.IsNullOrEmpty(transaction.StaffId))
                {
                    ModelState.AddModelError("staffId", "Staff ID is required.");
                    return BadRequest(ModelState);
                }
                if (transaction.TransactionDate == default)
                {
                    ModelState.AddModelError("transactionDate", "Transaction date is required.");
                    return BadRequest(ModelState);
                }
                if (string.IsNullOrEmpty(transaction.CreatedBy))
                {
                    ModelState.AddModelError("createdBy", "Created by is required.");
                    return BadRequest(ModelState);
                }

                // Validate foreign keys if changed
                if (transaction.StaffId != existingTransaction.StaffId &&
                    !await _context.Staff.AnyAsync(s => s.StaffId == transaction.StaffId))
                {
                    ModelState.AddModelError("staffId", $"Staff with ID {transaction.StaffId} does not exist.");
                }
                if (transaction.FileId.HasValue && transaction.FileId != existingTransaction.FileId &&
                    !await _context.Files.AnyAsync(f => f.FileId == transaction.FileId))
                {
                    ModelState.AddModelError("fileId", $"File with ID {transaction.FileId} does not exist.");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                existingTransaction.UpdatedDate = DateTime.UtcNow;
                existingTransaction.UpdatedBy = transaction.UpdatedBy ?? "system";

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/transactions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            try
            {
                var transaction = await _context.Transactions.FindAsync(id);
                if (transaction == null)
                {
                    return NotFound();
                }

                _context.Transactions.Remove(transaction);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}