using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegistryAPI.Models
{
    public class DocumentType
    {
        [Column("doc_type_id")]
        public int DocTypeId { get; set; }
        [Column("type_name")]
        public string? TypeName { get; set; }
        [Column("description")]
        public string? Description { get; set; }
        [Column("created_by")]
        public string? CreatedBy { get; set; }
        [Column("create_date")]
        public DateTime? CreateDate { get; set; }
        [Column("updated_by")]
        public string? UpdatedBy { get; set; }
        [Column("update_date")]
        public DateTime? UpdateDate { get; set; }

        public ICollection<Document>? Documents { get; set; }
    }
}