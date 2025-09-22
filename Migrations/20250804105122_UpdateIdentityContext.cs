using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace RegistryAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateIdentityContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false),
                    Name = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false),
                    UserName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PasswordHash = table.Column<string>(type: "longtext", nullable: true),
                    SecurityStamp = table.Column<string>(type: "longtext", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "longtext", nullable: true),
                    PhoneNumber = table.Column<string>(type: "longtext", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "department",
                columns: table => new
                {
                    dept_id = table.Column<string>(type: "varchar(255)", nullable: false),
                    dept_code = table.Column<string>(type: "longtext", nullable: false),
                    dept_name = table.Column<string>(type: "longtext", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true),
                    create_date = table.Column<DateTime>(type: "datetime(6)", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_by = table.Column<string>(type: "longtext", nullable: true),
                    update_date = table.Column<DateTime>(type: "datetime(6)", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_department", x => x.dept_id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "document_type",
                columns: table => new
                {
                    doc_type_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    type_name = table.Column<string>(type: "longtext", nullable: true),
                    description = table.Column<string>(type: "longtext", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true),
                    create_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    updated_by = table.Column<string>(type: "longtext", nullable: true),
                    update_date = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_document_type", x => x.doc_type_id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "sequence",
                columns: table => new
                {
                    sequence_id = table.Column<string>(type: "varchar(255)", nullable: false),
                    dept_code = table.Column<string>(type: "longtext", nullable: true),
                    year = table.Column<int>(type: "int", nullable: true),
                    sequence_number = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true),
                    create_date = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sequence", x => x.sequence_id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<string>(type: "varchar(255)", nullable: false),
                    ClaimType = table.Column<string>(type: "longtext", nullable: true),
                    ClaimValue = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false),
                    ClaimType = table.Column<string>(type: "longtext", nullable: true),
                    ClaimValue = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "varchar(255)", nullable: false),
                    ProviderKey = table.Column<string>(type: "varchar(255)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "longtext", nullable: true),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false),
                    RoleId = table.Column<string>(type: "varchar(255)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false),
                    LoginProvider = table.Column<string>(type: "varchar(255)", nullable: false),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false),
                    Value = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "staff",
                columns: table => new
                {
                    staff_id = table.Column<string>(type: "varchar(255)", nullable: false),
                    first_name = table.Column<string>(type: "longtext", nullable: true),
                    last_name = table.Column<string>(type: "longtext", nullable: true),
                    email = table.Column<string>(type: "longtext", nullable: true),
                    dept_code = table.Column<string>(type: "varchar(255)", nullable: true),
                    employment_type = table.Column<string>(type: "longtext", nullable: true),
                    status = table.Column<string>(type: "longtext", nullable: true),
                    hire_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    termination_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    title = table.Column<string>(type: "longtext", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true),
                    create_date = table.Column<DateTime>(type: "datetime(6)", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_by = table.Column<string>(type: "longtext", nullable: true),
                    update_date = table.Column<DateTime>(type: "datetime(6)", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP"),
                    password = table.Column<string>(type: "longtext", nullable: false),
                    role = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.staff_id);
                    table.ForeignKey(
                        name: "FK_staff_department_dept_code",
                        column: x => x.dept_code,
                        principalTable: "department",
                        principalColumn: "dept_id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "files",
                columns: table => new
                {
                    file_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    staff_id = table.Column<string>(type: "varchar(255)", nullable: true),
                    file_status = table.Column<string>(type: "longtext", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true),
                    create_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    updated_by = table.Column<string>(type: "longtext", nullable: true),
                    update_date = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_files", x => x.file_id);
                    table.ForeignKey(
                        name: "FK_files_staff_staff_id",
                        column: x => x.staff_id,
                        principalTable: "staff",
                        principalColumn: "staff_id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "termination",
                columns: table => new
                {
                    termination_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    staff_id = table.Column<string>(type: "varchar(255)", nullable: true),
                    reason = table.Column<string>(type: "longtext", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true),
                    create_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    updated_by = table.Column<string>(type: "longtext", nullable: true),
                    update_date = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_termination", x => x.termination_id);
                    table.ForeignKey(
                        name: "FK_termination_staff_staff_id",
                        column: x => x.staff_id,
                        principalTable: "staff",
                        principalColumn: "staff_id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "documents",
                columns: table => new
                {
                    document_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    file_id = table.Column<int>(type: "int", nullable: true),
                    doc_type_id = table.Column<int>(type: "int", nullable: true),
                    document_name = table.Column<string>(type: "longtext", nullable: true),
                    document_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    description = table.Column<string>(type: "longtext", nullable: true),
                    scanned = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    scan_file_data = table.Column<byte[]>(type: "longblob", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true),
                    create_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    updated_by = table.Column<string>(type: "longtext", nullable: true),
                    update_date = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_documents", x => x.document_id);
                    table.ForeignKey(
                        name: "FK_documents_document_type_doc_type_id",
                        column: x => x.doc_type_id,
                        principalTable: "document_type",
                        principalColumn: "doc_type_id");
                    table.ForeignKey(
                        name: "FK_documents_files_file_id",
                        column: x => x.file_id,
                        principalTable: "files",
                        principalColumn: "file_id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "transactions",
                columns: table => new
                {
                    transaction_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    staff_id = table.Column<string>(type: "varchar(255)", nullable: false),
                    file_id = table.Column<int>(type: "int", nullable: true),
                    transaction_type = table.Column<string>(type: "longtext", nullable: true),
                    transaction_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    status = table.Column<string>(type: "longtext", nullable: true),
                    details = table.Column<string>(type: "longtext", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: false),
                    created_date = table.Column<DateTime>(type: "datetime(6)", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_by = table.Column<string>(type: "longtext", nullable: true),
                    updated_date = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("transaction_id", x => x.transaction_id);
                    table.ForeignKey(
                        name: "FK_transactions_files_file_id",
                        column: x => x.file_id,
                        principalTable: "files",
                        principalColumn: "file_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_transactions_staff_staff_id",
                        column: x => x.staff_id,
                        principalTable: "staff",
                        principalColumn: "staff_id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_documents_doc_type_id",
                table: "documents",
                column: "doc_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_documents_file_id",
                table: "documents",
                column: "file_id");

            migrationBuilder.CreateIndex(
                name: "IX_files_staff_id",
                table: "files",
                column: "staff_id");

            migrationBuilder.CreateIndex(
                name: "IX_staff_dept_code",
                table: "staff",
                column: "dept_code");

            migrationBuilder.CreateIndex(
                name: "IX_termination_staff_id",
                table: "termination",
                column: "staff_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_transactions_file_id",
                table: "transactions",
                column: "file_id");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_staff_id",
                table: "transactions",
                column: "staff_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "documents");

            migrationBuilder.DropTable(
                name: "sequence");

            migrationBuilder.DropTable(
                name: "termination");

            migrationBuilder.DropTable(
                name: "transactions");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "document_type");

            migrationBuilder.DropTable(
                name: "files");

            migrationBuilder.DropTable(
                name: "staff");

            migrationBuilder.DropTable(
                name: "department");
        }
    }
}
