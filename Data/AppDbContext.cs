using Microsoft.EntityFrameworkCore;
using RegistryAPI.Models;
using File = RegistryAPI.Models.File;

namespace RegistryAPI.Data
{
      public class AppDbContext : DbContext
      {
            public DbSet<Staff> Staff { get; set; }
            public DbSet<Department> Departments { get; set; }
            public DbSet<File> Files { get; set; }
            public DbSet<Document> Documents { get; set; }
            public DbSet<DocumentType> DocumentTypes { get; set; }
            public DbSet<Transaction> Transactions { get; set; }
            public DbSet<Termination> Terminations { get; set; }
            public DbSet<Sequence> Sequences { get; set; }

            public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
            {
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                  // Staff configuration (unchanged)
                  modelBuilder.Entity<Staff>(entity =>
                  {
                        entity.ToTable("staff");
                        entity.HasKey(s => s.StaffId).HasName("PRIMARY");
                        entity.Property(s => s.StaffId).HasColumnName("staff_id").IsRequired();
                        entity.Property(s => s.FirstName).HasColumnName("first_name");
                        entity.Property(s => s.LastName).HasColumnName("last_name");
                        entity.Property(s => s.Email).HasColumnName("email");
                        entity.Property(s => s.DeptCode).HasColumnName("dept_code").IsRequired(false);
                        entity.Property(s => s.EmploymentType).HasColumnName("employment_type");
                        entity.Property(s => s.Status).HasColumnName("status");
                        entity.Property(s => s.HireDate).HasColumnName("hire_date");
                        entity.Property(s => s.TerminationDate).HasColumnName("termination_date");
                        entity.Property(s => s.Title).HasColumnName("title");
                        entity.Property(s => s.CreatedBy).HasColumnName("created_by");
                        entity.Property(s => s.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                        entity.Property(s => s.UpdatedBy).HasColumnName("updated_by");
                        entity.Property(s => s.UpdateDate).HasColumnName("update_date").HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");
                        entity.Property(s => s.Password).HasColumnName("password").IsRequired();
                        entity.Property(s => s.Role).HasColumnName("role").IsRequired();
                        entity.HasOne(s => s.Department)
                      .WithMany(d => d.StaffMembers)
                      .HasForeignKey(s => s.DeptCode)
                      .OnDelete(DeleteBehavior.Restrict);
                  });

                  // Department configuration (unchanged)
                  modelBuilder.Entity<Department>(entity =>
                  {
                        entity.ToTable("department");
                        entity.HasKey(d => d.DeptId);
                        entity.Property(d => d.DeptId).HasColumnName("dept_id").IsRequired();
                        entity.Property(d => d.DeptCode).HasColumnName("dept_code").IsRequired();
                        entity.Property(d => d.DeptName).HasColumnName("dept_name");
                        entity.Property(d => d.CreatedBy).HasColumnName("created_by");
                        entity.Property(d => d.CreateDate).HasColumnName("create_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                        entity.Property(d => d.UpdatedBy).HasColumnName("updated_by");
                        entity.Property(d => d.UpdateDate).HasColumnName("update_date").HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");
                        entity.HasMany(d => d.StaffMembers)
                      .WithOne(s => s.Department)
                      .HasForeignKey(s => s.DeptCode);
                  });

                  // File configuration (unchanged)
                  modelBuilder.Entity<File>(entity =>
                  {
                        entity.ToTable("files");
                        entity.HasKey(f => f.FileId);
                        entity.Property(f => f.FileId).HasColumnName("file_id");
                        entity.Property(f => f.StaffId).HasColumnName("staff_id").IsRequired(false);
                        entity.Property(f => f.FileStatus).HasColumnName("file_status");
                        entity.Property(f => f.CreatedBy).HasColumnName("created_by");
                        entity.Property(f => f.CreateDate).HasColumnName("create_date");
                        entity.Property(f => f.UpdatedBy).HasColumnName("updated_by");
                        entity.Property(f => f.UpdateDate).HasColumnName("update_date");
                        entity.HasOne(f => f.Staff)
                      .WithMany(s => s.Files)
                      .HasForeignKey(f => f.StaffId)
                      .OnDelete(DeleteBehavior.Restrict);
                  });

                  // Document configuration (unchanged)
                  modelBuilder.Entity<Document>(entity =>
                  {
                        entity.ToTable("documents");
                        entity.HasKey(d => d.DocumentId);
                        entity.Property(d => d.DocumentId).HasColumnName("document_id");
                        entity.Property(d => d.FileId).HasColumnName("file_id");
                        entity.Property(d => d.DocTypeId).HasColumnName("doc_type_id");
                        entity.Property(d => d.DocumentName).HasColumnName("document_name");
                        entity.Property(d => d.DocumentDate).HasColumnName("document_date");
                        entity.Property(d => d.Description).HasColumnName("description");
                        entity.Property(d => d.Scanned).HasColumnName("scanned");
                        entity.Property(d => d.ScanFileData).HasColumnName("scan_file_data");
                        entity.Property(d => d.CreatedBy).HasColumnName("created_by");
                        entity.Property(d => d.CreateDate).HasColumnName("create_date");
                        entity.Property(d => d.UpdatedBy).HasColumnName("updated_by");
                        entity.Property(d => d.UpdateDate).HasColumnName("update_date");
                        entity.HasOne(d => d.File)
                      .WithMany(f => f.Documents)
                      .HasForeignKey(d => d.FileId);
                        entity.HasOne(d => d.DocumentType)
                      .WithMany(dt => dt.Documents)
                      .HasForeignKey(d => d.DocTypeId);
                  });

                  // Transaction configuration
                  modelBuilder.Entity<Transaction>(entity =>
                  {
                        entity.ToTable("transactions");
                        entity.HasKey(t => t.TransactionId).HasName("transaction_id");
                        entity.Property(t => t.TransactionId).HasColumnName("transaction_id").IsRequired();
                        entity.Property(t => t.StaffId).HasColumnName("staff_id").IsRequired();
                        entity.Property(t => t.FileId).HasColumnName("file_id").IsRequired(false);
                        entity.Property(t => t.TransactionType).HasColumnName("transaction_type");
                        entity.Property(t => t.TransactionDate).HasColumnName("transaction_date").IsRequired();
                        entity.Property(t => t.Status).HasColumnName("status");
                        entity.Property(t => t.Details).HasColumnName("details");
                        entity.Property(t => t.CreatedBy).HasColumnName("created_by").IsRequired();
                        entity.Property(t => t.CreatedDate).HasColumnName("created_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
                        entity.Property(t => t.UpdatedBy).HasColumnName("updated_by");
                        entity.Property(t => t.UpdatedDate).HasColumnName("updated_date"); // Corrected to updated_date
                        entity.HasOne(t => t.Staff)
                      .WithMany(s => s.Transactions)
                      .HasForeignKey(t => t.StaffId)
                      .OnDelete(DeleteBehavior.Restrict);
                        entity.HasOne(t => t.File)
                      .WithMany(f => f.Transactions)
                      .HasForeignKey(t => t.FileId)
                      .OnDelete(DeleteBehavior.Restrict);
                  });

                  // Termination configuration (unchanged)
                  modelBuilder.Entity<Termination>(entity =>
                  {
                        entity.ToTable("termination");
                        entity.HasKey(t => t.TerminationId);
                        entity.Property(t => t.TerminationId).HasColumnName("termination_id");
                        entity.Property(t => t.StaffId).HasColumnName("staff_id");
                        entity.Property(t => t.Reason).HasColumnName("reason");
                        entity.Property(t => t.CreatedBy).HasColumnName("created_by");
                        entity.Property(t => t.CreateDate).HasColumnName("create_date");
                        entity.Property(t => t.UpdatedBy).HasColumnName("updated_by");
                        entity.Property(t => t.UpdateDate).HasColumnName("update_date");
                        entity.HasOne(t => t.Staff)
                      .WithOne(s => s.Termination)
                      .HasForeignKey<Termination>(t => t.StaffId);
                  });

                  // Sequence configuration (unchanged)
                  modelBuilder.Entity<Sequence>(entity =>
                  {
                        entity.ToTable("sequence");
                        entity.HasKey(s => s.SequenceId);
                        entity.Property(s => s.SequenceId).HasColumnName("sequence_id");
                        entity.Property(s => s.DeptCode).HasColumnName("dept_code");
                        entity.Property(s => s.Year).HasColumnName("year");
                        entity.Property(s => s.SequenceNumber).HasColumnName("sequence_number");
                        entity.Property(s => s.CreatedBy).HasColumnName("created_by");
                        entity.Property(s => s.CreateDate).HasColumnName("create_date");
                  });

                  // DocumentType configuration (unchanged)
                  modelBuilder.Entity<DocumentType>(entity =>
                  {
                        entity.ToTable("document_type");
                        entity.HasKey(dt => dt.DocTypeId);
                        entity.Property(dt => dt.DocTypeId).HasColumnName("doc_type_id");
                        entity.Property(dt => dt.TypeName).HasColumnName("type_name");
                        entity.Property(dt => dt.Description).HasColumnName("description");
                        entity.Property(dt => dt.CreatedBy).HasColumnName("created_by");
                        entity.Property(dt => dt.CreateDate).HasColumnName("create_date");
                        entity.Property(dt => dt.UpdatedBy).HasColumnName("updated_by");
                        entity.Property(dt => dt.UpdateDate).HasColumnName("update_date");
                  });
            }
      }
}