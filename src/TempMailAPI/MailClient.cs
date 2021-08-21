using System;
using System.Net.Http;
using System.Net.Http.Headers;
using SmorcIRL.TempMail.Helpers;

namespace SmorcIRL.TempMail
{
    public sealed partial class MailClient
    {
        //========================================//

        private static readonly HttpClient HttpClient;

        private static Uri _apiUri;

        public static Uri ApiUri
        {
            get => _apiUri;
            set
            {
                if (value == null || !value.IsAbsoluteUri || value.Scheme != "https" && value.Scheme != "http")
                {
                    throw new FormatException("Invalid api uri");
                }

                _apiUri = value;
            }
        }

        static MailClient()
        {
            ApiUri = new Uri(Endpoints.DefaultApiUri);

            HttpClient = new HttpClient();

            HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        //========================================//

        public string Email { get; private set; }

        public string Password { get; private set; }

        public string BearerToken { get; private set; }

        public string AccountId { get; private set; }

        public bool IsDeleted { get; private set; }

        //========================================//

        private void EnsureAlive()
        {
            if (IsDeleted)
            {
                throw new InvalidOperationException("Account under this client is already deleted");
            }
        }

        private static Uri CreateUri(string endpoint, params object[] args)
        {
            var formattedEndpoint = args == null
                ? endpoint
                : string.Format(endpoint, args);

            return new Uri(ApiUri, formattedEndpoint);
        }
    }
}