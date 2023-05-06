namespace BlogV2
{
    public static class Configuration
    {
        public static string JwtKey = "MDI4YTcxNDAtZTRjOS00NDhjLWE5NGEtOGZhMjJlMjYxODI3=";
        public static string ApiKeyName = "api_key";
        public static string ApiKeyValue = "course_api_P2VWrjndi7Ujd/ihQKtGuw==";
        public static SmtpConfiguration Smtp = new();

        public class SmtpConfiguration
        {
            public string Host { get; set; }
            public int Port { get; set; } = 25;
            public string User { get; set; }
            public string Password { get; set; }
        }
    }
}
