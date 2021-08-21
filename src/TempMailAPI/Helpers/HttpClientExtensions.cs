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
        private static readonly HttpMethod HttpPatchMethod = new HttpMethod("PATCH");


        public static async Task<Result<TResponse>> GetAsync<TResponse>(this HttpClient client, Uri endpoint, string token = default)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, endpoint))
            {
                SetAuthorizationHeader(request, token);

                using (var response = await client.SendAsync(request).ConfigureAwait(false))
                {
                    return new Result<TResponse>
                    {
                        Message = response,
                        Data = await GetData<TResponse>(response),
                    };
                }
            }
        }

        public static async Task<Result<TResponse>> PostAsync<TRequest, TResponse>(this HttpClient client, Uri endpoint, TRequest content, string token = default)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, endpoint))
            {
                SetAuthorizationHeader(request, token);
                SetContent(request, content);

                using (var response = await client.SendAsync(request).ConfigureAwait(false))
                {
                    return new Result<TResponse>
                    {
                        Message = response,
                        Data = await GetData<TResponse>(response),
                    };
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
                    return new Result { Message = response };
                }
            }
        }

        public static async Task<Result> PatchAsync<TRequest>(this HttpClient client, Uri endpoint, TRequest content, string token = default)
        {
            using (var request = new HttpRequestMessage(HttpPatchMethod, endpoint))
            {
                SetAuthorizationHeader(request, token);
                SetContent(request, content);

                using (var response = await client.SendAsync(request).ConfigureAwait(false))
                {
                    return new Result { Message = response };
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

        private static void SetContent<T>(HttpRequestMessage request, T content)
        {
            var json = Serializer.Serialize(content);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
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