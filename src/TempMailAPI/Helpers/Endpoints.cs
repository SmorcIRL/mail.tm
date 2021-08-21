namespace SmorcIRL.TempMail.Helpers
{
    internal static class Endpoints
    {
        public const string DefaultApiUri = "https://api.mail.tm";

        public const string PostAccount = "/accounts";
        public const string GetAccount = "/accounts/{0}";
        public const string DeleteAccount = "/accounts/{0}";
        public const string GetMe = "/me";

        public const string GetDomains = "/domains";
        public const string GetDomain = "/domains/{0}";

        public const string GetMessages = "/messages?page={0}";
        public const string GetMessage = "/messages/{0}";
        public const string DeleteMessage = "/messages/{0}";
        public const string PatchMessage = "/messages/{0}";

        public const string GetSource = "/sources/{0}";

        public const string PostToken = "/token";
    }
}