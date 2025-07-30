using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegistryAPI.Models
{
    public class Document
    {
        [Column("document_id")]
        public int DocumentId { get; set; }
        [Column("file_id")]
        public int? FileId { get; set; }
        [Column("doc_type_id")]
        public int? DocTypeId { get; set; }
        [Column("document_name")]
        public string? DocumentName { get; set; }
        [Column("document_date")]
        public DateTime? DocumentDate { get; set; }
        [Column("description")]
        public string? Description { get; set; }
        [Column("scanned")]
        public bool? Scanned { get; set; }
        [Column("scan_file_data")]
        public byte[]? ScanFileData { get; set; }
        [Column("created_by")]
        public string? CreatedBy { get; set; }
        [Column("create_date")]
        public DateTime? CreateDate { get; set; }
        [Column("updated_by")]
        public string? UpdatedBy { get; set; }
        [Column("update_date")]
        public DateTime? UpdateDate { get; set; }

        public File? File { get; set; }
        public DocumentType? DocumentType { get; set; }
    }
}