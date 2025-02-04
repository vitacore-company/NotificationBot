namespace NotificationsBot.Interfaces;

public interface IUsersDataService
{
    public Task SaveNewUser(string login, long chatId);
    public Task UpdateUser(string newLogin, long chatId);
}
