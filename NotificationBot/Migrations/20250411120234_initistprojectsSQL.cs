using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NotificationsBot.Migrations
{
    /// <inheritdoc />
    public partial class initistprojectsSQL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"insert into public.""NotificationTypesProjects"" (""ProjectsId"", ""NotificationTypesId"")
SELECT pr.""Id"", nt.""Id"" FROM public.""Projects"" as pr CROSS JOIN public.""NotificationTypes"" as nt;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
