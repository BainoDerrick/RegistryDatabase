using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RegistryAPI.Data;
using RegistryAPI.Models;

namespace RegistryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DocumentController(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // GET: api/document
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Document>>> GetDocuments()
        {
            try
            {
                return await _context.Documents
                    .Include(d => d.File)
                    .Include(d => d.DocumentType)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/document/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Document>> GetDocument(int id)
        {
            try
            {
                var document = await _context.Documents
                    .Include(d => d.File)
                    .Include(d => d.DocumentType)
                    .FirstOrDefaultAsync(d => d.DocumentId == id);

                if (document == null)
                {
                    return NotFound();
                }

                return document;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/document
        [HttpPost]
        public async Task<ActionResult<Document>> CreateDocument(Document document)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                document.CreateDate = document.CreateDate ?? DateTime.UtcNow;
                document.UpdateDate = document.UpdateDate ?? DateTime.UtcNow;
                document.CreatedBy = document.CreatedBy ?? "system";
                document.UpdatedBy = document.UpdatedBy ?? "system";

                _context.Documents.Add(document);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetDocument), new { id = document.DocumentId }, document);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/document/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDocument(int id, Document document)
        {
            try
            {
                if (id != document.DocumentId)
                {
                    return BadRequest("Document ID mismatch.");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingDocument = await _context.Documents.FindAsync(id);
                if (existingDocument == null)
                {
                    return NotFound();
                }

                // Attach and update the entity to ensure tracking
                _context.Entry(existingDocument).CurrentValues.SetValues(document);

                // Validate foreign keys for update
                if (document.FileId.HasValue && !await _context.Files.AnyAsync(f => f.FileId == document.FileId))
                {
                    ModelState.AddModelError("fileId", $"File with ID {document.FileId} does not exist.");
                }
                if (document.DocTypeId.HasValue && !await _context.DocumentTypes.AnyAsync(dt => dt.DocTypeId == document.DocTypeId))
                {
                    ModelState.AddModelError("docTypeId", $"DocumentType with ID {document.DocTypeId} does not exist.");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                existingDocument.UpdateDate = DateTime.UtcNow; // Ensure update date is current

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/document/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDocument(int id)
        {
            try
            {
                var document = await _context.Documents.FindAsync(id);
                if (document == null)
                {
                    return NotFound();
                }

                _context.Documents.Remove(document);
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