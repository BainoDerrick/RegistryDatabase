using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RegistryAPI.Data;
using RegistryAPI.Models;
using System.Linq;

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

        // Helper to generate a new Staff ID. Uses deptCode as prefix when available.
        private async Task<string> GenerateStaffIdAsync(string? deptCode)
        {
            var prefix = string.IsNullOrWhiteSpace(deptCode) ? "STF" : deptCode!.ToUpper().Replace(" ", "");

            // Get existing IDs that start with the prefix
            var ids = await _context.Staff
                .Where(s => s.StaffId != null && s.StaffId.StartsWith(prefix))
                .Select(s => s.StaffId)
                .ToListAsync();

            var max = 0;
            foreach (var id in ids)
            {
                if (id == null) continue;
                if (id.Length <= prefix.Length) continue;
                var numericPart = new string(id.Skip(prefix.Length).Where(char.IsDigit).ToArray());
                if (int.TryParse(numericPart, out var n))
                {
                    if (n > max) max = n;
                }
            }

            var next = max + 1;
            return $"{prefix}{next.ToString("D3")}";
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Staff>>> GetStaff(
            [FromQuery] string? search = null,
            [FromQuery] string? firstName = null,
            [FromQuery] string? lastName = null,
            [FromQuery] string? email = null,
            [FromQuery] string? deptCode = null,
            [FromQuery] string? status = null,
            [FromQuery] string? role = null)
        {
            try
            {
                IQueryable<Staff> query = _context.Staff;

                if (!string.IsNullOrEmpty(search))
                {
                    var t = search.Trim().ToLower();
                    query = query.Where(x => (x.StaffId != null && x.StaffId.ToLower().Contains(t))
                                            || (x.FirstName != null && x.FirstName.ToLower().Contains(t))
                                            || (x.LastName != null && x.LastName.ToLower().Contains(t))
                                            || (x.Email != null && x.Email.ToLower().Contains(t))
                                            || (x.DeptCode != null && x.DeptCode.ToLower().Contains(t))
                                            || (x.Role != null && x.Role.ToLower().Contains(t))
                                            || (x.Title != null && x.Title.ToLower().Contains(t)));
                }

                if (!string.IsNullOrEmpty(firstName)) { var t = firstName.Trim().ToLower(); query = query.Where(x => x.FirstName != null && x.FirstName.ToLower().Contains(t)); }
                if (!string.IsNullOrEmpty(lastName)) { var t = lastName.Trim().ToLower(); query = query.Where(x => x.LastName != null && x.LastName.ToLower().Contains(t)); }
                if (!string.IsNullOrEmpty(email)) { var t = email.Trim().ToLower(); query = query.Where(x => x.Email != null && x.Email.ToLower().Contains(t)); }
                if (!string.IsNullOrEmpty(deptCode)) { var t = deptCode.Trim().ToLower(); query = query.Where(x => x.DeptCode != null && x.DeptCode.ToLower() == t); }
                if (!string.IsNullOrEmpty(status)) { var t = status.Trim().ToLower(); query = query.Where(x => x.Status != null && x.Status.ToLower() == t); }

                // Exclude terminated staff by default unless a status filter is provided
                if (string.IsNullOrEmpty(status))
                {
                    query = query.Where(x => !((x.Status != null && x.Status.ToLower() == "terminated") || x.TerminationDate.HasValue));
                }
                if (!string.IsNullOrEmpty(role)) { var t = role.Trim().ToLower(); query = query.Where(x => x.Role != null && x.Role.ToLower() == t); }

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

                // Generate a StaffId if not provided. Staff IDs follow the pattern {PREFIX}{NNN}.
                if (string.IsNullOrEmpty(staff.StaffId))
                {
                    staff.StaffId = await GenerateStaffIdAsync(staff.DeptCode);
                }

                // Password is optional for registry workers; ensure not null to avoid DB NOT NULL issues
                staff.Password ??= string.Empty;

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

                // If the incoming payload doesn't include a password, preserve the existing one
                if (string.IsNullOrEmpty(staff.Password))
                {
                    staff.Password = existingStaff.Password;
                }

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
                Console.WriteLine($"Verified update - FirstName: {updatedStaff?.FirstName}, EmploymentType: {updatedStaff?.EmploymentType}");

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