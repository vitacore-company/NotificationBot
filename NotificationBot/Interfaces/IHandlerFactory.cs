namespace NotificationsBot.Interfaces
{
    public interface IHandlerFactory
    {
        Task ProcessHandler(Type handlerType, string json);
    }
}
