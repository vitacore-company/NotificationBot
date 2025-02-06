using Microsoft.EntityFrameworkCore;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using MinimalTelegramBot;
using MinimalTelegramBot.Builder;
using MinimalTelegramBot.Handling;
using MinimalTelegramBot.StateMachine.Extensions;
using NotificationsBot.Interfaces;
using NotificationsBot.Interfaces.Impl;
using NotificationsBot.States;
using Telegram.Bot.Types.Enums;

namespace NotificationsBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Uri uri = new Uri("https://tfs.dev.vitacore.ru/tfs");
            //VssConnection connection = new VssConnection(uri, new VssBasicCredential(string.Empty, "AzureDevopsToken"));

            var botBuilder = BotApplication.CreateBuilder(args);
            string connectionString = botBuilder.Configuration.GetConnectionString("DefaultConnection");
            botBuilder.Services.AddDbContext<AppContext>(oprions =>
            {
                oprions.UseSqlite(connectionString);
            });
            botBuilder.Services.AddScoped<IUsersDataService, UsersDataService>();
            botBuilder.Services.AddScoped<INotificationService, TelegramNotificationService>();
            botBuilder.Services.AddStateMachine();
            botBuilder.Services.AddControllers()
                .AddXmlSerializerFormatters();
            botBuilder.Services.AddEndpointsApiExplorer();
            botBuilder.Services.AddSwaggerGen();
            BotApplication bothost = botBuilder.Build();
            bothost.UseStateMachine();
            bothost.HandleCommand("/start", () => "Hello, World!");
            bothost.HandleCommand("/register", async (BotRequestContext context) =>
            {
                var state = new RegisterState.AskLoginState();
                await context.SetState(state);
                return "Enter your login";
            });
            bothost.HandleUpdateType(UpdateType.Message, async (string messageText, BotRequestContext context, IUsersDataService usersDataService) =>
            {
                try
                {
                    var result = usersDataService?.SaveNewUser(messageText, context.ChatId);
                    if (result == null)
                    {
                        throw new NullReferenceException("Регистрация клиента недоступна");
                    }
                }
                catch (NullReferenceException ex)
                {
                    return ex.Message;
                }
                catch(Exception ex)
                {
                    return "Наелся и спит -_-";
                }
                await context.DropState();
                return "Все заебок";
            }).FilterState<RegisterState.AskLoginState>();

            if (bothost.WebApplicationAccessor.Environment.IsDevelopment())
            {
                bothost.WebApplicationAccessor.UseSwagger();
                bothost.WebApplicationAccessor.UseSwaggerUI();
            }

            bothost.WebApplicationAccessor.UseHttpsRedirection();

            bothost.WebApplicationAccessor.MapControllers();


            bothost.Run();
        }
    }
}
