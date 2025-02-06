using NotificationsBot.Interfaces.Impl;
using NotificationsBot.Interfaces;
using Microsoft.EntityFrameworkCore;
using MinimalTelegramBot.StateMachine.Extensions;
using MinimalTelegramBot.Builder;
using MinimalTelegramBot;
using NotificationsBot.States;
using Telegram.Bot.Types.Enums;
using MinimalTelegramBot.Handling;

namespace NotificationsBot.Middleware;

public static class ServiceExtension
{
    public static IServiceCollection AddImplInterfaces(this IServiceCollection services, IConfigurationManager configurationManager)
    {

        string connectionString = configurationManager.GetConnectionString("DefaultConnection");
        services.AddDbContext<AppContext>(oprions =>
        {
            oprions.UseSqlite(connectionString);
        });
        services.AddScoped<IUsersDataService, UsersDataService>();
        services.AddScoped<INotificationService, TelegramNotificationService>();

        services.AddStateMachine();

        return services;
    }

    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        return services;
    }

    public static IHostApplicationBuilder ConfigureApplicationBuilder(this IHostApplicationBuilder hostApplicationBuilder)
    {
        hostApplicationBuilder.Services.AddImplInterfaces(hostApplicationBuilder.Configuration);

        hostApplicationBuilder.Services.AddControllers()
            .AddXmlSerializerFormatters();

        hostApplicationBuilder.Services.AddSwagger();

        return hostApplicationBuilder;
    }

    public static BotApplication ConfigureBotApplication(this BotApplication botApplication)
    {
        botApplication.UseStateMachine();
        botApplication.HandleCommand("/start", () => "Hello, World!");
        botApplication.HandleCommand("/register", async (BotRequestContext context) =>
        {
            var state = new RegisterState.AskLoginState();
            await context.SetState(state);
            return "Enter your login";
        });
        botApplication.HandleUpdateType(UpdateType.Message, async (string messageText, BotRequestContext context, IUsersDataService usersDataService) =>
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
            catch (Exception ex)
            {
                return "Наелся и спит -_-";
            }
            await context.DropState();
            return "Все заебок";
        }).FilterState<RegisterState.AskLoginState>();

        return botApplication;
    } 

    public static WebApplication ConfigureWebApplication(this WebApplication webApplication)
    {
        if (webApplication.Environment.IsDevelopment())
        {
            webApplication.UseSwagger();
            webApplication.UseSwaggerUI();
        }

        webApplication.UseHttpsRedirection();

        webApplication.MapControllers();


        return webApplication;
    }

    public static IBotApplicationBuilder ConfigureBotApplicationBuilder(this BotApplication botApplicationBuilder)
    {
        botApplicationBuilder.ConfigureBotApplication();
        botApplicationBuilder.WebApplicationAccessor.ConfigureWebApplication();
        return botApplicationBuilder;
    }

}
