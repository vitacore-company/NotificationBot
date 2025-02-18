namespace NotificationsBot.Interfaces.Impl;

/// <summary>
/// Фоновый сервис опроса сервера телеграмм на наличие  <seealso cref="Telegram.Bot.Types.Update">обновлений</seealso>
/// </summary>
/// <typeparam name="TReceiverService">The type of the receiver service.</typeparam>
/// <seealso cref="Microsoft.Extensions.Hosting.BackgroundService" />
public abstract class PollingServiceBase<TReceiverService>(IServiceProvider serviceProvider, ILogger<PollingServiceBase<TReceiverService>> logger)
    : BackgroundService where TReceiverService : IReceiverService
{
    /// <summary>
    /// Этот метод вызывается, когда запускается <see cref="T:Microsoft.Extensions.Hosting.IHostedService" />. Реализация должна возвращать задачу, которая представляет собой
    /// время жизни выполняемой длительной операции (операций).
    /// </summary>
    /// <param name="stoppingToken">Срабатывает, когда <see cref="M:Microsoft.Extensions.Hosting.IHostedService.StopAsync(System.Threading.CancellationToken)" /> вызывается.</param>
    /// <remarks>
    /// См. <see href="https://docs.microsoft.com/dotnet/core/extensions/workers">Worker Services in .NET</see> для руководства по реализации.
    /// </remarks>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Starting polling service");
        await DoWork(stoppingToken);
    }

    /// <summary>
    /// Запуск опроса
    /// </summary>
    /// <param name="stoppingToken">The stopping token.</param>
    private async Task DoWork(CancellationToken stoppingToken)
    {
        // Make sure we receive updates until Cancellation Requested
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Create new IServiceScope on each iteration. This way we can leverage benefits
                // of Scoped TReceiverService and typed HttpClient - we'll grab "fresh" instance each time
                using var scope = serviceProvider.CreateScope();
                var receiver = scope.ServiceProvider.GetRequiredService<TReceiverService>();

                await receiver.ReceiveAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError("Polling failed with exception: {Exception}", ex);
                // Cooldown if something goes wrong
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}
