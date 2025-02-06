using MinimalTelegramBot.Builder;
using NotificationsBot.Middleware;

namespace NotificationsBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Uri uri = new Uri("https://tfs.dev.vitacore.ru/tfs");
            //VssConnection connection = new VssConnection(uri, new VssBasicCredential(string.Empty, "AzureDevopsToken"));
            BotApplicationBuilder botBuilder = BotApplication.CreateBuilder(args);

            botBuilder.ConfigureApplicationBuilder();

            BotApplication bothost = botBuilder.Build();

            bothost.ConfigureBotApplicationBuilder();

            bothost.Run();
        }
    }
}
