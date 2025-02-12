using Microsoft.EntityFrameworkCore;
using NotificationsBot.Models;
using System.Diagnostics.CodeAnalysis;

namespace NotificationsBot.Interfaces.Impl;

public class UsersDataService : IUsersDataService
{
    readonly AppContext _context;
    public UsersDataService(AppContext appContext)
    {
        _context = appContext;
    }

    public Task CancelStatus(long chatId)
    {
        return ChangeStatus(chatId, null);
    }

    public Task ChangeStatus(long chatId, string? status)
    {
        User user = _context.Users.Find(chatId)
            ?? throw new Exception("Не найден пользователь");
        user.State = status;
        _context.SaveChanges();
        return Task.CompletedTask;
    }

    public async Task<string?> GetStatus(long chatId)
    {
        User user = await _context.Users.FindAsync(chatId)
            ?? throw new Exception("Не найден пользователь");
        return user.State;
    }

    public async Task<bool> IsContainUser(long chatId)
    {
        User? user = await _context.Users.FindAsync(chatId);
        if(user != null)
            return true;
        return false;
    }

    public Task SaveNewUser(string? login, long chatId)
    {
        _context.Users.Add(new Models.User() { ChatId = chatId, Login = login });
        _context.SaveChanges();
        return Task.CompletedTask;
    }

    public Task UpdateUser(string newLogin, long chatId)
    {
        User user = _context.Users.Find(chatId)
            ?? throw new Exception("Не найден пользователь");
        user.Login = newLogin;
        _context.Users.Update(user);
        _context.SaveChanges();
        return Task.CompletedTask;
    }
}

public class UserComparerChatId : IEqualityComparer<User>
{
    public bool Equals(User? x, User? y)
    {
        return x?.ChatId == y?.ChatId;
    }

    public int GetHashCode([DisallowNull] User obj)
    {
        return obj.ChatId.GetHashCode();
    }
}
