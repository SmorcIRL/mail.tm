using Newtonsoft.Json;

namespace SmorcIRL.TempMail.Models
{
    public class MessageSource
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("downloadUrl")]
        public string DownloadUrl { get; set; }

        [JsonProperty("data")]
        public string Data { get; set; }
    }
}