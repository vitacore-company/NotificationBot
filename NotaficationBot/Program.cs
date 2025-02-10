using NotificationsBot.Middleware;
using NotificationsBot.Models;

namespace NotificationsBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Uri uri = new Uri("https://tfs.dev.vitacore.ru/tfs");
            //VssConnection connection = new VssConnection(uri, new VssBasicCredential(string.Empty, "AzureDevopsToken"));
            WebApplicationBuilder botBuilder = WebApplication.CreateBuilder(args);

            botBuilder.ConfigureApplicationBuilder();
            

            WebApplication bothost = botBuilder.Build();

            bothost.ConfigureBotApplicationBuilder();

            bothost.Run();
        }
    }
}
