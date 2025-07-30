using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RegistryAPI.Data;
using RegistryAPI.Models;

namespace RegistryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DepartmentController(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // GET: api/department
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Department>>> GetDepartments()
        {
            return await _context.Departments
                .Include(d => d.StaffMembers)
                .ToListAsync();
        }

        // GET: api/department/6
        [HttpGet("{id}")]
        public async Task<ActionResult<Department>> GetDepartment(string id)
        {
            var department = await _context.Departments
                .Include(d => d.StaffMembers)
                .FirstOrDefaultAsync(d => d.DeptId == id);

            if (department == null)
            {
                return NotFound();
            }

            return department;
        }

        // POST: api/department
        [HttpPost]
        public async Task<ActionResult<Department>> CreateDepartment(Department department)
        {
            try
            {
                Console.WriteLine($"Received POST request: {System.Text.Json.JsonSerializer.Serialize(department)}");
                if (!ModelState.IsValid)
                {
                    Console.WriteLine("Error: Model state is invalid. Errors: " + string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                    return BadRequest(ModelState);
                }

                if (string.IsNullOrEmpty(department.DeptId))
                {
                    ModelState.AddModelError("deptId", "Department ID is required.");
                    Console.WriteLine("Error: Department ID is required.");
                    return BadRequest(ModelState);
                }

                if (await _context.Departments.AnyAsync(d => d.DeptId == department.DeptId))
                {
                    ModelState.AddModelError("deptId", "Department ID already exists.");
                    Console.WriteLine("Error: Department ID already exists.");
                    return BadRequest(ModelState);
                }

                if (string.IsNullOrEmpty(department.DeptCode))
                {
                    ModelState.AddModelError("deptCode", "Department code is required.");
                    Console.WriteLine("Error: Department code is required.");
                    return BadRequest(ModelState);
                }

                department.CreateDate = department.CreateDate ?? DateTime.UtcNow;
                department.UpdateDate = department.UpdateDate ?? DateTime.UtcNow;
                department.CreatedBy = department.CreatedBy ?? "system";
                department.UpdatedBy = department.UpdatedBy ?? "system";

                _context.Departments.Add(department);
                await _context.SaveChangesAsync();
                Console.WriteLine($"Department created successfully: {department.DeptId}");

                return CreatedAtAction(nameof(GetDepartment), new { id = department.DeptId }, department);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CreateDepartment Error: {ex.ToString()}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/department/6
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDepartment(string id, Department department)
        {
            try
            {
                Console.WriteLine($"Received PUT request for id: {id}, payload: {System.Text.Json.JsonSerializer.Serialize(department)}");
                if (id != department.DeptId)
                {
                    Console.WriteLine("Error: Department ID mismatch.");
                    return BadRequest("Department ID mismatch.");
                }

                if (!ModelState.IsValid)
                {
                    Console.WriteLine("Error: Model state is invalid. Errors: " + string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                    return BadRequest(ModelState);
                }

                var existingDepartment = await _context.Departments.FindAsync(id);
                if (existingDepartment == null)
                {
                    Console.WriteLine($"Error: Department with ID {id} not found.");
                    return NotFound($"Department with ID {id} not found.");
                }

                // Explicitly mark the entity as modified to ensure tracking
                _context.Entry(existingDepartment).State = EntityState.Modified;

                Console.WriteLine("Updating department with new values...");
                existingDepartment.DeptName = department.DeptName ?? existingDepartment.DeptName;
                existingDepartment.DeptCode = department.DeptCode ?? existingDepartment.DeptCode;
                existingDepartment.CreatedBy = department.CreatedBy ?? existingDepartment.CreatedBy;
                existingDepartment.CreateDate = department.CreateDate ?? existingDepartment.CreateDate;
                existingDepartment.UpdatedBy = department.UpdatedBy ?? "system";
                existingDepartment.UpdateDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                Console.WriteLine("Department updated successfully: " + id);

                // Verify the change by querying back (optional debug)
                var updatedDepartment = await _context.Departments.FindAsync(id);
                Console.WriteLine($"Verified update - DeptName: {updatedDepartment.DeptName}");

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UpdateDepartment Error: {ex.ToString()}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/department/6
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartment(string id)
        {
            try
            {
                Console.WriteLine($"Received DELETE request for id: {id}");
                var department = await _context.Departments.FindAsync(id);
                if (department == null)
                {
                    Console.WriteLine($"Error: Department with ID {id} not found.");
                    return NotFound($"Department with ID {id} not found.");
                }

                _context.Departments.Remove(department);
                await _context.SaveChangesAsync();
                Console.WriteLine("Department deleted successfully: " + id);
                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DeleteDepartment Error: {ex.ToString()}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}