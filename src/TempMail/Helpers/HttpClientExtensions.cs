using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using SmorcIRL.TempMail.Messaging;

namespace SmorcIRL.TempMail.Helpers
{
    internal static class HttpClientExtensions
    {
        // HttpMethod.Patch is available only starting from .NET Standard 2.1
        private static readonly HttpMethod HttpPatchMethod = new HttpMethod("PATCH");

        public static async Task<Result<TResponse>> GetAsync<TResponse>(this HttpClient client, Uri endpoint, string token = default) where TResponse : class
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, endpoint))
            {
                SetAuthorizationHeader(request, token);

                using (var response = await client.SendAsync(request).ConfigureAwait(false))
                {
                    return new Result<TResponse>(response, await GetData<TResponse>(response));
                }
            }
        }

        public static async Task<Result<TResponse>> PostAsync<TRequest, TResponse>(this HttpClient client, Uri endpoint, TRequest content, string token = default) where TResponse : class
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, endpoint))
            {
                SetAuthorizationHeader(request, token);
                SetContent(request, content, "application/json");
                
                using (var response = await client.SendAsync(request).ConfigureAwait(false))
                {
                    return new Result<TResponse>(response, await GetData<TResponse>(response));
                }
            }
        }

        public static async Task<Result> DeleteAsync(this HttpClient client, Uri endpoint, string token = default)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Delete, endpoint))
            {
                SetAuthorizationHeader(request, token);

                using (var response = await client.SendAsync(request).ConfigureAwait(false))
                {
                    return new Result(response);
                }
            }
        }

        public static async Task<Result> PatchAsync<TRequest>(this HttpClient client, Uri endpoint, TRequest content, string token = default)
        {
            using (var request = new HttpRequestMessage(HttpPatchMethod, endpoint))
            {
                SetAuthorizationHeader(request, token);
                SetContent(request, content, "application/merge-patch+json");
                
                using (var response = await client.SendAsync(request).ConfigureAwait(false))
                {
                    return new Result(response);
                }
            }
        }

        private static void SetAuthorizationHeader(HttpRequestMessage request, string token)
        {
            if (token != null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        private static void SetContent<T>(HttpRequestMessage request, T content, string contentType)
        {
            var json = Serializer.Serialize(content);
            request.Content = new StringContent(json, Encoding.UTF8, contentType);
        }

        private static async Task<T> GetData<T>(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                return default;
            }

            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return Serializer.Deserialize<T>(responseBody);
        }
    }
}