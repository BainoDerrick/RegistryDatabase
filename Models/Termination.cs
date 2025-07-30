using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegistryAPI.Models
{
    public class Termination
    {
        [Column("termination_id")]
        public int TerminationId { get; set; }
        [Column("staff_id")]
        public string? StaffId { get; set; }
        [Column("reason")]
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