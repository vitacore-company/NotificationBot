
namespace NotificationsBot.Interfaces.Impl;

public class UsersDataService : IUsersDataService
{
    readonly AppContext _context;
    public UsersDataService(AppContext appContext)
    {
        _context = appContext;
    }
    public Task SaveNewUser(string login, long chatId)
    {
        _context.Users.Add(new Models.User() { ChatId = chatId, Login = login });
        _context.SaveChanges();
        return Task.CompletedTask;
    }

    public Task UpdateUser(string newLogin, long chatId)
    {
        throw new NotImplementedException();
    }
}
