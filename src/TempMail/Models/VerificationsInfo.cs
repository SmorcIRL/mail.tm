using Newtonsoft.Json;

namespace SmorcIRL.TempMail.Models
{
    public class VerificationsInfo
    {
        [JsonProperty("tls")]
        public TlsInfo Tls { get; set; }

        [JsonProperty("spf")]
        public bool Spf { get; set; }

        [JsonProperty("dkim")]
        public bool Dkim { get; set; }
    }
}