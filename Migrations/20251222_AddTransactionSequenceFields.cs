using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegistryAPI.Migrations
{
    public partial class AddTransactionSequenceFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER TABLE `transactions` 
ADD COLUMN IF NOT EXISTS `sequence_number` INT NULL,
ADD COLUMN IF NOT EXISTS `sequence_id` VARCHAR(255) NULL,
ADD COLUMN IF NOT EXISTS `display_number` VARCHAR(100) NULL;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER TABLE `transactions` 
DROP COLUMN IF EXISTS `display_number`,
DROP COLUMN IF EXISTS `sequence_id`,
DROP COLUMN IF EXISTS `sequence_number`;");
        }
    }
}
