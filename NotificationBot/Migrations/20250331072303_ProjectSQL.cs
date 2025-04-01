using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace NotificationsBot.Migrations
{
    /// <inheritdoc />
    public partial class ProjectSQL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"insert into public.""Projects"" (""Name"") values" +
                "('ISZL(agile)')," +
                "('IES(agile)')," +
                "('EDO.TFOMS')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
