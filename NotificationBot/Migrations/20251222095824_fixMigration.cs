using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NotificationsBot.Migrations
{
    /// <inheritdoc />
    public partial class fixMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(" DO $$\r\nDECLARE\r\n    db_name text;\r\nBEGIN\r\n    SELECT current_database() INTO db_name;\r\n    EXECUTE format('ALTER DATABASE %I REFRESH COLLATION version', db_name);\r\nEND $$;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
