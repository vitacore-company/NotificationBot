using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NotificationsBot.Migrations
{
    /// <inheritdoc />
    public partial class AddTopicsToProject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProjectId",
                table: "Topics",
                newName: "ProjectsId");

            migrationBuilder.CreateIndex(
                name: "IX_Topics_ProjectsId",
                table: "Topics",
                column: "ProjectsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Topics_Projects_ProjectsId",
                table: "Topics",
                column: "ProjectsId",
                principalTable: "Projects",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Topics_Projects_ProjectsId",
                table: "Topics");

            migrationBuilder.DropIndex(
                name: "IX_Topics_ProjectsId",
                table: "Topics");

            migrationBuilder.RenameColumn(
                name: "ProjectsId",
                table: "Topics",
                newName: "ProjectId");
        }
    }
}
