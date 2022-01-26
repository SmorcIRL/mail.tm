using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SmorcIRL.TempMail
{
    public sealed partial class MailClient
    {
        private static readonly HttpClient HttpClient;

        static MailClient()
        {
            HttpClient = new HttpClient();
            HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        //========================================//

        public Uri ApiUri { get; }

        public string Email { get; private set; }

        public string BearerToken { get; private set; }

        public string AccountId { get; private set; }

        public MailClient() : this(Endpoints.DefaultApiUri)
        {
        }

        public MailClient(Uri apiUri)
        {
            if (apiUri == null)
            {
                throw new ArgumentNullException(nameof(apiUri));
            }

            if (!apiUri.IsAbsoluteUri || apiUri.Scheme != "https" && apiUri.Scheme != "http")
            {
                throw new FormatException("Invalid api uri format");
            }

            ApiUri = apiUri;
        }

        private Uri FormatUri(string endpoint, params object[] args)
        {
            return new Uri(ApiUri, args == null ? endpoint : string.Format(endpoint, args));
        }
    }
}