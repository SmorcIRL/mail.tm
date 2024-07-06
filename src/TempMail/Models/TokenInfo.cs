using Newtonsoft.Json;

namespace SmorcIRL.TempMail.Models
{
    public class TokenInfo
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }
    }
}