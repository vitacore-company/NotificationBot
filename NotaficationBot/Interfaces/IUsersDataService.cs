namespace NotificationsBot.Interfaces;

public interface IUsersDataService
{
    public Task SaveNewUser(string login, long chatId);
    public Task UpdateUser(string newLogin, long chatId);
    public Task<bool> IsContainUser(long chatId);
    public Task ChangeStatus(long chatId, string status);
    public Task CancelStatus(long chatId);
    public Task<string> GetStatus(long chatId);
}
