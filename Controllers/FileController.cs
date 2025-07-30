using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RegistryAPI.Data;
using RegistryAPI.Models;
using File = RegistryAPI.Models.File; // Resolves conflict with System.IO.File

namespace RegistryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FileController(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // GET: api/file
        [HttpGet]
        public async Task<ActionResult<IEnumerable<File>>> GetFiles()
        {
            try
            {
                return await _context.Files
                    .Include(f => f.Staff)
                    .Include(f => f.Documents)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/file/5
        [HttpGet("{id}")]
        public async Task<ActionResult<File>> GetFile(int id)
        {
            try
            {
                var file = await _context.Files
                    .Include(f => f.Staff)
                    .Include(f => f.Documents)
                    .FirstOrDefaultAsync(f => f.FileId == id);

                if (file == null)
                {
                    return NotFound();
                }

                return file;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/file
        [HttpPost]
        public async Task<ActionResult<File>> CreateFile(File file)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validate foreign key (StaffId) if provided
                if (!string.IsNullOrEmpty(file.StaffId) && !await _context.Staff.AnyAsync(s => s.StaffId == file.StaffId))
                {
                    ModelState.AddModelError("staffId", $"Staff with ID {file.StaffId} does not exist.");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                file.CreateDate = file.CreateDate ?? DateTime.UtcNow;
                file.UpdateDate = file.UpdateDate ?? DateTime.UtcNow;
                file.CreatedBy = file.CreatedBy ?? "system";
                file.UpdatedBy = file.UpdatedBy ?? "system";

                _context.Files.Add(file);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetFile), new { id = file.FileId }, file);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/file/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFile(int id, File file)
        {
            try
            {
                if (id != file.FileId)
                {
                    return BadRequest("File ID mismatch.");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingFile = await _context.Files.FindAsync(id);
                if (existingFile == null)
                {
                    return NotFound();
                }

                // Apply changes using SetValues to ensure tracking and update
                _context.Entry(existingFile).CurrentValues.SetValues(file);

                // Validate foreign key (StaffId) if provided or changed
                if (!string.IsNullOrEmpty(file.StaffId) && !await _context.Staff.AnyAsync(s => s.StaffId == file.StaffId))
                {
                    ModelState.AddModelError("staffId", $"Staff with ID {file.StaffId} does not exist.");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                existingFile.UpdateDate = DateTime.UtcNow; // Ensure update date is current

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/file/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFile(int id)
        {
            try
            {
                var file = await _context.Files.FindAsync(id);
                if (file == null)
                {
                    return NotFound();
                }

                _context.Files.Remove(file);
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