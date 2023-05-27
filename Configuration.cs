namespace BlogV2
{
    public static class Configuration
    {
        public static string JwtKey = "";
        public static string ApiKeyName = "";
        public static string ApiKeyValue = "";
        public static SmtpConfiguration Smtp = new();

        public class SmtpConfiguration
        {
            public string Host { get; set; } = string.Empty;
            public int Port { get; set; } = 25;
            public string User { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }
    }
}
