using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegistryAPI.Migrations
{
    public partial class IncreaseScanFileDataToLongblob_Custom : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Use raw SQL to alter column to LONGBLOB. Adjust table/schema names if necessary.
            migrationBuilder.Sql("ALTER TABLE `documents` MODIFY COLUMN `scan_file_data` LONGBLOB NULL;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert to MEDIUMBLOB by default. If your previous type was BLOB adjust accordingly.
            migrationBuilder.Sql("ALTER TABLE `documents` MODIFY COLUMN `scan_file_data` MEDIUMBLOB NULL;");
        }
    }
}
