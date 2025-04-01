using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace NotificationsBot.Migrations
{
    /// <inheritdoc />
    public partial class NotificationTypesSQL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"insert into public.""NotificationTypes"" (""EventType"", ""EventDescription"") values" +
                "('git.pullrequest.updated', 'Обновление пуллреквеста')," +
                "('ms.vss-code.git-pullrequest-comment-event', 'Комментирование пуллреквеста')," +
                "('git.pullrequest.created', 'Создание пуллреквеста')," +
                "('workitem.created', 'Создание рабочего элемента')," +
                "('workitem.updated', 'Обновление рабочего элемента')," +
                "('build.complete', 'Смена состояния сборки')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
