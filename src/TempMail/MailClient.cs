using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SmorcIRL.TempMail
{
    public sealed partial class MailClient
    {
        private static readonly HttpClient GlobalHttpClient;

        static MailClient()
        {
            GlobalHttpClient = new HttpClient();
            SetupHttpClient(GlobalHttpClient);
        }

        private static void SetupHttpClient(HttpClient httpClient)
        {
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        //========================================//

        private readonly HttpClient _httpClient;

        public Uri ApiUri { get; }

        public string Email { get; private set; }

        public string BearerToken { get; private set; }

        public string AccountId { get; private set; }

        public MailClient() : this(Endpoints.DefaultApiUri)
        {

        }

        public MailClient(Uri apiUri) : this(GlobalHttpClient, apiUri)
        {

        }

        public MailClient(HttpClient httpClient, Uri apiUri)
        {  
            if (httpClient == null)
            {
                throw new ArgumentNullException(nameof(httpClient));
            }

            if (apiUri == null)
            {
                throw new ArgumentNullException(nameof(apiUri));
            }

            if (!apiUri.IsAbsoluteUri || apiUri.Scheme != "https" && apiUri.Scheme != "http")
            {
                throw new FormatException("Invalid api uri format");
            }

            if (httpClient != GlobalHttpClient)
            {
                SetupHttpClient(httpClient);
            }

            ApiUri = apiUri;
            _httpClient = httpClient;
        }

        private Uri FormatUri(string endpoint, params object[] args)
        {
            return new Uri(ApiUri, args == null ? endpoint : string.Format(endpoint, args));
        }
    }
}