using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using NotificationsBot.Interfaces;
using NotificationsBot.Models.Database;
using NotificationsBot.Models.Locals;

namespace NotificationsBot.Services
{
    public class UserHolderService : IUserHolder
    {
        private readonly AppContext _context;
        private readonly IExistUserChecker _userChecker;
        private readonly IMemoryCache _memoryCache;

        public UserHolderService(AppContext context, IExistUserChecker userChecker, IMemoryCache memoryCache)
        {
            _context = context;
            _userChecker = userChecker;
            _memoryCache = memoryCache;
        }

        public async Task<List<long>> GetChatIdsByLogin(List<string> login)
        {
            List<long> chatIds = new List<long>();

            foreach (string userLogin in login)
            {
                if (!_memoryCache.TryGetValue(userLogin, out UserInfo userInfo))
                {
                    User? user = _context.Users.Where(x => x.Login != null && EF.Functions.ILike(x.Login, "%" + userLogin + "%")).FirstOrDefault();

                    if (user == null)
                    {
                        continue;
                    }

                    bool userExists = await _userChecker.CheckExistUser(user.UserId);
                    userInfo = new UserInfo(user.ChatId, userExists);

                    _memoryCache.Set(userLogin, userInfo, new MemoryCacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(10) });
                }

                if (userInfo.Available)
                {
                    chatIds.Add(userInfo.ChatId);
                }
            }

            return chatIds;
        }
    }
}
