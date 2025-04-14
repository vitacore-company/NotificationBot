using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NotificationsBot.Migrations
{
    /// <inheritdoc />
    public partial class RegionsSQL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"insert into public.""Projects"" (""Name"") values ('AKUZ2');

insert into public.""NotificationTypes"" (""EventType"", ""EventDescription"") values
('\\Regions\\NAO', 'NAO'),
('\\Regions\\Sevastopol', 'Sevastopol'),
('\\Regions\\Tatarstan', 'Tatarstan'),
('\\Regions\\Ulyanovsk', 'Ulyanovsk'),
('\\', 'Others');

insert into public.""NotificationTypesProjects"" (""ProjectsId"", ""NotificationTypesId"")
values
((select p.""Id"" from public.""Projects"" as p where p.""Name"" = 'AKUZ2'), (select nt.""Id"" from public.""NotificationTypes"" nt where nt.""EventType"" = '\\Regions\\NAO')),
((select p.""Id"" from public.""Projects"" as p where p.""Name"" = 'AKUZ2'), (select nt.""Id"" from public.""NotificationTypes"" nt where nt.""EventType"" = '\\Regions\\Sevastopol')),
((select p.""Id"" from public.""Projects"" as p where p.""Name"" = 'AKUZ2'), (select nt.""Id"" from public.""NotificationTypes"" nt where nt.""EventType"" = '\\Regions\\Tatarstan')),
((select p.""Id"" from public.""Projects"" as p where p.""Name"" = 'AKUZ2'), (select nt.""Id"" from public.""NotificationTypes"" nt where nt.""EventType"" = '\\Regions\\Ulyanovsk')),
((select p.""Id"" from public.""Projects"" as p where p.""Name"" = 'AKUZ2'), (select nt.""Id"" from public.""NotificationTypes"" nt where nt.""EventType"" = '\\'));");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
