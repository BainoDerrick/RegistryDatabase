using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegistryAPI.Models
{
    public class Transaction
    {
        [Column("transaction_id")]
        public int TransactionId { get; set; }

        [Column("staff_id")]
        public string StaffId { get; set; }

        [Column("file_id")]
        public int? FileId { get; set; }

        [Column("transaction_type")]
        public string? TransactionType { get; set; }

        [Column("transaction_date")]
        public DateTime TransactionDate { get; set; }

        [Column("status")]
        public string? Status { get; set; }

        [Column("details")]
        public string? Details { get; set; }

        [Column("created_by")]
        public string CreatedBy { get; set; }

        [Column("created_date")]
        public DateTime? CreatedDate { get; set; }

        [Column("updated_by")]
        public string? UpdatedBy { get; set; }

        [Column("updated_date")]
        public DateTime? UpdatedDate { get; set; }

        public Staff? Staff { get; set; }
        public File? File { get; set; }
    }
}