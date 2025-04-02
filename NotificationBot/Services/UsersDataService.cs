using NotificationsBot.Interfaces;
using NotificationsBot.Models.Database;
using System.Diagnostics.CodeAnalysis;

namespace NotificationsBot.Services;
/// <summary>
/// <inheritdoc cref="IUsersDataService"/>
/// </summary>
/// <seealso cref="IUsersDataService" />
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
    /// <exception cref="Exception">Не найден пользователь</exception>
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
    /// <exception cref="Exception">Не найден пользователь</exception>
    public async Task<string> GetStatus(long chatId)
    {
        User user = await _context.Users.FindAsync(chatId)
            ?? throw new Exception("Не найден пользователь");
        return user.State ?? string.Empty;
    }

    /// <summary>
    /// Определяет, есть ли [пользователь] с [указанный идентификатор чата].
    /// </summary>
    /// <param name="chatId">Идентификатор чата.</param>
    /// <returns></returns>
    public async Task<bool> IsContainUser(long chatId)
    {
        User? user = await _context.Users.FindAsync(chatId);
        if (user != null)
            return true;
        return false;
    }

    /// <summary>
    /// Сохраняет нового пользователя.
    /// </summary>
    /// <param name="login">Логин.</param>
    /// <param name="chatId">Идентификатор чата.</param>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentException">
    /// Параметр {nameof(login)} равен null или пуст - login
    /// or
    /// Параметр {nameof(chatId)} равный -1 не может быть сохранен - chatId
    /// or
    /// Параметр {nameof(userId)} равный -1 не может быть сохранен - userId
    /// </exception>
    public Task SaveNewUser(string? login, long chatId, long userId)
    {
        if(string.IsNullOrEmpty(login) )
        {
            throw new ArgumentException($"Параметр {nameof(login)} равен null или пуст", nameof(login));
        }
        if (chatId == -1 )
        {
            throw new ArgumentException($"Параметр {nameof(chatId)} равный -1 не может быть сохранен", nameof(chatId));
        }
        if (userId == -1)
        {
            throw new ArgumentException($"Параметр {nameof(userId)} равный -1 не может быть сохранен", nameof(userId));
        }
        _context.Users.Add(new User() { ChatId = chatId, Login = login, UserId = userId });
        _context.SaveChanges();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Обновляет пользователя.
    /// </summary>
    /// <param name="newLogin">Новый логин.</param>
    /// <param name="chatId">Идентификатор чата.</param>
    /// <returns></returns>
    /// <exception cref="Exception">Не найден пользователь</exception>
    public Task UpdateUser(string newLogin, long chatId)
    {
        return UpdateUser(newLogin, chatId, null);
    }

    /// <summary>
    /// Обновляет пользователя.
    /// </summary>
    /// <param name="newLogin">Новый логин.</param>
    /// <param name="chatId">Идентификатор чата.</param>
    /// <param name="userId">Идентификатор Телеграм аккаунта.</param>
    /// <returns></returns>
    /// <exception cref="Exception">Не найден пользователь</exception>
    /// <exception cref="System.ArgumentException">
    /// Параметр {nameof(login)} равен null или пуст - login
    /// or
    /// Параметр {nameof(chatId)} равный -1 не может быть сохранен - chatId
    /// or
    /// Параметр {nameof(userId)} равный -1 не может быть сохранен - userId
    /// </exception>
    public Task UpdateUser(string? newLogin, long chatId, long? userId)
    {
        if (string.IsNullOrEmpty(newLogin))
        {
            throw new ArgumentException($"Параметр {nameof(newLogin)} равен null или пуст", nameof(newLogin));
        }
        if (chatId == -1)
        {
            throw new ArgumentException($"Параметр {nameof(chatId)} равный -1 не может быть сохранен", nameof(chatId));
        }
        if (userId == -1)
        {
            throw new ArgumentException($"Параметр {nameof(userId)} равный -1 не может быть сохранен", nameof(userId));
        }
        User user = _context.Users.Find(chatId)
            ?? throw new Exception("Не найден пользователь");
        user.Login = newLogin ?? user.Login;
        user.UserId = userId ?? user.UserId;
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
