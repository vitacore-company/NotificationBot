using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NotificationsBot.Handlers;
using NotificationsBot.Interfaces;
using NotificationsBot.Interfaces.TelegramCallback;
using NotificationsBot.Services;
using NotificationsBot.Services.Background.Polling;
using System.Reflection;
using Telegram.Bot;
using Telegram.Bot.Polling;

namespace NotificationsBot.Middleware;

public static class ServiceExtension
{
    public static IServiceCollection AddImplInterfaces(this IServiceCollection services, IConfigurationManager configurationManager)
    {
        configurationManager.AddEnvironmentVariables();
        string connectionString = configurationManager.GetConnectionString("DefaultConnection") ?? string.Empty;
        if (string.IsNullOrEmpty(connectionString))
        {
            connectionString = $@"Host=postgresql;Database={configurationManager["POSTGRES_DB"]};Port={configurationManager["POSTGRES_EXPOSE_PORTS"]};Username={configurationManager["POSTGRES_USER"]};Password={configurationManager["POSTGRES_PASSWORD"]}";
        }
        services.Configure<NotificationsBot.Models.TelegramBotClientOptions>(configurationManager.GetSection("BotClientOptions"));
        services.AddDbContext<AppContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });
        services.AddHealthChecks();
        services.AddScoped<IUsersDataService, UsersDataService>();
        services.AddScoped<INotificationService, TelegramNotificationService>();
        services.AddScoped<ITelegramCommandHandler, TelegramCommandHandler>();
        services.AddScoped<IUpdateHandler, TelegramCommandHandler>();
        services.AddScoped<ReceiverService>();
        services.AddScoped<IExistUserChecker, ExistUserChecker>();
        services.AddScoped<IUserHolder, UserHolderService>();
        services.AddScoped<INotificationTypesService, NotificationTypesService>();
        services.AddHostedService<PollingService>();
        services.RegisterHandler();
        services.AddMessageHandlers(Assembly.GetExecutingAssembly());
        services.AddScoped<IHandlerFactory, HandlerFactory>();
        services.AddTransient<ExceptionMiddleware>();
        services.AddMemoryCache();
        services.AddScoped<ICommandService, CommandService>();
        services.AddScoped<ICallbackQueryService, CallbackQueryService>();
        services.AddScoped<INotificationCacheService, NotificationCacheService>();
        services.AddScoped<ICacheService, CacheService>();

        services.AddHttpClient("telegram_bot_client")
            .AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
            {
                Models.TelegramBotClientOptions options = sp.GetRequiredService<IOptions<NotificationsBot.Models.TelegramBotClientOptions>>().Value ??
                new Models.TelegramBotClientOptions();

                if (string.IsNullOrEmpty(options.Token))
                {
                    options.Token = configurationManager.GetValue<string>("BotToken");
                }

                TelegramBotClientOptions ctorOptions = new Telegram.Bot.TelegramBotClientOptions(options.Token ?? "", options?.BaseUrl, options?.UseTestEnvironment ?? false)
                {
                    RetryCount = options!.RetryCount,
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
#if DEBUG
        foreach (KeyValuePair<string, string?> c in hostApplicationBuilder.Configuration.AsEnumerable())
        {
            Console.WriteLine(c.Key + " = " + c.Value);
        }
#endif
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

        webApplication.UseHealthChecks("/health");

        webApplication.MapControllers();

        using (IServiceScope scope = webApplication.Services.CreateScope())
        {
            IServiceProvider services = scope.ServiceProvider;

            try
            {
                AppContext dbContext = services.GetRequiredService<AppContext>();
                dbContext.Database.Migrate();
            }
            catch (Exception ex)
            {
                ILogger<Program> logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while migrating or seeding the database.");
                throw;
            }
        }

        webApplication.UseMiddleware<ExceptionMiddleware>();
        webApplication.UseMiddleware<DomainWhitelistMiddleware>();

        return webApplication;
    }
}
