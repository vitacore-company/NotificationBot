using NotificationsBot.Middleware;

namespace NotificationsBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Uri uri = new Uri("https://tfs.dev.vitacore.ru/tfs");
            //VssConnection connection = new VssConnection(uri, new VssBasicCredential(string.Empty, "AzureDevopsToken"));

            WebApplication.CreateBuilder(args)
                .AddAllServicesInApplicationBuilder()
                .Build()
                .ConfigureWebApplication()
                .Run();
        }
    }
}
