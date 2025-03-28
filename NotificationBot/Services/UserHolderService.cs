﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using NotificationsBot.Interfaces;
using NotificationsBot.Models;
using System;
using System.Collections.Concurrent;

namespace NotificationsBot.Services
{
    public class UserHolderService : IUserHolder
    {
        private readonly AppContext _context;
        private readonly IExistUserChecker _userChecker;

        public UserHolderService(AppContext context, IExistUserChecker userChecker)
        {
            _context = context;
            _userChecker = userChecker;
        }

        public async Task ClearAsync()
        {
            Interlocked.CompareExchange(ref LocalUsers.Users, new ConcurrentDictionary<string, UserInfo>(), LocalUsers.Users);
        }

        public async Task<List<long>> GetChatIdsByLogin(List<string> login)
        {
            List<long> chatIds = new List<long>();

            foreach (string userLogin in login)
            {
                if (!LocalUsers.Users.TryGetValue(userLogin, out UserInfo userInfo))
                {
                    User? user = _context.Users.Where(x => EF.Functions.ILike(x.Login, "%" + userLogin + "%")).FirstOrDefault();

                    if (user == null)
                    {
                        continue;
                    }

                    bool userExists = await _userChecker.CheckExistUser(user.UserId);
                    userInfo = new UserInfo(user.ChatId, userExists);

                    LocalUsers.Users.TryAdd(userLogin, userInfo);
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
