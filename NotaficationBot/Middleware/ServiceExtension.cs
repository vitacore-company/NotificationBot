using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NotificationsBot.Handlers;
using NotificationsBot.Interfaces;
using NotificationsBot.Interfaces.Impl;
using Telegram.Bot;
using Telegram.Bot.Polling;

namespace NotificationsBot.Middleware;

public static class ServiceExtension
{
    public static IServiceCollection AddImplInterfaces(this IServiceCollection services, IConfigurationManager configurationManager)
    {

        string connectionString = configurationManager.GetConnectionString("DefaultConnection");
        services.Configure<NotificationsBot.Models.TelegramBotClientOptions>(configurationManager.GetSection("BotClientOptions"));
        services.AddDbContext<AppContext>(oprions =>
        {
            oprions.UseNpgsql(connectionString);
        });
        services.AddScoped<IUsersDataService, UsersDataService>();
        services.AddScoped<INotificationService, TelegramNotificationService>();
        services.AddScoped<ITelegramCommandHandler, TelegramCommandHandler>();
        services.AddScoped<IUpdateHandler, TelegramCommandHandler>();
        services.AddScoped<ReceiverService>();
        services.AddHostedService<PollingService>();


        services.AddHttpClient("telegram_bot_client")
            .AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
            {
                var options = sp.GetRequiredService<IOptions<NotificationsBot.Models.TelegramBotClientOptions>>().Value;

                if (options.Token is null)
                {
                    throw new InvalidOperationException("Cannot instantiate a bot client without a configured bot token.");
                }


                var ctorOptions = new Telegram.Bot.TelegramBotClientOptions(options.Token, options?.BaseUrl, options?.UseTestEnvironment ?? false)
                {
                    RetryCount = options.RetryCount,
                    RetryThreshold = options.RetryThreshold
                };
                return new TelegramBotClient(ctorOptions, httpClient);
            });

        return services;
    }

    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        return services;
    }

    public static WebApplicationBuilder AddAllServicesInApplicationBuilder(this WebApplicationBuilder hostApplicationBuilder)
    {
        hostApplicationBuilder.Services.AddImplInterfaces(hostApplicationBuilder.Configuration);

        hostApplicationBuilder.Services.AddControllers()
            .AddXmlSerializerFormatters();

        hostApplicationBuilder.Services.AddSwagger();

        return hostApplicationBuilder;
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
}
