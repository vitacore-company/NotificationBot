namespace NotificationsBot.Utils
{
    public static class Utilites
    {
        public static object GetUniqueUser(object user)
        {
            if (user is not string str)
            {
                str = user.ToString();
            }

            int startIndex = str.IndexOf("<") + 1;
            int endIndex = str.IndexOf(">");
            return str.Substring(startIndex, endIndex - startIndex);
        }
    }
}
