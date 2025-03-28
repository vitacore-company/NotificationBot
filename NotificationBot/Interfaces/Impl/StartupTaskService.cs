
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Extensions;

namespace NotificationsBot.Interfaces.Impl
{
    public class StartupTaskService : IStartupTask
    {
        private readonly AppContext _context;
        private readonly ITelegramBotClient _telegramBotClient;

        public StartupTaskService(AppContext context, ITelegramBotClient telegramBotClient)
        {
            _context = context;
            _telegramBotClient = telegramBotClient;
        }

        public async Task ExecuteAsync(CancellationToken token)
        {
            string versionText = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "version.json"));

            VersionParser version = JsonConvert.DeserializeObject<VersionParser>(versionText);
            string? appVersion = _context.Versions.Select(x => x.Version).SingleOrDefault();

            if (string.IsNullOrEmpty(appVersion) || !appVersion.Equals(version.Version))
            {
                List<long> chatIds = await _context.Users.Select(x => x.ChatId).ToListAsync();
                string escapedMessage = Markdown.Escape(version.Note);

                StringBuilder sb = new StringBuilder();
                sb.Append("*");
                sb.Append(escapedMessage);
                sb.Append("*");

                escapedMessage = sb.ToString();

                foreach (long chatId in chatIds)
                {
                    await _telegramBotClient.SendMessage(chatId, escapedMessage, Telegram.Bot.Types.Enums.ParseMode.MarkdownV2);
                }

                if (string.IsNullOrEmpty(appVersion))
                {
                    _context.Versions.Add(new Models.AppVersion() { Version = version.Version });
                }
                else
                {
                    await _context.Versions.ExecuteUpdateAsync(update => update.SetProperty(v => v.Version, version.Version), token);
                }

                await _context.SaveChangesAsync();
            }
        }
    }
}

file class VersionParser
{
    public string Version { get; set; }

    public string Note { get; set; }
}
