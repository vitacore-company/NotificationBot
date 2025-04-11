using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NotificationsBot.Migrations
{
    /// <inheritdoc />
    public partial class initistprojects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NotificationTypesProjects",
                columns: table => new
                {
                    NotificationTypesId = table.Column<int>(type: "integer", nullable: false),
                    ProjectsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationTypesProjects", x => new { x.NotificationTypesId, x.ProjectsId });
                    table.ForeignKey(
                        name: "FK_NotificationTypesProjects_NotificationTypes_NotificationTyp~",
                        column: x => x.NotificationTypesId,
                        principalTable: "NotificationTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NotificationTypesProjects_Projects_ProjectsId",
                        column: x => x.ProjectsId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NotificationTypesProjects_ProjectsId",
                table: "NotificationTypesProjects",
                column: "ProjectsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotificationTypesProjects");
        }
    }
}
