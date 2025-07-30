using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegistryAPI.Models
{
    public class Department
    {
        [Column("dept_id")]
        [Required]
        public string DeptId { get; set; }

        [Column("dept_code")]
        [Required]
        public string DeptCode { get; set; }

        [Column("dept_name")]
        public string? DeptName { get; set; }

        [Column("created_by")]
        public string? CreatedBy { get; set; }

        [Column("create_date")]
        public DateTime? CreateDate { get; set; }

        [Column("updated_by")]
        public string? UpdatedBy { get; set; }

        [Column("update_date")]
        public DateTime? UpdateDate { get; set; }

        // Navigation property
        public ICollection<Staff>? StaffMembers { get; set; }
    }
}