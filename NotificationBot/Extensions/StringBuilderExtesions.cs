using System.Text;
using Telegram.Bot.Extensions;

namespace NotificationsBot.Extensions
{
    public static class StringBuilderExtesions
    {
        public static StringBuilder AddMainInfo(this StringBuilder sb, string mainText)
        {
            sb.AppendLine(mainText);

            return sb;
        }

        public static StringBuilder AddProject(this StringBuilder sb, string projectText)
        {
            sb.AppendLine();
            sb.Append("*Project*: ");
            sb.Append(projectText);

            return sb;
        }

        public static StringBuilder AddTitle(this StringBuilder sb, string titleText)
        {
            sb.AppendLine();
            sb.Append("*Title*: ");
            sb.Append(titleText);

            return sb;
        }

        public static StringBuilder AddDescription(this StringBuilder sb, string descrText)
        {
            sb.AppendLine();
            sb.Append("*Description*: ");
            sb.AppendLine(descrText);

            return sb;
        }

        public static StringBuilder AddDefinition(this StringBuilder sb, string defText)
        {
            sb.AppendLine();
            sb.Append("*Definition*: ");
            sb.AppendLine(defText);

            return sb;
        }

        public static StringBuilder AddTags(this StringBuilder sb, string projectName, string eventType)
        {
            sb.AppendLine();
            sb.Append(Markdown.Escape($"#{projectName.Replace('.', '_').Replace("(agile)", "")} #{eventType}"));

            return sb;
        }
    }
}
