using NotificationsBot.Interfaces;

namespace NotificationsBot.Services.Background
{
    public class BackgroundUserService : BackgroundService
    {
        private readonly IServiceProvider serviceProvider;
        public BackgroundUserService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (IServiceScope scope = serviceProvider.CreateScope())
                {
                    IUserHolder userHolder = scope.ServiceProvider.GetRequiredService<IUserHolder>();

                    userHolder.Clear();
                }

                // каждые 3 часа очищается пул юзеров
                await Task.Delay(TimeSpan.FromHours(10), stoppingToken);
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }
    }
}
