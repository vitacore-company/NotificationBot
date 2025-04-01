using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NotificationsBot.Migrations
{
    /// <inheritdoc />
    public partial class RenameUserIdToChatId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NotificationsOnProjectChat_Users_UserId",
                table: "NotificationsOnProjectChat");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "NotificationsOnProjectChat",
                newName: "ChatId");

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationsOnProjectChat_Users_ChatId",
                table: "NotificationsOnProjectChat",
                column: "ChatId",
                principalTable: "Users",
                principalColumn: "ChatId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NotificationsOnProjectChat_Users_ChatId",
                table: "NotificationsOnProjectChat");

            migrationBuilder.RenameColumn(
                name: "ChatId",
                table: "NotificationsOnProjectChat",
                newName: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationsOnProjectChat_Users_UserId",
                table: "NotificationsOnProjectChat",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "ChatId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
