using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegistryAPI.Models
{
    public class File
    {
        [Column("file_id")]
        public int FileId { get; set; }
        [Column("staff_id")]
        public string? StaffId { get; set; }
        [Column("file_status")]
        public string? FileStatus { get; set; }
        [Column("created_by")]
        public string? CreatedBy { get; set; }
        [Column("create_date")]
        public DateTime? CreateDate { get; set; }
        [Column("updated_by")]
        public string? UpdatedBy { get; set; }
        [Column("update_date")]
        public DateTime? UpdateDate { get; set; }

        public Staff? Staff { get; set; }
        public ICollection<Document>? Documents { get; set; }
        public ICollection<Transaction>? Transactions { get; set; }
    }
}