namespace NotificationsBot.Interfaces;

/// <summary>
/// Сервис данных пользователя
/// </summary>
public interface IUsersDataService
{
    /// <summary>
    /// Сохраняет нового пользователя.
    /// </summary>
    /// <param name="login">Логин.</param>
    /// <param name="chatId">Идентификатор чата.</param>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <returns></returns>
    public Task SaveNewUser(string? login, long chatId, long userId);
    /// <summary>
    /// Обновляет пользователя.
    /// </summary>
    /// <param name="newLogin">Новый логин.</param>
    /// <param name="chatId">Идентификатор чата.</param>
    /// <returns></returns>
    public Task UpdateUser(string newLogin, long chatId);
    /// <summary>
    /// Обновляет пользователя.
    /// </summary>
    /// <param name="newLogin">Новый логин.</param>
    /// <param name="chatId">Идентификатор чата.</param>
    /// <returns></returns>
    public Task UpdateUser(string? newLogin, long chatId, long? userId);
    /// <summary>
    /// Определяет, есть ли [пользователь] с [указанный идентификатор чата].
    /// </summary>
    /// <param name="chatId">Идентификатор чата.</param>
    /// <returns></returns>
    public Task<bool> IsContainUser(long chatId);
    /// <summary>
    /// Изменяет статус.
    /// </summary>
    /// <param name="chatId">Идентификатор чата.</param>
    /// <param name="status">Статус.</param>
    /// <returns></returns>
    public Task ChangeStatus(long chatId, string status);
    /// <summary>
    /// Отменяет статус.
    /// </summary>
    /// <param name="chatId">Идентификатор чата.</param>
    /// <returns></returns>
    public Task CancelStatus(long chatId);
    /// <summary>
    /// Получает статус.
    /// </summary>
    /// <param name="chatId">Идентификатор чата.</param>
    /// <returns></returns>
    public Task<string> GetStatus(long chatId);
}
