using Microsoft.EntityFrameworkCore;
using NotificationsBot.Models;
using System.Diagnostics.CodeAnalysis;

namespace NotificationsBot.Interfaces.Impl;
/// <summary>
/// <inheritdoc cref="IUsersDataService"/>
/// </summary>
/// <seealso cref="NotificationsBot.Interfaces.IUsersDataService" />
public class UsersDataService : IUsersDataService
{
    readonly AppContext _context;
    public UsersDataService(AppContext appContext)
    {
        _context = appContext;
    }

    /// <summary>
    /// Отменяет статус.
    /// </summary>
    /// <param name="chatId">Идентификатор чата.</param>
    /// <returns></returns>
    public Task CancelStatus(long chatId)
    {
        return ChangeStatus(chatId, null);
    }

    /// <summary>
    /// Изменяет статус.
    /// </summary>
    /// <param name="chatId">Идентификатор чата.</param>
    /// <param name="status">Статус.</param>
    /// <returns></returns>
    /// <exception cref="System.Exception">Не найден пользователь</exception>
    public Task ChangeStatus(long chatId, string? status)
    {
        User user = _context.Users.Find(chatId)
            ?? throw new Exception("Не найден пользователь");
        user.State = status;
        _context.SaveChanges();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Получает статус.
    /// </summary>
    /// <param name="chatId">Идентификатор чата.</param>
    /// <returns></returns>
    /// <exception cref="System.Exception">Не найден пользователь</exception>
    public async Task<string?> GetStatus(long chatId)
    {
        User user = await _context.Users.FindAsync(chatId)
            ?? throw new Exception("Не найден пользователь");
        return user.State;
    }

    /// <summary>
    /// Определяет, есть ли [пользователь] с [указанный идентификатор чата].
    /// </summary>
    /// <param name="chatId">Идентификатор чата.</param>
    /// <returns></returns>
    public async Task<bool> IsContainUser(long chatId)
    {
        User? user = await _context.Users.FindAsync(chatId);
        if(user != null)
            return true;
        return false;
    }

    /// <summary>
    /// Сохраняет нового пользователя.
    /// </summary>
    /// <param name="login">Логин.</param>
    /// <param name="chatId">Идентификатор чата.</param>
    /// <returns></returns>
    public Task SaveNewUser(string? login, long chatId)
    {
        _context.Users.Add(new Models.User() { ChatId = chatId, Login = login });
        _context.SaveChanges();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Обновляет пользователя.
    /// </summary>
    /// <param name="newLogin">Новый логин.</param>
    /// <param name="chatId">Идентификатор чата.</param>
    /// <returns></returns>
    /// <exception cref="System.Exception">Не найден пользователь</exception>
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
