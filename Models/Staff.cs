using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegistryAPI.Models
{
    public class Staff
    {
        [Column("staff_id")]
        [Required]
        public string StaffId { get; set; }

        [Column("first_name")]
        public string? FirstName { get; set; }

        [Column("last_name")]
        public string? LastName { get; set; }

        [Column("email")]
        public string? Email { get; set; }

        [Column("dept_code")]
        public string? DeptCode { get; set; }

        [Column("employment_type")]
        public string? EmploymentType { get; set; }

        [Column("status")]
        public string? Status { get; set; }

        [Column("hire_date")]
        public DateTime? HireDate { get; set; }

        [Column("termination_date")]
        public DateTime? TerminationDate { get; set; }

        [Column("title")]
        public string? Title { get; set; }

        [Column("created_by")]
        public string? CreatedBy { get; set; }

        [Column("create_date")]
        public DateTime? CreateDate { get; set; }

        [Column("updated_by")]
        public string? UpdatedBy { get; set; }

        [Column("update_date")]
        public DateTime? UpdateDate { get; set; }

        [Column("password")] // Already added
        [Required]
        public string Password { get; set; }

        [Column("role")] // Added for the new column
        [Required] // Since it's NOT NULL in the database
        public string Role { get; set; } // Default to null for now, will be set via API

        // Navigation properties
        public Department? Department { get; set; }
        public ICollection<File>? Files { get; set; }
        public ICollection<Transaction>? Transactions { get; set; }
        public Termination? Termination { get; set; }
    }
}