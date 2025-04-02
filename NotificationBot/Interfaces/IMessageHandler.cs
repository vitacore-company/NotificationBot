namespace NotificationsBot.Interfaces
{
    public interface IMessageHandler<T>
    {
        public Task Handle(T resource);
    }
}
