using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RegistryAPI.Data;
using RegistryAPI.Models;

namespace RegistryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StaffController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StaffController(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Staff>>> GetStaff([FromQuery] string? deptCode = null, [FromQuery] string? status = null)
        {
            try
            {
                IQueryable<Staff> query = _context.Staff;
                if (!string.IsNullOrEmpty(deptCode)) query = query.Where(s => s.DeptCode == deptCode);
                if (!string.IsNullOrEmpty(status)) query = query.Where(s => s.Status == status);
                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Staff>> GetStaff(string id)
        {
            try
            {
                var staff = await _context.Staff.Include(s => s.Department).FirstOrDefaultAsync(s => s.StaffId == id);
                if (staff == null) return NotFound($"Staff with ID {id} not found.");
                return staff;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Staff>> CreateStaff(Staff staff)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (string.IsNullOrEmpty(staff.StaffId))
                {
                    ModelState.AddModelError("staffId", "Staff ID is required.");
                    return BadRequest(ModelState);
                }

                if (string.IsNullOrEmpty(staff.Password))
                {
                    ModelState.AddModelError("password", "Password is required.");
                    return BadRequest(ModelState);
                }

                if (string.IsNullOrEmpty(staff.Role))
                {
                    ModelState.AddModelError("role", "Role is required.");
                    return BadRequest(ModelState);
                }

                if (!string.IsNullOrEmpty(staff.DeptCode))
                {
                    var department = await _context.Departments.FirstOrDefaultAsync(d => d.DeptCode == staff.DeptCode);
                    if (department == null)
                    {
                        ModelState.AddModelError("deptCode", $"Department with code {staff.DeptCode} does not exist.");
                        return BadRequest(ModelState);
                    }
                }

                staff.CreateDate = staff.CreateDate ?? DateTime.UtcNow;
                staff.UpdateDate = staff.UpdateDate ?? DateTime.UtcNow;
                staff.CreatedBy = staff.CreatedBy ?? "system";
                staff.UpdatedBy = staff.UpdatedBy ?? "system";

                if (await _context.Staff.AnyAsync(s => s.StaffId == staff.StaffId))
                {
                    ModelState.AddModelError("staffId", "Staff ID already exists.");
                    return BadRequest(ModelState);
                }

                _context.Staff.Add(staff);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetStaff), new { id = staff.StaffId }, staff);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CreateStaff Error: {ex.ToString()}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStaff(string id, Staff staff)
        {
            try
            {
                Console.WriteLine($"Received PUT request for id: {id}, payload: {System.Text.Json.JsonSerializer.Serialize(staff)}");
                if (id != staff.StaffId) return BadRequest("Staff ID mismatch.");

                // Validate required fields for PUT
                if (string.IsNullOrEmpty(staff.StaffId))
                {
                    ModelState.AddModelError("staffId", "Staff ID is required.");
                    Console.WriteLine("Error: Model state is invalid. Errors: " + string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                    return BadRequest(ModelState);
                }
                if (string.IsNullOrEmpty(staff.Password))
                {
                    ModelState.AddModelError("password", "Password is required.");
                    Console.WriteLine("Error: Model state is invalid. Errors: " + string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                    return BadRequest(ModelState);
                }
                if (string.IsNullOrEmpty(staff.Role))
                {
                    ModelState.AddModelError("role", "Role is required.");
                    Console.WriteLine("Error: Model state is invalid. Errors: " + string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                    return BadRequest(ModelState);
                }

                if (!ModelState.IsValid)
                {
                    Console.WriteLine("Error: Model state is invalid. Errors: " + string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                    return BadRequest(ModelState);
                }

                var existingStaff = await _context.Staff.FindAsync(id);
                if (existingStaff == null) return NotFound($"Staff with ID {id} not found.");

                if (!string.IsNullOrEmpty(staff.DeptCode) && staff.DeptCode != existingStaff.DeptCode)
                {
                    var department = await _context.Departments.FirstOrDefaultAsync(d => d.DeptCode == staff.DeptCode);
                    if (department == null)
                    {
                        ModelState.AddModelError("deptCode", $"Department with code {staff.DeptCode} does not exist.");
                        return BadRequest(ModelState);
                    }
                }

                // Apply the recent fix using SetValues to ensure all properties are updated
                _context.Entry(existingStaff).CurrentValues.SetValues(staff);

                // Override UpdateDate to current time
                existingStaff.UpdateDate = DateTime.UtcNow;
                existingStaff.UpdatedBy = staff.UpdatedBy ?? "system";

                await _context.SaveChangesAsync();
                Console.WriteLine("Staff updated successfully: " + id);

                // Verify the change by querying back
                var updatedStaff = await _context.Staff.FindAsync(id);
                Console.WriteLine($"Verified update - FirstName: {updatedStaff.FirstName}, EmploymentType: {updatedStaff.EmploymentType}");

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UpdateStaff Error: {ex.ToString()}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStaff(string id)
        {
            try
            {
                var staff = await _context.Staff.FindAsync(id);
                if (staff == null) return NotFound($"Staff with ID {id} not found.");
                _context.Staff.Remove(staff);
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