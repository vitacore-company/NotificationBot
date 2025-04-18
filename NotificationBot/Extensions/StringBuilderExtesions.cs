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
            sb.Append("*Project*: ");
            sb.Append(projectText);
            sb.AppendLine();

            return sb;
        }

        public static StringBuilder AddTitle(this StringBuilder sb, string titleText)
        {
            sb.Append("*Title*: ");
            sb.Append(titleText);
            sb.AppendLine();

            return sb;
        }

        public static StringBuilder AddDescription(this StringBuilder sb, string descrText)
        {
            sb.Append("*Description*: ");
            sb.AppendLine(descrText);
            sb.AppendLine();

            return sb;
        }

        public static StringBuilder AddDefinition(this StringBuilder sb, string defText)
        {
            sb.Append("*Definition*: ");
            sb.AppendLine(defText);
            sb.AppendLine();

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
