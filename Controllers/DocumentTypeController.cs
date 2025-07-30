using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RegistryAPI.Data;
using RegistryAPI.Models;

namespace RegistryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentTypeController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DocumentTypeController(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // GET: api/documenttype
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DocumentType>>> GetDocumentTypes()
        {
            try
            {
                return await _context.DocumentTypes
                    .Include(dt => dt.Documents)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/documenttype/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DocumentType>> GetDocumentType(int id)
        {
            try
            {
                var docType = await _context.DocumentTypes
                    .Include(dt => dt.Documents)
                    .FirstOrDefaultAsync(dt => dt.DocTypeId == id);

                if (docType == null)
                {
                    return NotFound();
                }

                return docType;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/documenttype
        [HttpPost]
        public async Task<ActionResult<DocumentType>> CreateDocumentType(DocumentType docType)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                docType.CreateDate = docType.CreateDate ?? DateTime.UtcNow;
                docType.UpdateDate = docType.UpdateDate ?? DateTime.UtcNow;
                docType.CreatedBy = docType.CreatedBy ?? "system";
                docType.UpdatedBy = docType.UpdatedBy ?? "system";

                _context.DocumentTypes.Add(docType);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetDocumentType), new { id = docType.DocTypeId }, docType);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/documenttype/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDocumentType(int id, DocumentType docType)
        {
            try
            {
                if (id != docType.DocTypeId)
                {
                    return BadRequest("DocumentType ID mismatch.");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingDocType = await _context.DocumentTypes.FindAsync(id);
                if (existingDocType == null)
                {
                    return NotFound();
                }

                // Apply changes using SetValues to ensure tracking and update
                _context.Entry(existingDocType).CurrentValues.SetValues(docType);

                existingDocType.UpdateDate = DateTime.UtcNow; // Ensure update date is current

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/documenttype/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDocumentType(int id)
        {
            try
            {
                var docType = await _context.DocumentTypes.FindAsync(id);
                if (docType == null)
                {
                    return NotFound();
                }

                _context.DocumentTypes.Remove(docType);
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