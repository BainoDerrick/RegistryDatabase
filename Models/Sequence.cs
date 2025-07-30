using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegistryAPI.Models
{
    public class Sequence
    {
        [Column("sequence_id")]
        public string SequenceId { get; set; }
        [Column("dept_code")]
        public string? DeptCode { get; set; }
        [Column("year")]
        public int? Year { get; set; }
        [Column("sequence_number")]
        public int? SequenceNumber { get; set; }
        [Column("created_by")]
        public string? CreatedBy { get; set; }
        [Column("create_date")]
        public DateTime? CreateDate { get; set; }
    }
}