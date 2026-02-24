using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegistryAPI.Models
{
    public class Termination
    {
        [Column("termination_id")]
        [System.ComponentModel.DataAnnotations.Schema.DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int TerminationId { get; set; }
        [Column("staff_id")]
        public string? StaffId { get; set; }
        [Column("reason", TypeName = "text")]
        [MaxLength(1000, ErrorMessage = "Reason cannot exceed 1000 characters.")]
        public string? Reason { get; set; }
        [Column("created_by")]
        public string? CreatedBy { get; set; }
        [Column("create_date")]
        public DateTime? CreateDate { get; set; }
        [Column("updated_by")]
        public string? UpdatedBy { get; set; }
        [Column("update_date")]
        public DateTime? UpdateDate { get; set; }

        public Staff? Staff { get; set; }
    }
}